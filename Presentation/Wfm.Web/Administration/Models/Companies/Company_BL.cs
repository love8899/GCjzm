using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.JobOrder;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Companies;
using Wfm.Services.Features;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Messages;
using Wfm.Services.Security;


namespace Wfm.Admin.Models.Companies
{
    public class Company_BL
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IPermissionService _permissionService;
        private readonly IUserFeatureService _userFeatureService;
        private readonly IClientNotificationService _clientNotificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger _logger;

        #endregion


        #region Ctor

        public Company_BL(IWorkContext workContext,
            ICompanyService companyService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            ICompanyVendorService companyVendorService,
            ICompanyBillingService companyBillingService,
            IJobOrderService jobOrderService,
            IPermissionService permissionService,
            IUserFeatureService userFeatureService,
            IClientNotificationService clientNotificationService,
            ILocalizationService localizationService,
            IActivityLogService activityLogService,
            ILogger logger)
        {
            _workContext = workContext;
            _companyService = companyService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyVendorService = companyVendorService;
            _companyBillingService = companyBillingService;
            _jobOrderService = jobOrderService;
            _permissionService = permissionService;
            _userFeatureService = userFeatureService;
            _clientNotificationService = clientNotificationService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _logger = logger;
        }

        #endregion


        public List<CompanyLocationListModel> GetCompanyLocationList(Guid? companyGuid)
        {
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null || companyGuid==Guid.Empty)
                return new List<CompanyLocationListModel>();
            int companyId = company.Id;
            var companyLocations = _companyDivisionService.SQL_GetAllCompanyLocationsByCompanyId(companyId, false);            

            return companyLocations.Select(x=>x.ToListModel()).ToList();
        }


        public List<CompanyDepartmentModel> GetCompanyDepartmentList(Guid companyGuid)
        {
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var companyDepartments = _companyDepartmentService.GetAllCompanyDepartmentsByCompanyId(companyId, false);

            List<CompanyDepartmentModel> companyDepartmentList = new List<CompanyDepartmentModel>();

            foreach (var item in companyDepartments)
            {
                CompanyDepartmentModel compDeptModel = item.ToModel();

                CompanyLocation compLocation = _companyDivisionService.GetCompanyLocationById(compDeptModel.CompanyLocationId);
                compDeptModel.CompanyLocationName = compLocation == null ? "" : compLocation.LocationName;

                companyDepartmentList.Add(compDeptModel);
            }

            return companyDepartmentList;
        }


        public Guid CreateOrUpdateCompany(CompanyBasicInformation companyModel, int currentUserId, out int companyId)
        {
            Company company = new Company();
            //if company exists
            if (companyModel.CompanyGuid != Guid.Empty)
            {
                company = _companyService.GetCompanyByGuid(companyModel.CompanyGuid);
                _companyService.UpdateCompany(companyModel.ToEntity(company));
                //activity log
                _activityLogService.InsertActivityLog("UpdateCompany", _localizationService.GetResource("ActivityLog.UpdateCompanyProfile"), companyModel.CompanyName);

            }
            else
            {
                company = companyModel.ToEntity();
                InsertNewCompany(company, currentUserId);
                DefaultCompanySetting(company.Id);
                //activity log
                _activityLogService.InsertActivityLog("AddNewCompany", _localizationService.GetResource("ActivityLog.AddNewCompany"), companyModel.CompanyName);

            }
            companyId = company.Id;
            return company.CompanyGuid;
        }


        private void DefaultCompanySetting(int companyId)
        {
            //setup default Vendor
            _companyVendorService.SetDefaultCompanyVendor(companyId);
            //setup default user feature
            _userFeatureService.EnableAllFeaturesForNewUser("Client", companyId);

            // setup default notifications
            _clientNotificationService.SetupDefaultNotifications(companyId);
        }


        public void InsertNewCompany(Company company,int userID)
        {
            company.OwnerId = userID;
            company.EnteredBy = userID;
            company.UpdatedOnUtc = System.DateTime.UtcNow;
            company.CreatedOnUtc = System.DateTime.UtcNow;
            _companyService.InsertCompany(company);
        }


        public DataSourceResult GetCompanyModelList(Account currentAccount, DataSourceRequest request)
        {
            var companies = _companyService.Secure_GetAllCompanies(currentAccount, true);           
            return companies.ToDataSourceResult(request,model=>model.ToModel());
        }


        public Company GetCompanyByIdForView(Guid guid)
        {

            Company company = _companyService.GetCompanyByGuid(guid);
            
            return company;
        }


        public Company GetCompanyByIdForEdit(Guid guid)
        {
            Company company = _companyService.GetCompanyByGuid(guid);
           
            return company;
        }


        public DataSourceResult GetCompanyJobOrderList(DataSourceRequest request, 
            Guid companyGuid, DateTime startDate, DateTime endDate)
        {
            var joborders = _jobOrderService.GetAllJobOrdersAsQueryable(_workContext.CurrentAccount)
                .Where(x => x.Company.CompanyGuid == companyGuid)
                .Where(x => (!x.EndDate.HasValue || x.EndDate >= startDate) && x.StartDate <= endDate);

            var rates = _companyBillingService.GetAllCompanyBillingRatesAsQueryable()
                .Where(x => x.Company.CompanyGuid == companyGuid)
                .Where(x => (!x.DeactivatedDate.HasValue || x.DeactivatedDate >= startDate) && x.EffectiveDate <= endDate);

            var accountId = _workContext.CurrentAccount.Id;
            var authorized = _permissionService.Authorize(StandardPermissionProvider.UpdateJobOrder);
            var result = from j in joborders
                         from r in rates
                            .Where(r => j.CompanyLocationId == r.CompanyLocationId)
                            .Where(r => j.BillingRateCode == r.RateCode)
                            .DefaultIfEmpty()
                         select new JobOrderModel()
                         {
                             Id = j.Id,
                             JobOrderGuid = j.JobOrderGuid,
                             JobTitle = j.JobTitle,
                             PayRate = r.RegularPayRate,
                             CompanyLocationId = j.CompanyLocationId,
                             CompanyDepartmentId = j.CompanyDepartmentId,
                             CompanyContactId = j.CompanyContactId,
                             JobOrderTypeId = j.JobOrderTypeId,
                             JobOrderCategoryId = j.JobOrderCategoryId,
                             JobOrderStatusId = j.JobOrderStatusId,
                             StartDate = j.StartDate,
                             EndDate = j.EndDate,
                             StartTime = j.StartTime,
                             EndTime = j.EndTime,
                             BillingRateCode = j.BillingRateCode,
                             CreatedOnUtc = j.CreatedOnUtc,
                             UpdatedOnUtc = j.UpdatedOnUtc,
                             isUpdateable = authorized || accountId == j.OwnerId || accountId == j.RecruiterId,
                             IsDirectHire = j.JobOrderType.IsDirectHire,
                             JobPostingId = j.JobPostingId
                         };

            return result.ToDataSourceResult(request);
        }


        public void WriteFilesIntoEmailTemplates(CompanyEmailTemplateModel model, 
            IEnumerable<HttpPostedFileBase> files, IAttachmentTypeService _attachmentTypeService, out string errors)
        {
            errors = string.Empty;
            if (!model.KeepFile1)
            {
                model.AttachmentFile = null;
                model.AttachmentFileName = null;
                model.AttachmentTypeId = null;
            }
            if (!model.KeepFile2)
            {
                model.AttachmentFile2 = null;
                model.AttachmentFileName2 = null;
                model.AttachmentTypeId2 = null;
            }
            if (!model.KeepFile3)
            {
                model.AttachmentFile3 = null;
                model.AttachmentFileName3 = null;
                model.AttachmentTypeId3 = null;
            }
            int available = (string.IsNullOrEmpty(model.AttachmentFileName) ? 1 : 0) 
                            + (string.IsNullOrEmpty(model.AttachmentFileName2) ? 1 : 0) 
                            + (string.IsNullOrEmpty(model.AttachmentFileName3) ? 1 : 0);
            //save files
            if (files != null && files.Count() > 0 && files.Count() <= available)
            {
                foreach (var file in files)
                {
                    using (Stream inputStream = file.InputStream)
                    {
                        var fileBinary = new byte[inputStream.Length];
                        inputStream.Read(fileBinary, 0, fileBinary.Length);
                        var attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(file.FileName));
                        if (string.IsNullOrEmpty(model.AttachmentFileName))
                        {
                            model.AttachmentFile = fileBinary;
                            if (attachmentType != null)
                                model.AttachmentTypeId = attachmentType.Id;
                            model.AttachmentFileName = file.FileName;
                            continue;
                        }
                        if (string.IsNullOrEmpty(model.AttachmentFileName2))
                        {
                            model.AttachmentFile2 = fileBinary;
                            if (attachmentType != null)
                                model.AttachmentTypeId2 = attachmentType.Id;
                            model.AttachmentFileName2 = file.FileName;
                            continue;
                        }
                        if (string.IsNullOrEmpty(model.AttachmentFileName3))
                        {
                            model.AttachmentFile3 = fileBinary;
                            if (attachmentType != null)
                                model.AttachmentTypeId3 = attachmentType.Id;
                            model.AttachmentFileName3 = file.FileName;
                            continue;
                        }

                    }
                }
            }
            else if(files!=null && files.Count()>available)
            {
                errors = "There are only three attachments allowed to be uploaded, please remove existed or uploading file(s)!";
            }
        }
    }
}
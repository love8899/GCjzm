using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Companies;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Companies;
using System.IO;
using Wfm.Services.ExportImport;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Common;
using Kendo.Mvc.UI;
using Wfm.Web.Framework;
using System.Text.RegularExpressions;
using Wfm.Core;


namespace Wfm.Admin.Models.CompanyBilling
{
    public class CompanyBillingRate_BL
    {
        private readonly IExportManager _exportManager;
        private readonly IActivityLogService _activityLogService;
        //private readonly IAccountService _accountService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        public CompanyBillingRate_BL(IWorkContext workContext, ICompanyBillingService companyBillingService, IActivityLogService activityLogService, ILocalizationService localizationService, ILogger logger, IExportManager exportManager)
        {
            _activityLogService = activityLogService;
            //_accountService = accountService;
            _workContext = workContext;
            _localizationService = localizationService;
            _logger = logger;
            _companyBillingService = companyBillingService;
            _exportManager = exportManager;
        }

        public CompanyBillingRateModel CreateNewBillingRate(int companyId, Guid companyGuid)
        {
            CompanyBillingRateModel result = new CompanyBillingRateModel();
            result.CompanyId = companyId;
            result.CompanyGuid = companyGuid;
            result.FranchiseId = _workContext.CurrentFranchise.Id;
            result.IsActive = true;
            result.CompanyIsFiltered = companyId > 0 ? true : false;
            result.FranchiseIsFiltered = !_workContext.CurrentFranchise.IsDefaultManagedServiceProvider;

            return result;
        }

        public CompanyBillingRateModel CopyBillingRate(int originalId, bool filterCompany)
        {
            CompanyBillingRate originalRecord = _companyBillingService.GetCompanyBillingRateById(originalId);
            if (originalRecord == null)
                return null;

            CompanyBillingRateModel model = originalRecord.ToModel();
            model.EffectiveDate = null; // we don't need effective date in copy
            model.DeactivatedDate = null; // we don't need DeactivatedDate  in copy
            model.Id = 0;
            model.IsActive = true;
            model.CompanyIsFiltered = filterCompany;
            model.FranchiseIsFiltered = !_workContext.CurrentFranchise.IsDefaultManagedServiceProvider;
            model.CompanyGuid = originalRecord.Company.CompanyGuid;

            // remove quotations
            model.Quotations = new List<QuotationModel>();

            if (model.Note != null)
                model.Note = HttpUtility.HtmlDecode(model.Note);

            return model;
        }

        public DataSourceResult GetAllCompanyBillingRates(DataSourceRequest request, int? companyId)
        {
            var rates = _companyBillingService.GetAllCompanyBillingRatesAsQueryable();
            if (companyId.HasValue && companyId > 0)
                rates = rates.Where(x => x.CompanyId == companyId);

            // TODO: if without vendor specific rates, use MSP's
            if (_workContext.CurrentAccount.IsLimitedToFranchises)
                rates = rates.Where(x => x.FranchiseId == _workContext.CurrentAccount.FranchiseId);

            // active only, for recruiters
            var timeNow = DateTime.Now;
            if (_workContext.CurrentAccount.IsRecruiterOrRecruiterSupervisor())
                rates = rates.Where(x => x.EffectiveDate <= timeNow && (!x.DeactivatedDate.HasValue || x.DeactivatedDate >= timeNow));

            IPagedList<CompanyBillingRate> billingRates = rates.PagedForCommand(request);

            List<CompanyBillingRateModel> modelList = new List<CompanyBillingRateModel>();

            foreach (var item in billingRates)
            {
                CompanyBillingRateModel billingRateModel = MappingExtensions.ToModel(item);
                if (billingRateModel.Note != null)
                    billingRateModel.Note = Regex.Replace(HttpUtility.HtmlDecode(billingRateModel.Note), "<.*?>", String.Empty);

                if (companyId.HasValue)
                    billingRateModel.CompanyIsFiltered = true;

                billingRateModel.FranchiseIsFiltered = !_workContext.CurrentFranchise.IsDefaultManagedServiceProvider;

                modelList.Add(billingRateModel);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = modelList, // Process data (paging and sorting applied)
                Total = billingRates.TotalCount // Total number of records
            };
            return result;
        }

        public CompanyBillingRateModel GetCompanyBillingRateById(int id, bool filterCompany)
        {
            CompanyBillingRate companyBilling = _companyBillingService.GetCompanyBillingRateById(id);
            if (companyBilling == null)
                return null;

            CompanyBillingRateModel model = companyBilling.ToModel();
            if (model.Note != null)
                model.Note = HttpUtility.HtmlDecode(model.Note);
            model.CompanyName = companyBilling.Company.CompanyName;
            model.CompanyIsFiltered = filterCompany;
            model.FranchiseIsFiltered = !(_workContext.CurrentFranchise.IsDefaultManagedServiceProvider);
            model.CompanyGuid = companyBilling.Company.CompanyGuid;
            return model;
        }


        public bool SaveCompanyBillingRate(CompanyBillingRateModel model, out string errorMessage, IEnumerable<HttpPostedFileBase> files = null)
        {
            errorMessage = null;
            bool isNew = (model.Id <= 0);

            CompanyBillingRate billingRate = null;
            if (!isNew)
            {
                billingRate = _companyBillingService.GetCompanyBillingRateById(model.Id);
                if (billingRate == null)
                    return false;
            }

            billingRate = model.ToEntity(billingRate);
            billingRate = this.adjustBillingRateProperties(billingRate);

            // add quotations
            if (files != null)
                _PopulateQuotations(billingRate, files);

            try
            {
                billingRate.EnteredBy = _workContext.CurrentAccount.Id;

                if (isNew)
                {
                    billingRate.IsActive = true;
                    _companyBillingService.Insert(billingRate);
                    model.Id = billingRate.Id;
                }
                else
                {
                    _companyBillingService.Update(billingRate);
                }

                model.Quotations = billingRate.Quotations.Select(x => x.ToModel()).ToList();
            }
            catch (Exception ex)
            {
                if(ex.InnerException.InnerException.Message.Contains("OverlapError"))
                {
                    errorMessage = "The Company Billing Rate cannot be created beacause it already exist in the system!";
                    return false;
                }
            }

            //activity log
            _activityLogService.InsertActivityLog("UpdateBillingRate", _localizationService.GetResource("ActivityLog.UpdateBillingRate"), billingRate.RateCode);

            return true;
        }


        public bool SaveCopiedBillingRate(CompanyBillingRateModel newRecord, int OrigBillingRateId, out string errorMessage, IEnumerable<HttpPostedFileBase> files = null)
        {
            bool result = false;
            errorMessage = null;

            CompanyBillingRate originalRecord = _companyBillingService.GetCompanyBillingRateById(OrigBillingRateId);
            if (originalRecord == null)
                return false;

            // validate StartDate, EndDate
            if (originalRecord.FranchiseId == newRecord.FranchiseId)
            {
                if (newRecord.EffectiveDate <= originalRecord.EffectiveDate)
                {
                    errorMessage = _localizationService.GetResource("Admin.CopyBillingRate.GreaterEffectiveDate");
                    return false;
                }
            }
 
            // Update Original copy Deactivated date.Only if Vendor is same.
            if (originalRecord.FranchiseId == newRecord.FranchiseId && 
                originalRecord.CompanyLocationId == newRecord.CompanyLocationId &&
                originalRecord.PositionId == newRecord.PositionId &&
                originalRecord.ShiftCode == newRecord.ShiftCode)
            {
                originalRecord.DeactivatedDate = Convert.ToDateTime(newRecord.EffectiveDate).Date.AddSeconds(-1);
                _companyBillingService.Update(originalRecord);
                //activity log
                _activityLogService.InsertActivityLog("UpdateBillingRate", _localizationService.GetResource("ActivityLog.UpdateBillingRate"), originalRecord.RateCode);
            }

            try
            {
                // Insert new copy
                newRecord.Id = 0;
                result = SaveCompanyBillingRate(newRecord, out errorMessage, files);

                //activity log
                if (result)
                    _activityLogService.InsertActivityLog("AddNewBillingRate", _localizationService.GetResource("ActivityLog.CopyCompanyBillingRate"), newRecord.RateCode);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("OverlapError"))
                {
                    errorMessage = "The Company Billing Rate cannot be created beacause it already exist in the system!";
                    return false;
                }
            }

            return result;
        }


        private void _PopulateQuotations(CompanyBillingRate billingRate, IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                byte[] fileBinary;
                using (var output = new MemoryStream())
                {
                    file.InputStream.CopyTo(output);
                    fileBinary = output.ToArray();
                }
                billingRate.Quotations.Add(new Quotation()
                {
                    FileName = file.FileName,
                    Stream = fileBinary,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                });
            }
        }


        private CompanyBillingRate adjustBillingRateProperties(CompanyBillingRate billingRate)
        {
            billingRate.ShiftCode = billingRate.ShiftCode.Trim();

            if (billingRate.OvertimeBillingRate <= 0) billingRate.OvertimeBillingRate = billingRate.RegularBillingRate * 1.5m;
            if (billingRate.OvertimePayRate <= 0) billingRate.OvertimePayRate = billingRate.RegularPayRate * 1.5m;
            if (billingRate.BillingTaxRate <= 0) billingRate.BillingTaxRate = .15m;

            billingRate.EnteredBy = _workContext.CurrentAccount.Id;

            if (billingRate.DeactivatedDate.HasValue)
            {
                billingRate.DeactivatedDate = Convert.ToDateTime(billingRate.DeactivatedDate).Date.AddDays(1).AddSeconds(-1);
            }

            return billingRate;
        }

        public byte[] ExportSelectedCompanyBillingRates(string selectedIds, out string fileName)
        {
            fileName = string.Empty;
            IList<CompanyBillingRate> companyBillingRates = _companyBillingService.GetAllCompanyBillingRatesByIds(selectedIds);
            if (companyBillingRates.Count() == 0)
                return null;

            // Export to Xlsx
            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                fileName = _exportManager.ExportCompanyBillingRate(stream, companyBillingRates);
                bytes = stream.ToArray();
            }

            //activity log
            _activityLogService.InsertActivityLog("ExportTimeChart", _localizationService.GetResource("ActivityLog.ExportCompanyBillingRate"), "Xlsx" + "/" + selectedIds);

            return bytes;
        }

        public byte[] ExportCompanyBillingRatesToPDF(string selectedIds, IPdfService _pdfService, out string fileName)
        {
            fileName = string.Empty;
            IList<CompanyBillingRate> companyBillingRates = _companyBillingService.GetAllCompanyBillingRatesByIds(selectedIds);
            if (companyBillingRates.Count() == 0)
                return null;

            // Export to Xlsx
            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _pdfService.ExportCompanyBillingRateToPDF(stream, companyBillingRates);
                bytes = stream.ToArray();
            }

            //activity log
            _activityLogService.InsertActivityLog("ExportCompanyBillingRatesToPDF", _localizationService.GetResource("ActivityLog.ExportCompanyBillingRate"), "Xlsx" + "/" + selectedIds);
            return bytes;
        }
    }
}
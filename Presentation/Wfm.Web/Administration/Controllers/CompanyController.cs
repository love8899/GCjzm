using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.Accounts;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Models.CompanyBilling;
using Wfm.Admin.Models.Features;
using Wfm.Admin.Models.Messages;
using Wfm.Admin.Models.TimeSheet;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.ExportImport;
using Wfm.Services.Features;
using Wfm.Services.Invoices;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Payroll;
using Wfm.Services.Security;
using Wfm.Services.TimeSheet;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework;
using Wfm.Services.Franchises;
using Wfm.Shared.Models.Accounts;
using System.Text;
using Wfm.Services.Common;
using System.IO;
using Wfm.Services.Media;
using System.Web;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Controllers
{
    public class CompanyController : BaseAdminController
    {
        #region Fields

        private readonly IExportManager _exportManager;
        private readonly IPdfService _pdfService;
        private readonly IOvertimeRuleSettingService _overtimeRuleSettingService;
        private readonly ICompanyService _companyService;
        private readonly ICompanySettingService _companySettingService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateBlacklistService _candidateBalcklistService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IActivityLogService _activityLogService;
        private readonly IAccountService _accountService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IRepository<CandidateWorkTime> _workTimeRepository;
        private readonly IMissingHourService _missingHourService;
        private readonly IMissingHourDocumentService _missingHourDocService;
        private readonly IQuotationService _quotationService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IImportManager _importManager;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IFranchiseService _franchiseService;
        private readonly IUserFeatureService _userFeatureService;
        private readonly IClientNotificationService _clientNotificationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly ICompanyActivityService _companyActivityService;
        private readonly ICompanyStatusService _companyStatusService;
        private readonly ICompanyOvertimeRuleService _companyOvertimeRuleService;
        private readonly IPositionService _positionService;
        private readonly IInvoiceIntervalService _invoiceIntervalService;
        private readonly ICompanyEmailTemplateService _companyEmailTemplateService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IAttachmentTypeService _attachmentTypeService;
        private readonly ICompanyAttachmentService _companyAttachmentService;

        private readonly Company_BL _bl;

        #endregion


        #region Ctor

        public CompanyController(
            IOvertimeRuleSettingService overtimeRuleSettingService,
            ICompanyService companyService,
            ICompanySettingService companySettingService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            ICompanyContactService companyContactService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateService candidateService,
            ICandidateBlacklistService candidateBalcklistService,
            ICompanyCandidateService companyCandidateService,
            ICompanyVendorService companyVendorService,
            IActivityLogService activityLogService,
            IAccountService accountService,
            IJobOrderService jobOrderService,
            IRepository<CandidateWorkTime> workTimeRepository,
            IMissingHourService missingHourService,
            IMissingHourDocumentService missingHourDocService,
            IQuotationService quotationService,
            IWorkContext workContext,
            ICompanyBillingService companyBillingService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            IPdfService pdfService,
            IExportManager exportManager,
            IImportManager importManger,
            IRecruiterCompanyService recruiterCompanyService,
            IFranchiseService franchiseService,
            IUserFeatureService userFeatureService,
            IClientNotificationService clientNotificationService,
            IQueuedEmailService queuedEmailService,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            ICompanyActivityService companyActivityService,
            ICompanyStatusService companyStatusService,
            ICompanyOvertimeRuleService companyOvertimeRuleService,
            IPositionService positionService,
            IInvoiceIntervalService invoiceIntervalService,
            ICompanyEmailTemplateService companyEmailTemplateService,
            IWorkflowMessageService workflowMessageService,
            IAttachmentTypeService attachmentTypeService,
            ICompanyAttachmentService companyAttachmentService
            )
        {

            _overtimeRuleSettingService = overtimeRuleSettingService;
            _companyService = companyService;
            _companySettingService = companySettingService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyContactService = companyContactService;
            _companyVendorService = companyVendorService;
            _candidateJobOrderService = candidateJobOrderService;
            _candidateService = candidateService;
            _candidateBalcklistService = candidateBalcklistService;
            _companyCandidateService = companyCandidateService;
            _activityLogService = activityLogService;
            _accountService = accountService;
            _jobOrderService = jobOrderService;
            _workTimeRepository = workTimeRepository;
            _missingHourService = missingHourService;
            _missingHourDocService = missingHourDocService;
            _quotationService = quotationService;
            _workContext = workContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _importManager = importManger;
            _companyBillingService = companyBillingService;
            _pdfService = pdfService;
            _exportManager = exportManager;
            _recruiterCompanyService = recruiterCompanyService;
            _franchiseService = franchiseService;
            _userFeatureService = userFeatureService;
            _clientNotificationService = clientNotificationService;
            _queuedEmailService = queuedEmailService;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _companyActivityService = companyActivityService;
            _companyStatusService = companyStatusService;
            _companyOvertimeRuleService = companyOvertimeRuleService;
            _positionService = positionService;
            _invoiceIntervalService = invoiceIntervalService;
            _companyEmailTemplateService = companyEmailTemplateService;
            _workflowMessageService = workflowMessageService;
            _attachmentTypeService = attachmentTypeService;
            _companyAttachmentService = companyAttachmentService;

            _bl = new Company_BL(_workContext, _companyService, _companyDivisionService, _companyDepartmentService,
                _companyVendorService, _companyBillingService, _jobOrderService, _permissionService, _userFeatureService, 
                _clientNotificationService, _localizationService, _activityLogService, _logger);
        }

        #endregion


        //Company

        #region GET :/Company/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            if (_workContext.CurrentAccount.IsClientAdministrator())
            {
                if (_workContext.CurrentAccount.CompanyId > 0)
                {
                    var guid = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyGuid;
                    return RedirectToAction("Details", new { guid = guid });
                }
                else
                    return AccessDeniedView();
            }

            return View();
        }

        #endregion

        #region POST:/Company/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            var result = _bl.GetCompanyModelList(_workContext.CurrentAccount, request);

            return Json(result);
        }

        #endregion

        //#region GET :/Company/Create

        //[HttpGet]
        //public ActionResult Create()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
        //        return AccessDeniedView();

        //    CompanyEditModel companyModel = new CompanyEditModel()
        //    {
        //        IsActive = true,
        //        IsDeleted = false,
        //        CreatedOnUtc = System.DateTime.UtcNow,
        //        UpdatedOnUtc = System.DateTime.UtcNow
        //    };

        //    return View(companyModel);
        //}

        //#endregion

        //#region POST:/Company/Create

        //[HttpPost]
        //public ActionResult Create(CompanyEditModel companyModel)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
        //        return AccessDeniedView();

        //    try
        //    {
        //        var _bl = new Company_BL(_companyService, _companyDivisionService, _companyDepartmentService, _clientNotificationService, _logger);
        //        var result = _bl.SaveNewCompany(companyModel, _workContext.CurrentAccount.Id, _activityLogService, _localizationService, _userFeatureService,_companyVendorService);

        //        if (result != Guid.Empty) // save was successful
        //        {
        //            SuccessNotification(_localizationService.GetResource("Admin.Companies.Company.Added"));
        //            return RedirectToAction("Details", new { guid = result });
        //        }
        //        else
        //            ErrorNotification(_localizationService.GetResource("Admin.Companies.Company.FailedToAdd"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(String.Format("Error Saving a new company by user {0} ", _workContext.CurrentAccount.Id), ex, userAgent: Request.UserAgent);
        //    }

        //    //If we got this far, something failed, re-display form
        //    return View(companyModel);
        //}

        //#endregion

        #region GET :/Company/Edit
        public ActionResult Edit(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            if (guid == null)
            {
                ErrorNotification("The company does not exist!");
                return RedirectToAction("Index");
            }

            Company company = _bl.GetCompanyByIdForView(guid.Value);
            if (company == null)
                //No company found with the specified id
                return RedirectToAction("Index");

            CompanyBasicInformation model = company.ToBasicInformationModel();

            return View(model);
        }
        #endregion

        #region POST:/Company/Edit

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CompanyBasicInformation companyModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();

            Company company = _bl.GetCompanyByIdForEdit(companyModel.CompanyGuid);
            if (company == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                company = companyModel.ToEntity(company);
                company.UpdatedOnUtc = System.DateTime.UtcNow;

                _companyService.UpdateCompany(company);

                //activity log
                _activityLogService.InsertActivityLog("UpdateCompanyProfile", _localizationService.GetResource("ActivityLog.UpdateCompanyProfile"), companyModel.CompanyName);

                SuccessNotification(_localizationService.GetResource("Admin.Companies.Company.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { guid = company.CompanyGuid }) : RedirectToAction("Details", new { guid = company.CompanyGuid });
            }
            return View(companyModel);
        }

        #endregion


        #region GET :/Company/Detials/guid
        public ActionResult Details(Guid? guid, string tabId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();
            if (guid == null)
            {
                ErrorNotification("The company does not exist!");
                return RedirectToAction("Index");
            }

            Company company = _bl.GetCompanyByIdForView(guid.Value);
            if (company == null)
                return RedirectToAction("Index");

            if (!String.IsNullOrWhiteSpace(tabId))
                ViewBag.TabId = tabId;

            CompanyModel model = company.ToModel();
            model.CompanyStatusName = _companyStatusService.Retrieve(company.CompanyStatusId).StatusName;
            ViewBag.InvoiceIntervalCode = _invoiceIntervalService.GetInvoiceIntervalById(model.InvoiceIntervalId).Code;
            return View(model);
        }
        #endregion


        #region Settings

        [HttpGet]
        public ActionResult _TabSettings(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanySettings))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;

            return View();
        }


        [HttpPost]
        public ActionResult _Settings([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanySettings))
                return AccessDeniedView();

            var companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var settings = _companySettingService.GetAllSettings(companyId);
            var result = settings.ProjectTo<CompanySettingModel>();

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _EditSetting(int settingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanySettings))
                return AccessDeniedView();

            var setting = _companySettingService.GetSettingById(settingId);

            return PartialView(setting.ToModel());
        }


        [HttpPost]
        public ActionResult _SaveSetting(CompanySettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanySettings))
                return AccessDeniedView();

            var errorMessage = new StringBuilder();

            if (ModelState.IsValid)
            {
                try
                {
                    var setting = _companySettingService.GetSettingById(model.Id);
                    model.ToEntity(setting);
                    setting.UpdatedOnUtc = DateTime.UtcNow;

                    _companySettingService.UpdateSetting(setting);
                }
                catch (Exception exc)
                {
                    errorMessage.AppendLine(exc.Message);
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetMaxAnnualHours(int companyId)
        {
            var maxAnnualHours = _companySettingService.GetMaxAnnualHours(companyId);
            
            return Json(new { 
                StartDate = maxAnnualHours.StartDate.ToString("MM-dd"), 
                EndDate = maxAnnualHours.EndDate.ToString("MM-dd"), 
                MaxHours = maxAnnualHours.MaxHours, 
                Threshold = maxAnnualHours.Threshold
            });
        }

        #endregion


        //CompanyLocation

        #region GET :/CompanyDetails/_CompanyLocationList
        [HttpGet]
        public ActionResult _CompanyLocationList(Guid? companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
                return new EmptyResult();
            ViewBag.CompanyId = company.Id;
            ViewBag.CompanyGuid = companyGuid;

            return View();
        }
        #endregion

        #region POST:/CompanyDetails/_CompanyLocationList
        [HttpPost]
        public ActionResult _CompanyLocationList([DataSourceRequest] DataSourceRequest request, Guid? companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            var result = _bl.GetCompanyLocationList(companyGuid);

            return Json(result.ToDataSourceResult(request));
        }


        #endregion

        #region POST:/CompanyDetails/CreateLocation
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _CreateNewLocation([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyLocationListModel> models)
        {
            var results = new List<CompanyLocationListModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {

                    var entity = model.ToEntity();
                    entity.EnteredBy = _workContext.CurrentAccount.Id;
                    entity.PrimaryPhone = model.PrimaryPhone.ExtractNumericText();
                    entity.SecondaryPhone = model.SecondaryPhone.ExtractNumericText();
                    _companyDivisionService.InsertCompanyLocation(entity);
                    model.Id = entity.Id;
                    _activityLogService.InsertActivityLog("AddNewCompanyLocation", _localizationService.GetResource("ActivityLog.AddNewCompanyLocation"), model.LocationName);
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyLocation.Added"));
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        #endregion

        #region POST:/CompanyDetails/EditLocation
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditLocation([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyLocationListModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _companyDivisionService.GetCompanyLocationById(model.Id);
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    model.ToEntity(entity);

                    _companyDivisionService.UpdateCompanyLocation(entity);
                    _activityLogService.InsertActivityLog("UpdateCompanyLocation", _localizationService.GetResource("ActivityLog.UpdateCompanyLocation"), model.LocationName);
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyLocation.Updated"));
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete Location
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _DeleteLocation([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyLocationListModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _companyDivisionService.GetCompanyLocationById(model.Id);
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    model.ToEntity(entity);
                    entity.IsDeleted = true;
                    _companyDivisionService.UpdateCompanyLocation(entity);
                    _activityLogService.InsertActivityLog("UpdateCompanyLocation", _localizationService.GetResource("ActivityLog.UpdateCompanyLocation"), model.LocationName);
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyLocation.Updated"));
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion


        //CompanyDepartment

        #region GET :/CompanyDetails/_CompanyDepartmentList

        [HttpGet]
        public ActionResult _CompanyDepartmentList(Guid? companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
            {
                ErrorNotification("The company does not exist!");
                return new EmptyResult();
            }
            ViewBag.CompanyId = company.Id;
            ViewBag.CompanyGuid = companyGuid;
            ViewBag.CompanyLocations = _companyDivisionService.GetAllCompanyLocationsByCompanyIdAsSelectList(company.Id);
            return View();
        }

        #endregion

        #region POST:/CompanyDetails/_CompanyDepartmentList

        [HttpPost]
        public ActionResult _CompanyDepartmentList([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            var result = _bl.GetCompanyDepartmentList(companyGuid);

            return Json(result.ToDataSourceResult(request));
        }

        #endregion

        #region POST:/CompanyDetails/CreateDepartment
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _CreateNewDepartment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyDepartmentModel> models)
        {
            var results = new List<CompanyDepartmentModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.EnteredBy = _workContext.CurrentAccount.Id;
                    model.PhoneNumber = model.PhoneNumber.ExtractNumericText();
                    model.FaxNumber = model.FaxNumber.ExtractNumericText();
                    var entity = model.ToEntity();
                    _companyDepartmentService.Insert(entity);
                    model.Id = entity.Id;
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewCompanyDepartment", _localizationService.GetResource("ActivityLog.AddNewCompanyDepartment"), model.DepartmentName);
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyDepartment.Added"));
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region POST:/CompanyDetail/EditDepartment
        //_EditDepartment
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditDepartment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyDepartmentModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _companyDepartmentService.GetCompanyDepartmentById(model.Id);
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    model.PhoneNumber = model.PhoneNumber.ExtractNumericText();
                    model.FaxNumber = model.FaxNumber.ExtractNumericText();
                    model.ToEntity(entity);

                    _companyDepartmentService.Update(entity);
                    //activity log
                    _activityLogService.InsertActivityLog("UpdateCompanyDepartment", _localizationService.GetResource("ActivityLog.UpdateCompanyDepartment"), model.DepartmentName);
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyDepartment.Updated"));
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete Department
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _DeleteDepartment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyDepartmentModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _companyDepartmentService.GetCompanyDepartmentById(model.Id);
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    model.IsDeleted = true;
                    model.ToEntity(entity);

                    _companyDepartmentService.Update(entity);
                    _activityLogService.InsertActivityLog("UpdateCompanyLocation", _localizationService.GetResource("ActivityLog.UpdateCompanyLocation"), model.DepartmentName);
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyLocation.Updated"));
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion


        //CompanyJobOrderList

        #region GET :/CompanyDetails/_TabCompanyJobOrderList

        [HttpGet]
        public ActionResult _TabCompanyJobOrderList(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid; // for partial view to retrieve company job order
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company != null)
                ViewBag.CompanyId = company.Id;

            return PartialView();
        }

        #endregion

        #region POST:/CompanyDetails/_TabCompanyJobOrderList

        [HttpPost]
        public ActionResult _TabCompanyJobOrderList([DataSourceRequest] DataSourceRequest request, 
            Guid companyGuid, DateTime? startDate, DateTime? endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.MaxValue;
            var result = _bl.GetCompanyJobOrderList(request, companyGuid, start, end);

            return Json(result);
        }

        #endregion


        //CompanyPlacement

        #region GET :/CompanyDetails/_TabCompanyPlacement

        [HttpGet]
        public ActionResult _TabCompanyPlacement(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;

            return PartialView();
        }

        #endregion

        #region POST:/CompanyDetails/_TabCompanyPlacement

        [HttpPost]
        public ActionResult _GetPlacementSummary([DataSourceRequest] DataSourceRequest request, Guid companyGuid, DateTime refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            if (_workContext.CurrentAccount.IsRecruiterOrRecruiterSupervisor())
            {
                var Ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(_workContext.CurrentAccount.Id);
                if (Ids.Count == 0 || !Ids.Contains(companyId))
                    return AccessDeniedView();
            }

            var businessLogic = new CompanyPlacement_BL(_accountService, _candidateBalcklistService, _companyCandidateService, _jobOrderService, _companyContactService, _permissionService, _candidateJobOrderService, _workContext);
            var summary = businessLogic.GetCompanyPlacementSummary(_workContext.CurrentAccount, companyId, refDate);

            return Json(summary.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _SavePlacementSummary([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PlacementSummaryModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                var businessLogic = new CompanyPlacement_BL(_accountService, _candidateBalcklistService, _companyCandidateService, _jobOrderService, _companyContactService, _permissionService, _candidateJobOrderService, _workContext);
                foreach (var model in models)
                    if (model.Opening >= 0)
                        businessLogic.SavePlacementSummary(model);
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _GetPlacementDetails([DataSourceRequest] DataSourceRequest request, Guid companyGuid, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            if (_workContext.CurrentAccount.IsRecruiterOrRecruiterSupervisor())
            {
                var Ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(_workContext.CurrentAccount.Id);
                if (Ids.Count == 0 || !Ids.Contains(companyId))
                    return AccessDeniedView();
            }

            var businessLogic = new CompanyPlacement_BL(_accountService, _candidateBalcklistService, _companyCandidateService, _jobOrderService, _companyContactService, _permissionService, _candidateJobOrderService, _workContext);
            var details = businessLogic.GetCompanyPlacementDetails(_workContext.CurrentAccount, companyId, startDate, endDate);

            return Json(details.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _SavePlacementDetails([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PlacementDetailsModel> models)
        {
            bool result = true;
            string msg = null;

            if (models != null && ModelState.IsValid)
            {
                try
                {
                    var businessLogic = new CompanyPlacement_BL(_accountService, _candidateBalcklistService, _companyCandidateService, _jobOrderService, _companyContactService, _permissionService, _candidateJobOrderService, _workContext);
                    foreach (var model in models)
                    {
                        msg += businessLogic.SavePlacementDetails(model);
                    }
                    if (!String.IsNullOrEmpty(msg)) result = false;
                }
                catch (Exception ex)
                {
                    result = false;
                    msg += ex.Message;
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                msg += String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                result = false;
            }

            if (!result)
                ModelState.AddModelError("CurrentCandidateJobOrderId", msg);

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        // for display template
        public ActionResult GetJobOrdersByDate(DateTime refDate)
        {
            var jobOrders = _jobOrderService.GetJobOrdersByDateRangeAsQueryable(refDate, refDate, true);

            var jobOrderList = new List<SelectListItem>();
            foreach (var j in jobOrders)
                jobOrderList.Add(new SelectListItem() { Text = String.Concat(j.Id.ToString() , " (" , j.JobTitle , ")") ,
                                                        Value = j.Id.ToString() });

            return Json(jobOrderList, JsonRequestBehavior.AllowGet);
        }


        // for edit template
        public JsonResult GetActiveJobOrders(Guid companyGuid, string refDateStr)
        {
            var companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var refDate = new DateTime();
            var jobOrderList = new List<SelectListItem>();
            jobOrderList.Add(new SelectListItem() { Text = "--Select--", Value = "0" });

            if (companyId > 0 && DateTime.TryParse(refDateStr, out refDate))
            {
                var jobOrders = _jobOrderService.GetJobOrdersByAccountAndCompany(_workContext.CurrentAccount, companyId, refDate);
                foreach (var j in jobOrders)
                {
                    var item = new SelectListItem()
                    {
                        Text = String.Concat(j.Id.ToString() , " (" , j.JobTitle , ")" ),
                        Value = j.Id.ToString()
                    };
                    jobOrderList.Add(item);
                }
            }

            return Json(jobOrderList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JobOrderPipelinePlaced([DataSourceRequest] DataSourceRequest request, int jobOrderId, DateTime refDate)
        {
            var businessLogic = new CompanyPlacement_BL(_accountService, _candidateBalcklistService, _companyCandidateService, _jobOrderService, _companyContactService, _permissionService, _candidateJobOrderService, _workContext);
            var summary = businessLogic.JobOrderPipelinePlaced(jobOrderId, refDate);

            return Json(summary.ToDataSourceResult(request));
        }

        #endregion


        // DailyAttendanceList

        #region GET :/CompanyDetails/_TabDailyAttendanceList

        [HttpGet]
        public ActionResult _TabDailyAttendanceList(Guid companyGuid, DateTime? refDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;
            ViewBag.RefDate = refDate.HasValue ? refDate.Value : DateTime.Today;

            return PartialView();
        }

        #endregion

        #region POST:/CompanyDetails/_TabDailyAttendanceList

        [HttpPost]
        public ActionResult _TabDailyAttendanceList([DataSourceRequest] DataSourceRequest request, Guid companyGuid, DateTime? refDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            if (!refDate.HasValue) refDate = DateTime.Today;

            var businessLogic = new DailyAttendance_BL();
            var result = businessLogic.GetDailyAttendance(_candidateJobOrderService, _accountService, _companyContactService, _workTimeRepository, _workContext.CurrentAccount, companyId, refDate.Value);

            return Json(result.ToDataSourceResult(request));
        }

        [HttpGet]
        public ActionResult ExportDailyAttendantList(Guid companyGuid, DateTime refDate, string selectedIds)
        {
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                try
                {
                    int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
                    var businessLogic = new DailyAttendance_BL();
                    var ids = selectedIds.Split(',').Select(x => int.Parse(x)).ToArray();

                    return businessLogic.GetDailyAttendantList(_candidateService, _exportManager, _workContext.CurrentAccount, companyId, refDate, ids);
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc);
                    return RedirectToAction("Index");
                }
            }

            else
                errorMessage += "\r\n No candidate selected.";

            return Json(new { ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportAllDailyAttendantList(Guid companyGuid, DateTime refDate)
        {
            string errorMessage = string.Empty;

            try
            {
                int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
                var businessLogic = new DailyAttendance_BL();
                var result = businessLogic.GetDailyAttendance(_candidateJobOrderService, _accountService, _companyContactService, _workTimeRepository, _workContext.CurrentAccount, companyId, refDate);

                int[] candidateJobOrderids = result.ToList().Select(c => c.CandidateJobOrderId).ToArray();
                return businessLogic.GetDailyAttendantList(_candidateService, _exportManager, _workContext.CurrentAccount, companyId, refDate, candidateJobOrderids);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Index");
            }

        }

        #endregion



        // Billing Rates
        #region CreateNewBillingRate

        [HttpGet]
        public ActionResult CreateNewBillingRate(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
            CompanyBillingRateModel companyBillingRateModel = companyBillingRate_BL.CreateNewBillingRate(companyId, companyGuid);

            return View(companyBillingRateModel);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult CreateNewBillingRate(CompanyBillingRateModel companyBillingRateModel, IEnumerable<HttpPostedFileBase> moreQuotations, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                companyBillingRateModel.CompanyIsFiltered = true;
                companyBillingRateModel.FranchiseIsFiltered = !_workContext.CurrentFranchise.IsDefaultManagedServiceProvider;

                string errorMessage;
                CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);

                bool saved = companyBillingRate_BL.SaveCompanyBillingRate(companyBillingRateModel, out errorMessage, moreQuotations);
                if (!saved)
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        ErrorNotification(errorMessage);
                    else
                        return RedirectToAction("Index");
                }
                else
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyBillingRate.Added"));
                    return continueEditing ? RedirectToAction("EditBillingRate", new { id = companyBillingRateModel.Id }) : RedirectToAction("Details", "Company", new { guid = companyBillingRateModel.CompanyGuid, tabId = "tab-billing-rate" });
                }
            }

            return View(companyBillingRateModel);
        }

        #endregion


        #region EditBillingRate

        public ActionResult EditBillingRate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
            CompanyBillingRateModel model = companyBillingRate_BL.GetCompanyBillingRateById(id, true);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult EditBillingRate(CompanyBillingRateModel model, IEnumerable<HttpPostedFileBase> moreQuotations, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                string errorMessage;
                CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);

                bool saved = companyBillingRate_BL.SaveCompanyBillingRate(model, out errorMessage, moreQuotations);
                if (!saved)
                {
                    if (String.IsNullOrWhiteSpace(errorMessage))
                        return RedirectToAction("Index");
                    else
                        ErrorNotification(errorMessage);
                }
                else
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyBillingRate.Updated"));
                    if (!continueEditing)
                        return RedirectToAction("Details", "Company", new { guid = model.CompanyGuid, tabId = "tab-billing-rate" });
                }
            }

            return View(model);
        }

        #endregion


        #region _TabCompanyBillingRates

        [HttpGet]
        public ActionResult _TabCompanyBillingRates(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanyBillingRates) &&
                !_permissionService.Authorize(StandardPermissionProvider.ViewCompanyPayRates))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _TabCompanyBillingRates([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanyBillingRates) &&
                !_permissionService.Authorize(StandardPermissionProvider.ViewCompanyPayRates))
                return AccessDeniedView();

            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
            DataSourceResult result = companyBillingRate_BL.GetAllCompanyBillingRates(request, companyId);

            return Json(result);
        }

        #endregion


        #region CopyBillingRate

        public ActionResult CopyBillingRate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            var companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
            var model = companyBillingRate_BL.CopyBillingRate(id, true);

            if (model == null)
                return RedirectToAction("Index");

            ViewBag.OrigBillingRateId = id;

            return View(model);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult CopyBillingRate(CompanyBillingRateModel companyBillingRateModel, IEnumerable<HttpPostedFileBase> moreQuotations, bool continueEditing, int OrigBillingRateId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            ViewBag.OrigBillingRateId = OrigBillingRateId;

            if (ModelState.IsValid)
            {
                string errorMessage;

                var companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
                bool copied = companyBillingRate_BL.SaveCopiedBillingRate(companyBillingRateModel, OrigBillingRateId, out errorMessage, moreQuotations);

                if (!copied)
                {
                    if (errorMessage == null)
                        return RedirectToAction("Index");
                    else
                    {
                        ErrorNotification(errorMessage);
                        return View(companyBillingRateModel);
                    }
                }
                else
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Companies.CompanyBillingRate.Added"));
                    return continueEditing ? RedirectToAction("EditBillingRate", new { id = companyBillingRateModel.Id }) : RedirectToAction("Details", "Company", new { guid = companyBillingRateModel.CompanyGuid, tabId = "tab-billing-rate" });
                }
            }

            return View(companyBillingRateModel);
        }

        #endregion


        #region Quotation

        [HttpPost]
        public ActionResult _Quotations(int billingRateId, bool? quotationDeletable)
        {
            var model = _quotationService.GetAllQuotationsByBillingRateId(billingRateId).ToList().Select(x => x.ToModel());

            if (quotationDeletable.HasValue)
                ViewData["quotationDeletable"] = quotationDeletable.Value;

            return PartialView("_Quotations", model);
        }


        [HttpPost]
        public ActionResult _DeleteQuotation(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            var quotation = _quotationService.GetQuotationById(id);
            if (quotation == null)
                return Content("The quotation with Id [{0}] does not exist.");

            _quotationService.DeleteQuotation(quotation);

            return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadQuotation(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            var quotation = _quotationService.GetQuotationById(id);
            if (quotation == null)
                return Content("The quotation with Id [{0}] does not exist.");

            return File(quotation.Stream, System.Net.Mime.MediaTypeNames.Application.Octet, quotation.FileName);
        }

        #endregion


        #region  Export CompanyBillingRate

        [HttpPost]
        public ActionResult ExportCompanyBillingRateToXlsx(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            try
            {
                string fileName;
                CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
                byte[] file = companyBillingRate_BL.ExportSelectedCompanyBillingRates(selectedIds, out fileName);
                if (file == null)
                {
                    ErrorNotification("Please select at least one company billing rate!");
                    return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
                }
                else
                    return File(file, "text/xls", fileName);
            }
            catch (Exception exc)
            {
                _logger.Error("ExportSelectedCompanyBillingRates():", exc, userAgent: Request.UserAgent);
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }


        [HttpPost]
        public ActionResult ExportCompanyBillingRateToPDF(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();

            try
            {
                string fileName;
                CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_workContext, _companyBillingService, _activityLogService, _localizationService, _logger, _exportManager);
                byte[] file = companyBillingRate_BL.ExportCompanyBillingRatesToPDF(selectedIds, _pdfService, out fileName);
                if (file == null)
                {
                    ErrorNotification("Please select at least one company billing rate!");
                    return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
                }
                else
                    return File(file, "application/pdf", "CompanyBillingRates.pdf");
            }
            catch (Exception exc)
            {
                _logger.Error("ExportCompanyBillingRatesToPDF():", exc, userAgent: Request.UserAgent);
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }

        #endregion


        #region Billing Rates Audit

        public ActionResult _BillingRatesAudit()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult _BillingRatesAudit([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BillingRatesAuditLogReport))
                return AccessDeniedView();

            var result = _companyBillingService.GetBillingRatesUpdated(startDate, endDate).ProjectTo<CompanyBillingRateModel>();

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        //CompanyContact
        #region GET :/CompanyDetails/_TabCompanyContactList
        [HttpGet]
        public ActionResult _TabCompanyContactList(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewContacts))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;
            ViewBag.CompanyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            //ViewBag.FranchiseId = _franchiseService.GetDefaultMSPId();
            return View();
        }
        #endregion

        #region POST:/CompanyDetails/_TabCompanyContactList
        [HttpPost]
        public ActionResult _TabCompanyContactList([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewContacts))
                return AccessDeniedView();

            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
                return AccessDeniedView();

            int companyId = company.Id;
            var companyContacts = _companyContactService.GetCompanyContactsByCompanyId(companyId);
            var clientAdmins = _companyContactService.GetCompanyClientAdminsByCompanyId(companyId);
            companyContacts = companyContacts.Union(clientAdmins).ToList();

            List<AccountModel> companyContactList = new List<AccountModel>();

            foreach (var item in companyContacts)
            {
                AccountModel i = MappingExtensions.ToModel(item);

                AccountRole ar = item.AccountRoles.FirstOrDefault();
                i.AccountRoleSystemName = ar == null ? "" : ar.AccountRoleName;

                CompanyLocation compLoc = _companyDivisionService.GetCompanyLocationById(i.CompanyLocationId);
                i.CompanyLocationName = compLoc == null ? "" : compLoc.LocationName;

                CompanyDepartment compDep = _companyDepartmentService.GetCompanyDepartmentById(i.CompanyDepartmentId);
                i.CompanyDepartmentName = compDep == null ? "" : compDep.DepartmentName;

                i.ShiftName = item.Shift == null ? "" : item.Shift.ShiftName;
                companyContactList.Add(i);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = companyContactList, // Process data (paging and sorting applied)
                Total = companyContacts.Count // Total number of records
            };

            return Json(result);
        }
        #endregion

        #region POST:/CompanyDetails/CreateContact

        public ActionResult _CreateCompanyContact(Guid? companyGuid)
        {
            var company = _companyService.GetCompanyByGuid(companyGuid);
            AccountFullModel model = new AccountFullModel();
            if (company == null)
            {
                ErrorNotification("The company does not exist!");
                return View(model);
            }
            model.CompanyId = company.Id;
            model.FranchiseId = _franchiseService.GetDefaultMSPId();
            model.Username = "(default to email)";
            ViewBag.CompanyGuid = companyGuid;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateContact(AccountFullModel accountModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContacts))
                return AccessDeniedView();

            ModelState["CompanyDepartmentId"].Errors.Clear(); // Do not throw an error if Department is not defined

            StringBuilder error = new StringBuilder();
            bool valid = _accountPasswordPolicyService.ApplyPasswordPolicy(0, "Client", accountModel.Password, String.Empty, Core.Domain.Accounts.PasswordFormat.Clear, String.Empty, out error);
            if (!valid)
                ModelState.AddModelError("Password", error.ToString());

            if (ModelState.IsValid)
            {
                accountModel.Username = accountModel.Email.Trim(); // IMPORTANT: Client User Name = Email address
                
                Account account = accountModel.ToEntity();

                // Prepare account information
                var _BL = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                _BL.prepareAccountForRegistration(account, accountModel.AccountRoleSystemName);

                account.FranchiseId = _franchiseService.GetDefaultMSPId();
                account.IsLimitedToFranchises =  accountModel.AccountRoleSystemName == AccountRoleSystemNames.ClientAdministrators;
                account.IsClientAccount = accountModel.AccountRoleSystemName != AccountRoleSystemNames.ClientAdministrators;
                account.IsSystemAccount = false;
                account.SystemName = null;
                account.IsDeleted = false;

                string registerationError = _accountService.RegisterAccount(account);
                if (registerationError == null) // there was no error in registration
                {
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewContact", _localizationService.GetResource("ActivityLog.AddNewContact"), accountModel.Username);
                    return Json(new { Message = _localizationService.GetResource("Admin.Accounts.Account.Added"), Error = false });
                }
                else
                {
                    _logger.Warning(string.Format("Create new contact failed: --- ERRORS --- {0}.", registerationError), account: _workContext.CurrentAccount, userAgent: Request.UserAgent);
                    //ErrorNotification(_localizationService.GetResource("Admin.Accounts.Account.Added.Failed"));
                    return Json(new { Message = registerationError, Error = true });
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                string errMsg = String.Join(" ", errors.Select(o => o.ToString()).ToArray());
                _logger.Warning(string.Format("Model state is invalid: --- ERRORS --- {0}.", errMsg), account: _workContext.CurrentAccount, userAgent: Request.UserAgent);
                return Json(new { Message = errMsg, Error = true });
            }
        }

        #endregion

        #region GET :/CompanyDetails/EditContact

        public ActionResult EditContact(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContacts))
                return AccessDeniedView();

            Account account = _accountService.GetAccountByGuid(guid);

            if (account == null)
                return RedirectToAction("Index");

            AccountModel accountModel = account.ToModel();
            accountModel.AccountRoleSystemName = account.AccountRoles.FirstOrDefault() == null ? "" : account.AccountRoles.FirstOrDefault().SystemName;
            var company = _companyService.GetCompanyById(account.CompanyId);
            accountModel.CompanyName = company == null ? "" : company.CompanyName;
            ViewBag.CompanyGuid = company.CompanyGuid;

            return View(accountModel);
        }

        #endregion

        #region POST:/CompanyDetails/EditContact

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult EditContact(AccountModel accountModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContacts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                // rewrite these properties to avoid any overwrites from client 
                accountModel.IsClientAccount = true;
                accountModel.IsSystemAccount = false;

                string errorMsg;
                Wfm.Shared.Models.Accounts.Account_BL bl = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);

                if (bl.EditCompanyContactProfile(accountModel, out errorMsg))
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Accounts.Account.Updated"));
                    return continueEditing ? RedirectToAction("EditContact", new { guid = accountModel.AccountGuid }) : RedirectToAction("CompanyContactDetails", new { guid = accountModel.AccountGuid });
                }
                else
                {
                    if (String.IsNullOrEmpty(errorMsg))
                        ErrorNotification(errorMsg);

                    return View(accountModel);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                var exceps = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                _logger.Warning(string.Format("Error occurred during updating account to database: {0} - {1}", errors, exceps), userAgent: Request.UserAgent);
            }

            return View(accountModel);
        }
        #endregion //end at edit post

        #region GET :/CompanyDetails/CompanyContactDetails

        public ActionResult CompanyContactDetails(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContacts))
                return AccessDeniedView();

            Account account = _accountService.GetAccountByGuid(guid);

            if (account == null)
                return RedirectToAction("Index");

            AccountModel model = account.ToModel();

            model.AccountRoleSystemName = account.AccountRoles.FirstOrDefault() == null ? "" : account.AccountRoles.FirstOrDefault().AccountRoleName;
            model.ShiftName = account.Shift == null ? "" : account.Shift.ShiftName;

            var company = _companyService.GetCompanyById(account.CompanyId);
            model.CompanyName = company == null ? "" : company.CompanyName;

            var department = _companyDepartmentService.GetCompanyDepartmentById(account.CompanyDepartmentId);
            model.CompanyDepartmentName = department == null ? "" : department.DepartmentName;

            var location = _companyDivisionService.GetCompanyLocationById(account.CompanyLocationId);
            model.CompanyLocationName = location == null ? "" : location.LocationName;


            ViewBag.CompanyGuid = _companyService.GetCompanyById(model.CompanyId).CompanyGuid;

            return View(model);
        }

        #endregion

        #region GET :/CompanyDetails/ResetPassword

        [HttpGet]
        public ActionResult ResetPassword(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContacts))
                return AccessDeniedView();

            Account account = _accountService.GetAccountByGuid(guid);
            if (account == null)
                return RedirectToAction("Index", "CompanyContact");

            AccountResetPasswordModel_BL model_BL = new AccountResetPasswordModel_BL(_accountPasswordPolicyService, _accountService);
            var model = model_BL.GetResetPasswordModel(account);
            return PartialView(model);
        }

        #endregion

        #region POST:/CompanyDetails/ResetPassword

        [HttpPost]
        public ActionResult _ResetPassword(AccountResetPasswordModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContacts))
                return AccessDeniedView();

            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                Wfm.Shared.Models.Accounts.Account_BL bl = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                errorMessage = bl.ResetPassword(model, false);

                if (String.IsNullOrWhiteSpace(errorMessage))
                {
                    return Json(new { Error = false, Message = _localizationService.GetResource("Common.PasswordHasBeenChanged") });
                }
                else
                {
                    _logger.Warning(errorMessage, userAgent: Request.UserAgent);
                    return Json(new { Error = true, Message = errorMessage });
                }
            }
            else
            {
                var errors = String.Join("\n", ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage)));
                var exceps = String.Join("\n", ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception)));
                errorMessage = string.Format("Error occurred during updating account to database:\n {0} \n {1}", errors, exceps);
                _logger.Warning(errorMessage, userAgent: Request.UserAgent);
                return Json(new { Error = true, Message = errors });
            }
        }

        #endregion


        //CompanyOvertimeRule

        #region GET :/CompanyDetails/_TabCompanyOvertimeRule

        [HttpGet]
        public ActionResult _TabCompanyOvertimeRule(Guid? companyGuid)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
            //    return AccessDeniedView();
            ViewBag.CompanyGuid = companyGuid;
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
            {
                ErrorNotification("Cannot load data!");
                return new EmptyResult();
            }
            ViewBag.CompanyId = company.Id;
            return PartialView();
        }

        #endregion


        #region POST:/CompanyDetails/_TabCompanyOvertimeRule

        [HttpPost]
        public ActionResult _TabCompanyOvertimeRule([DataSourceRequest] DataSourceRequest request, Guid? companyGuid)
        {

            var result = _companyOvertimeRuleService.GetAllCompanyOvertimeRuleByCompanyGuid(companyGuid);
            return Json(result.ToDataSourceResult(request, x => x.ToModel()));
        }

        #endregion

        private bool _hasConflictingActiveRule(CompanyOvertimeRuleModel model)
        {
            bool result = false;

            // allow any inactive rule
            if (!model.IsActive)
                return result;

            var newRule = _overtimeRuleSettingService.GetOvertimeRuleSettingById(model.OvertimeRuleSettingId);
            // other existing rules
            var existingRules = _companyOvertimeRuleService.GetAllCompanyOvertimeRules()
                                .Where(x => x.Id != model.Id && x.CompanyId == model.CompanyId && x.IsActive && x.OvertimeRuleSetting.TypeId == newRule.TypeId)
                                .OrderBy(x => x.OvertimeRuleSetting.ApplyAfter)
                                .ToList();
            if (existingRules != null)
                foreach (var rule in existingRules)
                    if (newRule.Rate == rule.OvertimeRuleSetting.Rate ||
                            (newRule.Rate < rule.OvertimeRuleSetting.Rate && newRule.ApplyAfter > rule.OvertimeRuleSetting.ApplyAfter) ||
                            (newRule.Rate > rule.OvertimeRuleSetting.Rate && newRule.ApplyAfter < rule.OvertimeRuleSetting.ApplyAfter))
                    {
                        result = true;
                        break;
                    }

            return result;
        }




        //CompanyCandidate

        #region GET :/CompanyDetails/_TabCompanyCandidateList
        [HttpGet]
        public ActionResult _TabCompanyCandidateList(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;

            return View();
        }
        #endregion


        #region POST:/CompanyDetails/_TabCompanyCandidateList

        [HttpPost]
        public JsonResult _TabCompanyCandidateList([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var companyCandidate_BL = new CompanyCandidateModel_BL();
            var result = companyCandidate_BL.GetCompanyCandidateList(_workContext.CurrentAccount, companyId, _companyCandidateService, _candidateBalcklistService)
                .ProjectTo<CompanyCandidateViewModel>();

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region POST :/CompanyDetails/ImportCandidate

        [HttpPost]
        public ActionResult ImportCandidate(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            //only admin company can import time sheet
            if (!_workContext.CurrentAccount.Franchise.IsDefaultManagedServiceProvider)
                return AccessDeniedView();

            try
            {
                IList<string> errors;
                int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    errors = _importManager.ImportCandidateFromXlsx(file.InputStream, companyId);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                    return RedirectToAction("Datails", "Company", new { guid = companyGuid });
                }

                //activity log
                _activityLogService.InsertActivityLog("ImportCandidate", _localizationService.GetResource("ActivityLog.ImportCandidate"), file.FileName + " : " + String.Join(", ", errors));

                // Display error message if any
                if (errors.Count > 0)
                    foreach (var error in errors) ErrorNotification(error);
                else
                    SuccessNotification(_localizationService.GetResource("Admin.Company.ImportCandidate.Imported"));

                return RedirectToAction("Details", "Company", new { guid = companyGuid, tabId = "tab-candidate" });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Details", "Company", new { guid = companyGuid, tabId = "tab-candidate" });
            }

        }

        #endregion

        #region GET :/CompanyDetails/_SearchCandidate

        public ActionResult _SearchCandidate(Guid companyGuid)
        {
            ViewBag.CompanyGuid = companyGuid;
            return View();
        }

        #endregion

        #region POST:/CompanyDetails/_SearchCandidate

        [HttpPost]
        public ActionResult _SearchCandidate([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            var companyCandidate_BL = new CompanyCandidate_BL();
            var result = companyCandidate_BL.GetAllCandidatesForCompanyPool(_workContext, _companyService, _companyCandidateService, _candidateBalcklistService, request, companyGuid);

            return Json(result);
        }

        #endregion

        #region GET :/CompanyDetails/_AddCandidateToPool

        public ActionResult _AddCandidateToPool(Guid companyGuid)
        {
            ViewBag.CompanyGuid = companyGuid;
            return PartialView("_AddCandidateToPool");
        }

        #endregion

        #region POST:/CompanyDetails/AddCandidateToPool

        [HttpPost]
        public ActionResult AddCandidateToPool(Guid companyGuid, string selectedIds, DateTime startDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            List<int> ids = new List<int>();
            if (selectedIds != null)
            {
                ids = selectedIds
                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => Convert.ToInt32(x))
                                .ToList();
            }

            // Validate selected items
            if (ids.Count() == 0)
            {
                ErrorNotification("Please select one or more items");
                return RedirectToAction("Details", new { guid = companyGuid, tabId = "tab-candidate" });
            }

            // for log
            List<string> result = new List<string>();

            foreach (var candidateId in ids)
            {
                if (_companyCandidateService.GetCompanyCandidatesByCompanyIdAndCandidateId(companyId, candidateId, startDate, true).LastOrDefault() == null)
                {
                    CompanyCandidate companyCandidate = new CompanyCandidate()
                    {
                        CompanyId = companyId,
                        CandidateId = candidateId,
                        StartDate = startDate,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    };

                    _companyCandidateService.InsertCompanyCandidate(companyCandidate);

                    //log what has been changed
                    result.Add(candidateId.ToString() + "/" + companyId.ToString());

                    SuccessNotification(_localizationService.GetResource("Admin.Companies.Company.Candidate.Added.Successful") + " : " + candidateId.ToString());
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Companies.Company.Candidate.Added.Duplicated") + " : " + candidateId.ToString());
                }

            }

            //activity log
            _activityLogService.InsertActivityLog("AddCandidateToCompanyPool", _localizationService.GetResource("ActivityLog.AddCandidateToCompanyPool"), String.Join(", ", result));

            return RedirectToAction("Details", new { guid = companyGuid, tabId = "tab-candidate" });
        }

        #endregion

        #region GET:/Company/RemoveCandidateFromPool

        public ActionResult RemoveCandidateFromPool(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var model = _companyCandidateService.GetCompanyCandidateById(Id).ToModel();
            if (model.StartDate.ToString() == DateTime.MaxValue.ToString())
                model.StartDate = null;

            return PartialView("_RemoveCompanyCandidate", model);
        }

        #endregion

        #region POST:/Company/RemoveCandidateFromPool

        [HttpPost]
        public JsonResult RemoveCandidateFromPool(CompanyCandidateModel companyCandidateModel)
        {
            string errorMessage = string.Empty;
            bool result = true;

            if (ModelState.IsValid)
            {
                var companyCandidate = _companyCandidateService.GetCompanyCandidateById(companyCandidateModel.Id);

                if (companyCandidateModel.EndDate == null)
                {
                    errorMessage += "Both 'End Date' and 'Reason for Leave' are required.";
                    result = false;
                }
                else
                {
                    companyCandidate.EndDate = companyCandidateModel.EndDate;
                    companyCandidate.ReasonForLeave = companyCandidateModel.ReasonForLeave;
                    companyCandidate.Note = companyCandidateModel.Note;
                    companyCandidate.UpdatedOnUtc = DateTime.UtcNow;

                    _companyCandidateService.UpdateCompanyCandidate(companyCandidate);
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage += String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                result = false;
            }

            if (result)
                _activityLogService.InsertActivityLog("RemoveFromPool", "Remove candidate from pool: " + companyCandidateModel.CandidateId);

            return Json(new { Result = result, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GET:/Company/EditCompanyCandidate

        public ActionResult EditCompanyCandidate(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var model = _companyCandidateService.GetCompanyCandidateById(Id).ToModel();
            if (model.StartDate.ToString() == DateTime.MaxValue.ToString())
                model.StartDate = null;

            return PartialView("_EditCompanyCandidate", model);
        }

        #endregion

        #region POST:/Company/EditCompanyCandidate

        [HttpPost]
        public JsonResult EditCompanyCandidate(CompanyCandidateModel companyCandidateModel)
        {
            string errorMessage = string.Empty;
            bool result = true;

            if (ModelState.IsValid)
            {
                var companyCandidate = _companyCandidateService.GetCompanyCandidateById(companyCandidateModel.Id);

                if (companyCandidateModel.StartDate != null)
                    companyCandidate.StartDate = (DateTime)companyCandidateModel.StartDate;
                if (companyCandidateModel.Position != null)
                    companyCandidate.Position = companyCandidateModel.Position;
                if (companyCandidateModel.Note != null)
                    companyCandidate.Note = companyCandidateModel.Note;
                companyCandidate.UpdatedOnUtc = DateTime.UtcNow;

                _companyCandidateService.UpdateCompanyCandidate(companyCandidate);
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage += String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                result = false;
            }

            if (result)
                _activityLogService.InsertActivityLog("EditCompanyCandidate", "Update candidate from pool: " + companyCandidateModel.CandidateId);

            return Json(new { Result = result, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region POST:/CompanyDetails/UpdateCompanyCandidateRatingValue

        [HttpPost]
        public ActionResult UpdateCompanyCandidateRatingValue(int companyCandidateId, decimal rating)
        {
            _companyCandidateService.UpdateCompanyCandidateRatingValue(companyCandidateId, rating);
            return new EmptyResult();
        }

        #endregion


        #region GET :/CompanyDetails/ExportCandidate

        public ActionResult ExportCandidate(Guid companyGuid)
        {
            string errorMessage = string.Empty;

            try
            {
                int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
                var businessLogic = new CompanyCandidateModel_BL();
                return businessLogic.GetCompanyCandidateList(_companyCandidateService, _exportManager, _workContext.CurrentAccount, companyId);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Details", new { guid = companyGuid, tabId = "tab-candidate" });
            }

        }

        #endregion


        // Company Vendor

        #region _TabCompanyVendors

        public ActionResult _TabCompanyVendors(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
            {
                ErrorNotification("Unable to load data!");
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = company.Id;
            ViewBag.CompanyGuid = companyGuid;
            //ViewBag.Vendors = _companyVendorService.GetAllCompanyVendorsByCompanyIdAsSelectList(company.Id);

            return View();
        }


        [HttpPost]
        public ActionResult _CompanyVendorList([DataSourceRequest] DataSourceRequest request, Guid? companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendors = _companyVendorService.GetAllCompanyVendorsByCompanyGuid(companyGuid, activeOnly: false);

            return Json(vendors.ToDataSourceResult(request, x => x.ToModel()));
        }


        [HttpPost]
        public ActionResult _AddCompanyVendor([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyVendorModel> models)
        {
            var results = new List<CompanyVendorModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var anyDuplicate = _companyVendorService.GetCompanyVendorByComanyAndVendor(model.CompanyId, model.VendorId);
                    if (anyDuplicate != null)
                    {
                        ModelState.AddModelError("FranchiseId", String.Format("The vendor [{0}] is duplicated!", anyDuplicate.Vendor.FranchiseName));
                        continue;
                    }

                    var entity = model.ToEntity();
                    entity.CreatedOnUtc = DateTime.UtcNow;
                    entity.UpdatedOnUtc = entity.CreatedOnUtc;
                    _companyVendorService.InsertCompanyVendor(entity);

                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditCompanyVendor([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyVendorModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var anyDuplicate = _companyVendorService.GetCompanyVendorByComanyAndVendor(model.CompanyId, model.VendorId);
                    if (anyDuplicate != null && anyDuplicate.Id != model.Id)
                    {
                        ModelState.AddModelError("FranchiseId", String.Format("The vendor [{0}] is duplicated!", anyDuplicate.Vendor.FranchiseName));
                        continue;
                    }

                    var entity = _companyVendorService.GetCompanyVendorById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _companyVendorService.UpdateCompanyVendor(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _RemoveCompanyVendor([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyVendorModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _companyVendorService.GetCompanyVendorById(model.Id);
                    _companyVendorService.DeleteCompanyVendor(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        // Company Feature

        #region _TabFeatures

        public ActionResult _TabFeatures(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
            {
                _logger.Error("_FeatureList():Unable to load data!");
                return AccessDeniedView();
            }
            ViewBag.CompanyId = company.Id;
            ViewBag.CompanyGuid = companyGuid;

            return View();
        }


        [HttpPost]
        public ActionResult _FeatureList([DataSourceRequest] DataSourceRequest request, Guid? companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
            {
                _logger.Error("_FeatureList():Unable to load data!");
                return Json(null);
            }
            var features = _userFeatureService.GetAllUserFeatures().Where(x => x.UserId == company.Id);
            var result = features.ProjectTo<UserFeatureModel>();

            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _EditFeature([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyVendorModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _userFeatureService.GetUserFeatureById(model.Id);
                    if (entity.IsActive != model.IsActive)
                    {
                        entity.IsActive = model.IsActive;
                        entity.UpdatedOnUtc = DateTime.UtcNow;
                        _userFeatureService.UpdateUserFeature(entity);
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        // Notification settings

        #region _TabNotifications

        public ActionResult _TabNotifications(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();

            var clientNotificationMapping_BL = new ClientNotificationMapping_BL(_clientNotificationService);
            var companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var model = clientNotificationMapping_BL.GetClientNotificationMappingByCompany(companyId);
            ViewBag.CompanyId = companyId;

            return View(model);
        }


        [HttpPost]
        public ActionResult _GetNotificationRoleMaps(int companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();

            var clientNotificationMapping_BL = new ClientNotificationMapping_BL(_clientNotificationService);
            var maps = clientNotificationMapping_BL.GetNotificationRoleMaps(companyId);
            var result = new DataSourceResult()
            {
                Data = maps,
                Total = maps.Count
            };

            return Json(result);
        }


        [HttpPost]
        public ActionResult _SaveNotificationMapping(int companyId, string notificationNames, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();

            try
            {
                var clientNotificationMapping_BL = new ClientNotificationMapping_BL(_clientNotificationService);
                clientNotificationMapping_BL.SaveClientNotificationMapping(companyId, notificationNames, form);

                SuccessNotification(_localizationService.GetResource("Admin.ClientNotification.Updated"));
            }
            catch (Exception ex)
            {
                ErrorNotification(_localizationService.GetResource("Admin.ClientNotification.UpdateFailed"));
                _logger.Error(String.Format("Error updating client notifications by user {0} ", _workContext.CurrentAccount.Id), ex, userAgent: Request.UserAgent);
            }
            var companyGuid = _companyService.GetCompanyById(companyId).CompanyGuid;

            return RedirectToAction("Details", new { guid = companyGuid, tabid = "tab-notifications" });
        }

        #endregion


        // JsonResult


        #region // JsonResult: //GetCompanyLocations

        public JsonResult GetCompanyLocations(Guid companyGuid)
        {
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var locations = _companyDivisionService.GetAllCompanyLocationsByCompanyId(companyId);
            var locationDropDownList = new List<SelectListItem>();

            var no_location = new SelectListItem()
            {
                Text = "None",
                Value = "0"
            };

            locationDropDownList.Add(no_location);
            foreach (var c in locations)
            {
                var item = new SelectListItem()
               {
                   Text = c.LocationName,
                   Value = c.Id.ToString()
               };
                locationDropDownList.Add(item);
            }
            return Json(locationDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: //GetCompanyDepartments

        public JsonResult GetCompanyDepartments(string companyLocationId)
        {
            var departments = _companyDepartmentService.GetAllCompanyDepartmentByLocationId(Convert.ToInt32(companyLocationId));
            var departmentDropDownList = new List<SelectListItem>();

            var no_department = new SelectListItem()
                {
                    Text = "None",
                    Value = "0"
                };

            departmentDropDownList.Add(no_department);
            foreach (var c in departments)
            {
                var item = new SelectListItem()
                {
                    Text = c.DepartmentName,
                    Value = c.Id.ToString()
                };
                departmentDropDownList.Add(item);
            }
            return Json(departmentDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region _TabRecruiters
        [HttpGet]
        public ActionResult _TabRecruiters(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecruiters))
                return AccessDeniedView();
            ViewBag.CompanyGuid = companyGuid;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabRecruiters([DataSourceRequest] DataSourceRequest request, Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecruiters))
                return AccessDeniedView();
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            RecruiterCompanyModel_BL recruiterCompany_BL = new RecruiterCompanyModel_BL();
            DataSourceResult result = recruiterCompany_BL.GetRecruiterCompanyModelByCompanyId(_workContext.CurrentAccount, companyId, _recruiterCompanyService);
            return Json(result);
        }

        #endregion

        #region _SelectRecruiters
        [HttpGet]
        public ActionResult _SelectRecruiters(Guid companyGuid)
        {
            ViewBag.CompanyGuid = companyGuid;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _SelectRecruiters(Guid companyGuid, int recruiterId)
        {
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            RecruiterCompanyModel_BL recruiterCompany_BL = new RecruiterCompanyModel_BL();
            recruiterCompany_BL.AddNewRecruiterToCompany(companyId, recruiterId, _recruiterCompanyService);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _DeleteRecruiters
        [HttpPost]
        public ActionResult _DeleteRecruiters(string ids)
        {
            RecruiterCompanyModel_BL recruiterCompany_BL = new RecruiterCompanyModel_BL();
            recruiterCompany_BL.DeleteRecruiterCompany(ids, _recruiterCompanyService);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _TabCompanyActivities
        public ActionResult _TabCompanyActivities(Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
            {
                ErrorNotification("Cannot Load data!");
                return RedirectToAction("Details", new { guid = guid });
            }
            ViewBag.CompanyId = company.Id;
            ViewBag.CompanyGuid = company.CompanyGuid;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _CompanyActivityList(DataSourceRequest request, Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
            {
                ErrorNotification("Cannot Load data!");
                return RedirectToAction("Details", new { guid = guid });
            }
            var activities = company.CompanyActivities.ToDataSourceResult(request, x => x.ToModel());
            return Json(activities);
        }

        [HttpPost]
        public ActionResult _AddCompanyActivity([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyActivityModel> models)
        {
            var results = new List<CompanyActivityModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _companyActivityService.Create(model.ToEntity());
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditCompanyActivity([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyActivityModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _companyActivityService.Update(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _RemoveCompanyActivity([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyActivityModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _companyActivityService.Delete(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Wizard Company Create
        public ActionResult NewCompany(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            CompanyBasicInformation model = new CompanyBasicInformation();
            //create
            if (guid == null || guid == Guid.Empty)
            {
                ViewBag.CreateOrEdit = _localizationService.GetLocaleStringResourceByName("Admin.Company.AddNew").ResourceValue;
                model.IsActive = true;

                var status = _companyStatusService.GetCompanyStatusByName("Active");
                if (status != null)
                    model.CompanyStatusId = status.Id;

                var intervalId = _invoiceIntervalService.GetInvoiceIntervalIdByCode("WEEKLY");
                if (intervalId > 0) model.InvoiceIntervalId = intervalId;
            }
            //Edit
            else
            {
                ViewBag.CreateOrEdit = _localizationService.GetLocaleStringResourceByName("Admin.Companies.Company.EditCompanyDetails").ResourceValue;
                var company = _companyService.GetCompanyByGuid(guid);
                if (company == null)
                {
                    ErrorNotification("Cannot load company data!");
                    return RedirectToAction("Index");
                }
                model = company.ToBasicInformationModel();
            }
            ViewBag.NewPage = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateBasicInformation(CompanyBasicInformation model)
        {
            int companyId = 0;
            Guid guid = _bl.CreateOrUpdateCompany(model, _workContext.CurrentAccount.Id, out companyId);

            return Json(new { guid = guid, companyId = companyId });
        }

        [HttpGet]
        public ActionResult _CreateOrUpdateCompanyLocation()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }

        public ActionResult _CreateOrUpdateCompanyDepartment()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }
        public ActionResult _CreateOrUpdateCompanyVendor()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }
        public ActionResult _CreateOrUpdateCompanyRecruiter()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }
        public ActionResult _CreateOrUpdateCompanyFeature()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }
        public ActionResult _CreateOrUpdateCompanyOvertimeRules()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }
        public ActionResult _CreateOrUpdateCompanyPosition()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }

        public ActionResult _CreateOrUpdateCompanyContact()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();
            return PartialView();
        }
        #endregion

        #region Company Recruiter - Batch Create/Edit

        [HttpPost]
        public ActionResult _CompanyRecruiterList(DataSourceRequest request, Guid? companyGuid)
        {
            var result = _recruiterCompanyService.GetAllRecruitersByCompanyGuid(companyGuid);
            return Json(result.ToDataSourceResult(request, x => x.ToSimpleModel()));
        }

        [HttpPost]
        public ActionResult _AddCompanyRecruiter([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RecruiterCompanySimpleModel> models)
        {
            var results = new List<RecruiterCompanySimpleModel>();
            
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {

                    var entity = model.ToEntity();
                    entity.CreatedOnUtc = DateTime.UtcNow;
                    entity.UpdatedOnUtc = entity.CreatedOnUtc;
                    _recruiterCompanyService.InsertCompanyRecruiter(entity);

                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditCompanyRecruiter([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RecruiterCompanySimpleModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _recruiterCompanyService.GetRecruiterCompanyById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _recruiterCompanyService.UpdateCompanyRecruiter(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _RemoveCompanyRecruiter([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RecruiterCompanySimpleModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _recruiterCompanyService.GetRecruiterCompanyById(model.Id);
                    _recruiterCompanyService.DeleteCompanyRecruiter(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Company Overtime Rules Batch Create
        [HttpPost]
        public ActionResult _AddOvertimeRule([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyOvertimeRuleModel> models)
        {
            var results = new List<CompanyOvertimeRuleModel>();
            StringBuilder error = new StringBuilder();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    if (!_hasConflictingActiveRule(model))
                    {
                        var entity = model.ToEntity();
                        _companyOvertimeRuleService.Create(entity);
                        //activity log
                        _activityLogService.InsertActivityLog("AddNewCompanyOvertimeRule", _localizationService.GetResource("ActivityLog.AddNewCompanyOvertimeRule"), model.Id);
                        results.Add(model);
                    }
                    else
                        error.AppendLine(String.Format("Rule {0} is overlapping some other rules and create conflicting settings. Please double check your input or deactivate the existing rules before entering the new rule.", model.Description));

                }
            }
            if (error.Length > 0)
                ModelState.AddModelError("OvertimeRuleSettingId", error.ToString());

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult _EditOvertimeRule([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyOvertimeRuleModel> models)
        {
            StringBuilder error = new StringBuilder();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    if (!_hasConflictingActiveRule(model))
                    {
                        var entity = _companyOvertimeRuleService.Retrieve(model.Id);
                        model.ToEntity(entity);

                        _companyOvertimeRuleService.Update(entity);
                    }
                    else
                        error.AppendLine(String.Format("Rule {0} is overlapping some other rules and create conflicting settings. Please double check your input or deactivate the existing rules before entering the new rule.", model.Description));

                }
            }
            if (error.Length > 0)
                ModelState.AddModelError("OvertimeRuleSettingId", error.ToString());
            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult _DeleteOvertimeRule([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyOvertimeRuleModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _companyOvertimeRuleService.Retrieve(model.Id);
                    _companyOvertimeRuleService.Delete(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Company Contact - Batch Create/Edit
        [HttpPost]
        public ActionResult _CompanyContactList(DataSourceRequest request, Guid? companyGuid)
        {
            var company = _companyService.GetCompanyByGuid(companyGuid);
            if (company == null)
            {
                _logger.Error("_CompanyContactList():Unable to load data!");
                return Json(null);
            }
            var result = _accountService.GetAllClientAccountForTask().Where(x => x.CompanyId == company.Id).ToList();
            return Json(result.ToDataSourceResult(request, x => x.ToCompanyContactModel()));
        }
        [HttpPost]
        public ActionResult _AddCompanyContact([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models", Exclude = "AccountGuid")]IEnumerable<CompanyContactModel> models)
        {
            var results = new List<CompanyContactModel>();
            StringBuilder error = new StringBuilder();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    // Prepare account information
                    var _BL = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                    //miss franchise id 

                    Account account = model.ToEntity();
                    account.Username = model.Email;//client username  = email
                    account.Password = "Passw0rd";//default password
                    account.FranchiseId = 1;//No meaning for Client account
                    account.IsClientAccount = true;
                    _BL.prepareAccountForRegistration(account, model.AccountRoleSystemName);


                    string registerationError = _accountService.RegisterAccount(account);


                    if (String.IsNullOrEmpty(registerationError))
                    {
                        model.Id = account.Id;
                        model.AccountGuid = account.AccountGuid;
                        results.Add(model);
                    }
                    else
                        error.AppendLine(registerationError);
                }
            }
            if (error.Length > 0)
                ModelState.AddModelError("Id", error.ToString());

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditCompanyContact([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyContactModel> models)
        {
            StringBuilder error = new StringBuilder();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    string msg = string.Empty;
                    var bl = new Wfm.Admin.Models.Accounts.Account_BL(_accountService, _localizationService, _activityLogService);
                    bl.EditCompanyContactProfile(model, out msg);
                    if (!String.IsNullOrEmpty(msg))
                        error.AppendLine(msg);
                }
            }
            if (error.Length > 0)
                ModelState.AddModelError("Id", error.ToString());
            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _RemoveCompanyContact([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyContactModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _accountService.GetAccountById(model.Id);
                    entity.IsDeleted = true;
                    entity.IsActive = false;
                    _accountService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion


        #region CancelAllChanges


        [HttpPost]
        public ActionResult CancelAllChanges(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return Json(new { Error = false, Message = "" });

            try
            {
                //delete positions
                _positionService.DeleteAllPositionsByCompanyGuid(guid);
                //delete OT rule
                _companyOvertimeRuleService.DeleteAllCompanyOvertimeRulesByCompanyGuid(guid);
                //delete company recruiters
                _recruiterCompanyService.DeleteCompanyRecruitersByCompanyGuid(guid);
                //delete company vendors 
                _companyVendorService.DeleteAllCompanyVendorsByCompanyGuid(guid);
                //delete company contacts
                _companyContactService.DeleteAllCompanyContactsByCompanyGuid(guid);
                //delete User features 
                _userFeatureService.DeleteAllUserFeaturesByCompanyGuid(guid);
                //delete Notifications
                _clientNotificationService.DeleteAllClientNotificationsByCompanyGuid(guid);
                //delete company departments
                _companyDepartmentService.DeleteAllCompanyDepartmentByCompanyGuid(guid);
                //delete company locations
                _companyDivisionService.DeleteAllCompanyLocationsByCompanyGuid(guid);
                //delete company
                _companyService.DeleteCompanyByGuid(guid);

                return Json(new { Error = false, Message = "" });
            }
            catch (Exception ex)
            {
                _logger.Error("CancelAllChanges", ex);
                return Json(new { Error = true, Message = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue });
            }
        }
        #endregion


        #region Email Template Tab
        public ActionResult _TabCompanyEmailTemplate(Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
            {
                ErrorNotification("The company does not exist!");
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = company.Id;
            ViewBag.CompanyGuid = guid;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _CompanyEmailTemplates(DataSourceRequest request, Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
            {
                _logger.Error("The company does not exist!");
                return Json(null);
            }
            var result = _companyEmailTemplateService.GetAllEmailTemplateByCompanyId(company.Id);
            return Json(result.ToDataSourceResult(request, x => x.ToModel()));
        }

        public ActionResult CreateOrUpdateCompanyEmailTemplate(Guid? guid, int? templateId)
        {
            var company = _companyService.GetCompanyByGuid(guid);

            if (company == null)
            {
                ErrorNotification("The company does not exist!");
                return RedirectToAction("Index");
            }
            if (!templateId.HasValue)
            {
                CompanyEmailTemplateModel model = new CompanyEmailTemplateModel();
                model.CompanyId = company.Id;
                model.CompanyGuid = company.CompanyGuid;
                model.KeepFile1 = true;
                model.KeepFile2 = true;
                model.KeepFile3 = true;
                return View("CreateNewCompanyEmailTemplate", model);
            }
            else
            {
                var template = _companyEmailTemplateService.Retrieve(templateId.Value);
                if (template == null)
                {
                    ErrorNotification("The email template does not exist!");
                    return RedirectToAction("Details", "Company", new { guid = guid, tabId = "tab-company-email-template" });
                }
                else
                {
                    return View("CreateNewCompanyEmailTemplate", template.ToModel());
                }
            }
        }
        public ActionResult CopyEmailTemplate(int templateId)
        {
            var template = _companyEmailTemplateService.Retrieve(templateId);
            if (template == null)
            {
                ErrorNotification("The email template does not exist!");
                return RedirectToAction("Details", "Company", new { guid = template.Company.CompanyGuid, tabId = "tab-company-email-template" });
            }
            else
            {
                template.Id = 0;//indicate it is a copy one 
                return View("CreateNewCompanyEmailTemplate", template.ToModel());
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdateCompanyEmailTemplate(CompanyEmailTemplateModel model, IEnumerable<HttpPostedFileBase> files)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    string error = string.Empty;
                    _bl.WriteFilesIntoEmailTemplates(model, files, _attachmentTypeService, out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        var entity = model.ToEntity();
                        if (!_companyEmailTemplateService.DuplicateEmailTemplate(entity))
                        {
                            if (model.Id > 0)
                            {
                                //update
                                _companyEmailTemplateService.Update(entity);
                                SuccessNotification("The email template has been updated sucessfully!");
                            }
                            else
                            {
                                //create
                                _companyEmailTemplateService.Create(entity);
                                SuccessNotification("The email template has been saved sucessfully!");
                            }
                            return RedirectToAction("Details", "Company", new { guid = model.CompanyGuid, tabId = "tab-company-email-template" });
                        }
                        else
                        {
                            ErrorNotification("The email template is duplicate!");
                        }
                    }
                    else
                    {
                        _logger.Error("CreateOrUpdateCompanyEmailTemplate():" + error);
                        ErrorNotification(error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("CreateOrUpdateCompanyEmailTemplate():", ex);
                    ErrorNotification(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            return View("CreateNewCompanyEmailTemplate", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _RemoveCompanyEmailTemplate([DataSourceRequest] DataSourceRequest request, CompanyEmailTemplateModel model)
        {
            if (model != null)
            {

                _companyEmailTemplateService.Delete(model.ToEntity());

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        #endregion


        #region _EmailJobOrderConfirmation
        public ActionResult _EmailJobOrderConfirmation(Guid? companyGuid, Guid? candidateGuid, DateTime from, DateTime? to)
        {
            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
            {
                _logger.Error("The candidate does not exist!");
                return new EmptyResult();
            }
            var company = _companyService.GetCompanyByGuid(companyGuid);
            CompanyConfirmationEmailModel model = new CompanyConfirmationEmailModel();
            model.CandidateId = candidate.Id;
            model.CandidateGuid = candidate.CandidateGuid;
            model.CompanyId = company == null ? 0 : company.Id;
            model.Start = from;
            model.End = to;
            model.CandidateName = candidate.GetFullName();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _SendJobOrderConfirmationEmail(CompanyConfirmationEmailModel model)
        {
            if (ModelState.IsValid)
            {
                var jobOrder = _jobOrderService.GetJobOrderByGuid(model.JobOrderGuid);
                if (jobOrder == null)
                    return Json(new { Error = true, Message = "The job order does not exist!" });
                //send email
                string message = _workflowMessageService.SendConfirmationToEmployeeNotification(jobOrder.Id, model.CandidateId, model.Start, model.End, (int)CompanyEmailTemplateType.Confirmation);
                if (String.IsNullOrEmpty(message))
                    return Json(new { Error = false, Message = String.Concat("A confirmation email has been sent to employee ", model.CandidateName, "!") });
                else
                    return Json(new { Error = true, Message = message });
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => x.ErrorMessage));
                string msg = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                return Json(new { Error = true, Message = msg });
            }
        }
        #endregion

        //#region Remove/Upload file
        //public ActionResult SaveUpload(IEnumerable<HttpPostedFileBase> files)
        //{
        //    if (Session["UploadFiles"] != null)
        //    {
        //        var arr = Session["UploadFiles"] as List<HttpPostedFileBase>;
        //        arr.AddRange(files.ToList());
        //        Session["UploadFiles"] = arr;
        //    }
        //    else
        //    {
        //        Session["UploadFiles"] = files.ToList(); 
        //    }

        //    return Content("");
        //}

        //public ActionResult RemoveUpload(string[] fileNames)
        //{
        //    //remove the file that just upload            
        //    if (Session["UploadFiles"] != null)
        //    {
        //        var arr = Session["UploadFiles"] as List<HttpPostedFileBase>;
        //        var justUploaded = arr.Where(x => fileNames.Contains(x.FileName)).FirstOrDefault();
        //        if (justUploaded != null)
        //        {
        //            fileNames = fileNames.Where(x => x != justUploaded.FileName).ToArray();
        //            arr.Remove(justUploaded);
        //            Session["UploadFiles"] = arr;
        //        }
        //    }

        //    if (Session["DeletedFiles"] != null)
        //    {
        //        var deleted = Session["DeletedFiles"] as List<string>;
        //        deleted.AddRange(fileNames);
        //        Session["DeletedFiles"] = deleted;
        //    }
        //    else
        //        Session["DeletedFiles"] = fileNames.ToList();
        //    return Content("");
        //}
        //#endregion


        #region CompanyAttachment

        public ActionResult _TabCompanyAttachment(Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
            {
                ErrorNotification("_TabCompanyAttachment():Cannot load data!");
                return RedirectToAction("Index");
            }
            ViewBag.CompanyGuid = guid;
            return PartialView();
        }


        [HttpPost]
        public ActionResult _CompanyAttachmentList([DataSourceRequest] DataSourceRequest request, Guid? guid)
        {
            var result = _companyAttachmentService.GetAllCompanyAttachmentsByCompanyGuid(_workContext.CurrentAccount, guid);

            return Json(result.ToDataSourceResult(request, x => x.ToModel(_workContext.CurrentAccount)));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteCompanyAttachment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyAttachmentModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadCompanyAttachment))
                return new NullJsonResult();
            
            if (ModelState.IsValid && models.Any())
            {
                foreach (var model in models)
                {
                    var companyAttachment = _companyAttachmentService.Retrieve(model.Id);
                    if (companyAttachment == null)
                    {
                        _logger.Error("DeleteCompanyAttachment():Cannot load company attachment data!");
                        continue;
                    }
                    else if (!_companyAttachmentService.IsCompanyAttachmentDeletable(companyAttachment, _workContext.CurrentAccount))
                        ModelState.AddModelError("", "You're not allowed to delete the attachment!");
                    else if (model != null)
                    {
                        var entity = model.ToEntity(companyAttachment);
                        _companyAttachmentService.Delete(entity);
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        public ActionResult _UploadAttachment()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult SaveCompanyAttachment(Guid? guid, bool isRestricted, IEnumerable<HttpPostedFileBase> files)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadCompanyAttachment))
                return new NullJsonResult();
            var company = _companyService.GetCompanyByGuid(guid);
            if (company == null)
            {
                _logger.Error("SaveCompanyAttachment():Cannot load company attachment data!");
                return new NullJsonResult();
            }
            StringBuilder errors = new StringBuilder();
            if (files != null && files.Count() > 0)
            {

                foreach (var file in files)
                {
                    CompanyAttachment attachment = new CompanyAttachment() { CompanyId = company.Id };
                    using (Stream inputStream = file.InputStream)
                    {
                        var fileBinary = new byte[inputStream.Length];
                        inputStream.Read(fileBinary, 0, fileBinary.Length);
                        attachment.AttachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(file.FileName));
                        if (attachment.AttachmentType == null)
                        {
                            errors.AppendLine(String.Concat("The attachment (", file.FileName, ") type is not supported!"));
                            //_logger.Error("SaveCompanyAttachment():the attachment type is not supported!");
                            continue;
                        }

                        attachment.AttachmentTypeId = attachment.AttachmentType.Id;
                        attachment.AttachmentFile = fileBinary;
                        attachment.AttachmentFileName = Path.GetFileName(file.FileName);
                        attachment.EnteredBy = _workContext.CurrentAccount.Id;
                        attachment.IsRestricted = isRestricted;
                    }
                    _companyAttachmentService.Create(attachment);
                }
            }

            if (errors.Length == 0)
                return new NullJsonResult();
            else
                return Content(errors.ToString());
        }


        public ActionResult DownloadCompanyAttachment(Guid? guid)
        {
            CompanyAttachment attachment = _companyAttachmentService.Retrieve(guid);
            if (attachment == null)
            {
                ErrorNotification("Cannot find the attachment!");
                return RedirectToAction("Index");
            }
            else if (!_companyAttachmentService.IsCompanyAttachmentAccessible(attachment, _workContext.CurrentAccount))
            {
                ErrorNotification("You're not allowed to access the attachment!");
                return RedirectToAction("Index");
            }

            return File(attachment.AttachmentFile, attachment.AttachmentType.TypeName, attachment.AttachmentFileName);

        }


        [HttpPost]
        public ActionResult UpdateCompanyAttachment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyAttachmentModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadCompanyAttachment))
                return AccessDeniedView();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var attachment = _companyAttachmentService.GetCompanyAttachmentById(model.Id);
                    if (attachment != null)
                    {
                        attachment.IsRestricted = model.IsRestricted;
                        attachment.UpdatedOnUtc = DateTime.UtcNow;
                        _companyAttachmentService.Update(attachment);
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region Missing Hours

        public ActionResult _TabMissingHours(Guid companyGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SubmitMissingHour))
                return AccessDeniedView();

            ViewBag.CompanyGuid = companyGuid;

            return PartialView();
        }


        [HttpPost]
        public ActionResult CandidateMissingHour([DataSourceRequest] DataSourceRequest request, Guid companyGuid, int candidateId = 0, DateTime? jobStartDate = null, DateTime? jobEndDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SubmitMissingHour))
                return AccessDeniedView();

            var companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
            var missingHours = _missingHour_BL.GetAllCandidateMissingHour(companyId, candidateId, jobStartDate, jobEndDate, _workContext.CurrentAccount, includeProcessed: true);
            var result = missingHours.ProjectTo<CandidateMissingHourModel>();

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _AddMissingHour(Guid? companyGuid)
        {
            var company = _companyService.GetCompanyByGuid(companyGuid);
            ViewBag.CompanyId = company != null ? company.Id : 0;

            return PartialView(new CandidateMissingHourModel());
        }


        public ActionResult GetOrigHours(int candidateId, int jobOrderId, DateTime workDate)
        {
            var origHours = _workTimeRepository.TableNoTracking
                            .Where(x => x.CandidateId == candidateId && x.JobOrderId == jobOrderId)
                            .Where(x => x.JobStartDateTime.Year == workDate.Year && x.JobStartDateTime.Month == workDate.Month && x.JobStartDateTime.Day == workDate.Day)
                            .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched).FirstOrDefault();

            var warning = String.Empty;
            var isPlaced = _candidateJobOrderService.IsCandidatePlacedInJobOrderWithinDateRange(jobOrderId, candidateId, workDate, workDate);
            if (!isPlaced)
            {
                warning = String.Concat(Environment.NewLine, "The candidate is not placed in the job order for the selected date.");
                JobOrderOpening[] openingChanges;
                var opening = _jobOrderService.GetJobOrderOpeningAvailable(jobOrderId, workDate, out openingChanges);
                if (opening <= 0)
                    warning = String.Concat(warning, Environment.NewLine, "And the job order placement is full for the selected date.");
            }

            return Json(new { OrigHours = origHours != null ? origHours.NetWorkTimeInHours : 0, Warning = warning }, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult _CopyMissingHour(int id)
        {
            var model = _missingHourService.GetMissingHourById(id).ToModel();
            //model.OrigHours = model.NewHours = 0m;
            //model.Note = null;

            return PartialView("_CopyMissingHour", model);
        }


        [HttpPost]
        public ActionResult _SaveMissingHour([Bind(Exclude = "Id")]CandidateMissingHourModel model)
        {
            var errorMessage = new StringBuilder();

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
                var msg = _missingHour_BL.SaveMissingHour(model);
                if (String.IsNullOrWhiteSpace(msg))
                {
                    _activityLogService.InsertActivityLog("AddMissingHour", _localizationService.GetResource("ActivityLog.AddMissingHour"),
                                                          model.JobOrderId, model.CandidateId, model.WorkDate.ToShortDateString(), model.NewHours);
                }
                else
                    errorMessage.AppendLine(msg);
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _AdjustMissingHour(int missingHourId)
        {
            var missingHour = _missingHourService.GetMissingHourById(missingHourId);

            return PartialView("_AdjustMissingHour", missingHour.ToModel());
        }


        public PartialViewResult _ChangeMissingHourStatus(int missingHourId)
        {
            var missingHour = _missingHourService.GetMissingHourById(missingHourId);

            return PartialView("_ChangeMissingHourStatus", missingHour.ToModel());
        }


        public ActionResult _UploadMissingHourAttachment(int missingHourId)
        {
            var model = new MissingHourDocumentModel()
            {
                CandidateMissingHourId = missingHourId,
            };

            return PartialView(model);
        }


        public ActionResult SaveMissingHourDocuments(IEnumerable<HttpPostedFileBase> missingHourDocuments, MissingHourDocumentModel model)
        {
            string errorMessage = string.Empty;

            if (!_permissionService.Authorize(StandardPermissionProvider.SubmitMissingHour))
                return AccessDeniedView();

            foreach (var file in missingHourDocuments)
            {
                var missngHourDoc = new MissingHourDocument()
                {
                    CandidateMissingHourId = model.CandidateMissingHourId,
                    FileName = file.FileName
                };

                using (Stream stream = file.InputStream)
                {
                    var fileBytes = new byte[stream.Length];
                    stream.Read(fileBytes, 0, fileBytes.Length);
                    missngHourDoc.Stream = fileBytes;
                }

                missngHourDoc.CreatedOnUtc = missngHourDoc.UpdatedOnUtc = DateTime.UtcNow;
                _missingHourDocService.InsertMissingHourDocument(missngHourDoc);
            }

            return Json(new { Success = true });
        }


        [HttpPost]
        public ActionResult _MissingHourDocuments(int missingHourId)
        {
            var model = _missingHourDocService.GetAllMissingHourDocumentsByMissingHourId(missingHourId).ToList().Select(x => x.ToModel());

            return PartialView("_MissingHourDocuments", model);
        }


        [HttpPost]
        public ActionResult _DeleteMissingHourDocument(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SubmitMissingHour))
                return AccessDeniedView();

            var error = String.Empty;

            var missingHourDoc = _missingHourDocService.GetMissingHourDocumentById(id);
            if (missingHourDoc == null)
                error = "The missing document document with Id [{0}] does not exist.";
            else if (missingHourDoc.CandidateMissingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Approved ||
                     missingHourDoc.CandidateMissingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Rejected)
                error = "You cannot delete the document. Please change the status first then try again.";
            else
                _missingHourDocService.DeleteMissingHourDocument(missingHourDoc);

            return Json(new { Success = String.IsNullOrWhiteSpace(error), Error = error });
        }


        public ActionResult DownloadMissingHourDocument(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SubmitMissingHour))
                return AccessDeniedView();

            var missingHourDoc = _missingHourDocService.GetMissingHourDocumentById(id);
            if (missingHourDoc == null)
                return Content("The missing document document with Id [{0}] does not exist.");

            return File(missingHourDoc.Stream, System.Net.Mime.MediaTypeNames.Application.Octet, missingHourDoc.FileName);
        }


        [HttpPost]
        public JsonResult _ChangeMissingHourStatus(CandidateMissingHourModel model)
        {
            var errorMessage = new StringBuilder();

            // check supproting documents
            if (model.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Approved ||
                model.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Rejected)
            {
                var missingHourDocs = _missingHourDocService.GetAllMissingHourDocumentsByMissingHourId(model.Id);
                if (missingHourDocs == null || !missingHourDocs.Any())
                    return Json(new { Result = false, ErrorMessage = "Missing hour document is required." }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
                var msg = _missingHour_BL.ChangeMissingHourStatus(model, _workContext.CurrentAccount.Id);
                if (String.IsNullOrWhiteSpace(msg))
                {
                    _activityLogService.InsertActivityLog("AddMissingHour", _localizationService.GetResource("ActivityLog.AddMissingHour"),
                                                          model.JobOrderId, model.CandidateId, model.WorkDate.ToShortDateString(), model.NewHours);
                }
                else
                    errorMessage.AppendLine(msg);
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _AskForApproval(string ids)
        {
            var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
            var model = _missingHour_BL.AskForApprovalEmailDraft(_exportManager, _accountService, _workflowMessageService, _franchiseService, _workContext.CurrentAccount, ids);
            if (model == null)
                return Content("No selected records are pending approval");

            ViewBag.SelectedIds = ids;

            return PartialView(model);
        }


        [HttpPost]
        public JsonResult _AskForApproval([Bind(Exclude = "Id")]QueuedEmailModel model, string selectedIds)
        {
            var errorMessage = new StringBuilder();

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
                var error = _missingHour_BL.SendAskForApprovalEmail(_queuedEmailService, _accountService, model, selectedIds);
                if (!String.IsNullOrWhiteSpace(error))
                    errorMessage.AppendLine(error);
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _ExportMissingHourToXlsx(Guid companyGuid, string selectedIds)
        {
            var errorMessage = new StringBuilder();

            var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
            var missingHours = _missingHour_BL.GetMissingHourByIds(selectedIds);

            if (missingHours.Any())
            {
                try
                {
                    string fileName = string.Empty;
                    byte[] bytes = null;
                    using (var stream = new MemoryStream())
                    {
                        fileName = _exportManager.ExportMissingHourToXlsx(stream, missingHours.ToList());
                        bytes = stream.ToArray();
                    }
                    return File(bytes, "text/xls", fileName);
                }
                catch (WfmException exc)
                {
                    ErrorNotification(exc.Message);
                }
                catch (Exception exc)
                {
                    ErrorNotification(_localizationService.GetResource("Common.UnexpectedError"));
                    _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                }
            }
            else
                ErrorNotification("Please select one or more items");

            return RedirectToAction("Details", new { guid = companyGuid, tabId = "tab-missinghour" });
        }

        #endregion

    }
}

using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System.Web;
using Wfm.Admin.Models.Candidate;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Tests;
using Wfm.Core;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.ExportImport;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Services.Test;
using Wfm.Services.TimeSheet;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework.Mvc;
using Wfm.Services.Incident;
using Wfm.Services.Companies;
using Wfm.Core.Domain.Incident;
using Wfm.Shared.Models.Incident;
using Wfm.Shared.Mapping;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Admin.Models.Messages;
using Kendo.Mvc;
using Wfm.Services.ClockTime;
using Wfm.Admin.Models.ClockTime;
using Wfm.Admin.Models.Common;
using Wfm.Shared.Extensions;
using Wfm.Web.Framework.Feature;


namespace Wfm.Admin.Controllers
{
    public class CandidateController : BaseAdminController
    {
        #region Fields

        private readonly IRepository<CandidateOnboardingStatus> _candidateOnboardingStatusRepository;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ISalutationService _salutationService;
        private readonly IShiftService _shiftService;
        private readonly ISkillService _skillService;
        private readonly IPositionService _positionService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IRepository<CandidateWorkOverTime> _overtimeRepository;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly IFranchiseService _franchisesService;
       // private readonly IFranchiseSettingService _franchisesettingservice;
        private readonly IActivityLogService _activityLogService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;
        private readonly ICandidateTestResultService _candidateTestResultService;
        private readonly ICandidateAddressService _candidateAddressService;
        private readonly ICandidateBankAccountService _candidateBankAccountService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAttachmentTypeService _attachmentTypeService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ITestService _testService;
       // private readonly ICandidateWorkHistoryService _candidateWorkHistoryService;
        private readonly IWorkContext _workContext;
        private readonly IAccountService _accountService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICandidateJobOrderStatusHistoryService _candidateJobOrderStatusHistoryService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly CandidateMassEmailSettings _candidateMassEmailSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IImportManager _importManager;
        private readonly ICandidateJobOrderPlacementService _candidateJobOrderPlacementService;
        private readonly IIncidentService _incidentService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly CreateEditCandidate_BL _candidateBL;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageService _messageService;
        private readonly IExportManager _exportManager;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly ISmartCardService _smartCardService;
        private readonly ITextMessageSender _textMessageSender;
        private readonly ICompanyEmailTemplateService _companyEmailTemplateService;
        #endregion

        #region Ctor

        public CandidateController(
            IRepository<CandidateOnboardingStatus> candidateOnboardingStatusRepository,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ISalutationService salutationService,
            IShiftService shiftService,
            ISkillService skillService,
            IPositionService positionService,
            ICandidateService candidateService,
            ICandidateBlacklistService candidateBlacklistService,
            ICandidatePictureService candidatePictureService,
            ICandidateJobOrderService candidateJobOrderService,
            IWorkTimeService workTimeService,
            IRepository<CandidateWorkOverTime> overtimeRepository,
            IPaymentHistoryService paymentHistoryService,
            IActivityLogService activityLogService,
            ICandidateKeySkillService candidateKeySkillsService,
            ICandidateTestResultService candidateTestResultService,
            ICandidateAddressService candidateAddressService,
            ICandidateBankAccountService candidateBankAccountService,
            IAttachmentService attachmentService,
            IAttachmentTypeService attachmentServiceType,
            IJobOrderService jobOrderService,
            ITestService testService,
      //      ICandidateWorkHistoryService candidateWorkHistoryService,
            IFranchiseService franchiseService,
       //     IFranchiseSettingService franchisesettingservice,
            IWorkContext workContext,
            IAccountService accountService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICandidateJobOrderStatusHistoryService candidateJobOrderStatusHistoryService,
            ILogger logger,
            IWebHelper webHelper,
            CandidateMassEmailSettings candidateMassEmailSettings,
            MediaSettings mediaSettings,
            IImportManager importManager,
            ICandidateJobOrderPlacementService candidateJobOrderPlacementService,
            IIncidentService incidentService,
            ICompanyService companyService,
            ICompanyCandidateService companyCandidateService,
            IEmailAccountService emailAccountService,
            IWorkflowMessageService workflowMessageService,
            IDocumentTypeService documentTypeService,
            IMessageTemplateService messageTemplateService,
            IMessageService messageService,
            IExportManager exportManager,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            IRecruiterCompanyService recruiterCompanyService,
            ISmartCardService smartCardService,
            ITextMessageSender textMessageSender,
            ICompanyEmailTemplateService companyEmailTemplateService
        )
        {
            _candidateOnboardingStatusRepository = candidateOnboardingStatusRepository;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _salutationService = salutationService;
            _shiftService = shiftService;
            _skillService = skillService;
            _positionService = positionService;
            _candidateService = candidateService;
            _candidateBlacklistService = candidateBlacklistService;
            _candidatePictureService = candidatePictureService;
            _candidateJobOrderService = candidateJobOrderService;
            _workTimeService = workTimeService;
            _overtimeRepository = overtimeRepository;
            _paymentHistoryService = paymentHistoryService;
            _activityLogService = activityLogService;
            _candidateKeySkillsService = candidateKeySkillsService;
            _candidateTestResultService = candidateTestResultService;
            _candidateAddressService = candidateAddressService;
            _candidateBankAccountService = candidateBankAccountService;
            _attachmentService = attachmentService;
            _attachmentTypeService = attachmentServiceType;
            _jobOrderService = jobOrderService;
            _testService = testService;
        //    _candidateWorkHistoryService = candidateWorkHistoryService;
            _franchisesService = franchiseService;
       //     _franchisesettingservice = franchisesettingservice;
            _workContext = workContext;
            _accountService = accountService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _candidateJobOrderStatusHistoryService = candidateJobOrderStatusHistoryService;
            _logger = logger;
            _webHelper = webHelper;
            _candidateMassEmailSettings = candidateMassEmailSettings;
            _mediaSettings = mediaSettings;
            _importManager = importManager;
            _candidateJobOrderPlacementService = candidateJobOrderPlacementService;
            _incidentService = incidentService;
            _companyService = companyService;
            _companyCandidateService = companyCandidateService;
            _emailAccountService = emailAccountService;
            _workflowMessageService = workflowMessageService;
            _documentTypeService = documentTypeService;
            _messageTemplateService = messageTemplateService;
            _messageService = messageService;
            _exportManager = exportManager;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _recruiterCompanyService = recruiterCompanyService;
            _smartCardService = smartCardService;
            _textMessageSender = textMessageSender;

            _companyEmailTemplateService = companyEmailTemplateService;
            _candidateBL = new CreateEditCandidate_BL(_workContext, _countryService, _stateProvinceService, _cityService, _salutationService, _shiftService, _skillService, _positionService,
                                                      _permissionService, _franchisesService, _accountService, _companyCandidateService, _candidateJobOrderService,
                                                      _candidateService, _candidateBlacklistService, _candidateKeySkillsService, _candidateAddressService, _candidatePictureService,
                                                      _emailAccountService, _activityLogService, _localizationService, _candidateMassEmailSettings, _mediaSettings = mediaSettings, _accountPasswordPolicyService,
                                                      _textMessageSender);
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Candidate candidate)
        {
            //foreach (var cp in candidate.CandidatePictures)
            //    _pictureService.SetSeoFilename(cp.PictureId, _pictureService.GetPictureSeName(candidate.FirstName));
        }


        private string ComposeModelStateError()
        {
            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
            string errMsg = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
            _logger.Warning(string.Format("Model state is invalid: --- ERRORS --- {0}.", errMsg), account: _workContext.CurrentAccount, userAgent: Request.UserAgent);

            return errMsg;
        }

        #endregion

        #region GET :/Candidate/Index
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #endregion

        #region GET :/Candidate/List
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Candidate/List

        [FeatureAuthorize(featureName: "Applicant Tracking")]
        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            bool isSortingRequired = true;
            if (request.Sorts.Any())
                isSortingRequired = false;

            var result = _candidateBL.GetCandidateList(isSortingRequired, request);

            return Json(result);
        }

        #endregion


        #region GET :/Candidate/Dnrlist

        public ActionResult Dnrlist()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateBlacklist))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Candidate/Dnrlist

        [HttpPost]
        public ActionResult Dnrlist([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateBlacklist))
                return AccessDeniedView();

            var result = _candidateBL.GetCandidateBlacklist();

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region Candidate Search

        [HttpPost]
        public ActionResult CandidateListBySearch([DataSourceRequest] DataSourceRequest request, string searchKey)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            if (String.IsNullOrEmpty(searchKey))
                return Json(new List<SimpleCandidateModel>());

            var candidates = _candidateService.SearchCandidates(searchKey);

            return Json(candidates.Select(x => x.ToSimpleModel()).ToDataSourceResult(request));
        }

        #endregion


        #region GET: //Candidate/_AddSelectedCandidatesToBlacklist

        public ActionResult _AddSelectedCandidatesToBlacklist(string searchKey, string viewName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            ViewBag.SearchKey = searchKey;

            return PartialView("_AddSelectedToBlacklist");
        }

        #endregion

        #region POST://Candidate/_AddSelectedCandidatesToBlacklist

        [HttpPost]
        public ActionResult _AddSelectedCandidatesToBlacklist(string selectedIds, DateTime startDate, int? clientId, string reason,string note,string clientName, int? jobOrderId)
        {
            string errorMessage = string.Empty;


            if (String.IsNullOrEmpty(reason) || startDate == null)
                errorMessage = _localizationService.GetResource("Admin.Candidate.AddToBlacklist.DateAndReasonRequired") + "\r\n";
            else
                errorMessage = _candidateBL.BanSelectedCandidates(selectedIds, startDate, clientId, reason, note, clientName, jobOrderId);

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region GET :/Candidate/_AddCandidateToBlacklist

        public ActionResult _AddCandidateToBlacklist(Guid? guid)
        {
            ViewBag.CandidateGuid = guid;

            return PartialView("_AddToBlacklist");
        }

        #endregion

        #region POST:/Candidate/_AddCandidateToBlacklist

        [HttpPost]
        public ActionResult _AddCandidateToBlacklist(Guid guid, DateTime startDate, int? clientId, string reason,string note,string clientName, int? jobOrderId )
        {
            string errorMessage = string.Empty;

            if (String.IsNullOrEmpty(reason) || startDate == null)
                errorMessage = _localizationService.GetResource("Admin.Candidate.AddToBlacklist.DateAndReasonRequired") + "\r\n";
            else
                errorMessage = _candidateBL.BanCandidate(guid, startDate, clientId, reason, note, clientName, jobOrderId);

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POST:/Candidate/_RemoveCandidateFromBlacklist

        [HttpPost]
        public ActionResult _RemoveCandidateFromBlacklist(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateBlacklist))
                return AccessDeniedView();

            string errorMessage = _candidateBL.RemoveCandidateFromBlacklist(id);

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region GET :/Candidate/ListByPicture
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        public ActionResult ListByPicture()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Candidate/ListByPicture
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        [HttpPost]
        public ActionResult ListByPicture([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var result = _candidateBL.GetCandidateListByPicture(request);

            return Json(result);
        }

        #endregion

        #region GET :/Candidate/ListByResume
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        public ActionResult ListByResume()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Candidate/ListByResume
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        [HttpPost]
        public ActionResult ListByResume([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            foreach (var descriptor in request.Filters)
            {
                switch (((FilterDescriptor)descriptor).Member)
                {
                    case "FirstName": ((FilterDescriptor)descriptor).Member = "Candidate.FirstName"; break;
                    case "LastName": ((FilterDescriptor)descriptor).Member = "Candidate.LastName"; break;
                    case "UseForDirectPlacement": ((FilterDescriptor)descriptor).Member = "Candidate.UseForDirectPlacement"; break;
                    case "Note": ((FilterDescriptor)descriptor).Member = "Candidate.Note"; break;
                    case "EmployeeId": ((FilterDescriptor)descriptor).Member = "Candidate.EmployeeId"; break;
                }
            }

            foreach (var descriptor in request.Sorts)
            {
                switch (((SortDescriptor)descriptor).Member)
                {
                    case "FirstName": ((SortDescriptor)descriptor).Member = "Candidate.FirstName"; break;
                    case "LastName": ((SortDescriptor)descriptor).Member = "Candidate.LastName"; break;
                    case "UseForDirectPlacement": ((SortDescriptor)descriptor).Member = "Candidate.UseForDirectPlacement"; break;
                    case "Note": ((SortDescriptor)descriptor).Member = "Candidate.Note"; break;
                }
            }

            var resumeCode = Convert.ToString(CandidateDocumentTypeEnum.RESUME);
            var attachments = _attachmentService.GetAllCandidateAttachmentsAsQueryable(true, true).Where(x => x.DocumentType.InternalCode == resumeCode);
            return Json(attachments.ToDataSourceResult(request, x => x.ToModel()));
        }

        #endregion

        #region GET :/Candidate/ListBySkills
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        public ActionResult ListBySkills()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Candidate/ListBySkills
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        [HttpPost]
        public ActionResult ListBySkills([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var result = _candidateBL.GetCandidateListBySkills(request);

            return Json(result);
        }

        #endregion

        #region GET :/Candidate/ListByAddress
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        public ActionResult ListByAddress()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Candidate/ListByAddress
        [FeatureAuthorize(featureName: "Applicant Tracking")]
        [HttpPost]
        public ActionResult ListByAddress([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var result = _candidateBL.GetCandidateListByAddress(request);

            return Json(result);
        }

        #endregion


        #region  GET :/Candidate/Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowAddCandidates))
                return AccessDeniedView();

            CreateEditCandidateModel model = new CreateEditCandidateModel();
            model.PasswordPolicyModel = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel();
            model.Entitled = true;
            model.IsActive = true;
            model.IsBanned = false;
            model.CandidateAddressModel = new AddressModel() { CountryId = 2 }; // Default is Canada

            model.UseForDirectPlacement = false;
            ViewBag.NewPage = true;
            return View(model);
        }

        #endregion

        #region POST:/Candidate/CreateOrUpdate

        [HttpPost]
        public ActionResult Create(CreateEditCandidateModel model, int currentIndex)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowAddCandidates))
                return AccessDeniedView();

            ViewBag.StartIndex = 0;
            if (model.Id > 0)
            {
                ModelState.Remove("Password");
                ModelState.Remove("RePassword");
            }
            if (ModelState.IsValid)
            {
                var result = _CreateOrUpdateCandidate(model);
                if (String.IsNullOrWhiteSpace(result))
                {
                    if (model.Id == 0)
                        SuccessNotification(_localizationService.GetResource("Admin.Candidate.Candidate.Added"));
                    else
                        return RedirectToAction("Create");

                    var candidate = _candidateService.GetCandidateByUsername(model.Username);
                    model.Id = candidate.Id;
                    model.CandidateGuid = candidate.CandidateGuid;
                    model.EmployeeId = candidate.EmployeeId;
                    model.FranchiseId = candidate.FranchiseId;
                    model.Password = model.RePassword = null;
                    model.EnteredBy = candidate.EnteredBy;
                    model.SearchKeys = candidate.SearchKeys;
                    ModelState.Clear();
                    ViewBag.StartIndex = currentIndex <= 5 ? 6 : currentIndex;
                }
                else
                    ErrorNotification(result);
            }

            ViewBag.NewPage = false;
            return View(model);
        }


        private string _CreateOrUpdateCandidate(CreateEditCandidateModel model)
        {
            string errorMessage = null;
            var saved = false;

            if (model.Id > 0)
                saved = _candidateBL.UpdateCandidateData((CandidateModel)model, out errorMessage);
            else
                saved = _candidateBL.RegisterCandidateFromAdmin(model, out errorMessage);

            return errorMessage;
        }

        #endregion


        #region GET :/Candidate/Edit/Guid

        public ActionResult Edit(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
            {
                return AccessDeniedView();
            }

            if (guid == null)
            {
                _logger.Error("Null value is passed for 'guid' to the Edit controller action. ", null, null, userAgent: Request.UserAgent);
                return AccessDeniedView();
            }

            var model = _candidateBL.GetCandidateBasicDataById(guid.Value);
            if (model == null)
                return AccessDeniedView();
            else
            {
                model.SocialInsuranceNumber = model.SocialInsuranceNumber.ToMaskedSocialInsuranceNumber();
                return View(model);
            }
        }

        #endregion

        #region POST:/Candidate/Edit/Guid

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CandidateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                string errorMessage = String.Empty;

                bool saved = _candidateBL.UpdateCandidateData(model, out errorMessage);
                if (saved)
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Candidate.Candidate.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { guid = model.CandidateGuid }) : RedirectToAction("Details", new { guid = model.CandidateGuid });
                }
                else
                {
                    ErrorNotification(errorMessage);
                }
            }
            else
            {
                ErrorNotification(this.ComposeModelStateError());
            }

            return View(model);
        }

        #endregion


        #region GET :/Candidate/Details

        public ActionResult Details(Guid? guid, string tabId = null, DateTime? refDate = null, bool fromEmployeeView = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates) || guid == null)
                return AccessDeniedView();

            var model = _candidateBL.GetCandidateDetailDataById(guid.Value);
            if (model == null)
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(tabId))
                ViewBag.TabId = tabId;
            ViewBag.RefDate = refDate ?? DateTime.Today;
            ViewBag.FromEmployee = fromEmployeeView;

            return View(model);
        }

        #endregion


        #region GET :/Candidate/ReloadContentText
        [HttpPost]
        public ActionResult ReloadContentText(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            _attachmentService.ReloadContentTextById(id);

            return RedirectToAction("ListByResume");
        }
        #endregion


        // Candidate TestResult

        #region GET :/Candidate/DownloadCandidateTestResult

        public ActionResult DownloadCandidateTestResult(int id)
        {
            CandidateTestResult testResult = _candidateTestResultService.GetCandidateTestResultById(id);
            if (testResult == null)
                return new NullJsonResult();

            // test result file
            string basePath = _webHelper.GetRootDirectory();
            string testResultFileWithPath = Path.Combine(basePath, testResult.ScoreFilePath);

            var fileInfo = new FileInfo(testResultFileWithPath);
            string fileName = fileInfo.Name;
            string contentType = _attachmentService.GetMimeType(fileName);

            TestCategory testCategory = _testService.GetTestCategoryById(testResult.TestCategoryId);
            var originalFileName = String.Concat(testResult.CandidateId, "_", testCategory.TestCategoryName, ".txt");

            if (System.IO.File.Exists(testResultFileWithPath))
                return File(testResultFileWithPath, contentType, originalFileName);

            //return new EmptyResult();
            //return RedirectToAction("Details", new { id = attachment.CandidateId });
            ErrorNotification("File not found. Please contact the system administrator.");
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        #endregion


        #region POST:/Candidate/CreateKeySkill

        [HttpPost]
        public ActionResult CreateKeySkill([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateKeySkillModel> models)
        {
            var results = new List<CandidateKeySkillModel>();
            if (models != null && ModelState.IsValid)
            {
                var candidate = _candidateService.GetCandidateById(models.FirstOrDefault().CandidateId);
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    _candidateKeySkillsService.InsertCandidateKeySkill(entity);
                    //model.Id = entity.Id;
                    results.Add(model);
                    candidate.SearchKeys = String.Concat(candidate.SearchKeys, ",", model.KeySkill);
                    _activityLogService.InsertActivityLog("Candidate.AddNewKeySkill", _localizationService.GetResource("ActivityLog.Candidate.AddNewKeySkill"), candidate, candidate.GetFullName() + " / " + model.KeySkill);
                }
                _candidateService.UpdateCandidate(candidate);



            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region POST:/Candidate/EditKeySkill

        [HttpPost]
        public ActionResult EditKeySkill([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateKeySkillModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var msg = new List<string>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    try
                    {
                        _candidateBL.SaveKeySkill(model);
                    }
                    catch (Exception ex)
                    {
                        msg.Add(ex.Message);
                    }
                }
            }
            if (msg.Count > 0)
                ModelState.AddModelError("CandidateId", String.Join("\n", msg));

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region GET :/Candidate/DeleteKeySkill

        //public ActionResult KeySkillDelete(int id)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
        //        return AccessDeniedView();

        //    CandidateKeySkill candidateKeySkill = _candidateKeySkillsService.GetCandidateKeySkillById(id);
        //    int candidateId = candidateKeySkill.CandidateId;
        //    _candidateKeySkillsService.DeleteCandidateKeySkill(candidateKeySkill);

        //    //activity log
        //    _activityLogService.InsertActivityLog("DeleteCandidateKeySkill", _localizationService.GetResource("ActivityLog.DeleteCandidateKeySkill"), candidateKeySkill.KeySkill);


        //    // return RedirectToAction("_TabCandidateKeySkillList", new { candidateId = candidateId });
        //    return RedirectToAction("Details", new { id = candidateId });
        //}

        #endregion

        #region GET :/Candidate/CandidateWorkHistory
        public ActionResult CandidateWorkHistory(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            Candidate candidate = _candidateService.GetCandidateByGuid(guid);

            CandidateModel _candidateModel = candidate.ToModel();
            //_candidateModel.ShiftModel = candidate.Shift.ToModel();
            //_candidateModel.GenderModel = candidate.Gender.ToModel();
            //_candidateModel.VetranTypeModel = candidate.VetranType.ToModel();
            //_candidateModel.TransportationModel = candidate.Transportation.ToModel();
            //_candidateModel.SourceModel = candidate.Source.ToModel();
            //_candidateModel.SalutationModel = candidate.Salutation.ToModel();
            //_candidateModel.EthnicTypeModel = candidate.EthnicType.ToModel();
            //_candidateModel.FranchiseName = _accountService.GetFranchiseNameByAccountId(candidate.EnteredBy);

            //_candidateModel.CandidateAddressModel = _candidatesService.GetCandidateHomeAddressByCandidateId(candidate.Id).ToModel();

            //var candidateBillingAddress = _candidatesService.GetCandidateBillingAddressByCandidateId(candidate.Id);
            //if (candidateBillingAddress != null)
            //{
            //    _candidateModel.BillingAddressModel = candidateBillingAddress.ToModel();
            //}

            //_candidateModel.TotalPipeline = _jobOrderService.GetPipelineTotalForIndivisaulCandidateByCandidateId(id);

            return PartialView(_candidateModel);
        }

        #endregion


        // Candidate Address

        #region GET :/Candidate/_AddressList/5

        public ActionResult _AddressList(Guid? candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
            {
                ErrorNotification("The candidate does not exist!");
                return new EmptyResult();
            }
            ViewBag.CandidateId = candidate.Id;
            ViewBag.CandidateGuid = candidateGuid;

            return View();
        }

        [HttpPost]
        public ActionResult _AddressList([DataSourceRequest] DataSourceRequest request, Guid? candidateGuid)
        {
            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
            {
                return Json(null);
            }
            var candidateAddresses = _candidateAddressService.GetAllCandidateAddressesByCandidateId(candidate.Id);
            var addressModels = candidateAddresses.Select(x => x.ToModel()).ToList();
            for (int i = 0; i < addressModels.Count(); i++)
            {
                addressModels[i].CountryName = addressModels[i].CountryId > 0 ? _countryService.GetCountryById(addressModels[i].CountryId).CountryName : "";
                addressModels[i].StateProvinceName = addressModels[i].StateProvinceId > 0 ? _stateProvinceService.GetStateProvinceById(addressModels[i].StateProvinceId).StateProvinceName : "";
                addressModels[i].CityName = addressModels[i].CityId > 0 ? _cityService.GetCityById(addressModels[i].CityId).CityName : "";
            }
            return Json(addressModels.ToDataSourceResult(request));
        }
        #endregion

        #region POST:/Candidate/CreateAddress

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _CreateNewAddress([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateAddressModel> addresses)
        {
            var results = new List<CandidateAddressModel>();
            if (addresses != null && ModelState.IsValid)
            {
                foreach (var address in addresses)
                {
                    address.IsActive = true;
                    address.IsDeleted = false;
                    address.UpdatedOnUtc = DateTime.UtcNow;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    address.EnteredBy = _workContext.CurrentAccount.Id;
                    address.CountryName = address.CountryId > 0 ? _countryService.GetCountryById(address.CountryId).CountryName : "";
                    address.StateProvinceName = address.StateProvinceId > 0 ? _stateProvinceService.GetStateProvinceById(address.StateProvinceId).StateProvinceName : "";
                    address.CityName = address.CityId > 0 ? _cityService.GetCityById(address.CityId).CityName : "";
                    var entity = address.ToEntity();
                    _candidateAddressService.InsertCandidateAddress(entity);
                    address.Id = entity.Id;
                    results.Add(address);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region POST:/Candidate/EditAddress/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditAddress([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateAddressModel> addresses)
        {
            if (addresses != null && ModelState.IsValid)
            {
                foreach (var address in addresses)
                {
                    var entity = _candidateAddressService.GetCandidateAddressById(address.Id);
                    address.UpdatedOnUtc = DateTime.UtcNow;
                    address.ToEntity(entity);
                    _candidateAddressService.UpdateCandidateAddress(entity);
                }
            }

            return Json(addresses.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete Address
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _DeleteAddress([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateAddressModel> addresses)
        {
            if (addresses.Any())
            {
                foreach (var address in addresses)
                {
                    var entity = _candidateAddressService.GetCandidateAddressById(address.Id);
                    address.UpdatedOnUtc = DateTime.UtcNow;
                    address.IsDeleted = true;
                    address.ToEntity(entity);
                    _candidateAddressService.UpdateCandidateAddress(entity);
                }
            }

            return Json(addresses.ToDataSourceResult(request, ModelState));
        }
        #endregion


        // Candidate Attachment

        public ActionResult GetFileStartNameByDocumentId(int documentTypeId)
        {
            string fileName = string.Empty;

            var documentType = _documentTypeService.GetDocumentTypeById(documentTypeId);
            if (documentType != null && !string.IsNullOrEmpty(documentType.FileName))
            {
                fileName = documentType.FileName.Replace("*.*", "").ToUpper();
            }

            return Json(new { FileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        #region GET :/Candidate/SaveAttachments

        public ActionResult SaveAttachments(IEnumerable<HttpPostedFileBase> attachments, Guid candidateGuid, int documentTypeId, DateTime? expiryDate, int? companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
            {
                return Content("The candidate does not exist!");
            }
            // The Name of the Upload component is "attachments"
            foreach (var file in attachments)
            {
                // prepare
                var fileName = Path.GetFileName(file.FileName);
                var contentType = file.ContentType;

                // not supported file format
                AttachmentType attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(fileName));
                if (attachmentType == null)
                    return Content("File format is not supported, please contact administrator.");

                using (Stream stream = file.InputStream)
                {
                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    // upload attachment
                    string result = _attachmentService.UploadCandidateAttachment(candidate.Id, fileBinary, fileName, contentType, documentTypeId, expiryDate, companyId);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return Content(result);
                    }
                }


                //activity log
                _activityLogService.InsertActivityLog("AddNewCandidateAttachment", _localizationService.GetResource("ActivityLog.AddNewCandidateAttachment"), fileName);
            }


            // Return an empty string to signify success
            return Content("");
        }
        #endregion

        //#region GET :/Candidate/DeleteAttachment

        public ActionResult DeleteAttachment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            CandidateAttachment attachment = _attachmentService.GetAttachmentById(id, noTracking: false);
            if (attachment == null)
                return RedirectToAction("List");


            _attachmentService.DeleteAttachment(attachment);


            //activity log
            _activityLogService.InsertActivityLog("DeleteCandidateAttachment", _localizationService.GetResource("ActivityLog.DeleteCandidateAttachment"), attachment.Id + "/" + attachment.OriginalFileName);


            //return RedirectToAction("Details", new { guid = attachment.Candidate.CandidateGuid });
            return new EmptyResult();
        }

        //#endregion

        #region GET :/Candidate/DownloadCandidateAttachment

        public ActionResult DownloadCandidateAttachment(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            CandidateAttachment attachment = _attachmentService.GetAttachmentByGuid(guid);
            if (attachment == null)
                return RedirectToAction("List");

            if (!String.IsNullOrEmpty(attachment.StoredPath))
            {
                string contentType = attachment.ContentType;
                string originalFileName = attachment.OriginalFileName;
                string basePath = _webHelper.GetRootDirectory();
                string folderPath = Path.Combine(basePath, attachment.StoredPath);

                string filePath = Path.Combine(folderPath, attachment.StoredFileName);

                if (System.IO.File.Exists(filePath))
                    return File(filePath, contentType, originalFileName);

                ErrorNotification("File not found. Please contact the system administrator.");
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                return File(attachment.AttachmentFile, attachment.ContentType, attachment.OriginalFileName);
            }
        }

        #endregion


        // Candidate Picture

        #region GET :/Candidate/CandidatePicture

        //[HttpGet]
        //public ActionResult CandidatePictureList(int candidateId)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
        //        return AccessDeniedView();

        //    var candidateModel = new CandidateModel()
        //    {
        //        Id = candidateId,
        //    };

        //    var candidatePictures = _candidatePictureService.GetCandidatePicturesByCandidateId(candidateId, 20);
        //    candidateModel.CandidatePictureModels = candidatePictures
        //        .Select(x =>
        //        {
        //            var candidatePictureModel = x.ToModel();
        //            candidatePictureModel.PictureUrl = _candidatePictureService.GetCandidatePictureUrl(x.Id);
        //            return candidatePictureModel;
        //        })
        //        .ToList();

        //    return View(candidateModel);
        //}

        [HttpPost]
        public ActionResult CandidatePictureList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var candidatePictures = _candidatePictureService.GetCandidatePicturesByCandidateId(candidateId);
            var candidatePictureModelList = candidatePictures
                .Select(x =>
                {
                    var candidatePictureModel = x.ToModel();
                    candidatePictureModel.PictureUrl = _candidatePictureService.GetCandidatePictureUrl(x.Id);
                    return candidatePictureModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = candidatePictureModelList,
                Total = candidatePictureModelList.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult CandidatePictureUpdate(CandidatePictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidatePicture = _candidatePictureService.GetCandidatePictureById(model.Id);
            if (candidatePicture == null)
                throw new ArgumentException("No candidate picture found with the specified id");

            candidatePicture.DisplayOrder = model.DisplayOrder;

            _candidatePictureService.UpdateCandidatePicture(candidatePicture);


            //activity log
            _activityLogService.InsertActivityLog("UpdateCandidatePicture", _localizationService.GetResource("ActivityLog.UpdateCandidatePicture"), candidatePicture.DisplayOrder);


            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult CandidatePictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidatePicture = _candidatePictureService.GetCandidatePictureById(id);
            if (candidatePicture == null)
                throw new ArgumentException("No candidate picture found with the specified id");

            _candidatePictureService.DeleteCandidatePicture(candidatePicture);


            //activity log
            _activityLogService.InsertActivityLog("DeleteCandidatePicture", _localizationService.GetResource("ActivityLog.DeleteCandidatePicture"), candidatePicture.Id + "/" + candidatePicture.CandidateId);


            return new NullJsonResult();
        }

        #endregion


        // Start Candidate Onboarding

        #region GET :/Candidate/StartOnboarding

        public PartialViewResult StartOnboarding(Guid guid, string viewName = "_StartCandidateOnborading")
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            CandidateOnboardingModel model = AutoMapper.Mapper.Map<CandidateOnboardingModel>(candidate);
            var smartCard = candidate.SmartCards.Where(x => x.IsActive && !x.IsDeleted).FirstOrDefault();
            if (smartCard != null)
                model.SmartCardUid = smartCard.SmartCardUid;
            ViewBag.NewPage = true;
            ViewBag.CandidateGuid = guid;
            return PartialView(viewName, model);
        }

        #endregion

        #region POST:/Candidate/StartOnboarding

        [HttpPost]
        public JsonResult StartOnboarding(CandidateOnboardingModel model)
        {
            bool anyError = false;
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {


                var candidate = _candidateService.GetCandidateById(model.Id);
                // var attachmentLOH = _attachmentService.GetAttachmentsByCandidateId(model.Id).Where(d => d.DocumentType.InternalCode == CandidateDocumentTypeEnum.LETTEROFHIRE.ToString()).FirstOrDefault();
                if (candidate == null)
                {
                    anyError = true;
                    errorMessage += "\r\n" + "Candidate does not exist!";
                }


                else
                {

                    if (!String.IsNullOrEmpty(model.SmartCardUid))
                    {
                        var existed = _smartCardService.GetCandidateBySmartCardUid(model.SmartCardUid) == candidate;
                        if (!existed)
                        {
                            if (!_smartCardService.IsDuplicate(model.SmartCardUid))
                                _smartCardService.InsertDefaultSmartCard(model.SmartCardUid, candidate.Id);
                            else
                            {
                                anyError = true;
                                errorMessage += "The smart card uid has already been used by some other candiates!";
                                return Json(new { Result = !anyError, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    candidate.CandidateOnboardingStatusId = (int)CandidateOnboardingStatusEnum.Started;
                    candidate.OwnerId = _workContext.CurrentAccount.Id;
                    candidate.SocialInsuranceNumber = model.SocialInsuranceNumber;
                    candidate.SINExpiryDate = model.SINExpiryDate;

                    _candidateService.UpdateCandidate(candidate);

                    //log
                    _activityLogService.InsertActivityLog("StartCandidateOnboarding", _localizationService.GetResource("ActivityLog.StartCandidateOnboarding"), model.Id);

                }

            }
            else
            {
                anyError = true;
                errorMessage = String.Join("\r\n", ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage)));
            }

            return Json(new { Result = !anyError, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ValidateSin(string sin)
        {
            return Json(new { Result = CommonHelper.IsValidCanadianSin(sin) }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        // Cancel Candidate Onboarding

        #region GET :/Candidate/CancelOnboarding

        public PartialViewResult CancelOnboarding(Guid guid)
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            CandidateOnboardingModel model = AutoMapper.Mapper.Map<CandidateOnboardingModel>(candidate);

            return PartialView("_CancelCandidateOnborading", model);
        }

        #endregion

        #region POST:/Candidate/CancelOnboarding

        [HttpPost]
        public JsonResult CancelOnboarding(CandidateOnboardingModel model, string reason)
        {
            var errorMessage = _candidateBL.CancelCandidateOnboarding(model, reason);

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        // import candidate

        #region  GET :/Candidate/Import

        public ActionResult Import()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowAddCandidates))
                return AccessDeniedView();

            var model = new VendorCandidateImportModel();
            model.VendorId = _workContext.CurrentAccount.FranchiseId;

            return View(model);
        }

        #endregion

        #region POST:/Candidate/Import

        [HttpPost]
        public ActionResult ImportAccount()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowAddCandidates))
                return AccessDeniedView();

            try
            {
                var importResult = new ImportResult();
                var file = Request.Files["account-file"];
                if (file != null && file.ContentLength > 0)
                    importResult = _importManager.ImportVendorCandidateFromXlsx(file.InputStream, _workContext.CurrentAccount.FranchiseId);
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                    return RedirectToAction("Import");
                }

                if (importResult.Attempted == 0)
                {
                    if (importResult.ErrorMsg.Count > 0)
                        ErrorNotification(String.Join(" | ", importResult.ErrorMsg));
                    else
                        ErrorNotification("The excel file is empty.");
                }
                else
                {
                    if (importResult.Imported > 0)
                        SuccessNotification(String.Format("[{0}] of [{1}] candidate accounts imported.", importResult.Imported, importResult.Attempted));
                    if (importResult.Attempted > 0 && importResult.NotImported > 0)
                        ErrorNotification(String.Format("[{0}] of [{1}] candidate accounts NOT imported. See details in the section below.", importResult.NotImported, importResult.Attempted));
                }

                var model = new VendorCandidateImportModel();
                model.VendorId = _workContext.CurrentAccount.FranchiseId;
                model.AccountImportedOn = DateTime.Now;
                model.AccountImportResult = importResult;

                return View("Import", model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Import");
            }

        }

        [HttpPost]
        public ActionResult ImportAddress()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            try
            {
                var importResult = new ImportResult();

                var file = Request.Files["address-file"];
                if (file != null && file.ContentLength > 0)
                {
                    importResult = _importManager.ImportVendorCandidateAddressFromXlsx(file.InputStream, _workContext.CurrentAccount.FranchiseId);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                    return RedirectToAction("Import");
                }

                if (importResult.Attempted == 0)
                {
                    if (importResult.ErrorMsg.Count > 0)
                        ErrorNotification(String.Join(" | ", importResult.ErrorMsg));
                    else
                        ErrorNotification("The excel file is empty");
                }
                else
                {
                    if (importResult.Imported > 0)
                        SuccessNotification(String.Format("[{0}] of [{1}] candidate addresses imported.", importResult.Imported, importResult.Attempted));
                    if (importResult.Attempted > 0 && importResult.NotImported > 0)
                        ErrorNotification(String.Format("[{0}] of [{1}] candidate addresses NOT imported. See details in the section below.", importResult.NotImported, importResult.Attempted));
                }

                var model = new VendorCandidateImportModel();
                model.VendorId = _workContext.CurrentAccount.FranchiseId;
                model.AddressImportedOn = DateTime.Now;
                model.AddressImportResult = importResult;

                return View("Import", model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Import");
            }

        }

        [HttpPost]
        public ActionResult ImportSkill()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            try
            {
                var importResult = new ImportResult();
                var file = Request.Files["skill-file"];
                if (file != null && file.ContentLength > 0)
                    importResult = _importManager.ImportVendorCandidateSkillFromXlsx(file.InputStream, _workContext.CurrentAccount.FranchiseId);
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                    return RedirectToAction("Import");
                }

                if (importResult.Attempted == 0)
                {
                    if (importResult.ErrorMsg.Count > 0)
                        ErrorNotification(String.Join(" | ", importResult.ErrorMsg));
                    else
                        ErrorNotification("The excel file is empty");
                }
                else
                {
                    if (importResult.Imported > 0)
                        SuccessNotification(String.Format("[{0}] of [{1}] candidate skills imported.", importResult.Imported, importResult.Attempted));
                    if (importResult.Attempted > 0 && importResult.NotImported > 0)
                        ErrorNotification(String.Format("[{0}] of [{1}] candidate skills NOT imported. See details in the section below.", importResult.NotImported, importResult.Attempted));
                }

                var model = new VendorCandidateImportModel();
                model.VendorId = _workContext.CurrentAccount.FranchiseId;
                model.SkillImportedOn = DateTime.Now;
                model.SkillImportResult = importResult;

                return View("Import", model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Import");
            }

        }
        [HttpPost]
        public ActionResult ImportBankAccount()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            try
            {
                var importResult = new ImportResult();
                var file = Request.Files["bank-account-file"];
                if (file != null && file.ContentLength > 0)
                    importResult = _importManager.ImportVendorCandidateBankAccountFromXlsx(file.InputStream, _workContext.CurrentAccount.FranchiseId);
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                    return RedirectToAction("Import");
                }

                if (importResult.Attempted == 0)
                {
                    if (importResult.ErrorMsg.Count > 0)
                        ErrorNotification(String.Join(" | ", importResult.ErrorMsg));
                    else
                        ErrorNotification("The excel file is empty");
                }
                else
                {
                    if (importResult.Imported > 0)
                        SuccessNotification(String.Format("[{0}] of [{1}] candidate bank accounts imported.", importResult.Imported, importResult.Attempted));
                    if (importResult.Attempted > 0 && importResult.NotImported > 0)
                        ErrorNotification(String.Format("[{0}] of [{1}] candidate bank accounts NOT imported. See details in the section below.", importResult.NotImported, importResult.Attempted));
                }

                var model = new VendorCandidateImportModel();
                model.VendorId = _workContext.CurrentAccount.FranchiseId;
                model.BankAccountImportedOn = DateTime.Now;
                model.BankAccountImportResult = importResult;

                return View("Import", model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Import");
            }

        }
        #endregion


        // Add to Companies Pool
        private IList<SelectListItem> GetAllCompanies(Account account, Guid guid)
        {
            var companies = _companyService.GetAllCompaniesAsQueryable(account);

            var candidateId = _candidateService.GetCandidateByGuid(guid).Id;
            var bannedCompanies = _candidateBlacklistService.GetAllCandidateBlacklistsByDate(DateTime.Today).Where(x => x.CandidateId == candidateId).Select(x => x.ClientId);
            if (bannedCompanies.Any() && !bannedCompanies.FirstOrDefault().HasValue)
                return new List<SelectListItem>();

            companies = companies.Where(x => !bannedCompanies.Contains(x.Id));

            if (account.IsRecruiterOrRecruiterSupervisor())
            {
                List<int> companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                companies = companies.Where(x => companyIds.Contains(x.Id)).OrderBy(c => c.DisplayOrder).ThenBy(c => c.CompanyName);
            }

            var companiesDropDownList = new List<SelectListItem>();
            foreach (var c in companies)
            {
                var item = new SelectListItem()
                {
                    Text = c.CompanyName,
                    Value = c.Id.ToString()
                };
                companiesDropDownList.Add(item);
            }

            return companiesDropDownList;
        }

        #region GET :/Candidate/_AddCandidateToCompaniesPool

        public ActionResult _AddCandidateToCompaniesPool(Guid guid)
        {
            ViewBag.CandidateGuid = guid;
            ViewBag.Companies = GetAllCompanies(_workContext.CurrentAccount, guid);

            return PartialView("_AddCandidateToCompaniesPool");
        }
        #endregion

        #region POST:/Candidate/AddCandidateToCompaniesPool
        [HttpPost]
        public ActionResult AddCandidateToCompaniesPool(Guid guid, string ids, DateTime startDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuid(guid).Id;
            // Validate selected items
            if (string.IsNullOrEmpty(ids))
            {
                ErrorNotification("Please select one or more companies.");
                return RedirectToAction("Details", new { guid = guid, tabId = "tab-basic" });
            }

            List<int> selectedIds = new List<int>();
            if (ids != null)
            {
                selectedIds = ids
                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => Convert.ToInt32(x))
                                .ToList();
            }

            var bannedCompanies = _candidateBlacklistService.GetAllCandidateBlacklistsByDate(DateTime.Today).Where(x => x.CandidateId == candidateId).Select(x => x.ClientId);
            if (bannedCompanies.Any() && !bannedCompanies.FirstOrDefault().HasValue)
            {
                ErrorNotification(String.Format("The candidate cannot be added into any pools, as he/she is banned for all compnies!"));
                return RedirectToAction("Details", new { guid = guid, tabId = "tab-basic" });
            }

            foreach (var companyId in selectedIds.Where(x => !bannedCompanies.Contains(x)))
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
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Companies.Company.Candidate.Added.Successful") + " : " + candidateId.ToString());
            return RedirectToAction("Details", new { guid = guid, tabId = "tab-basic" });
        }
        #endregion

        // Tabs

        #region GET :/Candidate/_TabCandidateJobOrderList

        public ActionResult _TabCandidateJobOrderList(Guid candidateGuid, DateTime? weekStartDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            ViewBag.CandidateGuid = candidateGuid;
            if (!weekStartDate.HasValue)
                weekStartDate = DateTime.Today.AddDays((int)(DayOfWeek.Sunday - DateTime.Today.DayOfWeek)).AddDays(-7 * 26);
            ViewBag.WeekStartDate = weekStartDate;
            ViewBag.WeekStartDay = (int)DayOfWeek.Sunday;

            return PartialView();
        }

        #endregion

        #region POST:/Candidate/_TabCandidateJobOrderList

        [HttpPost]
        public ActionResult _TabCandidateJobOrderList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid, DateTime? weekStartDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var summary = new CandidateJobHistorySummary();
            if (!weekStartDate.HasValue)
                weekStartDate = DateTime.Today.AddDays((int)(DayOfWeek.Sunday - DateTime.Today.DayOfWeek)).AddDays(-7 * 26);
            var result = summary.GetCandidateJobHistorySummary(candidateId, weekStartDate.Value,
                                                               _workContext, _workTimeService, _jobOrderService, _candidateJobOrderService);
            ViewBag.CandidateGuid = candidateGuid;
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult CandidateTimeSheetForJobOrder([DataSourceRequest] DataSourceRequest request, Guid candidateGuid, int jobOrderId, DateTime? weekStartDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var summary = new CandidateJobHistorySummary();
            if (!weekStartDate.HasValue)
                weekStartDate = DateTime.Today.AddDays((int)(DayOfWeek.Sunday - DateTime.Today.DayOfWeek)).AddDays(-7 * 26);
            var result = summary.GetCandidateJobHistory(candidateId, jobOrderId, weekStartDate.Value,
                                                        _workContext, _workTimeService, _overtimeRepository, _jobOrderService, _candidateJobOrderService);

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region Pay History

        public ActionResult _TabPayHistory(Guid candidateGuid, DateTime? refDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            ViewBag.CandidateGuid = candidateGuid;

            if (!refDate.HasValue)
                refDate = DateTime.Today.AddYears(-1);
            ViewBag.RefDate = refDate;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _TabPayHistory([DataSourceRequest] DataSourceRequest request, Guid candidateGuid, DateTime? refDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (!refDate.HasValue)
                refDate = DateTime.Today.AddYears(-1);
            var result = _paymentHistoryService.GetPaymentHistoryByCandidatIdAndDate(candidate.Id, candidate.FranchiseId, refDate.Value);

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult Paystub(Guid id)
        {
            // Force the pdf document to be displayed in the browser
            Response.AppendHeader("Content-Disposition", "inline; filename=paystub.pdf;");

            var paystub = _paymentHistoryService.GetPaystub(id);
            if (paystub == null)
                return Content(_localizationService.GetResource("Common.FileNotFound"));

            return File(paystub, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        #endregion


        #region Get:/Candidate/_TabPlacement
        public ActionResult _TabPlacement(Guid candidateGuid)
        {
            ViewBag.CandidateGuid = candidateGuid;
            ViewBag.WeekStartDay = (int)DayOfWeek.Sunday;
            //DateTime startDate=DateTime.Today.StartOfWeek(DayOfWeek.Sunday);
            //IList<CandidateJobOrderPlacement> model = _candidateJobOrderPlacementService.GetCandidateJobOrderPlacement(candidateId, startDate);
            return PartialView();
        }
        #endregion

        #region _GetCandidateJobOrderPlacement
        [HttpPost]
        public ActionResult _GetCandidateJobOrderPlacement([DataSourceRequest] DataSourceRequest request, Guid candidateGuid, DateTime startDate)
        {
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            IList<CandidateJobOrderPlacement> model = _candidateJobOrderPlacementService.GetCandidateJobOrderPlacement(candidateId, startDate);
            var gridModel = new DataSourceResult()
            {
                Data = model,
                Total = model.Count,
            };

            return Json(gridModel);
        }
        #endregion


        #region GET :/Candidate/_TabSchedule

        public ActionResult _TabSchedule(Guid candidateGuid, DateTime? refDate = null)
        {
            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            ViewBag.CandidateGuid = candidateGuid;
            ViewBag.CandidateName = candidate.GetFullName();
            ViewBag.CandidateId = candidate.Id;
            ViewBag.IsActive = candidate.IsActive;
            ViewBag.OnBoarded = candidate.IsEmployee ||
                (candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started || candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished);
            ViewBag.RefDate = refDate ?? DateTime.Today;

            return PartialView();
        }

        #endregion

        #region POST:/Candidate/_TabSchedule

        [HttpPost]
        public ActionResult _TabSchedule([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var businessLogic = new CandidateSchedule_BL(_accountService, _candidateJobOrderService, _permissionService, _workContext, _jobOrderService);
            var cjo = businessLogic.GetAllPlacementsByCandidateIdAsQueryable(_workContext.CurrentAccount, candidateId);

            return Json(cjo.ToDataSourceResult(request));
        }

        [HttpPost]
        public JsonResult SavePlacement(CandidateScheduleModel placement)
        {
            bool result = true;
            string msg = null;

            if (ModelState.IsValid)
            {
                try
                {
                    var businessLogic = new CandidateSchedule_BL(_accountService, _candidateJobOrderService, _permissionService, _workContext, _jobOrderService);
                    msg = businessLogic.CreateOrSavePlacements(placement);
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
                //msg += "Placement information is incomplete:\r\n";
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                msg += String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                result = false;
            }

            return Json(new { Succeed = result, Error = msg });
        }

        [HttpPost]
        public JsonResult RemovePlacement(CandidateScheduleModel placement)
        {
            bool result = true;
            string msg = null;

            try
            {
                var existingCjo = _candidateJobOrderService.GetCandidateJobOrderById(placement.CandidateJobOrderId);
                msg = existingCjo != null ? _candidateJobOrderService.RemovePlacement(existingCjo) : "The placement doesn't exist.";
                if (!String.IsNullOrEmpty(msg)) result = false;
            }
            catch (Exception ex)
            {
                return Json(new { Succeed = result, Error = ex.Message });
            }

            return Json(new { Succeed = result, Error = msg });
        }

        #endregion


        #region GET :/Candidate/_TabCandidateJobOrderStatusHistoryList

        public ActionResult _TabCandidateJobOrderStatusHistoryList(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            ViewBag.CandidateGuid = candidateGuid; // for partial view to retrieve candidate job order

            return PartialView();
        }

        #endregion

        #region POST:/Candidate/_TabCandidateJobOrderStatusHistoryList
        [HttpPost]

        public ActionResult _TabCandidateJobOrderStatusHistoryList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var candidateJobOrderStatusHistories = _candidateJobOrderStatusHistoryService.GetCandidateJobOrderStatusHistoryByCandidateIdAsQueryable(candidateId);

            var gridModel = candidateJobOrderStatusHistories.ToDataSourceResult(request,x=>x.ToModel());

            ViewBag.CandidateId = candidateId; // for partial view to retrieve candidate job order

            return Json(gridModel);
        }

        #endregion


        #region GET :/Candidate/_TabCandidateKeySkillList

        public ActionResult _TabCandidateKeySkillList(Guid? candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
            {
                ErrorNotification("The candidate does not exist!");
                return Redirect(Request.UrlReferrer.ToString());
            }
            ViewBag.CandidateGuid = candidateGuid;
            ViewBag.CandidateId = candidate.Id;
            return View();
        }

        #endregion

        #region POST:/Candidate/_TabCandidateKeySkillList

        [HttpPost]
        public ActionResult _TabCandidateKeySkillList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            IList<CandidateKeySkill> candidateKeySkills = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidateId);

            var result = new DataSourceResult()
            {
                Data = candidateKeySkills.Select(x => x.ToModel()),
                Total = candidateKeySkills.Count,
            };

            ViewBag.CandidateGuid = candidateGuid;

            return Json(result);
        }

        #endregion


        #region GET :/Candidate/_TabCandidateTestResultList

        public ActionResult _TabCandidateTestResultList(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            IList<CandidateTestResult> testResultList = _candidateTestResultService.GetCandidateTestResultsByCandidateId(candidateId);
            List<CandidateTestResultModel> testResultModel = new List<CandidateTestResultModel>();

            foreach (var item in testResultList)
            {
                CandidateTestResultModel i = item.ToModel();
                testResultModel.Add(i);
            }
            return View(testResultModel);
        }

        #endregion

        #region GET :/Candidate/_TabCandidateAttachmentList

        public ActionResult _TabCandidateAttachmentList(Guid candidateGuid, bool displayHeader = true)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            ViewBag.CandidateId = candidateId; // for partial view
            ViewBag.CandidateGuid = candidateGuid;
            ViewBag.DisplayHeader = displayHeader;

            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabCandidateAttachmentList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            //int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var attachments = _attachmentService.GetAllCandidateAttachmentsAsQueryable(true, true).Where(x => x.Candidate.CandidateGuid == candidateGuid);
            return Json(attachments.ToDataSourceResult(request, x => x.ToModel()));
        }

        #endregion

        #region GET :/Candidate/_TabCandidatePictureList
        public ActionResult _TabCandidatePictureList(Guid candidateGuid, bool displayHeader = true)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var caididateModel = new CandidateModel()
            {
                Id = candidateId,
                AddPictureModel = new CandidatePictureModel()
                {
                    CandidateId = candidateId,
                },
                CandidateGuid = candidateGuid
            };

            ViewBag.DisplayHeader = displayHeader;
            return View(caididateModel);
        }
        #endregion

        #region GET:/Candidate/_TabIncidents
        public ActionResult _TabCandidateIncidents(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            ViewBag.CandidateGuid = candidateGuid;
            return PartialView(new IncidentReportModel[] { });
        }
        [HttpPost]
        public ActionResult _TabCandidateIncidentsList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var entities = _incidentService.GetCandidateIncidentReports(candidateId);
            var model = new List<IncidentReportModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }
        public ActionResult _NewIncidentReport(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;
            var model = new IncidentReportModel()
            {
                CandidateId = candidateId,
                CompanyList = new SelectList(_companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount).OrderBy(x => x.CompanyName).ToArray(),
                    "Id", "CompanyName"),
                IncidentCategoryList = new SelectList(_incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true).ToArray(),
                    "Id", "Description"),
                IncidentDateTimeUtc = DateTime.Today
            };
            return PartialView("_CreateOrUpdateIncident", model);
        }
        public ActionResult _EditIncidentReport(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            var model = _incidentService.GetIncidentReport(incidentReportId).ToModel();
            model.CompanyList = new SelectList(_companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount).OrderBy(x => x.CompanyName).ToArray(),
                    "Id", "CompanyName");
            model.IncidentCategoryList = new SelectList(_incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true).ToArray(),
                    "Id", "Description");
            return PartialView("_CreateOrUpdateIncident", model);
        }
        [HttpPost]
        public JsonResult _EditIncidentReport(IncidentReportModel model)
        {
            bool anyError = false;
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.ReportedByAccountId = _workContext.CurrentAccount.Id;
                try
                {
                    if (model.Id == 0)
                    {
                        _incidentService.InsertIncidentReport(entity);
                    }
                    else
                    {
                        _incidentService.UpdateIncidentReport(entity);
                    }
                }
                catch (Exception ex)
                {
                    anyError = true;
                    errorMessage = ex.ToString();
                    _logger.Error("_EditIncidentReport()", ex, userAgent: Request.UserAgent);
                }
            }
            else
            {
                anyError = true;
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_EditIncidentReport():" + errorMessage, userAgent: Request.UserAgent);
            }
            return Json(new { Error = !anyError, Message = errorMessage }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult _RemoveIncidentReport(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            if (incidentReportId > 0)
            {
                _incidentService.DeleteIncidentReport(incidentReportId);
            }
            return Content("done");
        }

        public ActionResult _ManageIncidentReportFiles(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            ViewBag.IncidentReportId = incidentReportId;
            var model = _incidentService.GetIncidentReportFiles(incidentReportId).ToArray().Select(x => x.ToModel());
            return PartialView("_AddRemoveIncidentReportFile", model);
        }
        public ActionResult SaveIncidentReportFiles(IEnumerable<HttpPostedFileBase> files, int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            // The Name of the Upload component is "attachments"
            foreach (var file in files)
            {
                // prepare
                var fileName = Path.GetFileName(file.FileName);
                var contentType = file.ContentType;

                using (Stream stream = file.InputStream)
                {
                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    // upload attachment
                    var fileEntity = new IncidentReportFile()
                    {
                        IncidentReportId = incidentReportId,
                        FileName = fileName,
                        FileStream = fileBinary,
                    };
                    _incidentService.InsertIncidentReportFile(fileEntity);
                }
                //activity log
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteIncidentReportFile(int reportFileId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            _incidentService.RemoveIncidentReportFile(reportFileId);

            // Return an empty string to signify success
            return Content("");
        }
        public ActionResult DownloadIncidentReportFile(int reportFileId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            var file = _incidentService.GetIncidentReportFile(reportFileId);
            Response.ClearContent();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
            //  manually added System.Web to this project's References.
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.AddHeader("content-disposition", "attachment; filename=" + file.FileName);
            Response.ContentType = MimeMapping.GetMimeMapping(file.FileName);

            Response.BinaryWrite(file.FileStream);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        }
        #endregion


        #region GET:/Candidate/_TabCandidateSmartCards

        public ActionResult _TabCandidateSmartCards(Guid candidateGuid, bool displayHeader = true)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            ViewBag.CandidateId = candidate.Id;
            ViewBag.EmployeeId = candidate.EmployeeId;
            ViewBag.FullName = candidate.GetFullName();
            ViewBag.CandidateGuid = candidateGuid;
            ViewBag.FranchiseId = candidate.FranchiseId;
            ViewBag.DisplayHeader = displayHeader;
            ViewBag.QrCodeStr = _candidateService.GetCandidateQrCodeStr(candidate);

            return PartialView("_TabCandidateSmartCards");
        }


        [HttpPost]
        public ActionResult _TabCandidateSmartCardsList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuid(candidateGuid).Id;

            var entities = _smartCardService.GetAllSmartCardsAsQueryable();
            entities = entities.Where(x => x.CandidateId == candidateId);
            var model = new List<CandidateSmartCardModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _GetCandidatePhoto(int candidateId)
        {
            var photoStr = String.Empty;
            var candidatePic = _candidatePictureService.GetCandidatePicturesByCandidateId(candidateId).Where(x => x.IsActive).FirstOrDefault();
            if (candidatePic != null)
                photoStr = Convert.ToBase64String(_candidatePictureService.LoadCandidatePictureBinary(candidatePic));

            return Json(new { Success = photoStr != null && photoStr.Length > 0, PhotoStr = photoStr });
        }


        [HttpPost]
        public ActionResult _SendQrCode(int candidateId)
        {
            var candidate = _candidateService.GetCandidateById(candidateId);
            var msgId = _workflowMessageService.SendCandidateIdQrCode(candidate, 1);

            return Json(new { Succeed = msgId > 0 });
        }

        #endregion


        #region Hand Templates

        public ActionResult _TabHandTemplates(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            ViewBag.CandidateId = candidate.Id;

            return PartialView();
        }

        #endregion


        #region _TabCandidateMessages

        public ActionResult _TabCandidateMessages(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            ViewBag.CandidateGuid = candidateGuid;
            return PartialView();
        }


        [HttpPost]
        public ActionResult _TabCandidateMessageList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuid(candidateGuid);
            if (candidate != null)
            {
                _CustomizeSortsAndFilters(request);   // fix sort/filter issues
                var messages = _messageService.GetMessagesByAccount(candidate.Id, IsCandidate: true);
                return Json(messages.ToDataSourceResult(request, x => x.ToModel()));
            }

            return Json(null);
        }


        private void _CustomizeSortsAndFilters([DataSourceRequest] DataSourceRequest request)
        {
            if (request.Sorts.Any())
            {
                var sort = request.Sorts.FirstOrDefault(x => x.Member == "CreatedOn");
                if (sort != null)
                {
                    var order = sort.SortDirection;
                    request.Sorts.Remove(sort);
                    request.Sorts.Add(new SortDescriptor("MessageHistory.CreatedOnUtc", order));
                }

                sort = request.Sorts.FirstOrDefault(x => x.Member == "Subject");
                if (sort != null)
                {
                    var order = sort.SortDirection;
                    request.Sorts.Remove(sort);
                    request.Sorts.Add(new SortDescriptor("MessageHistory.Subject", order));
                }
            }

            if (request.Filters.Any())
            {
                var filter = request.Filters.OfType<FilterDescriptor>().FirstOrDefault(x => x.Member == "Subject");
                if (filter != null)
                {
                    var newFilter = new FilterDescriptor("MessageHistory.Subject", filter.Operator, filter.Value);
                    request.Filters.Remove(filter);
                    request.Filters.Add(newFilter);
                }
            }
        }


        public ActionResult _CandidateMessageDetails(int messageId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var model = _messageService.GetMessageById(messageId).ToModel();

            return PartialView("_CandidateMessageDetails", model);
        }

        #endregion


        #region Reset Password
        public ActionResult _ResetPassword(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowResetCandidatePassword))
                return AccessDeniedView();
            Candidate candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                ErrorNotification("The candidate does not exist!");
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult _ResetPassword(Guid? guid, string password)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowResetCandidatePassword))
                return AccessDeniedView();

            Candidate candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate != null)
            {
                try
                {
                    var result = _candidateService.ResetPassword(password, password, null, candidate.Username);
                    if (String.IsNullOrWhiteSpace(result))
                        return Json(new { Error = false, Message = "" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Error = true, Message = result });
                }
                catch (Exception ex)
                {
                    _logger.Error("_ResetPassword():", ex, userAgent: Request.UserAgent);
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { Error = true, Message = "Cannot find such a candidate!" }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Mass Message

        public ActionResult MassMessage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToCandidates))
                return AccessDeniedView();

            List<SelectListItem> templates = _messageTemplateService.GetAllMassEmailTemplates().ToList();
            templates.AddRange(_companyEmailTemplateService.GetAllEmailTemplateAsSelectedList());

            ViewBag.EmailTemplates = templates;
            return View();
        }


        [HttpPost]
        public ActionResult _GetTargetCandidates([DataSourceRequest] DataSourceRequest request, CandidateProfile candidateProfile, PoolProfile poolProfile, PipelineProfile pipelineProfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var result = _candidateBL.GetCandidateModelListByProfiles(_workContext.CurrentAccount, candidateProfile, poolProfile, pipelineProfile, request);

            return Json(result);
        }


        public ActionResult _PoolProfile()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return PartialView();
        }


        public ActionResult _PipelineProfile()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            return PartialView();
        }

        [HttpPost]
        public ActionResult GetEmailTemplate(string templateId)
        {
            if (templateId == "0")
                return Json(new { subject = "", message = "", globalPool = true, companyPool = true, joborderPipeline = true });

            string[] tem = templateId.Split(new char[] { '-' });
            if (tem[0] == "CMP")
            {
                var companyTemplate = _companyEmailTemplateService.Retrieve(Convert.ToInt32(tem[1]));
                return Json(new { subject = companyTemplate.Subject, message = companyTemplate.Body, globalPool = false, companyPool = false, joborderPipeline = true });
            }
            else // (tem[0] == "GE")
            {
                var generalTemplate = _messageTemplateService.GetMessageTemplateById(Convert.ToInt32(tem[1]));
                return Json(new { subject = generalTemplate.Subject, message = generalTemplate.Body, globalPool = true, companyPool = true, joborderPipeline = true });
            }
        }

        [HttpPost]
        public ActionResult _QuickTextMessage(string textMessage, string numbers)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToCandidates))
                return AccessDeniedView();

            var total = numbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
            var done = 0;
            string errorMessage = string.Empty;
            if (!String.IsNullOrWhiteSpace(textMessage) && !string.IsNullOrEmpty(numbers))
            {
                if (textMessage.Length <= 1600)
                {
                done = _textMessageSender.SendTextMessage(textMessage, numbers);
                _activityLogService.InsertActivityLog("SendMassMessage", _localizationService.GetResource("ActivityLog.SendMassMessageToCandidate"),
                    _workContext.CurrentAccount.FullName, done);
            }
            else
                    errorMessage += "\r\n Text message exceeds its max length!";
            }
            else
                errorMessage += "\r\n The text message or phone number is empty.";

            return Json(new { Done = done, Failed = total - done, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used for text messages or the free format emails (not from templates)
        /// </summary>
        /// <param name="textMessage"></param>
        /// <param name="systemAccount"></param>
        /// <param name="subject"></param>
        /// <param name="emailMessage"></param>
        /// <param name="selectedIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _MassMessage2Selected(string textMessage, bool systemAccount, string subject, string emailMessage, string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToCandidates))
                return AccessDeniedView();

            var total = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
            var done = 0;
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                // text message
                if (!String.IsNullOrWhiteSpace(textMessage))
                {                 
                    if (textMessage.Length <= 1600)
                    {
                    done = _candidateBL.SendMassMessageToSelectedCandidates(selectedIds, textMessage);
                    _activityLogService.InsertActivityLog("SendMassMessage", _localizationService.GetResource("ActivityLog.SendMassMessageToCandidate"),
                        _workContext.CurrentAccount.FullName, done);
                }
                    else
                        errorMessage += "\r\n Text message exceeds its max length!";
                }
                // email message
                if (!String.IsNullOrWhiteSpace(emailMessage))
                {
                        _candidateBL.SendMassEmailToSelectedCandidates(_workContext.CurrentAccount, selectedIds, subject, emailMessage, 0, systemAccount);
                        done = total;
                    }
                }
            else
                errorMessage += "\r\n No candidates are selected.";

            return Json(new { Done = done, Failed = total - done, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _MassMessage2All(string textMessage, bool systemAccount, string subject, string emailMessage, CandidateProfile candidateProfile, PoolProfile poolProfile, PipelineProfile pipelineProfile)
        {
            var ids = _candidateBL.GetCandidateListByProfiles(_workContext.CurrentAccount, candidateProfile, poolProfile, pipelineProfile).Select(x => x.Id);

            return _MassMessage2Selected(textMessage, systemAccount, subject, emailMessage, String.Join<int>(",", ids));
        }


        /// <summary>
        /// Preview the email
        /// </summary>
        /// <returns></returns>
        public ActionResult _PreviewEmail(int? jobOrderId, DateTime? startDate, DateTime? endDate, string templateId)
        {
            if (String.IsNullOrWhiteSpace(templateId) || templateId == "0")
                return null;

            string error = string.Empty;

            if (!startDate.HasValue) startDate = DateTime.MinValue;
            QueuedEmailModel model;
            QueuedMailModel_BL model_bl = new QueuedMailModel_BL(_jobOrderService, _candidateJobOrderService, _candidateService, _exportManager, _workflowMessageService, _workContext, _companyEmailTemplateService,_logger);

            bool isfromCompanyTemplates;
            bool isFromGenericTemplates;

            int _id = this.convertCodedTemplateId(templateId, out isfromCompanyTemplates, out isFromGenericTemplates);
            if (isfromCompanyTemplates)
            {
                if (!jobOrderId.HasValue) jobOrderId = 0;

                model = model_bl.GetPreviewEmailModel("CMP", jobOrderId.Value, startDate.Value, endDate, _id, out error);
            }
            else
            {
                model = model_bl.GetPreviewEmailModel("GE", 0, startDate.Value, endDate, _id, out error);
            }

            if (error.Length > 0)
            {
                _logger.Error(error, userAgent: Request.UserAgent);
                ErrorNotification(error);
            }

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _SendTemplateEmail(bool fromUserAccount, int? jobOrderId, DateTime? startDate, DateTime? endDate, string templateId, string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToCandidates))
                return AccessDeniedView();

            string error = string.Empty;
            QueuedMailModel_BL model_bl = new QueuedMailModel_BL(_jobOrderService, _candidateJobOrderService, _candidateService, _exportManager, _workflowMessageService, _workContext, _companyEmailTemplateService,_logger);


            bool isfromCompanyTemplates;
            bool isFromGenericTemplates;

            int _id = this.convertCodedTemplateId(templateId, out isfromCompanyTemplates, out isFromGenericTemplates);
            if (isfromCompanyTemplates)
            {
                if (!jobOrderId.HasValue || jobOrderId <= 0) return Json(new { message = "Invalid job order!", error = true });
                if (!startDate.HasValue) return Json(new { message = "Start date is required!", error = true });

                model_bl.SendJobOrderConfirmationEmail(jobOrderId.Value, startDate.Value, endDate, _id, selectedIds, out error);
            }
            else
            {
                model_bl.SendGeneralEmail(_id, selectedIds,jobOrderId,startDate,endDate, out error);
            }

            if (error.Length > 0)
            {
                _logger.Error(error, userAgent: Request.UserAgent);
                return Json(new { message = error, error = true });
            }
            else
            {
                return Json(new { message = "Email has been sent to selected candidates!", error = false });
            }
        }

        [HttpPost]
        public ActionResult _SendTemplateEmailToAll(string textMessage, bool systemAccount, string subject, string emailMessage, CandidateProfile candidateProfile, PoolProfile poolProfile, PipelineProfile pipelineProfile, string templateId)
        {
            var ids = _candidateBL.GetCandidateListByProfiles(_workContext.CurrentAccount,candidateProfile, poolProfile, pipelineProfile).Select(x => x.Id);
            if(pipelineProfile!=null)
                return _SendTemplateEmail(!systemAccount, pipelineProfile.JobOrderId, pipelineProfile.StartDate, pipelineProfile.EndDate, templateId, String.Join<int>(",", ids));
            else
                return _SendTemplateEmail(!systemAccount, null, null, null, templateId, String.Join<int>(",", ids));
        }
        private int convertCodedTemplateId(string strTemplateId, out bool isfromCompanyTemplates, out bool isFromGenericTemplates)
        {
            int result = 0;

            string[] temp = strTemplateId.Split(new char[] { '-' });
            if (temp.Length == 2)
            {
                isfromCompanyTemplates = (temp[0] == "CMP");
                isFromGenericTemplates = !isfromCompanyTemplates;
                result = Convert.ToInt32(temp[1]);
            }
            else
            {
                isfromCompanyTemplates = false;
                isFromGenericTemplates = false;
            }

            return result;
        }
        #endregion


        #region Show Candidate Note before View or Edit
        public ActionResult CandidateNote(Guid? guid)
        {
            var entity = _candidateService.GetCandidateByGuid(guid);
            if (entity == null)
            {
                _logger.Error("CandidateNote():Candidate does not exist!");
                ErrorNotification("Fail to read data!");
                return PartialView();
            }
            return PartialView("CandidateNote", entity.Note);
        }
        #endregion


        #region Bank Accounts

        public ActionResult _TabBankAccounts(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuid(guid);
            ViewBag.CandidateId = candidate != null ? candidate.Id : 0;
            ViewBag.CandidateGuid = guid;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _CandidateBankAccounts(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuid(guid);
            var accounts = _candidateBankAccountService.GetAllCandidateBankAccountsByCandidateId(candidate != null ? candidate.Id : 0).Where(x => !x.IsDeleted);
            var models = accounts.ProjectTo<CandidateBankAccountModel>().ToList();
            models.ForEach(x => x.AccountNumber = x.AccountNumber.MaskExceptLastDigits(4));

            return Json(new DataSourceResult() { Data = models, Total = models.Count });
        }

        #endregion


        #region Helper actions
        private IEnumerable<SelectListItem> _getEmployeeList(int franchiseId, string idString, int maxNum, bool activeOnly)
        {
            var result = Enumerable.Empty<SelectListItem>();

            idString = idString.Trim();

            if (!String.IsNullOrWhiteSpace(idString))
                result = _candidateService.GetAllEmployeesAsQueryable(_workContext.CurrentAccount, !activeOnly, !activeOnly)
                        .Where(x => x.FranchiseId == franchiseId && x.Id.ToString().StartsWith(idString))
                        .OrderBy(x => x.EmployeeId)
                        .Select(x => new SelectListItem()
                        {
                            Value = x.Id.ToString(),
                            Text = String.Concat(x.Id.ToString(), " - ", x.FirstName, " ", x.LastName)
                        }).Take(maxNum).OrderBy(x => x.Text);

            return result;
        }

        [HttpGet]
        // This action only returns the active employees and is used for Creating new accounts. We shouldn't show inactive employees for new accounts
        public JsonResult GetCascadeActiveEmployees(int franchiseId, string idString, int maxNum = 100)
        {
            var result = this._getEmployeeList(franchiseId, idString, maxNum, true).AsQueryable();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // This action only returns the active and inactive employees and is used for editing new accounts. 
        // We should show both active and inactive employees for the existing account
        public JsonResult GetCascadeEmployees(int franchiseId, string idString, int maxNum = 100)
        {
            var result = this._getEmployeeList(franchiseId, idString, maxNum, false).AsQueryable();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetAllCandidatesAsSelectList(string idString, int maxNum = 100)
        {
            var result = Enumerable.Empty<SelectListItem>().AsQueryable();

            if (!String.IsNullOrWhiteSpace(idString))
                result = _candidateService.GetAllCandidatesAsQueryable(_workContext.CurrentAccount)
                        .Where(x => x.Id.ToString().StartsWith(idString))
                        .Select(x => new SelectListItem()
                        {
                            Value = x.Id.ToString(),
                            Text = String.Concat(x.Id.ToString(), " - ", x.FirstName, " ", x.LastName)
                        }).Take(maxNum).OrderBy(x => x.Text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class EmployeeName
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        [HttpGet]
        public ActionResult _GetEmployeeName(int franchiseId, int candidateId)
        {
            var result = new EmployeeName() { FirstName = String.Empty, LastName = String.Empty};

            if (franchiseId > 0 && candidateId > 0)
            {
                var candidate = _candidateService.GetCandidateById(candidateId);
                if (candidate.FranchiseId == franchiseId)
                {
                    result.FirstName = candidate.FirstName;
                    result.LastName = candidate.LastName;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}

using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.JobOrder;
using Wfm.Admin.Models.TimeSheet;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.ExportImport;
using Wfm.Services.Features;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Logging;
using Wfm.Services.Payroll;
using Wfm.Services.Policies;
using Wfm.Services.Security;
using Wfm.Services.Seo;
using Wfm.Services.Test;
using Wfm.Services.TimeSheet;
using Wfm.Web.Framework.Controllers;
using Wfm.Services.Messages;
using Wfm.Admin.Models.Messages;
using Wfm.Shared.Models.Accounts;


namespace Wfm.Admin.Controllers
{

    public class JobOrderController : BaseAdminController
    {
        #region Fields

        private readonly IOvertimeRuleSettingService _overtimeRuleSettingService;
        private readonly IRepository<JobOrderOvertimeRule> _jobOrderOvertimeRuleRepository;
        private readonly IJobOrderService _jobOrderService;
        private readonly IJobOrderTestCategoryService _jobOrderTestCategoryService;
        private readonly IActivityLogService _activityLogService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateAvailabilityService _availabilityService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ICandidateService _candidatesService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ITestService _testService;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IExportManager _exportManager;
        private readonly IPdfService _pdfService;
        private readonly ISchedulePolicyService _schedulePolicyService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IJobOrderStatusService _jobOrderStatusService;
        private readonly IAttendanceListService _attendanceListService;
        private readonly IBasicJobOrderInfoService _basicJobOrderService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IFranchiseService _franchiseService;
        private readonly JobOrderModel_BL _jobOrderModel_BL;
        private readonly JobOrderCandidateModel_BL _jobOrderCandidate_BL;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPositionService _positionService;
        private readonly IShiftService _shiftService;
        private readonly IWebHelper _webHelper;
        private readonly IJobBoardService _jobBoardService;
        private readonly ICompanyEmailTemplateService _companyEmailTemplateService;
        private readonly IWebService _webService;
        private readonly IUserFeatureService _userFeatureService;
        
        #endregion


        #region Ctor

        public JobOrderController(IJobOrderService jobOrderService,
            IJobOrderTestCategoryService jobOrderTestCategoryService,
            IActivityLogService activityLogService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateAvailabilityService availabilityService,
            ICompanyService companyService,
            ICompanyCandidateService companyCandidateService,
            ICompanyDepartmentService companyDepartmentService,
            ICompanyDivisionService companyDivisionService,
            ICompanyContactService companyContactService,
            ICandidateService candidateService,
            ITestService testService,
            IAccountService accountService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ICompanyBillingService companyBillingService,
            ILocalizationService localizationService,
            IPdfService pdfService,
            ILogger logger,
            IExportManager exportManager,
            ISchedulePolicyService schedulePolicyService,
            IUrlRecordService urlRecordService,
            IWorkTimeService workTimeService,
            IOvertimeRuleSettingService overtimeRuleSettingService,
            IRepository<JobOrderOvertimeRule> jobOrderOvertimeRuleRepository,
            IAttendanceListService attendanceListService,
            IBasicJobOrderInfoService basicJobOrderService,
            IRecruiterCompanyService recruiterCompanyService,
            IFranchiseService franchiseService,
            IJobOrderStatusService jobOrderStatusService,
            IWorkflowMessageService workflowMessageService,
            IQueuedEmailService queuedEmailService,
            IStateProvinceService stateProvinceService,
            IPositionService positionService,
            IShiftService shiftService,
            IWebHelper webHelper,
            ICompanyEmailTemplateService companyEmailTemplateService,
            IJobBoardService jobBoardService,
            IWebService webService,
            IUserFeatureService userFeatureService
        )
        {
            _overtimeRuleSettingService = overtimeRuleSettingService;
            _jobOrderOvertimeRuleRepository = jobOrderOvertimeRuleRepository;
            _jobOrderService = jobOrderService;
            _jobOrderTestCategoryService = jobOrderTestCategoryService;
            _activityLogService = activityLogService;
            _candidateJobOrderService = candidateJobOrderService;
            _availabilityService = availabilityService;
           
            _companyService = companyService;
            _companyCandidateService = companyCandidateService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyContactService = companyContactService;
            _candidatesService = candidateService;
            _testService = testService;
            _accountService = accountService;
            _workContext = workContext;
            _permissionService = permissionService;
            _companyBillingService = companyBillingService;
            _localizationService = localizationService;
            _logger = logger;
            _pdfService = pdfService;
            _exportManager = exportManager;
            _schedulePolicyService = schedulePolicyService;
            _urlRecordService = urlRecordService;
            _workTimeService = workTimeService;
            _attendanceListService = attendanceListService;
            _basicJobOrderService = basicJobOrderService;
            _recruiterCompanyService = recruiterCompanyService;
            _franchiseService = franchiseService;
            _jobOrderStatusService = jobOrderStatusService;
            _workflowMessageService = workflowMessageService;
            _queuedEmailService = queuedEmailService;
            _stateProvinceService = stateProvinceService;
            _positionService = positionService;
            _shiftService = shiftService;
            _webHelper = webHelper;
            _companyEmailTemplateService = companyEmailTemplateService;
            _jobBoardService = jobBoardService;
            _webService = webService;
            _userFeatureService = userFeatureService;

            _jobOrderModel_BL = new JobOrderModel_BL(_workContext, _localizationService, _jobOrderService, _jobOrderTestCategoryService,_positionService,_companyService, _accountService);
            _jobOrderCandidate_BL = new JobOrderCandidateModel_BL(_jobOrderService, _candidateJobOrderService, 
                _workTimeService, _companyCandidateService, _candidatesService, _availabilityService,
                _accountService, _franchiseService, _workContext, _companyService,
                _workflowMessageService);
        }

        #endregion


        #region Helper
        private string GetCandidateKeySkillSet(List<CandidateKeySkill> s)
        {
            IList<String> ltStr = new List<String>();
            foreach (var item in s)
            {
                ltStr.Add(item.KeySkill);
            }

            return string.Join(" / ", ltStr);
        }

        #endregion

        #region GET :/JobOrder/Index
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            return View();
        }
        #endregion

        #region POST:/JobOrder/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request, DateTime? startDate, DateTime? endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();
            IList<BasicJobOrderInfo> jobOrders = _basicJobOrderService.GetAllBasicJobOrderInfoByDate(startDate, endDate);

            return Json(jobOrders.ToDataSourceResult(request));
        }

        #endregion

        #region GET :/JobOrder/Create

        public ActionResult Create(int companyId = 0)
        {
            if (!_jobOrderModel_BL.PermitCreateJobOrder(_workContext, _permissionService))
                return AccessDeniedView();
            JobOrderModel model = _jobOrderModel_BL.CreateNewJobOrderModel(companyId, _workContext, _jobOrderStatusService);
            return View(model);
        }

        #endregion

        #region POST:/JobOrder/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(JobOrderModel model, bool continueEditing, int[] RequiredTests)
        {
            if (!_jobOrderModel_BL.PermitCreateJobOrder(_workContext, _permissionService))
                return AccessDeniedView();

            #region Key data validation

            var msg = _jobOrderModel_BL.ValidateJobOrderModel(model);
            if (!String.IsNullOrEmpty(msg))
            {
                ErrorNotification(msg);
                return View(model);
            }
            ValidateCompanyAndVendor(model);
            #endregion

            if (ModelState.IsValid)
            {
                #region Warning message

                msg = _jobOrderModel_BL.CheckJobOrderCompleteness(model);
                if (!String.IsNullOrEmpty(msg))
                    WarningNotification(msg);

                #endregion

                //add new
                var jobOrder = _jobOrderModel_BL.CreateJobOrderFromModel(model);

                JobOrderModel jobOrderSEO = jobOrder.ToModel();
                jobOrderSEO.SeName = jobOrder.Id + "-" + jobOrder.JobTitle.ToLower().Replace(" ", "-");
                _urlRecordService.SaveSlug(jobOrder, jobOrderSEO.SeName, 1);

                //activity log
                _activityLogService.InsertActivityLog("AddNewJobOrder", _localizationService.GetResource("ActivityLog.AddNewJobOrder"), jobOrder.Id.ToString() + " / " + jobOrder.JobTitle);

                //add tests
                if (RequiredTests != null)
                    _jobOrderModel_BL.AddJobOrderTestCategories(jobOrder.Id, RequiredTests);

                SuccessNotification(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added"));
                return continueEditing ? RedirectToAction("Edit", new { guid = jobOrder.JobOrderGuid }) : RedirectToAction("Details", new { guid = jobOrder.JobOrderGuid });
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region GET :/JobOrder/CreatejobOrder

        public ActionResult CreateJobOrder()
        {
            if (!_jobOrderModel_BL.PermitCreateJobOrder(_workContext, _permissionService))
                return AccessDeniedView();

            ViewBag.NewPage = true;
            JobOrderModel model = _jobOrderModel_BL.CreateNewJobOrderModel(0, _workContext, _jobOrderStatusService);
            return View(model);
        }

        #endregion
        #region POST:/JobOrder/CreateJobOrder

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult CreateJobOrder(JobOrderModel model, bool continueEditing, int[] RequiredTests)
        {
            if (!_jobOrderModel_BL.PermitCreateJobOrder(_workContext, _permissionService))
                return AccessDeniedView();

            #region Key data validation 

            var msg = _jobOrderModel_BL.ValidateJobOrderModel(model);
            if (!string.IsNullOrEmpty(msg))
            {
                ErrorNotification(msg);
                ViewBag.NewPage = false;
                ViewBag.StartIndex = 1;
                return View(model);
            }
            ValidateCompanyAndVendor(model);

            #endregion

            if (ModelState.IsValid)
            {
                #region Warning message

                msg = _jobOrderModel_BL.CheckJobOrderCompleteness(model);
                if (!String.IsNullOrEmpty(msg))
                    WarningNotification(msg);

                #endregion

                //add new
                var jobOrder = _jobOrderModel_BL.CreateJobOrderFromModel(model);

                JobOrderModel jobOrderSEO = jobOrder.ToModel();
                jobOrderSEO.SeName = jobOrder.Id + "-" + jobOrder.JobTitle.ToLower().Replace(" ", "-");
                _urlRecordService.SaveSlug(jobOrder, jobOrderSEO.SeName, 1);

                //activity log
                _activityLogService.InsertActivityLog("AddNewJobOrder", _localizationService.GetResource("ActivityLog.AddNewJobOrder"), jobOrder.Id.ToString() + " / " + jobOrder.JobTitle);

                //add tests
                if (RequiredTests != null)
                    _jobOrderModel_BL.AddJobOrderTestCategories(jobOrder.Id, RequiredTests);

                SuccessNotification(_localizationService.GetResource("Admin.JobOrder.JobOrder.Added"));
                return continueEditing ? RedirectToAction("Edit", new { guid = jobOrder.JobOrderGuid }) : RedirectToAction("Details", new { guid = jobOrder.JobOrderGuid });
            }

            //If we got this far, something failed, redisplay form
            ViewBag.NewPage = false;
            return View(model);
        }

        #endregion
        #region Job Order Wizard      
      
        public ActionResult _CreateJobOrderInformation(JobOrderModel model)
        {
            if (!_jobOrderModel_BL.PermitCreateJobOrder(_workContext, _permissionService))
                return AccessDeniedView();
           
            return PartialView(model);
        }


        public JsonResult GetCascadeShift(string companyId, bool withRate = true) 
        {
            var cmpId = String.IsNullOrEmpty(companyId) ? 0 : Convert.ToInt32(companyId);
            var companyShift = _userFeatureService.CheckFeatureByName(cmpId, "Rescheduling");
            var shifts = _shiftService.GetAllShifts(companyId: companyShift ? cmpId : 0);
            if (withRate)
            {
                var companyBillingRateShift = _companyBillingService.GetAllCompanyBillingRatesByCompanyIdAndRefDate(cmpId, null).Select(x => x.ShiftCode).ToList();
                shifts = shifts.Where(s => companyBillingRateShift.Contains(s.ShiftName)).ToList();
            }

            return Json(shifts.Select(x => new SelectListItem() { Text = x.ShiftName, Value = x.Id.ToString() }), JsonRequestBehavior.AllowGet);
        }

       #endregion

        #region GET :/JobOrder/Edit/5

        [HttpGet]
        public ActionResult Edit(Guid? guid)
        {
            JobOrder jobOrder = _jobOrderService.GetJobOrderByGuid(guid);

            if (jobOrder == null)
                return RedirectToAction("Index");
            if (!_jobOrderModel_BL.PermitEditJobOrder(jobOrder, _workContext, _permissionService))
                return AccessDeniedView();

            JobOrderModel model = jobOrder.ToModel();
            var vendor = _franchiseService.GetFranchiseById(model.FranchiseId);
            if (vendor != null)
                model.FranchiseGuid = vendor.FranchiseGuid;
            return View(model);
        }

        #endregion

        #region POST:/JobOrder/Edit/5

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(JobOrderModel model, bool continueEditing, int[] RequiredTests)
        {
            ValidateCompanyAndVendor(model);
            if (ModelState.IsValid)
            {
                string errorMsg = String.Empty;

                int errorCode = _jobOrderModel_BL.EditJobOrder(model, RequiredTests, _permissionService, out errorMsg);

                switch (errorCode)
                {
                    case 0: // No errors found
                        //activity log
                        _activityLogService.InsertActivityLog("UpdateJobOrder", _localizationService.GetResource("ActivityLog.UpdateJobOrder"), model.Id.ToString() + " / " + model.JobTitle);

                        if (!String.IsNullOrWhiteSpace(errorMsg)) WarningNotification(errorMsg);

                        SuccessNotification(_localizationService.GetResource("Admin.JobOrder.JobOrder.Updated"));
                        return continueEditing ? RedirectToAction("Edit", new { guid = model.JobOrderGuid }) : RedirectToAction("Details", new { guid = model.JobOrderGuid });
                    case -1:
                        return RedirectToAction("Index");
                    case -2:
                        return AccessDeniedView();
                    case -3:
                        ErrorNotification(errorMsg);
                        return View(model);
                }
            }
            return View(model);
        }

        #endregion


        #region GET :/JobOrder/_SetJobOrderEndDate

        [HttpGet]
        public PartialViewResult _SetJobOrderEndDate(Guid? guid)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder != null)
            {
                ViewBag.StartDate = jobOrder.StartDate;
                if (jobOrder.EndDate.HasValue)
                    ViewBag.EndDate = jobOrder.EndDate;
                if (jobOrder.JobOrderCloseReason.HasValue)
                    ViewBag.JobOrderCloseReason = ((int)(jobOrder.JobOrderCloseReason)).ToString();
            }

            return PartialView("_CloseJobOrder");
        }

        #endregion


        #region POST:/JobOrder/_SetJobOrderEndDate

        [HttpPost]
        public JsonResult _SetJobOrderEndDate(Guid? guid, DateTime? EndDate, int? JobOrderCloseReason)
        {
            bool anyError = false;
            string errorMessage = string.Empty;

            if (!EndDate.HasValue || !JobOrderCloseReason.HasValue)
            {
                anyError = true;
                errorMessage += "\r\n" + "End Date and Close Reason is required.";
            }

            if (!anyError)
            {
                var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
                if (jobOrder == null)
                {
                    anyError = true;
                    errorMessage += "\r\nJob Order does not exist !";
                }
                else
                {
                    jobOrder.EndDate = EndDate;
                    jobOrder.JobOrderCloseReason = (JobOrderCloseReasonCode)JobOrderCloseReason;

                    // unnecessary from here, or need to do more check before close !!!
                    // jobOrder.JobOrderStatusId = (int)JobOrderStatusEnum.Closed;

                    //Change all placed candidates end date 
                    _candidateJobOrderService.SetEndDateAfterClosingJobOrder(jobOrder.Id, EndDate.Value);

                    // update
                    _jobOrderService.UpdateJobOrder(jobOrder);

                    //log
                    _activityLogService.InsertActivityLog("SetJobOrderEndDate", _localizationService.GetResource("ActivityLog.SetJobOrderEndDate"), jobOrder.Id);
                }
            }

            return Json(new { Result = !anyError, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region GET :/JobOrder/JobOrderTest

        public ActionResult JobOrderTest(Guid? guid,bool editMode = false)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            IList<TestCategory> availableTests = _testService.GetAllCategories();
            ViewBag.AvailableTests = availableTests;
            List<JobOrderTestCategory> testCategory = new List<JobOrderTestCategory>();
            ViewBag.JobOrderTestCategory = testCategory;
            if (jobOrder != null)
            {
                var JobOrderTestList = _jobOrderTestCategoryService.GetJobOrderTestCategoryByJobOrderId(jobOrder.Id);
                if (JobOrderTestList.Count() > 0)
                {
                    testCategory = JobOrderTestList.ToList();
                    ViewBag.JobOrderTestCategory = testCategory;
                }
            }

            ViewBag.EditMode = editMode;

            return PartialView("JobOrderTest");
        }

        #endregion


        #region GET :/JobOrder/Details/5
        [HttpGet]
        public ActionResult Details(Guid? guid, DateTime? refDate, string tabId)
        {
            JobOrder jobOrder = _jobOrderService.GetJobOrderByGuid(guid);

            if (jobOrder == null)
            {
                ErrorNotification("Job order not found!");
                return RedirectToAction("Index");
            }

            if (!_jobOrderModel_BL.PermitViewDetaials(jobOrder, _workContext, _recruiterCompanyService, _permissionService))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(tabId))
                ViewBag.TabId = tabId;

            if (TempData["IncludePlacedCandidates"] != null)
            {
                ViewBag.IncludePlacedCandidates = true;
            }
            else
            {
                ViewBag.IncludePlacedCandidates = false;
            }
            JobOrderModel model = _jobOrderModel_BL.GetModelFromJobOrder(refDate, jobOrder, _companyDivisionService, _companyDepartmentService, _schedulePolicyService, _companyContactService, _companyBillingService, _accountService);
            return View(model);

        }
        #endregion


        #region GET :/JobOrder/CopyJobOrder
        [HttpGet]
        public ActionResult CopyJobOrder(Guid? guid)
        {
            JobOrder jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
                return RedirectToAction("Index");

            if (!_jobOrderModel_BL.PermitCreateJobOrder( _workContext,_permissionService) || jobOrder.JobPostingId!=null)
                return AccessDeniedView();
            
            JobOrderModel model = _jobOrderModel_BL.GetACopyOfJobOrder(jobOrder, _workContext);
            var vendor = _franchiseService.GetFranchiseById(model.FranchiseId);
            if (vendor != null)
                model.FranchiseGuid = vendor.FranchiseGuid;
            ViewBag.OrigJobOrderGuid = guid;
            
            return View(model);
        }
        #endregion

        #region POST:/JobOrder/CopyJobOrder
        [HttpPost]
        public ActionResult CopyJobOrder(JobOrderModel model, int[] TestRequired, Guid? origJobOrderGuid)
        {
            if (!_jobOrderModel_BL.PermitCreateJobOrder(_workContext, _permissionService))
                return AccessDeniedView();

            ViewBag.OrigJobOrderGuid = origJobOrderGuid;
            #region Key data validation

            var msg = _jobOrderModel_BL.ValidateJobOrderModel(model);
            if (!String.IsNullOrEmpty(msg))
            {
                ErrorNotification(msg);
                return View(model);
            }
            ValidateCompanyAndVendor(model);
            #endregion

            if (ModelState.IsValid)
            {
                #region Warning message

                msg = _jobOrderModel_BL.CheckJobOrderCompleteness(model);
                if (!String.IsNullOrEmpty(msg))
                    WarningNotification(msg);

                #endregion

                //copy as new
                JobOrder jobOrderCopy = model.ToEntity();
                jobOrderCopy.StartTime = jobOrderCopy.StartDate.Date + jobOrderCopy.StartTime.TimeOfDay;
                jobOrderCopy.EndTime = jobOrderCopy.StartDate.AddDays(jobOrderCopy.EndTime.TimeOfDay > jobOrderCopy.StartTime.TimeOfDay ? 0 : 1).Date +
                                       jobOrderCopy.EndTime.TimeOfDay;
                jobOrderCopy.EnteredBy = _workContext.CurrentAccount.Id;
                jobOrderCopy.JobPostingId = null; // Do not copy the original record's job posting Id, because it was generated by system using the Job Posting's publish
                var jo = _jobOrderService.CopyJobOrder(jobOrderCopy, origJobOrderGuid);

                //activity log
                _activityLogService.InsertActivityLog("CopyJobOrder", _localizationService.GetResource("ActivityLog.CopyJobOrder"), jo.Id + "/" + jo.JobTitle);

                SuccessNotification(_localizationService.GetResource("Admin.JobOrder.JobOrder.Copied"));
                return RedirectToAction("Details", new { guid = jo.JobOrderGuid });
            }

            return View(model);
        }

        #endregion


        #region Export Attandent List to Pdf

        public ActionResult ExportPdfAll(Guid? guid)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            var candidatesInPlaced = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndStatusId(jobOrder.Id, (int)CandidateJobOrderStatusEnum.Placed);
            List<Candidate> candidateList = new List<Candidate>();

            foreach (var item in candidatesInPlaced)
            {
                Candidate candidate = _candidatesService.GetCandidateById(item.CandidateId);
                candidateList.Add(candidate);
            }

            try
            {
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintCandidatesToPdf(stream, candidateList);
                    bytes = stream.ToArray();
                }

                string datetimename = System.DateTime.Now.ToString("yyyy-dd-MM-HH:mm:ss");
                return File(bytes, "text/pdf", "Attendant List" + datetimename + ".pdf");
            }

            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Index");
            }

        }

        #endregion

        #region Export Attandent List to Excel

        public ActionResult _ExportAttendantList(Guid? guid)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            var jobOrderDate = new JobOrderDateModel();
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            ViewBag.JobOrderGuid = guid;
                jobOrderDate.JobOrderId = jobOrder.Id;
                jobOrderDate.StartDate = jobOrder.StartDate;
                jobOrderDate.EndDate = jobOrder.EndDate ?? null;
            

            return PartialView("_ExportAttendantList", jobOrderDate);
        }
        public ActionResult _ExportAttendanceListForJobOrders(string ids, Guid companyGuid)
        {
            ViewBag.Ids = ids;
            ViewBag.CompanyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            return PartialView("_ExportAttendanceListForJobOrders");
        }


        public ActionResult ExportExcelAll(Guid? guid, DateTime? startDate)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            if (!startDate.HasValue || startDate < jobOrder.StartDate)
                startDate = jobOrder.StartDate;

            // whole week anyway
            var fromDate = startDate.Value.AddDays(DayOfWeek.Sunday - startDate.Value.DayOfWeek);
            var toDate = fromDate.AddDays(6);

            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportAttandentListToXlsxByDateRange(stream, jobOrder, fromDate, toDate);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", fileName);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Index");
            }

        }


        public ActionResult ExportExcelAllForJobOrders(string ids, int companyId, DateTime? refDate)
        {
            var fromDate = refDate.Value.AddDays(DayOfWeek.Sunday - refDate.Value.DayOfWeek);
            var toDate = fromDate.AddDays(6);

            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportAtetanceListForJobOrders(stream, ids, fromDate, toDate, companyId);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", fileName);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Index");
            }
            //return View();
        }

        #endregion


        #region SEO

        [NonAction]
        protected void UpdateLocales(JobOrder joborder, JobOrderModel model)
        {
            var seName = joborder.ValidateSeName(model.SeName, model.SeName, false);
            _urlRecordService.SaveSlug(joborder, seName, 1);
        }

        #endregion


        //JobOrderOvertimeRule

        #region GET :/JobOrder/_TabJobOrderOvertimeRule

        [HttpGet]
        public ActionResult _TabJobOrderOvertimeRule(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            ViewBag.JobOrderGuid = guid;
            ViewBag.JobOrderId = jobOrder.Id;
            var overtimeRuleSettings = _overtimeRuleSettingService.GetAllOvertimeRuleSettingsAsQueryable()
                                            .Select(x => new SelectListItem()
                                            {
                                                Text = x.Code + " - " + x.Description,
                                                Value = x.Id.ToString(),
                                            }).ToList();
            ViewBag.OvertimeRuleSettings = overtimeRuleSettings;

            return View();
        }

        #endregion


        #region POST:/JobOrder/_TabJobOrderOvertimeRule

        [HttpPost]
        public ActionResult _TabJobOrderOvertimeRule([DataSourceRequest] DataSourceRequest request, Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if(jobOrder==null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }

            var jobORderOvertimeRules = _jobOrderOvertimeRuleRepository.Table.Where(x => x.JobOrderId == jobOrder.Id).OrderBy(x => x.OvertimeRuleSetting.ApplyAfter).ToList();

            var jobOrderOvertimeRuleList = new List<JobOrderOvertimeRuleModel>();

            foreach (var item in jobORderOvertimeRules)
            {
                JobOrderOvertimeRuleModel i = item.ToModel();
                i.JobTitle = item.JobOrder.JobTitle;
                i.OvertimeRuleSettingModel = item.OvertimeRuleSetting.ToModel();

                jobOrderOvertimeRuleList.Add(i);
            }

            var result = new DataSourceResult()
            {
                Data = jobOrderOvertimeRuleList,
                Total = jobORderOvertimeRules.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POST:/JobOrder/CreateOvertimeRule

        [HttpPost]
        public ActionResult CreateOvertimeRule([Bind(Exclude = "Id")] JobOrderOvertimeRuleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                if (!_hasConflictingActiveRule(model))
                {
                    var overtimeRule = model.ToEntity();
                    overtimeRule.CreatedOnUtc = DateTime.UtcNow;
                    overtimeRule.UpdatedOnUtc = DateTime.UtcNow;

                    _jobOrderOvertimeRuleRepository.Insert(overtimeRule);

                    //activity log
                    _activityLogService.InsertActivityLog("AddNewJobOrderOvertimeRule", _localizationService.GetResource("ActivityLog.AddNewJobOrderOvertimeRule"), model.Id);

                    SuccessNotification(_localizationService.GetResource("Admin.JobOrders.JobOrderOvertimeRule.Added"));
                }
                else
                    ErrorNotification("Some overtime rules are overlapping and create conflicting settings. Please double check your input or deactivate the existing rules before entering the new rule.");

            }
            else
            {
                ErrorNotification(_localizationService.GetResource("Admin.JobOrders.JobOrderOvertimeRule.Added.Fail"));

                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));

                var exceps = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                string errMsg = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Warning(string.Format("Model state is invalid: --- ERRORS --- {0}. --- EXCEPTIONS --- {1}.", errMsg, exceps), userAgent: Request.UserAgent);
            }

            return RedirectToAction("Details", new { guid = model.JobOrderGuid, tabId = "tab-overtime-rule" });
        }

        private bool _hasConflictingActiveRule(JobOrderOvertimeRuleModel model)
        {
            bool result = false;

            // allow any inactive rule
            if (!model.IsActive)
                return result;

            var newRule = _overtimeRuleSettingService.GetOvertimeRuleSettingById(model.OvertimeRuleSettingId);
            // other existing rules
            var existingRules = _jobOrderOvertimeRuleRepository.Table
                                .Where(x => x.Id != model.Id && x.JobOrderId == model.JobOrderId && x.IsActive && x.OvertimeRuleSetting.TypeId == newRule.TypeId)
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

        #endregion

        #region GET :/JobOrder/EditOvertimeRule

        public ActionResult EditOvertimeRule(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            var overtimeRule = _jobOrderOvertimeRuleRepository.Table.Where(x => x.Id == Id).FirstOrDefault();
            var model = overtimeRule.ToModel();
            model.JobTitle = overtimeRule.JobOrder.JobTitle;
            model.OvertimeRuleSettingModel = overtimeRule.OvertimeRuleSetting.ToModel();
            model.OvertimeRuleSettings = _overtimeRuleSettingService.GetAllOvertimeRuleSettingsAsQueryable()
                .Where(x => x.OvertimeType.Id == overtimeRule.OvertimeRuleSetting.TypeId)
                .Select(x => new SelectListItem()
                {
                    Text = x.Code + " - " + x.Description,
                    Value = x.Id.ToString(),
                }).ToList();

            return View(model);
        }

        #endregion

        #region POST:/JobOrder/EditOvertimeRule

        [HttpPost]
        public ActionResult EditOvertimeRule(JobOrderOvertimeRuleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!_hasConflictingActiveRule(model))
                {
                    var overtimeRule = _jobOrderOvertimeRuleRepository.Table.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (overtimeRule != null)
                    {
                        //overtimeRule = model.ToEntity();
                        overtimeRule.OvertimeRuleSettingId = model.OvertimeRuleSettingId;
                        overtimeRule.IsActive = model.IsActive;
                        overtimeRule.Note = model.Note ?? null;
                        overtimeRule.UpdatedOnUtc = DateTime.UtcNow;
                        _jobOrderOvertimeRuleRepository.Update(overtimeRule);

                        //activity log
                        _activityLogService.InsertActivityLog("UpdateJobOrderOvertimeRule", _localizationService.GetResource("ActivityLog.UpdateJobOrderOvertimeRule"), model.Id);

                        SuccessNotification(_localizationService.GetResource("Admin.JobOrders.JobOrderOvertimeRule.Updated"));
                    }
                }
                else
                    ErrorNotification("Some overtime rules are overlapping and create conflicting settings. Please double check your input or deactivate the existing rules before entering the new rule.");
            }

            return RedirectToAction("Details", new { guid = model.JobOrderGuid, tabId = "tab-overtime-rule" });
        }

        #endregion


        // Job order pipeline

        #region GET :/JobOrder/_TabJobOrderPipeline
        public ActionResult _TabJobOrderPipeline(Guid? guid, string inquiryDateString = null, bool includePlacedCandidates = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if(jobOrder==null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            ViewBag.JobOrderGuid = guid;
            ViewBag.CompanyId = jobOrder.CompanyId;
            ViewBag.IncludePlacedCandidates = includePlacedCandidates;
            bool isShort;
            var model = _jobOrderCandidate_BL.GetJobOrderCandidateModel(guid, out isShort, inquiryDateString, includePlacedCandidates);
            ViewBag.shortJobOrder = isShort;
            ViewBag.ValidSendingEmail = _accountService.GetClientCompanyHRAccount(jobOrder.CompanyId)!=null || jobOrder.CompanyContactId>0;
            return PartialView("_TabJobOrderPipeline", model);
        }

        public ActionResult _IncludePlacedCandidates(Guid? guid)
        {
            var model = new AccountLoginModel();
            ViewBag.JobOrderGuid = guid;
            return PartialView("_IncludePlacedCandidates", model);
        }

        [HttpPost]
        public ActionResult IncludePlacedCandidates(AccountLoginModel model, Guid? guid)
        {
            string errorMessage = String.Empty;
            bool result = false;

            if (ModelState.IsValid)
            {
                var bl = new Wfm.Admin.Models.Accounts.Account_BL(_accountService, _localizationService,_activityLogService);
                result = bl.AuthenticateAdmin(model, out errorMessage);
                if (result)
                {
                    TempData["IncludePlacedCandidates"] = true;
                }
                else
                {
                    ErrorNotification(errorMessage);
                }
            }
            return RedirectToAction("Details", new { guid = guid, tabId = "tab-pipeline" });
        }


        [HttpPost]
        public ActionResult JobOrderPipelinePlaced([DataSourceRequest] DataSourceRequest request, Guid? guid, DateTime inquiryDate)
        {
            int openingAvailable;
            JobOrderOpeningModel[] openingChanges;
            var placed = _jobOrderCandidate_BL._TabJobOrderPipelinePlaced(guid, inquiryDate, out openingAvailable, out openingChanges);

            return Json(placed.ToDataSourceResult(request));
        }
        [HttpPost]
        public ActionResult JobOrderPipelinePooled([DataSourceRequest] DataSourceRequest request, Guid? guid, DateTime inquiryDate, bool includePlacedCandidates)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            if (jobOrder.FranchiseId == _workContext.CurrentAccount.FranchiseId || _workContext.CurrentFranchise.IsDefaultManagedServiceProvider)
            {
                var companyCandidates = _companyCandidateService.GetAvailablesFromCompanyCandidatePool(jobOrder, inquiryDate, includePlacedCandidates);
                var result = companyCandidates.ProjectTo<CandidatePoolModel>();
                return Json(result.ToDataSourceResult(request));
            }
            else
            {
                List<CompanyCandidatePoolVM> emptyResult = new List<CompanyCandidatePoolVM>();
                return Json(emptyResult.ToDataSourceResult(request));
            }
        }


        [HttpPost]
        public ActionResult _PostCandidateMove(Guid? guid, DateTime inquiryDate, int candidateId, bool moveIntoPipeline)
        {
            string msg = String.Empty;

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidatePlacement))
            {
                msg = String.Format("Cannot move {0} into/out pipeline since you do not have the permission to change the placement!", candidateId);
                return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
            }

            try
            {
                var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
                if(jobOrder==null)
                {
                    //ErrorNotification("The job order does not exist!");
                    msg = "The job order does not exist!";
                    return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
                }
                if (jobOrder.FranchiseId != _workContext.CurrentFranchise.Id)
                {
                    msg = "You are not allowed to change the placement since the job order belongs to another vendor!";
                    return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
                }
                if (_workContext.CurrentAccount.Id != jobOrder.OwnerId && _workContext.CurrentAccount.Id != jobOrder.RecruiterId)
                {
                    msg = "You are not allowed to change the placement since you are not the recruiter or owner of the job order!";
                    return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
                }
                //if (_smartCardService.GetActiveSmartCardByCandidateId(candidateId) == null)
                //{
                //    var devices = _clockDeviceService.GetClockDevicesByCompanyLocationId(jobOrder.CompanyLocationId);
                //    if(devices!=null && devices.Count>0)
                //    {
                //        msg = String.Concat("The candidate ", candidateId, " does not have an active smart card!");
                //        return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
                //    }
                //}
                if (moveIntoPipeline)
                    msg = _jobOrderCandidate_BL.AddCandidateIntoPipeline(jobOrder.Id, candidateId, inquiryDate);
                else
                    msg = _jobOrderCandidate_BL.RemoveCandidateFromPipeline(jobOrder.Id, candidateId, inquiryDate);
            }
            catch (InvalidOperationException ex)
            {
                msg = ex.Message;
            }

            return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
        }


        [HttpPost]
        public ActionResult UpdateCandidateJobOrderRatingValue(int candidateJobOrderId, decimal rating)
        {
            _candidateJobOrderService.UpdateCandidateJobOrderRatingValue(candidateJobOrderId, rating);
            return new EmptyResult();
        }


        public ActionResult _ExportAvailable(Guid companyGuid, DateTime refDate)
        {
            ViewBag.CompanyGuid = companyGuid;
            ViewBag.RefDate = refDate;

            return PartialView("_ExportAvailable");
        }


        public ActionResult ExportAvailable(Guid companyGuid, DateTime refDate)
        {
            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportAvailableToXlsx(stream, companyGuid, refDate);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", fileName);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Index");
            }
        }


        #endregion


        #region POST:/JobOrder/ToggleCandidatePipelineStatus

        [HttpPost]
        public JsonResult ToggleCandidatePipelineStatus(int candidateJobOrderId, DateTime currentDate)
        {
            string returnText = null;
            string errorMessage = _jobOrderCandidate_BL.ToggleCandidatePipelineStatus(candidateJobOrderId, currentDate, out returnText);

            return Json(new { Result = returnText, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region SendDailyConfirmation

        public ActionResult _DailyConfirmation(Guid jobOrderGuid, DateTime refDate)
        {
            ViewBag.JobOrderGuid = jobOrderGuid;
            ViewBag.RefDate = refDate;

            return PartialView();
        }


        [HttpPost]
        public JsonResult SendDailyConfirmation(Guid jobOrderGuid, DateTime refDate, int statusId)
        {
            var statusList = new List<int>();
            if (statusId > 0)
                statusList.Add(statusId);

            var errorMessage = _jobOrderCandidate_BL.SendDailyConfirmation(jobOrderGuid, refDate, out int done, statusList);

            return Json(new { Done = done, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POST:/_ManualJobOrderWorkTime
        [HttpPost]
        public ActionResult _ManualJobOrderWorkTime(Guid? guid, DateTime inquiryDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateTimeSheet))
                return AccessDeniedView();
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            var startDate = inquiryDate.StartOfWeek(DayOfWeek.Sunday);
            var endDate = startDate.AddDays(6).Date;
            

            var workTimeData = _workTimeService.GetAllWorkTimeByJobOrderAndStartEndDateAsQueryable(jobOrder.Id, startDate, endDate).ToArray();
            var model = new JobOrderWorkTimeModel()
            {
                JobOrder = jobOrder.ToModel(),
                WeekEndSaturdayDate = endDate,
                ManualWorkTimeEntries = _candidateJobOrderService.GetCandidateJobOrderByJobOrderId(jobOrder.Id, inquiryDate)
                    .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed)
                    .Select(x => new WeeklyManualWorkTimeModel()
                    {
                        CandidateFirstName = x.Candidate.FirstName,
                        CandidateId=x.Candidate.Id,
                        CandidateLastName=x.Candidate.LastName, 
                        WorkHoursWeekdays = GetWeeklyWorkHours(workTimeData, x.CandidateId, startDate),
                        WorkHoursEditableWeekdays = GetWeeklyWorkHoursEditable(guid, x.CandidateId, startDate),
                    }).ToList(),
            };
            var department = _companyDepartmentService.GetCompanyDepartmentById(model.JobOrder.CompanyDepartmentId.GetValueOrDefault());
            var location = _companyDivisionService.GetCompanyLocationById(model.JobOrder.CompanyLocationId);
            model.JobOrder.CompanyDepartmentName = (department != null) ? department.DepartmentName : string.Empty;
            model.JobOrder.CompanyLocationName = (location != null) ? location.LocationName : string.Empty;
            if (model.JobOrder.CompanyContactId.HasValue)
            {
                var _companyContact = _companyContactService.GetCompanyContactById(model.JobOrder.CompanyContactId.Value);
                if (_companyContact != null)
                {
                    model.JobOrder.Supervisor = _companyContact.FullName;
                }
            }
            model.ClientTimeSheetDocuments = _ClientDocumentList(jobOrder.Id, startDate, endDate).ToList();

            return PartialView("_ManualJobOrderWorkTime", model);
        }
        private decimal?[] GetWeeklyWorkHours(CandidateWorkTime[] workTimeData, int candidateId, DateTime startDate)
        {
            int[] days = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            return days.Select(x => workTimeData.Any(w => w.JobStartDateTime.Date == startDate.AddDays(x) && w.CandidateId == candidateId) ?
                new decimal?(workTimeData.First(w => w.JobStartDateTime.Date == startDate.AddDays(x) && w.CandidateId == candidateId).NetWorkTimeInHours) :
                null).ToArray();
        }
        private bool[] GetWeeklyWorkHoursEditable(Guid? guid, int candidateId, DateTime startDate)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return null;
            }
            var workTimeData = _workTimeService.GetAllWorkTimeByJobOrderAndStartEndDateAsQueryable(jobOrder.Id, startDate, startDate.AddDays(6)).ToArray();
            int[] days = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            bool[] result = days.Select(x => !workTimeData.Any(w => w.JobStartDateTime.Date == startDate.AddDays(x) && w.CandidateId == candidateId)
                                            || workTimeData.Any(w => w.JobStartDateTime.Date == startDate.AddDays(x) && w.CandidateId == candidateId
                                                && w.Source == "Manual")).ToArray();

            var cjoList = _candidateJobOrderService.GetAllCandidateJobOrdersByJobOrderIdAndCandidateId(jobOrder.Id, candidateId);
            for (int i = 0; i < 7; i++)
            {
                if (cjoList != null)
                {
                    var cjo = cjoList.Where(x => x.StartDate <= startDate.AddDays(i) &&
                                                (x.EndDate == null || x.EndDate >= startDate.AddDays(i)) &&
                                                 x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed
                                           ).Any();
                    result[i] &= cjo;
                }
                else
                    result[i] = false;
            }

            return result;
        }
        #endregion


        #region POST://JobOrder/SaveManualWorkTime

        [HttpPost]
        public JsonResult SaveManualWorkTime([Bind(Exclude = "Id")]JobOrderWorkTimeModel model)
        {
            string errorMessage = string.Empty;
            bool result = true;
            int daysWithHours = 0;

            // model check, including attachment
            var clientDocList = _workTimeService.GetAllClientTimeSheetDocumentsByJobOrderIdAndDateRange(model.JobOrder.Id, model.WeekEndSaturdayDate.AddDays(-6), model.WeekEndSaturdayDate);
            if (clientDocList == null || clientDocList.Count == 0)
            {
                return Json(new { Result = false, ErrorMessage = _localizationService.GetResource("Admin.JobOrder.ManualWorkTime.DocumentIsRequired") }, JsonRequestBehavior.AllowGet);
            }

            var jobOrder = _jobOrderService.GetJobOrderById(model.JobOrder.Id);
            var weekStartDate = model.WeekEndSaturdayDate.AddDays(-6);
            foreach (var weeklyManualWorkTime in model.ManualWorkTimeEntries)
            {
                var candidate = _candidatesService.GetCandidateById(weeklyManualWorkTime.CandidateId);

                for (int i = 0; i < 7; i++)

                    if (weeklyManualWorkTime.WorkHoursEditableWeekdays[i])

                        try
                        {
                            if (weeklyManualWorkTime.WorkHoursWeekdays != null &&
                                weeklyManualWorkTime.WorkHoursWeekdays[i] != null)
                            {
                                _workTimeService.InsertOrUpdateWorkTime(jobOrder.Id, candidate.Id, weekStartDate.AddDays(i),
                                                                        (Decimal)weeklyManualWorkTime.WorkHoursWeekdays[i],
                                                                        WorkTimeSource.Manual);
                                daysWithHours++;
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessage += "\r\n" + ex.Message;
                            result = false;
                        }

                _workTimeService.CalculateOTforWorktimeWithinDateRange(weekStartDate, weekStartDate.AddDays(6));

            }

            if (daysWithHours > 0)
            {
                if (result)
                    _activityLogService.InsertActivityLog("SaveManualWorkTime", "Save Manaul Work Time for Job Order " + model.JobOrder.Id);
            }
            else
            {
                errorMessage += "\r\n" + "No hours are updated.";
                result = false;
            }

            return Json(new { Result = result, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POST:/JobOrder/_ClientDocumentList
        [HttpPost]
        public ActionResult _ListClientTimeSheetDocuments(Guid? guid, DateTime startDate, DateTime endDate)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            var model = _ClientDocumentList(jobOrder.Id, startDate, endDate);
            return PartialView("_DocumentListView", model);
        }

        private IEnumerable<ClientTimeSheetDocumentModel> _ClientDocumentList(int jobOrderId, DateTime startDate, DateTime endDate)
        {

            var clientDocuments = _workTimeService.GetAllClientTimeSheetDocumentsByJobOrderIdAndDateRange(jobOrderId, startDate, endDate);

            return clientDocuments.Select(x => x.ToModel());
        }

        #endregion


        // Client Timesheet Documents

        #region POST:/Candidate/SaveClientDocuments

        public ActionResult SaveClientDocuments(IEnumerable<HttpPostedFileBase> clientDocuments, ClientTimeSheetDocumentModel clientDocModel)
        {
            string errorMessage = string.Empty;

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateTimeSheet))
                return AccessDeniedView();

            foreach (var file in clientDocuments)
            {
                var clientDoc = new ClientTimeSheetDocument();

                clientDoc.JobOrderId = clientDocModel.JobOrderId;
                clientDoc.StartDate = clientDocModel.StartDate;
                clientDoc.EndDate = clientDocModel.EndDate;
                clientDoc.Version = 1;
                clientDoc.FileType = _GetFileType(file.FileName);
                clientDoc.FileName = file.FileName;

                using (Stream stream = file.InputStream)
                {
                    var fileBytes = new byte[stream.Length];
                    stream.Read(fileBytes, 0, fileBytes.Length);
                    clientDoc.Stream = fileBytes;
                }

                clientDoc.Source = DocumentSource.Attachment;
                clientDoc.CreatedOnUtc = DateTime.UtcNow;
                clientDoc.UpdatedOnUtc = clientDoc.CreatedOnUtc;

                _workTimeService.InsertClientTimeSheetDocument(clientDoc);

                //activity log
                _activityLogService.InsertActivityLog("SaveClientDocument", _localizationService.GetResource("ActivityLog.AddNewCandidateAttachment"), file.FileName);
            }

            // Return an empty string to signify success
            return new EmptyResult();
        }

        public ActionResult _UploadAttachment(int jobOrderId,DateTime startDate,DateTime endDate)
        {
            ClientTimeSheetDocumentModel model = new ClientTimeSheetDocumentModel()
            {
                JobOrderId = jobOrderId,
                StartDate = startDate,
                EndDate = endDate
            };
            return PartialView(model);
        }

        private DocumentFileType _GetFileType(string fileName)
        {
            var fileExt = Path.GetExtension(fileName).ToLower();
            var fileType = DocumentFileType.Unknown;

            switch (fileExt)
            {
                case ".pdf":
                    fileType = DocumentFileType.PDF;
                    break;
                case ".xls":
                case ".xlsx":
                case ".csv":
                    fileType = DocumentFileType.Excel;
                    break;
                case ".txt":
                    fileType = DocumentFileType.Text;
                    break;
                case ".htm":
                case ".html":
                    fileType = DocumentFileType.Html;
                    break;
            }

            return fileType;
        }

        #endregion

        #region GET :/Candidate/DeleteClientDocument
        [HttpPost]
        public ActionResult DeleteClientDocument(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateTimeSheet))
                return AccessDeniedView();

            var clientDoc = _workTimeService.GetClientTimeSheetDocumentById(id);

            if (clientDoc == null)
                return Content("The client document with Id [{0}] does not exist.");

            _workTimeService.DeleteClientTimeSheetDocument(clientDoc);

            //activity log
            _activityLogService.InsertActivityLog("DeleteClientDocument", _localizationService.GetResource("ActivityLog.DeleteClientDocument"), clientDoc.Id + "/" + clientDoc.FileName);

            return Content("");
        }

        #endregion

        #region GET :/Candidate/DownloadClientDocument

        public ActionResult DownloadClientDocument(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateTimeSheet))
                return AccessDeniedView();

            var clientDoc = _workTimeService.GetClientTimeSheetDocumentById(id);

            if (clientDoc == null)
                return Content("The client document with Id [{0}] does not exist.");

            return File(clientDoc.Stream, System.Net.Mime.MediaTypeNames.Application.Octet, clientDoc.FileName);
        }

        #endregion


        #region _CalculateCandidateWorkTime

        public ActionResult _CalculateCandidateWorkTime(Guid? guid, DateTime jobOrderStartDate, DateTime? jobOrderEndDate, string inquiryDateString = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            var model = new CalculateCandidateWorkTimeModel();

            var inquiryDate = DateTime.Today.AddDays(-1);
            if (!string.IsNullOrEmpty(inquiryDateString))
                DateTime.TryParse(inquiryDateString, out inquiryDate);

            model.JobOrderId = jobOrder.Id;
            model.JobOrderGuid = jobOrder.JobOrderGuid;
            if (jobOrderStartDate > DateTime.Now.Date)
                jobOrderStartDate = DateTime.Now.Date;
            ViewBag.JobOrderStartDate = jobOrderStartDate;

            if (jobOrderEndDate == null || jobOrderEndDate > DateTime.Now.Date)
                jobOrderEndDate = DateTime.Now.Date;
            ViewBag.JobOrderEndDate = jobOrderEndDate;
            ViewBag.InquiryDate = inquiryDate;

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _CalculateCandidateWorkTime(CalculateCandidateWorkTimeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var jobOrder = _jobOrderService.GetJobOrderById(model.JobOrderId);
            var startDate = Convert.ToDateTime(model.FromDate);
            var endDate = Convert.ToDateTime(model.ToDate);
            _workTimeService.CalculateJobOrderCandidateWorkTime(jobOrder, startDate, endDate, _workContext.CurrentAccount, true, true, false);

            SuccessNotification(_localizationService.GetResource("Admin.TimeSheet.CalculateCandidateWorkTime.Calculated"));

            return RedirectToAction("Details", new { guid = model.JobOrderGuid });
        }


        [HttpPost]
        public ActionResult _CalculateOneCandidateWorkTime(int jobOrderId, DateTime refDate,Guid? candidateGuid)
        {
            try
            {
                var candidate = _candidatesService.GetCandidateByGuid(candidateGuid);
                if (candidate == null)
                    throw new ArgumentNullException("Candidate does not exist!");
                JobOrder jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
                var cwt = _workTimeService.PrepareCandidateWorkTimeByJobOrderAndDate(null /*account*/, jobOrder, refDate);
                cwt.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.PendingApproval;
                cwt.EnteredBy = _workContext.CurrentAccount.Id;
                var candidateAsList = new List<int>();
                candidateAsList.Add(candidate.Id);

                _workTimeService.CalculateWorkTimeForCandidates(candidateAsList, cwt);
            }
            catch (Exception ex)
            {
                _logger.Error("_CalculateOneCandidateWorkTime", ex);
                return Json(new { ErrorMessage = "Fail to calculate work time!", Result = false });
            }
            return Json(new { ErrorMessage = String.Empty, Result = true });
        }

        #endregion


        //JsonResult

        #region //JsonResult : GetCascadeCompanies

        public JsonResult GetCascadeCompanies()
        {
            var companies = _companyService.Secure_GetAllCompanies(_workContext.CurrentAccount).OrderBy(c => c.DisplayOrder).ThenBy(c => c.CompanyName);
            if (_workContext.CurrentAccount.IsRecruiterOrRecruiterSupervisor())
            {
                List<int> companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(_workContext.CurrentAccount.Id);
                companies = companies.Where(x => companyIds.Contains(x.Id)).OrderBy(c => c.DisplayOrder).ThenBy(c => c.CompanyName);
            }
            var companyDropDownList = new List<SelectListItem>();
            companyDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            foreach (var c in companies)
            {
                var item = new SelectListItem()
                {
                    Text = c.CompanyName,
                    Value = c.Id.ToString()
                };
                companyDropDownList.Add(item);
            }        
            return Json(companyDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region //JsonResult : GetCascadeContacts

        public JsonResult GetCascadeContacts(string companyId)
        {
            var cmpId = String.IsNullOrEmpty(companyId) ? 0 : Convert.ToInt32(companyId);

            var contactDropDownList = new List<SelectListItem>();
            // Add default zero value
            contactDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            if (cmpId > 0)
            {
                var contacts = _companyContactService.GetCompanyContactsByCompanyId(cmpId).OrderBy(x => x.FirstName).ThenBy(x => x.LastName);

                foreach (var c in contacts)
                {
                    var item = new SelectListItem()
                    {
                        Text = c.FullName,
                        Value = c.Id.ToString()
                    };
                    contactDropDownList.Add(item);
                }
            }

            return Json(contactDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region //JsonResult : GetCascadeCompanyBillingRates

        public JsonResult GetCascadeCompanyBillingRates(string companyId)
        {
            var cmpId = String.IsNullOrEmpty(companyId) ? 0 : Convert.ToInt32(companyId);

            var companybillingRateDropDownList = new List<SelectListItem>();
            if (cmpId > 0)
            {
                var companyBillingRates = _companyBillingService.GetAllCompanyBillingRatesByCompanyId(cmpId).OrderBy(c => c.RateCode);
                foreach (var b in companyBillingRates)
                {
                    var item = new SelectListItem()
                    {
                        Text = b.RateCode,
                        Value = b.Id.ToString()
                    };
                    companybillingRateDropDownList.Add(item);
                }
            }

            return Json(companybillingRateDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region //JsonResult : GetCascadeCompanyBillingRateCodes

        public JsonResult GetCascadeCompanyBillingRateCodes(int companyId,  int locationId, int vendorId, DateTime? refDate=null)
        {
            var companyBillingRateCodeDropDownList = new List<SelectListItem>();
            if (companyId > 0  && vendorId > 0)
            {
                var companyBillingRates = _companyBillingService.GetAllCompanyBillingRatesByCompanyIdAndRefDate(companyId, refDate)
                    .Where(c => c.FranchiseId == vendorId && c.CompanyLocationId == locationId).OrderBy(c => c.RateCode)
                    .Select(b => b.RateCode).Distinct();
                foreach (var rateCode in companyBillingRates)
                {
                    var item = new SelectListItem()
                    {
                        Text = rateCode,
                        Value = rateCode
                    };
                    companyBillingRateCodeDropDownList.Add(item);
                }
            }

            return Json(companyBillingRateCodeDropDownList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCascadePositionsWithRates(int companyId, int locationId, int vendorId,
            DateTime? refDate = null, string shiftCode = null)
        {
            var result = _companyBillingService.GetAllCompanyBillingRatesByCompanyIdAndRefDate(companyId, refDate)
                .Where(x => vendorId == 0 || x.FranchiseId == vendorId)
                .Where(x => locationId == 0 || x.CompanyLocationId == locationId)
                .Where(x => string.IsNullOrEmpty(shiftCode) || x.ShiftCode == shiftCode)
                .Select(x => new SelectListItem()
                {
                    //Text = string.Format("{0} / {1} - {2:c}", x.Position.Code.Trim(), x.ShiftCode.Trim(), x.RegularPayRate),
                    // RateCode = PositionName / ShiftCode
                    Text = string.Format("{0} / {1} - {2:c}", x.Position.Name.Trim(), x.ShiftCode.Trim(), x.RegularPayRate),
                    Value = x.PositionId.ToString()
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region //JsonResult : GetCascadeJobOrders

        public JsonResult GetCascadeJobOrders(string companyId, string locationId, string departmentId, string startDateString, string endDateString,bool selectGuid=false)
        {
            var cmpId = String.IsNullOrEmpty(companyId) ? 0 : Convert.ToInt32(companyId);
            var locId = String.IsNullOrEmpty(locationId) ? 0 : Convert.ToInt32(locationId);
            var deptId = String.IsNullOrEmpty(departmentId) ? 0 : Convert.ToInt32(departmentId);
            var account = _workContext.CurrentAccount;

            var startDate = DateTime.Today;
            if (!String.IsNullOrEmpty(startDateString))
                DateTime.TryParse(startDateString, out startDate);

            DateTime endDate = DateTime.MaxValue;
            if (!String.IsNullOrEmpty(endDateString))
                DateTime.TryParse(endDateString, out endDate);

            var jobOrders = _jobOrderService.GetAllJobOrdersByCompanyIdAsQueryable(cmpId)
                            .Where(x => (x.JobOrderStatusId == (int)JobOrderStatusEnum.Active || x.JobOrderStatusId == (int)JobOrderStatusEnum.Closed) &&
                                        x.StartDate <= endDate && (!x.EndDate.HasValue || x.EndDate >= startDate))
                            .OrderByDescending(x => x.Id).AsQueryable();
            if (locId > 0)
                jobOrders = jobOrders.Where(x => x.CompanyLocationId == locId);
            if (deptId > 0)
                jobOrders = jobOrders.Where(x => x.CompanyDepartmentId == deptId);

            if (account != null && account.IsLimitedToFranchises)
                jobOrders = jobOrders.Where(x => x.FranchiseId == account.FranchiseId);

            if (account != null && account.IsRecruiterOrRecruiterSupervisor())
                jobOrders = jobOrders.Where(x => x.OwnerId == account.Id || x.RecruiterId == account.Id);

            var jobOrderList = new List<SelectListItem>();
            //jobOrderList.Add(new SelectListItem() { Text = "None", Value = "0" });

            foreach (var j in jobOrders.ToList())
            {
                var item = new SelectListItem()
                {
                    Text = j.Id.ToString() + " --- " + j.JobTitle,
                    Value = selectGuid?j.JobOrderGuid.ToString():j.Id.ToString()
                };
                jobOrderList.Add(item);
            }

            return Json(jobOrderList, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region //JsonResult : GetCascadeCompanyPolicy

        public JsonResult GetCascadeCompanyPolicy(string companyId)
        {
            var cmpId = String.IsNullOrEmpty(companyId) ? 0 : Convert.ToInt32(companyId);

            var companyPolicyDropDownList = new List<SelectListItem>();
            // Add default zero value
            companyPolicyDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            if (cmpId > 0)
            {
                var schedulePolicies = _schedulePolicyService.GetSchedulePoliciesByCompanyId(cmpId);
                foreach (var i in schedulePolicies)
                {
                    var item = new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    };
                    companyPolicyDropDownList.Add(item);
                }
            }

            return Json(companyPolicyDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region //JsonResult : GetCascadeRecruiters

        public JsonResult GetCascadeRecruiters(Guid? companyGuid, Guid? vendorGuid, bool includeAdmin = false)
        {
            IList<Account> recruiters = new List<Account>();
            var company = _companyService.GetCompanyByGuid(companyGuid);
            var vendor = _franchiseService.GetFranchiseByGuid(vendorGuid);

            if (company!=null&&vendor!=null)
            {
                if (!_workContext.CurrentAccount.IsMSPRecruiter() && !_workContext.CurrentAccount.IsMSPRecruiterSupervisor())
                    recruiters = _accountService.GetAllRecruitersByCompanyIdAndVendorId(company.Id, vendor.Id, includeAdmin);
                else
                    recruiters = _accountService.GetAllRecruitersByCompanyId(company.Id, includeAdmin);  // This is for MSP users only
            }
            else
            {
                var query = _accountService.GetAllRecruitersAsQueryable(_workContext.CurrentAccount);
                if (vendor!=null)
                    query = query.Where(x => x.FranchiseId == vendor.Id);
                recruiters = query.ToList();
            }
            var recruiterDropDownList = new List<SelectListItem>();
            foreach (var r in recruiters)
            {
                var item = new SelectListItem()
                {
                    Text = r.LastName + ", " + r.FirstName,
                    Value = r.Id.ToString()
                };
                recruiterDropDownList.Add(item);
            }

            return Json(recruiterDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GET://JobOrder/_DirectGlobalSearchPopup
        public ActionResult GetDirectGlobalSearchPopup(DateTime inquiryDate, int companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidatePlacement))
                return AccessDeniedView();
            ViewBag.CompanyId = companyId;
            ViewBag.inquiryDate = inquiryDate;
            return PartialView("_DirectGlobalSearchPopup");
        }
        #endregion

        #region POST:// JobOrder/ListGlobalEmployees

        [HttpPost]
        public ActionResult ListGlobalEmployees([DataSourceRequest] DataSourceRequest request, DateTime inquiryDate, int companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            var result = _jobOrderCandidate_BL.GetCandidatesFromGlobalPool(request, inquiryDate, companyId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddEmmployeesIntoJobOrder(Guid? guid, int[] candidateIds, DateTime startDate, bool terminateCurrentPlacement, bool addToCompanyPool)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return Json(new { ErrorMessage = "The job order does not exist!", Success =false });
            }
            var errorMessage = _jobOrderCandidate_BL.AddCandidatesFromGlobalPoolIntoJobOrder(jobOrder.Id, candidateIds, startDate, terminateCurrentPlacement, addToCompanyPool);

            return Json(new { ErrorMessage = errorMessage, Success = String.IsNullOrEmpty(errorMessage) });
        }

        #endregion


        #region Send JobOrder Placement Email
        
        public ActionResult _EmailJobOrderPlacement(Guid? guid, DateTime? inquiryDate)
        {
            string error = string.Empty;
            QueuedMailModel_BL model_bl = new QueuedMailModel_BL(_jobOrderService, _candidateJobOrderService, _candidatesService, _exportManager, _workflowMessageService, _workContext, _companyEmailTemplateService,_logger);
            var model = model_bl.GetAttendanceListQueuedEmailModel(guid, inquiryDate,null, out error);
            if (error.Length > 0)
            {
                ErrorNotification(error);
                _logger.Error(error, userAgent: Request.UserAgent);
            }
            return PartialView(model);
        }



        [HttpPost]
        public ActionResult _SendJobOrderPlacementEmail(QueuedEmailModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    _queuedEmailService.InsertQueuedEmail(entity);
                    //SuccessNotification("An email has been sent to the supervisor!");
                    return Json(new { Message = String.Format("An email has been sent to {0}!",model.To), Error = false });
                }
                else { 
                    return Json(new { Message = ModelState.SerializeErrors().ToString(), Error = true }); 
                }
            }
            catch (Exception ex)
            {
                _logger.Error("_SendJobOrderPlacementEmail():", ex, userAgent: Request.UserAgent);
                //ErrorNotification("An error occurred during sending the email!");
                return Json(new { Message = ex.Message, Error = true }); 
            }
        }

        #endregion


        #region _SendJobOrderConfirmationEmail

        public ActionResult _EmailJobOrderConfirmation(Guid? candidateGuid,Guid? jobOrderGuid,DateTime start, DateTime? end)
        {

            string error = string.Empty;
            QueuedMailModel_BL model_bl = new QueuedMailModel_BL(_jobOrderService, _candidateJobOrderService, _candidatesService, _exportManager, _workflowMessageService, _workContext, _companyEmailTemplateService,_logger);
            var model = model_bl.GetCandidateConfirmationEmail(candidateGuid, jobOrderGuid, start, end, out error);
            if (error.Length > 0)
            {
                ErrorNotification(error);
                _logger.Error(error, userAgent: Request.UserAgent);
            }
            return PartialView("_EmailJobOrderPlacement",model);
        }


        [HttpPost]
        public ActionResult _SendJobOrderConfirmationEmail(QueuedEmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //send email
                    var entity = model.ToEntity();
                    entity.IncludeLogo = model.Body.Contains("%LogoPath%");
                    if (entity.IncludeLogo)
                    {
                        entity.LogoFilePath = Path.Combine(_webHelper.MapPath("~/Content/Images/"), "logo.png");
                    }

                    _workflowMessageService.SendConfirmationToEmployeeNotification(entity);
                    return Json(new { Error = false, Message = String.Format("Confirmation email is sent to {0}.", model.ToName) });
                }
                catch (Exception ex)
                {
                    _logger.Error("SendConfirmationToEmployeeNotification():", ex);
                    return Json(new { Error = true, Message = "Failed to send the confirmation email!" });
                }

            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                var messages = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                return Json(new { Error = true, Message = messages });
            }
                
        }
        #endregion


        #region GetAvailableOpening

        [HttpPost]
        public ActionResult GetAvailableOpeningById(int? jobOrderId,DateTime refDate)
        {
            if (jobOrderId.HasValue)
            {
                var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId.Value);
                return GetAvailableOpening(jobOrder, refDate);
            }
            return Json(new { remain = 0 });
        }


        [HttpPost]
        public ActionResult GetAvailableOpeningByGuid(Guid? jobOrderGuid, DateTime refDate)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(jobOrderGuid);
            return GetAvailableOpening(jobOrder, refDate);
        }


        private JsonResult GetAvailableOpening(JobOrder jobOrder, DateTime refDate)
        {
            if (jobOrder == null)
                return Json(new { remain = 0 });
            int placedCount = _candidateJobOrderService.GetNumberOfPlacedCandidatesByJobOrder(jobOrder.Id, refDate);
            JobOrderOpening[] _openingChanges;
            int openingAvailable = _jobOrderService.GetJobOrderOpeningAvailable(jobOrder.Id, refDate, out _openingChanges);
            int remain = openingAvailable - placedCount;
            return Json(new { remain = remain });
        }

        #endregion


        #region Openings

        public ActionResult _TabOpenings(Guid guid, DateTime? refDate)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("Cannot load the data!");
                return RedirectToAction("Index");
            }

            ViewBag.JobOrderGuid = guid;
            ViewBag.JobOrderId = jobOrder.Id;
            ViewBag.JobTitle = jobOrder.JobTitle;

            ViewBag.RefDate = refDate ?? DateTime.Today;
            ViewBag.ReadOnly = !(jobOrder.JobPostingId == null &&
                (_workContext.CurrentAccount.IsAdministrator()
                    || _workContext.CurrentAccount.IsVendorAdmin()
                    || _workContext.CurrentAccount.Id == jobOrder.RecruiterId
                    || _workContext.CurrentAccount.Id == jobOrder.OwnerId));

            return PartialView();
        }


        [HttpPost]
        public ActionResult _JobOrderOpenings([DataSourceRequest] DataSourceRequest request, Guid jobOrderGuid, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedJson();

            var jobOrder = _jobOrderService.GetJobOrderByGuid(jobOrderGuid);
            var location = _companyDivisionService.GetCompanyLocationById(jobOrder.CompanyLocationId);
            var holidays = _stateProvinceService.GetAllStatutoryHolidaysOfStateProvince(location.StateProvinceId)
                .Where(x => x.HolidayDate >= startDate && x.HolidayDate <= endDate).ToList();
            var openings = _jobOrderService.GetJobOrderOpeningsByDate(_workContext.CurrentAccount, jobOrderGuid, startDate, endDate)
                .Select(x => x.ToModel(startDate, endDate, holidays, _localizationService.GetResource("Admin.Openings.NoWork"), readOnly: false));

            return Json(openings.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult SaveOpening(OpeningModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedJson();

            var error = string.Empty;
            if (ModelState.IsValid && !model.ReadOnly)
            {
                try
                {
                    error = _jobOrderService.UpdateJobOrderOpenings(model.JobOrderId, model.Start, model.End, model.OpeningNumber, null);
                }
                catch (Exception ex)
                {
                    _logger.Error("SaveOpening(): Failed", ex, _workContext.CurrentAccount, Request.UserAgent);
                    error += _localizationService.GetResource("Common.UnexpectedError");
                }
            }
            else
            {
                var errs = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                error += String.Join(" | ", errs.Select(o => o.ToString()).ToArray());
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error });
        }


        [HttpPost]
        public ActionResult RemoveOpening(OpeningModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedJson();

            var error = string.Empty;
            if (!model.ReadOnly)
            {
                try
                {
                    //error = _jobOrderService.RemoveJobOrderOpening(model.OpeingId);
                    error = _jobOrderService.UpdateJobOrderOpenings(model.JobOrderId, model.Start, model.End, 0, null);
                }
                catch (Exception ex)
                {
                    _logger.Error("RemoveOpening(): Failed", ex, _workContext.CurrentAccount, Request.UserAgent);
                    error = _localizationService.GetResource("Common.UnexpectedError");
                }
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error });
        }

        #endregion


        #region Make a decision
        public ActionResult MakeADecision()
        {
            return PartialView();
        }
        #endregion

        #region Validate Company Vendor
        private void ValidateCompanyAndVendor(JobOrderModel model)
        {
            var company = _companyService.GetCompanyByGuid(model.CompanyGuid);
            if (company != null)
                model.CompanyId = company.Id;
            else
                ModelState.AddModelError("CompanyGuid", _localizationService.GetResource("Common.CompanyId.IsRequired"));
            var vendor = _franchiseService.GetFranchiseByGuid(model.FranchiseGuid);
            if (vendor != null)
                model.FranchiseId = vendor.Id;
            else
                ModelState.AddModelError("FranchiseGuid", _localizationService.GetResource("Common.FranchiseId.IsRequired"));
        }
        #endregion

        #region PublishJobOrderToOtherPlatform
        public ActionResult _PublishJobOrderToOtherPlatform()
        {
            var jobBoards = _jobBoardService.GetAllJobBoards();
            return PartialView(jobBoards);
        }

        [HttpPost]
        public ActionResult _PublishJobOrder(Guid? guid,string platforms)
        {
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                _logger.Error("The job order does not exist!");
                return Json(new { Result = false, ErrorMessage ="Cannot load data!"});
            }
            if (String.IsNullOrWhiteSpace(platforms))
                return Json(new { Result = false, ErrorMessage = "Please select at least one job board!" });
            try
            {
                string errors = _webService.PublishJobOrder(jobOrder, platforms);
                if (String.IsNullOrWhiteSpace(errors))
                {
                    jobOrder.IsPublished = true;
                    _jobOrderService.UpdateJobOrder(jobOrder);
                    return Json(new { Result = true, ErrorMessage = String.Empty });
                }
                else
                    return Json(new { Result = false, ErrorMessage = errors });
            }
            catch (Exception ex)
            {
                _logger.Error("PublishJobOrder():", ex);
                return Json(new { Result = false, ErrorMessage = "Fail to publish the job order!" });
            }
            
        }
        #endregion
    }
}

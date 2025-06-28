using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System.Threading.Tasks;
using System;
using Wfm.Admin.Models.TimeSheet;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Services.Accounts;
using Wfm.Services.ClockTime;
using Wfm.Services.Invoices;
using Wfm.Services.Logging;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.ExportImport;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Services.TimeSheet;
using Wfm.Core.Domain.Candidates;
using Wfm.Web.Framework.Feature;
using System.Text;


namespace Wfm.Admin.Controllers
{
    public class TimeSheetController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly ICandidateService _candidatesService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyLocationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly IExportManager _exportManager;
        private readonly IPdfService _pdfService;
        private readonly ILocalizationService _localizationService;
        private readonly IInvoiceIntervalService _invoiceIntervalServcie;
        private readonly IClockTimeService _clockTimeService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IRepository<CandidateWorkTimeLog> _workTimeLogRepository;
        private readonly ITimeSheetService _timeSheetService;
        private readonly IEmployeeTimeChartHistoryService _employeeTimeSheetHistoryService;
        private readonly IFranchiseService _franchiseService;
        private readonly IAccountService _accountService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ILogger _logger;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IMissingHourService _missingHourService;
        private readonly IMissingHourDocumentService _missingHourDocService;

        private readonly CandidateWorkTimeModel_BL _workTime_BL;

        #endregion

        #region Ctor

        public TimeSheetController(IActivityLogService activityLogService,
            CandidateWorkTimeSettings candidateWorkTimeSettings,
            ICandidateService candidatesService,
            ICandidateJobOrderService candidateJobOrderService,
            IJobOrderService jobOrderService,
            ICompanyService companyService,
            ICompanyDivisionService companyLocationService,
            IStateProvinceService stateProvinceService,
            IPermissionService permissionService,
            IImportManager importManager,
            IExportManager exportManager,
            IWorkContext workContext,
            IPdfService pdfService,
            ILocalizationService localizationService,
            IInvoiceIntervalService invoiceIntervalServcie,
            IClockTimeService clockTimeService,
            IWorkTimeService workTimeService,
            IRepository<CandidateWorkTimeLog> workTimeLogRepository,
            ITimeSheetService timeSheetService,
            IEmployeeTimeChartHistoryService employeeTimeSheetHistoryService,
            IFranchiseService franchiseService,
            IAccountService accountService,
            IWorkflowMessageService workflowMessageService,
            ILogger logger,
            IRecruiterCompanyService recruiterCompanyService,
            IMissingHourService missingHourService,
            IMissingHourDocumentService missingHourDocService
           )
        {
            _activityLogService = activityLogService;
            _candidateWorkTimeSettings = candidateWorkTimeSettings;
            _candidatesService = candidatesService;
            _candidateJobOrderService = candidateJobOrderService;
            _jobOrderService = jobOrderService;
            _companyService = companyService;
            _companyLocationService = companyLocationService;
            _stateProvinceService = stateProvinceService;
            _permissionService = permissionService;
            _importManager = importManager;
            _exportManager = exportManager;
            _workContext = workContext;
            _pdfService = pdfService;
            _localizationService = localizationService;
            _invoiceIntervalServcie = invoiceIntervalServcie;
            _clockTimeService = clockTimeService;
            _workTimeService = workTimeService;
            _workTimeLogRepository = workTimeLogRepository;
            _timeSheetService = timeSheetService;
            _employeeTimeSheetHistoryService = employeeTimeSheetHistoryService;
            _franchiseService = franchiseService;
            _accountService = accountService;
            _workflowMessageService = workflowMessageService;
            _logger = logger;
            _recruiterCompanyService = recruiterCompanyService;
            _missingHourService = missingHourService;
            _missingHourDocService = missingHourDocService;

            _workTime_BL = new CandidateWorkTimeModel_BL(_workContext, _logger, _localizationService, 
                                                         _candidateJobOrderService, _clockTimeService, _workTimeService, _recruiterCompanyService);
        }

        #endregion


        #region GET ://TimeSheet/CandidateAttendance

        public ActionResult CandidateAttendance(DateTime? firstDateOfWeek, int? companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            ViewBag.firstDateOfWeek = firstDateOfWeek ?? null;
            ViewBag.companyId = companyId??0;

            return View();
        }

        #endregion

        #region POST://TimeSheet/CandidateAttendance

        [HttpPost]
        public ActionResult CandidateAttendance([DataSourceRequest] DataSourceRequest request, DateTime? firstDateOfWeek, int? companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();
            CandidateAttendanceModel_BL model_BL = new CandidateAttendanceModel_BL();

            var gridModel = model_BL.GetAllAttendanceList(firstDateOfWeek, companyId, _jobOrderService, _workContext, _recruiterCompanyService);
            return Json(gridModel);
        }


        #region get company list

        public ActionResult GetCompanies(DataSourceRequest request)
        {
            CandidateAttendanceModel_BL model_BL = new CandidateAttendanceModel_BL();
            var companies = model_BL.GetCompanyList(_workContext, _companyService, _recruiterCompanyService);
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region GET ://TimeSheet/JobOrderWorkTime

        public ActionResult JobOrderWorkTime(Guid? guid, DateTime firstDateOfWeek)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();
            var jobOrder = _jobOrderService.GetJobOrderByGuid(guid);
            if (jobOrder == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            //ViewBag.jobOrderId = jobOrder.Id;
            ViewBag.jobOrderGuid = guid;
            ViewBag.jobOrderStartDate = jobOrder.StartDate.ToString("yyyy-MM-dd");
            ViewBag.JobOrderEndDate =jobOrder.EndDate.GetValueOrDefault(DateTime.Today).ToString("yyyy-MM-dd");
            ViewBag.firstDateOfWeek = firstDateOfWeek;
            ViewBag.companyId = jobOrder.CompanyId;
            
            return View();
        }

        #endregion

        #region POST://TimeSheet/JobOrderWorkTime

        [HttpPost]
        public ActionResult JobOrderWorkTime([DataSourceRequest] DataSourceRequest request, Guid? guid, DateTime firstDateOfWeek)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var jo = _jobOrderService.GetJobOrderByGuid(guid);
            if (jo == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }

            var businessLogic = new CandidateAttendanceModel_BL();
            var allWorkTimes = businessLogic.GetJobOrderWorkTime(_stateProvinceService, _companyLocationService, _candidateJobOrderService, _workTimeService, _candidatesService, jo, firstDateOfWeek);

            return Json(allWorkTimes.ToDataSourceResult(request));
        }

        #endregion


        #region GET ://TimeSheet/CandidateWorkTime

        public ActionResult CandidateWorkTime(int candidateId = 0, DateTime? jobStartDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            ViewBag.CandidateId = candidateId;
            ViewBag.JobStartDate = jobStartDate;

            return View();
        }

        #endregion


        #region POST://TimeSheet/CandidateWorkTime

        [HttpPost]
        public ActionResult CandidateWorkTime([DataSourceRequest] DataSourceRequest request, int candidateId = 0, DateTime? jobStartDate = null, DateTime? jobEndDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var workTimes = _workTime_BL.GetAllCandidateWorkTime(request, false, candidateId, jobStartDate, jobEndDate);
            var result = workTimes.ProjectTo<CandidateWorkTimeModel>();

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region GET ://TimeSheet/CandidateWorkTimeMatch

        public ActionResult CandidateWorkTimeMatch(Guid? guid = null, DateTime? clockInOut = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();
            ViewBag.CandidateGuid = guid;
            ViewBag.ClockInOut = clockInOut;

            return View();
        }

        #endregion


        #region POST://TimeSheet/CandidateWorkTimeMatch

        [HttpPost]
        public ActionResult CandidateWorkTimeMatch([DataSourceRequest] DataSourceRequest request, Guid? guid = null, DateTime? clockInOut = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var matchedWorkTimes = _workTime_BL.GetMatchedCandidateWorkTime();

            if (guid != null)
                matchedWorkTimes = matchedWorkTimes.Where(x => x.Candidate.CandidateGuid == guid);
            if (clockInOut.HasValue)
            {
                var minClockInOut = clockInOut.Value.AddMinutes(-5);
                var maxClockInOut = clockInOut.Value.AddMinutes(+5);
                matchedWorkTimes = matchedWorkTimes.Where(x => (x.ClockIn >= minClockInOut && x.ClockIn <= maxClockInOut) ||
                                                               (x.ClockOut >= minClockInOut && x.ClockOut <= maxClockInOut));
            }

            return Json(matchedWorkTimes.ProjectTo<CandidateWorkTimeModel>().ToDataSourceResult(request));
        }

        #endregion


        #region POST://TimeSheet/ConfirmWorkTime

        [HttpPost]
        public JsonResult ConfirmWorkTime(int id)
        {
            try
            {
                _workTime_BL.ConfirmWorkTime(id);
            }
            catch (WfmException ex)
            {
                return Json(new { Succeed = false, Error = ex.Message });
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                return Json(new { Succeed = false, Error = _localizationService.GetResource("Common.UnexpectedError") });
            }
           

            return Json(new { Succeed = true });
        }

        #endregion


        #region GET ://TimeSheet/CalculateCandidateWorkTime (Capture)

        [HttpGet]
        public ActionResult CalculateCandidateWorkTime()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var model = new CalculateCandidateWorkTimeModel();

            return View(model);
        }

        #endregion

        #region POST://TimeSheet/CalculateCandidateWorkTime (Capture)

        [HttpPost]
        public async Task<ActionResult> CalculateCandidateWorkTime(CalculateCandidateWorkTimeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();


            // Scan and calculate Candidate Work Time
            // -----------------------------------------------
            //var errors = _workTimeService.CaptureCandidateWorkTime(model.FromDate.Value, model.ToDate.Value, _workContext.CurrentAccount); // Synchronous
            //var errors = await _workTimeService.CaptureCandidateWorkTimeAsync(model.FromDate.Value, model.ToDate.Value, _workContext.CurrentAccount); // Asynchronous

            var errors = await System.Threading.Tasks.Task.Run<IList<string>>(() => _workTimeService.CaptureCandidateWorkTime(model.FromDate.Value, model.ToDate.Value, _workContext.CurrentAccount, true, false)); // Asynchronous



            //activity log
            _activityLogService.InsertActivityLog("CalculateCandidateWorkTime", _localizationService.GetResource("ActivityLog.CalculateCandidateWorkTime"), model.FromDate.Value.ToString("yyyy-MMM-dd") + " - " + model.ToDate.Value.ToString("yyyy-MMM-dd") + " : " + String.Join(", ", errors));


            // Display error message if any
            if (errors.Count > 0)
                foreach (var error in errors) ErrorNotification(error);
            else
                SuccessNotification(_localizationService.GetResource("Admin.TimeSheet.CalculateCandidateWorkTime.Calculated"));



            return RedirectToAction("CandidateWorkTime");
        }

        #endregion


        #region Adjust

        public ActionResult AdjustCandidateWorkTime(Guid? jobOrderGuid, Guid? candidateGuid, DateTime startDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return Content("Access denied!");

            var jobOrder = _jobOrderService.GetJobOrderByGuid(jobOrderGuid);
            if (jobOrder == null)
                return Content("The job order does not exist!");

            var candidate = _candidatesService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
                return Content("The candidate does not exist!");

            var model = new CandidateWorkTimeModel();
            var cwt = _workTimeService.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(candidate.Id, jobOrder.Id, startDate);
            if (cwt == null)
                return Content("The time sheet not found!");

            return PartialView("_AdjustCandidateWorkTime", _workTimeService.GetWorkTimeById(cwt.Id).ToModel());
        }


        [HttpPost]
        public JsonResult SaveAdjustedWorkTime([Bind(Exclude = "Id")]CandidateWorkTimeModel model)
        {
            var errorMessage = new StringBuilder();
            var result = true;

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    var cwt = _workTimeService.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(model.CandidateId, model.JobOrderId, model.JobStartDateTime);
                    if (cwt == null)
                        errorMessage.AppendLine("The time sheet not found!");
                    else
                    {
                        if (model.NetWorkTimeInHours == 0)
                            // void 0 hrs
                            _workTimeService.ChangeCandidateWorkTimeStatus(cwt, CandidateWorkTimeStatus.Voided, _workContext.CurrentAccount);
                        else
                            _workTimeService.ManualAdjustCandidateWorkTime(cwt.CandidateId, cwt.JobOrderId, cwt.JobStartDateTime, model.JobOrderId,
                                model.AdjustmentInMinutes, model.NetWorkTimeInHours, model.Note);
                    }
                }
                catch (WfmException ex)
                {
                    errorMessage.AppendLine(ex.Message);
                    result = false;
                }
                catch (Exception exc)
                {
                    errorMessage.AppendLine(_localizationService.GetResource("Common.UnexpectedError"));
                    _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                    result = false;
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
                result = false;
            }

            return Json(new { Result = result, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Change Status

        public ActionResult ChangeCandidateWorkTimeStatus(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return Content("Access denied!");

            var workTime = _workTimeService.GetWorkTimeById(id);
            if (workTime == null)
                return Content("The time sheet not found!");

            var model = new EditCandidateWorkTimeStatusModel()
            {
                CandidateWorkTimeId = id,
                CandidateWorkTimeStatusId = workTime.CandidateWorkTimeStatusId,

                EmployeeLastName = workTime.Candidate.LastName,
                EmployeeFirstName = workTime.Candidate.FirstName,
                CompanyName = workTime.JobOrder.Company.CompanyName,
                JobTitle = workTime.JobOrder.JobTitle,
                JobStartDateTime = workTime.JobStartDateTime,
                JobEndDateTime = workTime.JobEndDateTime,
            };

            return PartialView("_ChangeCandidateWorkTimeStatus", model);
        }


        [HttpPost]
        public JsonResult ChangeCandidateWorkTimeStatus(EditCandidateWorkTimeStatusModel model)
        {
            var errorMessage = new StringBuilder();

            //// Approved is not allowed
            //if (model.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved)
            //    errorMessage.AppendLine(_localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeStatus.NotAllowed"));

            if (errorMessage.Length == 0 && ModelState.IsValid)
            {
                var workTime = _workTimeService.GetWorkTimeById(model.CandidateWorkTimeId);
                if (workTime == null)
                    errorMessage.AppendLine("Time sheet does not exist !");
                else
                {
                    // paid work time is not allowed
                    if (workTime.Payroll_BatchId.HasValue && workTime.Payroll_BatchId != 0)
                        errorMessage.AppendLine(_localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeStatus.NotAllowed"));

                    // same status
                    else if (model.CandidateWorkTimeStatusId == workTime.CandidateWorkTimeStatusId)
                        errorMessage.AppendLine(_localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeStatus.StatusMustBeDifferent"));

                    // 0 hrs to be approved
                    else if (model.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved && workTime.NetWorkTimeInHours == 0)
                        errorMessage.AppendLine("0 hrs cannot be approved");

                    else
                    {
                        workTime.Note = model.Note;
                        _workTimeService.ChangeCandidateWorkTimeStatus(workTime, (CandidateWorkTimeStatus)model.CandidateWorkTimeStatusId, _workContext.CurrentAccount);

                        // re-calculation, for pending approval
                        if (workTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval
                            && workTime.ClockIn.HasValue && workTime.ClockOut.HasValue)
                        {
                            _workTimeService.CalculateAndSaveWorkTime(workTime);
                            _workTimeService.RemoveOtherMatchedWorkTimes(workTime);
                            _workTimeService.SetClockTimeStatusByWorkTime(workTime);
                        }

                        //log
                        _activityLogService.InsertActivityLog("EditCandidateWorkTimeStatus", _localizationService.GetResource("ActivityLog.EditCandidateWorkTimeStatus"), workTime.CandidateId + "/" + workTime.JobOrderId + "/" + workTime.JobStartDateTime.ToString("yyyy-MM-dd HH:mm") + " : " + Enum.GetName(typeof(CandidateWorkTimeStatus), workTime.CandidateWorkTimeStatusId));
                    }
                }
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POST://TimeSheet/TimeSheetApproval

        [HttpPost]
        public JsonResult ApproveTimeSheet(int companyId, int vendorId, DateTime startDate, DateTime endDate, int? supervisorId)
        {
            try
            {
                _workTimeService.ApproveWorkTimeByCompanyIdAndDateRange(companyId, supervisorId, startDate, endDate, vendorId);
            }
            catch (WfmException ex)
            {
                return Json(new { Succeed = false, Error = ex.Message });
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                return Json(new { Succeed = false, Error = _localizationService.GetResource("Common.UnexpectedError") });
            }

            return Json(new { Succeed = true });
        }

        [HttpGet]
        public JsonResult ApproveTimeSheetForSelectedDateRangeAndCompanies(DateTime startDate, DateTime endDate, string selectedIds)
        {
            int approved = 0, failed = 0;
            StringBuilder errorMessage = new StringBuilder();

            if (!string.IsNullOrEmpty(selectedIds))
            {
                var ids = FilterSelectedIds(selectedIds.Split(','));//companyId/vendorId/supervisorId
                foreach (var id in ids)
                {
                    var comboId = id.Split('/');
                    int companyId = Convert.ToInt32(comboId[0]);
                    int vendorId = Convert.ToInt32(comboId[1]);

                    int? supervisorId = null;
                    if (comboId.Count()==3)
                        supervisorId = Convert.ToInt32(comboId[2]);
                    try
                    {
                        _workTimeService.ApproveWorkTimeByCompanyIdAndDateRange(companyId, supervisorId, startDate, endDate, vendorId);
                        approved++;
                    }
                    catch (WfmException ex)
                    {
                        errorMessage.AppendLine(ex.Message);
                        failed++;
                    }
                    catch (Exception exc)
                    {
                        _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                        errorMessage.AppendLine(_localizationService.GetResource("Common.UnexpectedError"));
                        failed++;
                    }
                   
                }
            }
            else
                errorMessage.AppendLine("No company is selected.");

            return Json(new { Approved = approved, Failed = failed, ErrorMessage = String.Concat(approved," records are approved.\r\n",failed," records fail to be approved.") }, JsonRequestBehavior.AllowGet);
        }


        private List<string> FilterSelectedIds(string[] selectedIds)
        {
            //all string has the same format CompanyId/VendorId/SupervisorId
            List<string> result1 = new List<string>();//CompanyId/VendorId
            List<string> result2 = new List<string>();//CompanyId/VendorId/SupervisorId
            foreach (string id in selectedIds)
            {
                var arr = id.Split('/');
                if (arr.Count()==3)
                    result2.Add(id);
                else
                    result1.Add(id);
            }
            if (result1.Count <= 0)
                return result2;
            foreach (string result in result1)
            {
                int count = result2.RemoveAll(x => x.StartsWith(result));
            }
            result1.AddRange(result2);
            return result1;
        }

        [HttpGet]
        public JsonResult ApproveTimeSheetForSelectedDateRange(string interval, DateTime startDate, DateTime endDate)
        {
            var result = new JsonResult();
            string errorMessage = string.Empty;

            var intervalId = _invoiceIntervalServcie.GetInvoiceIntervalIdByCode(interval);
            var companyIds = _companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount)
                             .Where(x => x.InvoiceIntervalId == intervalId).Select(x => x.Id);
            var summaries = _workTimeService.GetTimeSheetSummaryForApprovalByDateRange(startDate, endDate)
                            .Where(x => x.SubmittedHours > 0 && companyIds.Contains(x.CompanyId))
                            .Select(x => x.CompanyId.ToString() + "/" + x.VendorId.ToString()).ToList();
            if (summaries.Count > 0)
                return ApproveTimeSheetForSelectedDateRangeAndCompanies(startDate, endDate, String.Join(",", summaries));
            else
            {
                errorMessage = "No companies need approval.";
                return Json(new { Approved = 0, Failed = 0, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region Missing Hour

        public ActionResult MissingHour(string tabId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ProcessMissingHour))
                return AccessDeniedView();
            if (!String.IsNullOrWhiteSpace(tabId))
                ViewBag.TabId = tabId;

            return View();
        }


        public ActionResult _TabMissingHour()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ProcessMissingHour))
                return AccessDeniedView();

            return PartialView();
        }


        [HttpPost]
        public ActionResult _MissingHour([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ProcessMissingHour))
                return AccessDeniedView();

            var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
            var missingHours = _missingHour_BL.GetAllCandidateMissingHour();
            missingHours = missingHours.Where(x => x.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Approved);
            var result = missingHours.ProjectTo<CandidateMissingHourModel>();

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _TabMissingHourHistory()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ProcessMissingHour))
                return AccessDeniedView();

            return PartialView();
        }


        [HttpPost]
        public ActionResult _MissingHourHistory([DataSourceRequest] DataSourceRequest request, DateTime? jobStartDate = null, DateTime? jobEndDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ProcessMissingHour))
                return AccessDeniedView();

            var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
            var missingHours = _missingHour_BL.GetAllCandidateMissingHour(jobStartDate: jobStartDate, jobEndDate: jobEndDate, includeProcessed: true);
            var result = missingHours.ProjectTo<CandidateMissingHourModel>();

            return Json(result.ToDataSourceResult(request));
        }


        public PartialViewResult _ReviewMissingHour(int missingHourId)
        {
            var missingHour = _missingHourService.GetMissingHourById(missingHourId);

            return PartialView("_ReviewMissingHour", missingHour.ToModel());
        }


        [HttpPost]
        public ActionResult _MissingHourDocuments(int missingHourId)
        {
            var model = _missingHourDocService.GetAllMissingHourDocumentsByMissingHourId(missingHourId).ToList().Select(x => x.ToModel());

            return PartialView("_MissingHourDocuments", model);
        }


        [HttpPost]
        public ActionResult _ProcessMissingHour(int missingHourId)
        {
            var warning = String.Empty;
            var error = String.Empty;

            // check supproting documents
            var missingHourDocs = _missingHourDocService.GetAllMissingHourDocumentsByMissingHourId(missingHourId);
            if (missingHourDocs == null || !missingHourDocs.Any())
                return Json(new { Result = false, ErrorMessage = "Missing hour document is required." }, JsonRequestBehavior.AllowGet);

            var _missingHour_BL = new CandidateMissingHour_BL(_logger, _localizationService, _missingHourService, _jobOrderService);
            error = _missingHour_BL.ProcessMissingHour(_candidateJobOrderService, _workTimeService, _workTimeLogRepository, 
                missingHourId, _workContext.CurrentAccount.Id, out warning);

            return Json(new { Result = String.IsNullOrWhiteSpace(error), ErrorMessage = error, WarningMessage = warning }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult _VoidMissingHour(int missingHourId, string reason)
        {
            var error = String.Empty;
            if (String.IsNullOrWhiteSpace(reason))
                error = "Reason is reuqired";
            else
            {
                var missingHour = _missingHourService.GetMissingHourById(missingHourId);
                if (missingHour == null)
                    error = "Cannot find the missing hour record";
                else
                {
                    missingHour.CandidateMissingHourStatusId = (int)CandidateMissingHourStatus.Voided;
                    missingHour.Note += String.Concat(" [voided by ", _workContext.CurrentAccount.FullName, ", reason: ", reason, "]");
                    _missingHourService.UpdateCandidateMissingHour(missingHour);
                }
            }

            return Json(new { Result = String.IsNullOrWhiteSpace(error), ErrorMessage = error }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _UpdateMissingHour(int missingHourId, decimal hours, string note)
        {
            var error = String.Empty;
            if (String.IsNullOrWhiteSpace(note))
                error = "Reason is reuqired";
            else
            {
                var missingHour = _missingHourService.GetMissingHourById(missingHourId);
                if (missingHour == null)
                    error = "Cannot find the missing hour record";
                else
                {
                    missingHour.PayrollNote = note;
                    missingHour.NewHours = hours;

                    _missingHourService.UpdateCandidateMissingHour(missingHour);
                }
            }

            return Json(new { Result = String.IsNullOrWhiteSpace(error), ErrorMessage = error }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ExportMissingHourToXlsx(string selectedIds)
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

            return RedirectToAction("MissingHour", new { tabId = "tab-history" });
        }

        #endregion


        #region Invoice

        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult Invoice(string tabId = null, string interval = null, DateTime? termStartDate = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return AccessDeniedView(); 

            if (!String.IsNullOrWhiteSpace(tabId))
                ViewBag.TabId = tabId;
            ViewBag.Interval = interval;
            ViewBag.TermStartDate = termStartDate;

            return View();
        }


        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult _TabInvoice(string interval, DateTime? termStartDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return AccessDeniedView();

            var refDate = termStartDate.HasValue ? termStartDate.Value : DateTime.Today.AddDays(-7);
            refDate = refDate.StartOfWeek(DayOfWeek.Sunday);

            ViewBag.TermStartDate = refDate;
            ViewBag.Interval = _invoiceIntervalServcie.GetInvoiceIntervalIdByCode(!String.IsNullOrWhiteSpace(interval) ? interval : "WEEKLY").ToString();

            return PartialView();
        }


        [HttpPost]
        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult TimeSheetSummaryForInvoice([DataSourceRequest] DataSourceRequest request, string interval, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return AccessDeniedView();

            var intervalId = _invoiceIntervalServcie.GetInvoiceIntervalIdByCode(interval);
            var companyIds = _companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount)
                             .Where(x => x.InvoiceIntervalId == intervalId).Select(x => x.Id);
            var totalData = _workTimeService.GetTimeSheetSummaryForApprovalByDateRange(startDate, endDate)
                             .Where(x => companyIds.Contains(x.CompanyId)).ToList();

            var gridModel = totalData.ToDataSourceResult(request);

            return Json(gridModel);
        }


        [HttpPost]
        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult CreateInvoice(int companyId, int vendorId, DateTime fromDate, DateTime toDate)
        {
            if (!(_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet) && 
                  _permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings)))
                return AccessDeniedView();

            var workTimes = _workTimeService.GetWorkTimeByStartEndDateAsQueryable(fromDate, toDate).Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved)
                            .Where(x => x.CompanyId == companyId && x.FranchiseId == vendorId);

            List<int> jobOrders = new List<int>();
            //if (supervisorId != null)
            //    workTimes = workTimes.Where(x => x.CompanyContactId == supervisorId);
            jobOrders = workTimes.Select(x => x.JobOrderId).Distinct().ToList();
            var jobOrderWithoutRates = _jobOrderService.DoAllJobOrdersHaveBillingRates(jobOrders, fromDate, toDate);

            if (jobOrderWithoutRates.Count > 0)
                ErrorNotification(String.Format("Billing Rate not defined for Job Order: {0}", String.Join(", ", jobOrderWithoutRates)));
            else
            {
                var timeCharts = _employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryForInvoice(fromDate, toDate, companyId, vendorId);
                try
                {
                    string fileName = string.Empty;
                    byte[] bytes = null;
                    using (var stream = new MemoryStream())
                    {
                        fileName = _exportManager.ExportEmployeeTimeChartToXlsxForAdmin(stream, timeCharts, fromDate, toDate, companyId, vendorId, true);
                        bytes = stream.ToArray();
                    }

                    _activityLogService.InsertActivityLog("CreateInvoice", _localizationService.GetResource("ActivityLog.CreateInvoice"), fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), _companyService.GetCompanyById(companyId).CompanyName);

                    _workTimeService.SetWorkTimeInvoiceDate(companyId, fromDate, toDate);

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

            var intervalCode = _invoiceIntervalServcie.GetInvoiceIntervalById(_companyService.GetCompanyById(companyId).InvoiceIntervalId).Code;
            return RedirectToAction("Invoice", new { tabId = "tab-invoice", interval = intervalCode, termStartDate = fromDate.ToString("yyyy-MM-dd") });
        }

        #endregion


        #region POST://TimeSheet/SendPendingApprovalReminder

        [HttpPost]
        public JsonResult SendPendingApprovalReminder(int companyId, DateTime startDate, DateTime endDate, int vendorId, int? supervisorId)
        {
            string errorMessage = string.Empty;

            try
            {
                var dueDay = endDate.AddDays(1 + _candidateWorkTimeSettings.PendingApprovalDueDay);
                var dueTime = DateTime.MinValue.AddHours(14);                                           // 14:00
                DateTime.TryParse(_candidateWorkTimeSettings.PendingApprovalDueTime, out dueTime);
                dueTime = dueDay.Date + dueTime.TimeOfDay;
                var todayNow = DateTime.Now;
                // due already
                if (dueTime < todayNow)
                    dueTime = todayNow.AddDays(_candidateWorkTimeSettings.PendingApprovalDueDay - _candidateWorkTimeSettings.PendingApprovalReminderDay).Date + dueTime.TimeOfDay;

                var receivers = new List<Account>();
                var account = new Account();

                // supervisor
                if (supervisorId.HasValue && supervisorId > 0)
                {
                    account = _accountService.GetAccountById(supervisorId.Value);
                    if (account != null)
                        receivers.Add(account);
                }
                // HR managers
                else
                {
                    receivers = _accountService.GetAllCompanyHrAccountsByCompany(companyId).ToList();
                    if (receivers.Any())
                        account = receivers.FirstOrDefault();       // first or any HR manager
                }

                if (receivers.Any())
                {
                    byte[] attachmentFile;
                    var attachmentFileName = _timeSheetService.GetTimeSheetAttachment(account, startDate, endDate, out attachmentFile, vendorId);
                    if (!String.IsNullOrWhiteSpace(attachmentFileName))
                        _workflowMessageService.SendPendingApprovalReminder(1, receivers, todayNow, dueTime, startDate, endDate, attachmentFileName, attachmentFile);

                    var nameList = String.Join(", ", receivers.Select(x => x.FullName));
                    return Json(new { Succeed = true, ErrorMessage = String.Format("A 'Pending Approval Reminder' will be sent to {0}.", nameList) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errorMessage += "The supervisor or company HR does not exist!";
                    return Json(new { Succeed = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (WfmException exc)
            {
                _logger.Error(string.Format("Error occurred while sending approval reminder. Error message : {0}", exc.Message), exc, userAgent: Request.UserAgent);
                return Json(new { Succeed = false, ErrorMessage = exc.Message });

            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while sending approval reminder. Error message : {0}", exc.Message), exc, userAgent: Request.UserAgent);
                return Json(new { Succeed = false, ErrorMessage = _localizationService.GetResource("Common.UnexpectedError") });
            }

        }

        #endregion


        #region GET://TimeSheet/ChangesAfterInvoice

        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult _TabInvoiceUpdates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return AccessDeniedView();

            return PartialView();
        }

        #endregion

        #region POST://TimeSheet/ChangesAfterInvoice

        [HttpPost]
        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult ChangesAfterInvoice([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return AccessDeniedView();
            WorkTimeChangeAfterInvoiceModel_BL model_BL = new WorkTimeChangeAfterInvoiceModel_BL();
            var changes = model_BL.GetWorkTimeAfterInvoiceModel(startDate, endDate, _workTimeService);
            var gridModel = changes.ToDataSourceResult(request);
            return Json(gridModel);
        }
        
        #endregion


        #region Export / Import

        #region Import Work Time from Excel Sheet

        [HttpPost]
        public ActionResult ImportExcel()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ImportTimeSheets))
                return AccessDeniedView();

            //only MSP can import time sheet, for the time being
            if (_workContext.CurrentFranchise.Id != _franchiseService.GetDefaultMSPId())
                return AccessDeniedView();

            try
            {
                IList<string> errors = new List<string>();
                IList<string> warnings = new List<string>();
                var isStdTmplt = false;

                var file = Request.Files["importexcelfile"];
                
                if (file == null || file.ContentLength == 0)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                    return RedirectToAction("EmployeeTimeChartHistory", "TimeSheet");
                }

                var fileName = file.FileName;
                if (Path.GetExtension(fileName) == ".xlsx")
                {
                    var fileNameSegments0 = file.FileName.Split('_');
                    var fileNameSegments1 = file.FileName.Split(' ');

                    // if standard template file name
                    if (fileNameSegments0.Count() >= 2 &&
                        fileNameSegments0[0] == "Employee" && fileNameSegments0[1] == "Timesheets")
                        isStdTmplt = true;
                }
                else
                    errors.Add("The file must be an Excel 2007 or higher version.");

                // import
                if (!errors.Any())
                    errors = _importManager.ImportWorkTimeFromXlsx(file.InputStream, out warnings, isStdTmplt);

                var extraLog = String.Empty;
                
                // if any errors
                if (errors.Any())
                {
                    foreach (var error in errors) ErrorNotification(error);
                    extraLog = String.Concat(" [Error-- ", String.Join(", ", errors), " --]");
                }
                else
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Payroll.ImportTimeSheet.Imported"));

                    // if any warnings
                    if (warnings.Any())
                    {
                        foreach (var warning in warnings) WarningNotification(warning);
                        extraLog = String.Concat(" [Warning-- ", String.Join(", ", warnings), " --]");
                    }
                }

                //activity log
                _activityLogService.InsertActivityLog("ImportWorkTime", _localizationService.GetResource("ActivityLog.ImportWorkTime"), String.Concat(file.FileName, extraLog));

                return RedirectToAction("EmployeeTimeChartHistory", "TimeSheet");
            }
            catch (WfmException exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("EmployeeTimeChartHistory", "TimeSheet");
            }
            catch (Exception exc)
            {
                ErrorNotification(_localizationService.GetResource("Common.UnexpectedError"));
                _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                return RedirectToAction("EmployeeTimeChartHistory", "TimeSheet");
            }

        }

        #endregion


        #region Export Work Times to Xlsx

        [HttpPost]
        public ActionResult ExportWorkTimesToXlsx(string selectedIds)
        {
            return ExportWorkTimes(selectedIds, type: "Xlsx");
        }

        [HttpPost]
        public ActionResult ExportWorkTimesToPdf(string selectedIds)
        {
            return ExportWorkTimes(selectedIds, type: "Pdf");
        }

        private ActionResult ExportWorkTimes(string selectedIds, string type = "Xlsx")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var urlString = Request.UrlReferrer.ToString();

            var workTimes = new List<CandidateWorkTime>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                workTimes.AddRange(_workTimeService.GetWorkTimeByIds(ids));
            }

            // Validate selected items
            if (workTimes.Count() == 0)
            {
                ErrorNotification("Please select one or more items");
                return Redirect(urlString);
            }

            // Export to Xlsx
            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = "";
                    if (type == "Pdf")
                    {
                        _pdfService.PrintCandidateWorkTimesToPdf(stream, workTimes);
                    }
                    else
                    {
                        _exportManager.ExportCandidateWorkTimeToXlsx(stream, workTimes);
                    }

                    bytes = stream.ToArray();
                }

                //activity log
                var paramName = type == "Pdf" ? "Pdf" : "Xlsx";
                _activityLogService.InsertActivityLog("ExportWorkTime", _localizationService.GetResource("ActivityLog.ExportWorkTime"), paramName + "/" + selectedIds);

                // return File(bytes, "text/xls", fileName);
                var contentType = type == "Pdf" ? "application/pdf" : "text/xls";
                var fileDownloadName = type == "Pdf" ? "DailyTimeSheets.pdf" : "DailyTimeSheets.xlsx";
                return File(bytes, contentType, fileDownloadName);
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

            return Redirect(urlString);
        }

        public async Task<ActionResult> ExportWorkTimeChangesToXlsx(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            var urlString = Request.UrlReferrer.ToString();

            var workTimes = _employeeTimeSheetHistoryService.GetInvoiceUpdatesFromDailyWorkTime(selectedIds);

            // Validate selected items
            if (workTimes.Count() == 0)
            {
                ErrorNotification("Please select one or more items");
                //return Redirect(urlString);
                return RedirectToAction("Invoice", new { tabId = "tab-updates" });
            }

            // Export to Xlsx
            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportWorkTimeChangesAfterInvoiceToXlsx(stream, workTimes);
                    bytes = stream.ToArray();
                }

                //activity log
                _activityLogService.InsertActivityLog("Export Invoice Update Details", _localizationService.GetResource("ActivityLog.ExportWorkTime"), "Xlsx" + "/" + selectedIds);
                await _workTimeService.SetWorkTimeInvoiceDate(selectedIds);
                // return File(bytes, "text/xls", fileName);
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

            //return Redirect(urlString);
            return RedirectToAction("Invoice", new { tabId = "tab-updates" });
        }

        #endregion

        #endregion


        #region GET ://TimeSheet/EmployeeTimeChartHistory

        [HttpGet]
        public ActionResult EmployeeTimeChartHistory()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();
            ViewBag.Status = _employeeTimeSheetHistoryService.GetAllCandidateWorkTimeStatusFromEnum();
            return View(ViewBag.Status);
        }

        #endregion


        #region POST://TimeSheet/EmployeeTimeChartHistory

        [HttpPost]
        public ActionResult EmployeeTimeChartHistory([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate, string status,string weeklyStatus)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            //all history time chart
            var employeeTimeSheetHistory = _employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByDateAndAccount(startDate, endDate, _workContext.CurrentAccount, status, weeklyStatus);

            return Json(employeeTimeSheetHistory.ToDataSourceResult(request));
        }

        #endregion


        #region Print Employee Time Charts to Pdf

        [HttpPost]
        public ActionResult PrintEmployeeTimeChartsToPdf(string selectedIds, DateTime hidenStartDate, DateTime hidenEndDate, string hidenStatus, string hidenWeeklyStatus)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();

            //var urlString = Request.UrlReferrer.ToString();

            var timeCharts = new List<EmployeeTimeChartHistory>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                timeCharts.AddRange(_employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByIds(ids,hidenStartDate,hidenEndDate,hidenStatus,hidenWeeklyStatus));
            }

            // Validate selected items
            if (timeCharts.Count() == 0)
            {
                ErrorNotification("Please select one or more items");
                return RedirectToAction("EmployeeTimeChartHistory");
            }
            var jobOrders = timeCharts.Select(x => x.JobOrderId).Distinct().ToList();
            var jobOrderWithoutRates = _jobOrderService.DoAllJobOrdersHaveBillingRates(jobOrders, hidenStartDate, hidenEndDate);

            if (jobOrderWithoutRates.Count > 0)
            {
                ErrorNotification(String.Format("Billing Rate not defined for Job Order: {0}", String.Join(", ", jobOrderWithoutRates)));
                return RedirectToAction("EmployeeTimeChartHistory");
            }

            // Print to Pdf
            try
            {
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintEmployeeTimeChartHistoryToPdfForAdmin(stream, timeCharts,
                        withRates: _permissionService.Authorize(StandardPermissionProvider.ViewCompanyBillingRates));
                    bytes = stream.ToArray();
                }


                //activity log
                _activityLogService.InsertActivityLog("ExportTimeChart", _localizationService.GetResource("ActivityLog.ExportTimeChart"), "Pdf" + "/" + selectedIds);


                return File(bytes, "application/pdf", "EmployeeTimeCharts.pdf");
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

            return RedirectToAction("EmployeeTimeChartHistory");
        }

        #endregion


        #region Export Employee Time Charts to Xlsx

        [HttpPost]
        public ActionResult ExportEmployeeTimeChartsToXlsx(string selectedIds, DateTime hidenStartDate, DateTime hidenEndDate,string hidenStatus,string hidenWeeklyStatus)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateWorkTime))
                return AccessDeniedView();


            var timeCharts = new List<EmployeeTimeChartHistory>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                timeCharts.AddRange(_employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByIds(ids, hidenStartDate, hidenEndDate, hidenStatus, hidenWeeklyStatus));
            }

            // Validate selected items
            if (timeCharts.Count() == 0)
            {
                ErrorNotification("Please select one or more items");
                return RedirectToAction("EmployeeTimeChartHistory");
            }
            var jobOrders = timeCharts.Select(x => x.JobOrderId).Distinct().ToList();
            var jobOrderWithoutRates = _jobOrderService.DoAllJobOrdersHaveBillingRates(jobOrders, hidenStartDate, hidenEndDate);

            if (jobOrderWithoutRates.Count > 0)
            {
                ErrorNotification(String.Format("Billing Rate not defined for Job Order: {0}", String.Join(", ", jobOrderWithoutRates)));
                return RedirectToAction("EmployeeTimeChartHistory");
            }


            // Export to Xlsx
            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportEmployeeTimeChartToXlsxForAdmin(stream, timeCharts,hidenStartDate,hidenEndDate, 
                        withRates: _permissionService.Authorize(StandardPermissionProvider.ViewCompanyBillingRates));
                    bytes = stream.ToArray();
                }

                //activity log
                _activityLogService.InsertActivityLog("ExportTimeChart", _localizationService.GetResource("ActivityLog.ExportTimeChart"), "Xlsx" + "/" + selectedIds);

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

            return RedirectToAction("EmployeeTimeChartHistory");
        }

        #endregion


        #region TimeSheetDetails
        [HttpPost]
        public ActionResult TimeSheetDetailsForInvoice([DataSourceRequest]DataSourceRequest request, int companyId, int vendorId, DateTime startDate, DateTime endDate)
        {
            var result = _timeSheetService.GetAllTimeSheetDetails(companyId, vendorId, startDate, endDate);

            return Json(result.ToDataSourceResult(request));
        }
        #endregion
    }
}

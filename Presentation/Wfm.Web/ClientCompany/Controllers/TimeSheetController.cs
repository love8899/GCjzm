using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wfm.Client.Extensions;
using Wfm.Client.Models.ClockTime;
using Wfm.Client.Models.TimeSheet;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.ExportImport;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Models.Search;
using Wfm.Web.Framework.Feature;
using Wfm.Services.Accounts;
using Wfm.Services.Configuration;

namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Time Sheet")]
    public class TimeSheetController : BaseClientController
    {
        #region Fields

        private readonly IJobOrderService _jobOrderService;
        private readonly IWorkContext _workContext;
        private readonly IClockTimeService _clockTimeService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IPermissionService _permissionService;
        private readonly IActivityLogService _activityLogService;
        private readonly IExportManager _exportManager;
        private readonly IPdfService _pdfService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrgNameService _orgNameService;
        private readonly IEmployeeTimeChartHistoryService _employeeTimeSheetHistoryService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateService _candidateService;
        private readonly IAccountService _accountService;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        private readonly SearchBusinessLogic _searchBL;

        #endregion


        #region Ctor

        public TimeSheetController(
            IJobOrderService jobOrderService,
            IWorkContext workContextService,
            IClockTimeService clockTimeService,
            IWorkTimeService workTimeService,
            IPermissionService permissionService,
            IExportManager exportManager,
            IActivityLogService activityLogService,
            IPdfService pdfService,
            ILocalizationService localizationService,
            IOrgNameService orgNameService,
            IEmployeeTimeChartHistoryService employeeTimeSheetHistoryService,
            IWorkflowMessageService workflowMessageService,
            ILogger logger,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateService candidateService,
            IAccountService accountService,
            ISettingService settingService
           )
        {
            _jobOrderService = jobOrderService;
            _workContext = workContextService;
            _clockTimeService = clockTimeService;
            _workTimeService = workTimeService;
            _permissionService = permissionService;
            _exportManager = exportManager;
            _activityLogService = activityLogService;
            _pdfService = pdfService;
            _localizationService = localizationService;

            _orgNameService = orgNameService;

            _employeeTimeSheetHistoryService = employeeTimeSheetHistoryService;
            _workflowMessageService = workflowMessageService;
            _logger = logger;
            _candidateJobOrderService = candidateJobOrderService;
            _candidateService = candidateService;
            _accountService = accountService;
            _settingService = settingService;
            _searchBL = new SearchBusinessLogic(_workContext, _orgNameService);
        }

        #endregion


        #region Clock Time

        public ActionResult Punches()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult Punches([DataSourceRequest] DataSourceRequest request, Account account, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            var model_BL = new CandidateClockTime_BL();
            var result = model_BL.GetAllCandidateClockTime(_workContext.CurrentAccount, _clockTimeService, startDate, endDate);

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region TimeSheet/Index

        public ActionResult Index()
        {
            return AccessDeniedView();
        }

        #endregion


        #region Daily Time Sheets


        public ActionResult EmployeeWorkTime()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            return View(_searchBL.GetSearchTimeSheetModel());
        }


        [HttpPost]
        public ActionResult EmployeeWorkTime([DataSourceRequest] DataSourceRequest request, SearchTimeSheetModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            var candidateWorkTimes = _workTimeService.GetEmployeeWorkTimeApprovalAsQueryable(model.sf_From, model.sf_To, _workContext.CurrentAccount);

            KendoHelper.CustomizePlacementBasedFilters(request, model);
            _CustomizeTimeSheetSorts(request);

            return Json(candidateWorkTimes.ToDataSourceResult(request, x => x.ToDailyTimeSheetModel()));
        }


        private void _CustomizeTimeSheetSorts([DataSourceRequest] DataSourceRequest request)
        {
            // for sorting on columns computed
            if (request.Sorts.Any())
            {
                var nameSort = request.Sorts.FirstOrDefault(x => x.Member == "EmployeeName");
                if (nameSort != null)
                {
                    var order = nameSort.SortDirection;
                    request.Sorts.Remove(nameSort);
                    request.Sorts.Add(new SortDescriptor("EmployeeFirstName", order));
                    request.Sorts.Add(new SortDescriptor("EmployeeLastName", order));
                }
            }
        }


        #endregion


        #region Time Sheet History

        [HttpGet]
        public ActionResult EmployeeTimeChartHistory()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            var from = DateTime.Today.StartOfWeek(DayOfWeek.Sunday).AddDays(-21);
            var to = from.AddDays(27);

            return View(_searchBL.GetSearchTimeSheetModel(from, to));
        }


        [HttpPost]
        public ActionResult EmployeeTimeChartHistory([DataSourceRequest] DataSourceRequest request, SearchTimeSheetModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            var status = ((int)CandidateWorkTimeStatus.Approved).ToString();
            var employeeTimeSheetHistory = _employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByDateAndAccount(model.sf_From, model.sf_To, _workContext.CurrentAccount, status);

            KendoHelper.CustomizePlacementBasedFilters(request, model,
                mapping: new Dictionary<string, string>()
                {
                    { "CandidateId", "EmployeeId" },
                    { "EmployeeId", "EmployeeNumber" },
                });

            return Json(employeeTimeSheetHistory.ToDataSourceResult(request));
        }


        #region Print Employee Time Charts to Pdf

        [HttpPost]
        public ActionResult PrintEmployeeTimeChartsToPdf(string selectedIds, DateTime hidenStartDate, DateTime hidenEndDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();


            var timeCharts = new List<EmployeeTimeChartHistory>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                timeCharts.AddRange(_employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByIds(ids, hidenStartDate, hidenEndDate, _workContext.CurrentAccount));
            }

            // Validate selected items
            if (timeCharts.Count() == 0)
            {
                ErrorNotification(_localizationService.GetResource("Common.SelectOneOrMoreItems"));
                return RedirectToAction("EmployeeTimeChartHistory");
            }


            // Print to Pdf
            try
            {
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintEmployeeTimeChartHistoryToPdfForClient(stream, timeCharts);
                    bytes = stream.ToArray();
                }


                //activity log
                _activityLogService.InsertActivityLog("ExportTimeChart", _localizationService.GetResource("ActivityLog.ExportTimeChart"), "Pdf" + "/" + selectedIds);


                return File(bytes, "application/pdf", "EmployeeTimeSheet.pdf");
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
        public ActionResult ExportEmployeeTimeChartsToXlsx(string selectedIds, DateTime hidenStartDate, DateTime hidenEndDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();


            var timeCharts = new List<EmployeeTimeChartHistory>();
            if (selectedIds != null)
            {
                var ids = selectedIds .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x)) .ToArray();
                timeCharts = _employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByIds(ids, 
                    hidenStartDate, hidenEndDate, _workContext.CurrentAccount).ToList();
                // TODO: apply the same filters as the grid
            }

            // Validate selected items
            if (timeCharts.Count == 0)
            {
                ErrorNotification(_localizationService.GetResource("Common.SelectOneOrMoreItems"));
                return RedirectToAction("EmployeeTimeChartHistory");
            }


            // Export to Xlsx
            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportEmployeeTimeChartToXlsxForClient(stream, timeCharts);
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

        #endregion


        #region Time Sheet Approval

        public ActionResult EmployeeWorkTimeApproval()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();
            //
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.CompanyLocationId = _workContext.CurrentAccount.CompanyLocationId;
            //

            var refDate = DateTime.Today.StartOfWeek(DayOfWeek.Sunday);
            ViewBag.WeekStartDate = refDate;
            ViewBag.WeekStartDay = (int)DayOfWeek.Sunday;

            var from = DateTime.Today.StartOfWeek(DayOfWeek.Sunday);
            var to = from.AddDays(6);
            var model = _searchBL.GetSearchTimeSheetModel(from, to);
            model.AvailableCandidateWorkTimeStatus = model.AvailableCandidateWorkTimeStatus
                .Where(x => x.Value == ((int)CandidateWorkTimeStatus.PendingApproval).ToString()).ToList();

            return View(model);
        }


        [HttpPost]
        public ActionResult EmployeeWorkTimeApproval([DataSourceRequest] DataSourceRequest request, SearchTimeSheetModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            KendoHelper.CustomizePlacementBasedFilters(request, model);

            CandidateWorkTimeBL _candidateWorkTimeBL = new CandidateWorkTimeBL(_workTimeService, _workContext);
            var result = _candidateWorkTimeBL.EmployeeWorkTimeApproval(request, model);

            return Json(result);
        }


        #region Reject

        [HttpGet]
        public PartialViewResult _RejectCandidateWorktime(int id, string formName)
        {
            if (!String.IsNullOrWhiteSpace(formName))
                ViewBag.FormName = formName;
            ViewBag.CandidateWorkTimeId = id;
            return PartialView();
        }


        [HttpPost]
        public ActionResult RejectWorktime(int id, string reason)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            if (_workTimeService.RejectWorkTimeEntry(id, reason, _workContext.CurrentAccount))
                return Json(new { Succeed = true });
            else
                return Json(new { Succeed = false, Error = _localizationService.GetResource("Common.UnexpectedError") });
        }

        #endregion


        [HttpPost]
        public ActionResult ApproveWorktime(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            string err;

            if (_workTimeService.ApproveWorkTimeEntry(id, _workContext.CurrentAccount, out err))
            {
                var signatureCompanies = _settingService.GetSettingByKey<string>("SignatureCompanies").Split(',')?.Select(Int32.Parse)?.ToList();

                if (signatureCompanies.Contains(_workContext.CurrentAccount.CompanyId))
                {
                    var signRet = _workTimeService.SignWorkTimeEntry(id, _workContext.CurrentAccount, out err);
                    if (!signRet)
                    {
                        return Json(new { Succeed = false, Error = err });
                    }
                }
                return Json(new { Succeed = true });
            }
            else
            {
                return Json(new { Succeed = false, Error = err }); 
            }
            
        }

        // this action isn't used for now. It meant for the case that "approve" and "sign" are conducted separated. 
        // Now the "sign" is conducted together with "approve" when the current account is in the signatureCompanies list.
        [HttpPost]
        public ActionResult SignWorktime(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            string err;

            if (_workTimeService.SignWorkTimeEntry(id, _workContext.CurrentAccount, out err))
                return Json(new { Succeed = true });
            else
                return Json(new { Succeed = false, Error = err });
            return Json(new { Succeed = true });
        }

        #region _AdjustCandidateWorkTime

        private CandidateWorkTimeModel InitializeWorkTimeModel(int id)
        {
            var workTime = _workTimeService.GetWorkTimeById(id);
            var model = workTime.ToModel();

            model.AvaliableCandidates = new List<SelectListItem>();
            model.AvaliableCandidates.Add(new SelectListItem()
            {
                Text = String.Concat(workTime.Candidate.EmployeeId, " - ", workTime.Candidate.GetFullName()),
                Value = model.CandidateId.ToString()
            });

            model.AvailableJobOrders = new List<SelectListItem>();
            model.AvailableJobOrders.Add(new SelectListItem()
            {
                Text = workTime.JobOrder.JobTitle,
                Value = model.JobOrderId.ToString()
            });

            model.AvaliableStartDateTimes = new List<SelectListItem>();
            model.AvaliableStartDateTimes.Add(new SelectListItem()
            {
                Text = model.JobStartDateTime.ToString("G"),
                Value = model.JobStartDateTime.ToString("G")
            });

            return model;
        }


        public PartialViewResult AdjustCandidateWorkTime(int id, int? candidateId, int? jobOrderId, DateTime? refDate, string formName)
        {
            var model = id > 0 ? InitializeWorkTimeModel(id) : 
                _InitializeWorkTimeModelFromPlacement(candidateId.Value, jobOrderId.Value, refDate.Value);
            if (!String.IsNullOrWhiteSpace(formName))
                ViewBag.FormName = formName;

            return PartialView("_AdjustCandidateWorkTime", model);
        }


        private CandidateWorkTimeModel _InitializeWorkTimeModelFromPlacement(int candidateId, int jobOrderId, DateTime refDate)
        {
            var candidate = _candidateService.GetCandidateById(candidateId);
            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            var model = new CandidateWorkTimeModel()
            {
                CandidateId = candidate.Id,
                EmployeeId = candidate.EmployeeId,
                EmployeeFirstName = candidate.FirstName,
                EmployeeLastName = candidate.LastName,
                JobOrderId = jobOrder.Id,
                JobTitle = jobOrder.JobTitle,
                JobStartDateTime = refDate.Date + jobOrder.StartTime.TimeOfDay,
                AvaliableCandidates = new List<SelectListItem>(),
                AvailableJobOrders = new List<SelectListItem>(),
                AvaliableStartDateTimes = new List<SelectListItem>()
            };

            model.AvaliableCandidates.Add(new SelectListItem()
            {
                Text = String.Concat(candidate.EmployeeId, " - ", candidate.GetFullName()),
                Value = candidateId.ToString()
            });

            model.AvailableJobOrders.Add(new SelectListItem()
            {
                Text = jobOrder.JobTitle,
                Value = jobOrder.Id.ToString()
            });

            model.AvaliableStartDateTimes.Add(new SelectListItem()
            {
                Text = model.JobStartDateTime.ToString("G"),
                Value = model.JobStartDateTime.ToString("G")
            });

            return model;
        }

        #endregion


        #region Approve Selected

        private void _ApproveMultipleEntries(int[] selectedIds, out int approved, out int failed)
        {
            approved = 0;
            failed = 0;
            string err;
            foreach (var id in selectedIds)
            {
                if (_workTimeService.ApproveWorkTimeEntry(id, _workContext.CurrentAccount, out err))
                    approved++;
                else
                    failed++;
            }
        }


        [HttpGet]
        public JsonResult DailyApprovalWorkTime(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return Json(new { Succeed = false, Approved = 0, Failed = 0, Error = "Access Denied!" }, JsonRequestBehavior.AllowGet);

            int approved = 0, failed = 0;
            string err = String.Empty;

            if (!string.IsNullOrEmpty(selectedIds))
            {
                var ids = selectedIds.Split(',').Select(x => int.Parse(x)).ToArray();
                this._ApproveMultipleEntries(ids, out approved, out  failed);
                if (failed > 0 || approved == 0)
                    err = _localizationService.GetResource("Common.UnexpectedError");
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(err), Approved = approved, Failed = failed, Error = err }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Approve All

        [HttpGet]
        public JsonResult DailyApproveAllWorkTime(DateTime weekStartDate)
        {
            int approved = 0, failed = 0;
            string errorMessage = string.Empty;

            var selectedIds = _workTimeService.GetOpenCandidateWorkTimeByAccountForApproval(weekStartDate, account: _workContext.CurrentAccount, submittedOnly: true)
                .Select(x => x.Id).ToArray();

            if (selectedIds.Length > 0)
            {
                this._ApproveMultipleEntries(selectedIds, out approved, out  failed);
                if (failed > 0 || approved == 0)
                    errorMessage = _localizationService.GetResource("Common.UnexpectedError");
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(errorMessage), Approved = approved, Failed = failed, Error = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region SaveManualWorkTime

        [HttpPost]
        public JsonResult SaveManualWorkTime([Bind(Exclude = "Id")]CandidateWorkTimeModel model)
        {
            StringBuilder errorMessage = new StringBuilder();
            bool result = true;

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var jobOrder = _jobOrderService.GetJobOrderById(model.JobOrderId);
                var shiftStartTime = model.JobStartDateTime.Date + jobOrder.StartTime.TimeOfDay;
                var shiftEndTime = model.JobStartDateTime.Date + jobOrder.EndTime.TimeOfDay;
                if (shiftEndTime.TimeOfDay < shiftStartTime.TimeOfDay)
                    shiftEndTime = shiftEndTime.AddDays(1);

                model.JobStartDateTime = shiftStartTime;
                model.JobEndDateTime = shiftEndTime;

                if (jobOrder.AllowSuperVisorModifyWorkTime)
                {
                    try
                    {
                        var _model = model.ToEntity();
                        _workTimeService.SaveManualCandidateWorkTime(_model, WorkTimeSource.Manual);
                    }
                    catch (WfmException ex)
                    {
                        errorMessage.AppendLine(ex.Message);
                        result = false;
                    }
                    catch (Exception ex2)
                    {
                        errorMessage.AppendLine(_localizationService.GetResource("Common.UnexpectedError"));
                        _logger.Error(ex2.Message, ex2, _workContext.CurrentAccount);
                        result = false;
                    }
                }
                else
                {
                    errorMessage.AppendLine(String.Format(_localizationService.GetResource("TimeSheet.Modification.NotAllowed.ByJobOrder"), model.JobOrderId));
                    result = false;
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
                result = false;
            }

            if (result)
                _activityLogService.InsertActivityLog("SaveManualWorkTime", String.Concat("Addd a Time sheet for Job Order ", model.JobOrderId, " on ", model.JobStartDateTime.Date));

            return Json(new { Succeed = result, Error = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveAdjustedWorkTime(CandidateWorkTimeModel model)
        {
            StringBuilder errorMessage = new StringBuilder();
            bool result = true;

            if (ModelState.IsValid)
            {
                if (_jobOrderService.GetJobOrderById(model.JobOrderId).AllowSuperVisorModifyWorkTime)
                {
                    try
                    {
                        if (model.Id == 0)
                            return SaveManualWorkTime(model);

                        var _model = model.ToEntity();
                        CandidateWorkTime cwt = _workTimeService.GetWorkTimeById(model.Id);
                        bool _success = _workTimeService.ManualAdjustCandidateWorkTime(cwt.CandidateId, cwt.JobOrderId, cwt.JobStartDateTime, _model.JobOrderId,
                                                                                       _model.AdjustmentInMinutes, model.NetWorkTimeInHours, _model.Note);

                        if (_success)
                            _workflowMessageService.SendWorkTimeAdjustmentRecruiterNotification(cwt, 1, _workContext.CurrentAccount);
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
                    }
                    
                }
                else
                {
                    errorMessage.AppendLine(String.Format(_localizationService.GetResource("TimeSheet.Modification.NotAllowed.ByJobOrder"), model.JobOrderId));
                    result = false;
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
                result = false;
            }


            return Json(new { Succeed = result, Error = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Export / Import

        #region Print Work Times to PDF

        [HttpPost]
        public ActionResult PrintClientWorkTimesToPdf(DateTime weekStartDate, bool submittedOnly = false)
        {
            weekStartDate = weekStartDate.StartOfWeek(DayOfWeek.Sunday);    // forced to by week start
            var ids = _workTimeService.GetOpenCandidateWorkTimeByAccountForApprovalByWeekStartDate(weekStartDate, account: _workContext.CurrentAccount,
                submittedOnly: submittedOnly).Select(x => x.Id).ToArray();
            return PrintWorkTimesToPdf(string.Join(",", ids));
        }


        //[HttpPost]
        private ActionResult PrintWorkTimesToPdf(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
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
                ErrorNotification(_localizationService.GetResource("Common.SelectOneOrMoreItems"));
                return Redirect(urlString);
            }

            var currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            // Export to PDF
            try
            {
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintCandidateWorkTimesToPdf(stream, workTimes);
                    bytes = stream.ToArray();
                }

                //activity log
                _activityLogService.InsertActivityLog("ExportWorkTime", _localizationService.GetResource("ActivityLog.ExportWorkTime"), String.Concat("Pdf", "/", selectedIds));

                return File(bytes, "application/pdf", "CandidateWorkTimes.pdf");
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

        #endregion


        #region Export Work Times to Xlsx

        [HttpPost]
        public ActionResult ExportClientWorkTimesToExcel(DateTime weekStartDate, bool submittedOnly = false)
        {
            weekStartDate = weekStartDate.StartOfWeek(DayOfWeek.Sunday);    // forced to by week start
            var ids = _workTimeService.GetOpenCandidateWorkTimeByAccountForApprovalByWeekStartDate(weekStartDate, account: _workContext.CurrentAccount,
                submittedOnly: submittedOnly).Select(x => x.Id).ToArray();
            return ExportWorkTimesToXlsx(string.Join(",", ids), true);
        }


        [HttpPost]
        public ActionResult ExportWorkTimesToXlsx(string selectedIds, bool all = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyTimeSheets))
                return AccessDeniedView();

            var urlString = Request.UrlReferrer.ToString();

            var workTimes = new List<CandidateWorkTime>();
            if (selectedIds != null)
            {
                var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x)).ToArray();
                workTimes.AddRange(_workTimeService.GetWorkTimeByIds(ids));
            }

            // Validate selected items
            if (!workTimes.Any())
            {
                ErrorNotification(!all ? _localizationService.GetResource("Common.SelectOneOrMoreItems") : "No time sheets to export.");
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
                    _exportManager.ExportCandidateWorkTimeToXlsx(stream, workTimes);
                    bytes = stream.ToArray();
                }

                //activity log
                _activityLogService.InsertActivityLog("ExportWorkTime", _localizationService.GetResource("ActivityLog.ExportWorkTime"), String.Concat("Xlsx", "/", selectedIds));

                // return File(bytes, "text/xls", fileName);
                return File(bytes, "text/xls", "CandidateWorkTimes.xlsx");
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

        #endregion

        #endregion


        #endregion


        #region DailyAttendanceList

        public ActionResult DailyAttendanceList() 
        {
            var from = DateTime.Today;
            var to = from;

            return View(_searchBL.GetSearchAttendanceModel(from, to));
        }


        [HttpPost]
        public ActionResult DailyAttendanceList([DataSourceRequest] DataSourceRequest request, SearchAttendanceModel model)
        {
            var candidateAttendanceList = _candidateJobOrderService.GetDailyAttendanceList(model.sf_From, model.sf_ClientTime, _workContext.CurrentAccount);

            if (model.sf_ShiftStartTime.HasValue)   // add date for time only filter
                model.sf_ShiftStartTime = model.sf_From.Date + model.sf_ShiftStartTime.Value.TimeOfDay;

            KendoHelper.CustomizePlacementBasedFilters(request, model, 
                skip: new List<string>() { "ClientTime" },
                nameCols: new List<string>() { "FranchiseId", "CompanyLocationId", "CompanyDepartmentId" });

            return Json(candidateAttendanceList.ToDataSourceResult(request, x => x.ToDailyTimeSheetModel()));
        }


        public ActionResult _RejectPlacement(int id, DateTime RefDate, string formName)
        {
            ViewBag.RefDate = RefDate.ToShortDateString();
            if (!String.IsNullOrWhiteSpace(formName))
                ViewBag.FormName = formName;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _RejectPlacement(int id, DateTime refDate, string reason, string comment)
        {
            var msg = String.Empty;
            if (String.IsNullOrWhiteSpace(reason))
            {
                msg = _localizationService.GetResource("Admin.Candidate.AddToBlacklist.ReasonMissing");
            }
            else if (String.IsNullOrWhiteSpace(comment))
            {
                msg = _localizationService.GetResource("Common.RequiredComment");
            }
            else
            {
                try
                {
                    var cjo = _candidateJobOrderService.GetCandidateJobOrderById(id);
                    int candidateId = cjo.CandidateId;
                    int jobOrderId = cjo.JobOrderId;
                    _candidateJobOrderService.DeleteCandidateJobOrder(cjo);

                    _activityLogService.InsertActivityLog("RejectPlacement", _localizationService.GetResource("ActivityLog.RejectPlacement"),
                                                              candidateId, jobOrderId, cjo.StartDate.ToString("yyyy-MM-dd"), reason, comment);
                    // navigation properties () were cleared by the deletion above !!!
                    cjo.Candidate = _candidateService.GetCandidateById(cjo.CandidateId);
                    cjo.JobOrder = _jobOrderService.GetJobOrderById(cjo.JobOrderId);
                    _workflowMessageService.SendNotificationToRecruiterForPlacmentRejection(cjo, reason, comment, _workContext.CurrentAccount, 1, refDate);
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

            return Json(new { Succeed = String.IsNullOrEmpty(msg), Error = msg });
        }


        [HttpPost]
        public ActionResult PrintDailyAttendanceListToPdf(string selectedIds, DateTime hdnDate, DateTime? clientTime, bool hideTotalHours)
        {
            var dailyAttendanceList = new List<DailyAttendanceList>();
            if (selectedIds != null)
            {
                var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x)).ToArray();
                dailyAttendanceList.AddRange(_candidateJobOrderService.GetDailyAttendanceList(hdnDate, clientTime, _workContext.CurrentAccount).Where(x => ids.Contains(x.CandidateJobOrderId)));
            }

            // Validate selected items
            if (!dailyAttendanceList.Any())
            {
                ErrorNotification(_localizationService.GetResource("Common.SelectOneOrMoreItems"));
                return RedirectToAction("DailyAttendanceList");
            }

            // Print to Pdf
            try
            {
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintDailyAttendanceListToPdfForClient(stream, dailyAttendanceList, hdnDate, hideTotalHours);
                    bytes = stream.ToArray();
                }
                //activity log
                _activityLogService.InsertActivityLog("DailyAttendanceList", _localizationService.GetResource("ActivityLog.ExportDailyAttendanceList"), "Pdf" + "/" + selectedIds);
                return File(bytes, "application/pdf", "DailyAttendanceList.pdf");
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

            return RedirectToAction("DailyAttendanceList");
        }


        [HttpPost]
        public ActionResult ExportDailyAttendanceListToXlsx(string selectedIds, DateTime hdnDate, DateTime? clientTime)
        {

            var dailyAttendanceList = new List<DailyAttendanceList>();
            if (selectedIds != null)
            {
                var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x)).ToArray();
                dailyAttendanceList.AddRange(_candidateJobOrderService.GetDailyAttendanceList(hdnDate, clientTime, _workContext.CurrentAccount).Where(x => ids.Contains(x.CandidateJobOrderId)));
            }
            // Validate selected items
            if (!dailyAttendanceList.Any())
            {
                ErrorNotification(_localizationService.GetResource("Common.SelectOneOrMoreItems"));
                return RedirectToAction("DailyAttendanceList");
            }
            // Export to Xlsx
            try
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportDailyAttendanceListForClient(stream, dailyAttendanceList,hdnDate);
                    bytes = stream.ToArray();
                }

                //activity log
                _activityLogService.InsertActivityLog("DailyAttendanceList", _localizationService.GetResource("ActivityLog.ExportDailyAttendanceList"), "Pdf" + "/" + selectedIds);

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


            return RedirectToAction("DailyAttendanceList");
        }

        #endregion
    }
}



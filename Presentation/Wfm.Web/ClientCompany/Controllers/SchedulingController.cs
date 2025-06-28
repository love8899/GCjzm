using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Scheduling;
using Wfm.Data;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.Employees;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Scheduling;
using Wfm.Services.Security;
using Wfm.Shared.Mapping;
using Wfm.Shared.Models.Scheduling;
using Wfm.Web.Framework.Feature;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Scheduling")]
    public class SchedulingController : BaseClientController
    {
        #region Fields
        private readonly IRepository<ShiftSchedule> _shiftScheduleRepository;
        private readonly IRepository<ShiftScheduleDailyDemandAdjustment> _shiftScheduleDailyDemandAdjustmentRepository;
        private readonly IRepository<EmployeeSchedule> _employeeScheduleRepository;
        private readonly IRepository<CompanyShiftJobRole> _companyShiftJobRoleRepository;
        private readonly IRepository<ScheduleJobOrder> _scheduleJobOrderRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly ISchedulingDemandService _schedulingDemandService;
        private readonly IAutoScheduleService _autoScheduleService;
        private readonly ICompanyService _companyService;
        private readonly ITimeoffService _timeoffService;
        private readonly IEmployeeService _employeeService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;

        private readonly WeeklyDemand_BL _weeklyDemand_BL;
        private readonly EmployeePlacement_BL _employeePlacement_BL;
        #endregion

        #region ctor
        public SchedulingController(
            IRepository<ShiftSchedule> shiftScheduleRepository,
            IRepository<ShiftScheduleDailyDemandAdjustment> shiftScheduleDailyDemandAdjustmentRepository,
            IRepository<EmployeeSchedule> employeeScheduleRepository,
            IRepository<CompanyShiftJobRole> companyShiftJobRoleRepository,
            IRepository<ScheduleJobOrder> scheduleJobOrderRepository,
            IRepository<JobOrder> jobOrderRepository,
            ISchedulingDemandService schedulingDemandService,
            IAutoScheduleService autoScheduleService,
            ICompanyService companyService,
            ITimeoffService timeoffService,
            IEmployeeService employeeService,
            ICandidateService candidateService,
            ICandidateJobOrderService candidateJobOrderService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IActivityLogService activityLogService,
            ILogger logger,
            IDbContext dbContext)
        {
            _shiftScheduleRepository = shiftScheduleRepository;
            _shiftScheduleDailyDemandAdjustmentRepository = shiftScheduleDailyDemandAdjustmentRepository;
            _employeeScheduleRepository = employeeScheduleRepository;
            _companyShiftJobRoleRepository = companyShiftJobRoleRepository;
            _scheduleJobOrderRepository = scheduleJobOrderRepository;
            _jobOrderRepository = jobOrderRepository;
            _schedulingDemandService = schedulingDemandService;
            _autoScheduleService = autoScheduleService;
            _companyService = companyService;
            _timeoffService = timeoffService;
            _employeeService = employeeService;
            _candidateService = candidateService;
            _candidateJobOrderService = candidateJobOrderService;
            _workContext = workContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _logger = logger;
            _dbContext = dbContext;

            _weeklyDemand_BL = new WeeklyDemand_BL(_companyService, _shiftScheduleRepository, _shiftScheduleDailyDemandAdjustmentRepository,
                                                   _companyShiftJobRoleRepository, scheduleJobOrderRepository, _jobOrderRepository, _schedulingDemandService);
            _employeePlacement_BL = new EmployeePlacement_BL(_companyService, _employeeService, _shiftScheduleRepository, _shiftScheduleDailyDemandAdjustmentRepository,
                                                             _employeeScheduleRepository, _companyShiftJobRoleRepository, _schedulingDemandService);
        }
        #endregion

        #region Methods
        public ActionResult ShiftAndRoles()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            return View();
        }
        public ActionResult SchedulePeriod()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult ListSchedulePeriod([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();

            var entities = _schedulingDemandService.GetAllSchedulePeriodsByAccount(_workContext.CurrentAccount);
            var model = new List<SchedulePeriodModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }

        [HttpGet]
        public ActionResult _NewSchedulePeriod()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            int companyId = _workContext.CurrentAccount.CompanyId;
            var scheduledTo = _schedulingDemandService.GetAllSchedulePeriods(_workContext.CurrentAccount.CompanyId)
                    .FirstOrDefault();

            var model = new SchedulePeriodModel()
            {
                CompanyId = companyId,
                PeriodStartDate = scheduledTo == null ? DateTime.Today : scheduledTo.PeriodEndUtc.Date.AddDays(1),
            };
            _activityLogService.InsertActivityLog("Client.NewSchedulePeriod", "Schedule CompanyId: {0}, PeriodStart: {1}, PeriodEnd: {2}", model.CompanyId, model.PeriodStartDate, model.PeriodEndDate);
            return PartialView("_CreateEditSchedulePeriod", model);
        }
        [HttpGet]
        public ActionResult _EditSchedulePeriod(int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var entity = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);
            var model = entity.ToModel();
            return PartialView("_CreateEditSchedulePeriod", model);
        }
        [HttpPost]
        public ActionResult _EditSchedulePeriod(SchedulePeriodModel model)
        {
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    if (model.Id == 0)
                    {
                        _schedulingDemandService.InsertCompanySchedulePeriod(entity);
                    }
                    else
                    {
                        _schedulingDemandService.UpdateCompanySchedulePeriod(entity);
                    }
                    _activityLogService.InsertActivityLog("Client.EndSchedulePeriod", "Schedule CompanyId: {0}, PeriodStart: {1}, PeriodEnd: {2}", model.CompanyId, model.PeriodStartDate, model.PeriodEndDate);
                    return Content("done");
                }
                catch (Exception ex)
                {
                    errorMessage = ex.ToString();
                    _logger.Error("_EditSchedulePeriod()", ex, userAgent: Request.UserAgent);

                    return Content(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_EditSchedulePeriod():" + errorMessage, userAgent: Request.UserAgent);
                return PartialView("_CreateEditSchedulePeriod", model);
            }
        }
        [HttpPost]
        public ActionResult _RemoveSchedulePeriod(int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            if (schedulePeriodId > 0)
            {
                _schedulingDemandService.DeleteCompanySchedulePeriod(schedulePeriodId);
                _activityLogService.InsertActivityLog("Client.RemoveSchedulePeriod", "Schedule Period Id: {0}", schedulePeriodId);
            }
            return Content("done");
        }
        [HttpGet]
        public ActionResult _EditingScheduleShifts(int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            ViewBag.SchedulePeriodId = schedulePeriodId;
            ViewBag.CompanyShifts = _schedulingDemandService.GetShiftsOfSchedulePeriod(schedulePeriodId)
                .Select(x => new CompanyShiftDropdownModel
                {
                    Id = x.CompanyShift.Id,
                    Name = string.Format("{0} - ({1}/{2})",
                        x.CompanyShift.Shift.ShiftName,
                        x.CompanyShift.CompanyShiftJobRoles.Sum(y => y.MandantoryRequiredCount),
                        x.CompanyShift.CompanyShiftJobRoles.Sum(y => y.ContingencyRequiredCount)),
                });
            return PartialView("_EditScheduleShifts", new ShiftScheduleModel[] { });
        }
        [HttpPost]
        public ActionResult _EditingScheduleShifts([DataSourceRequest] DataSourceRequest request, int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var entities = _schedulingDemandService.GetShiftsOfSchedulePeriod(schedulePeriodId);
            var model = new List<ShiftScheduleModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }
        [HttpPost]
        public ActionResult _EditingScheduleShiftsUpdate([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<ShiftScheduleModel> scheduleShifts, int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            if (scheduleShifts != null && ModelState.IsValid)
            {
                _schedulingDemandService.UpdateShiftsOfSchedulePeriod(schedulePeriodId, scheduleShifts.Select(x => x.ToEntity()));
                _activityLogService.InsertActivityLog("Client.EditingScheduleShiftsUpdate", "Schedule Peroid Id: {0}", schedulePeriodId);
            }
            return Json(scheduleShifts.ToDataSourceResult(request, ModelState));
        }
        public ActionResult _CopySchedulePeriod(int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var entity = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);
            var model = entity.ToModel();
            var duration = model.PeriodEndDate.GetValueOrDefault() - model.PeriodStartDate;
            model.PeriodStartDate = model.PeriodEndDate.GetValueOrDefault().AddDays(1);
            model.PeriodEndDate = model.PeriodStartDate.Add(duration);
            return PartialView("_CopySchedulePeriod", model);
        }
        [HttpPost]
        public ActionResult _CopySchedulePeriod(SchedulePeriodModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                _schedulingDemandService.CopySchedulePeriodAndShifts(model.Id, model.PeriodStartDate, model.PeriodEndDate.GetValueOrDefault());
                _activityLogService.InsertActivityLog("Client.CopySchedulePeriod", "Schedule Peroid Id: {0}, to Period {1} ~ {2}", model.Id, model.PeriodStartDate, model.PeriodEndDate);
                return Content("done");
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                var errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_CopySchedulePeriod():" + errorMessage, userAgent: Request.UserAgent);
                return PartialView("_CopySchedulePeriod", model);
            }
        }
        public ActionResult _showDailyShiftSchedule(int schedulePeriodId)
        {
            var schedulePeriod = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);
            if (schedulePeriod == null)
            {
                ErrorNotification("The schedule period does not exist!");
                return new EmptyResult();
            }
            ViewBag.SchedulePeriodId = schedulePeriodId;
            ViewBag.MaxDate = schedulePeriod.PeriodEndUtc.Date;
            ViewBag.MinDate = schedulePeriod.PeriodStartUtc.Date;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _dailyShiftSchedule(DataSourceRequest request, int schedulePeriodId)
        {
            DailyShiftScheduleModel_BL bl = new DailyShiftScheduleModel_BL(_schedulingDemandService, _companyShiftJobRoleRepository, _shiftScheduleDailyDemandAdjustmentRepository);
            var result = bl.GetDailyShiftScheduleModel(request, schedulePeriodId);
            return Json(result);
        }

        [HttpPost]
        public ActionResult SaveDailyShiftSchedule(DailyShiftScheduleModel model)
        {
            bool result = true;
            string msg = null;

            if (ModelState.IsValid)
            {
                try
                {
                    var businessLogic = new DailyShiftScheduleModel_BL(_schedulingDemandService, _companyShiftJobRoleRepository, _shiftScheduleDailyDemandAdjustmentRepository);
                    businessLogic.CreateOrUpdateDailyShiftSchedule(model, out msg);
                    if (!String.IsNullOrEmpty(msg)) result = false;
                }
                catch (Exception ex)
                {
                    result = false;
                    _logger.Error("SaveDailyShiftSchedule():", ex);
                    //msg += ex.Message;
                    msg = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;
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
        public ActionResult RemoveDailyShiftSchedule(DailyShiftScheduleModel model)
        {
            bool result = true;
            string msg = null;

            try
            {
                var entity = model.ToEntity();
                _schedulingDemandService.DeleteDailyShiftSchedule(entity);
                if (!String.IsNullOrEmpty(msg)) result = false;
            }
            catch (Exception ex)
            {
                _logger.Error("RemoveDailyShiftSchedule():", ex);
                msg = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;
                return Json(new { Succeed = result, Error = msg });
            }

            return Json(new { Succeed = result, Error = msg });
        }
        #endregion

        #region Employee View
        //
        public ActionResult EmployeeView()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            return View();
        }
        [HttpPost]
        public ActionResult GetEmployeeSchedules([DataSourceRequest] DataSourceRequest request, int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var model = _schedulingDemandService.GetEmployeeScheduleOfPeriod(schedulePeriodId).Select(e => e.ToModel()).ToList();
            return Json(model.ToDataSourceResult(request));
        }

        public JsonResult GetCascadeJobRoles(string companyShiftId, int schedulePeriodId)
        {
            var _companyShiftid = String.IsNullOrEmpty(companyShiftId) ? 0 : Convert.ToInt32(companyShiftId);

            var roleDropDownList = _companyService.GetCompanyShiftById(_companyShiftid).CompanyShiftJobRoles
                .Select(r => new SelectListItem
                {
                    Text = string.Format("{0} - {1} not filled in", r.CompanyJobRole.Name,
                            _schedulingDemandService.GetShiftVacancyInPeriod(r.CompanyShiftId, schedulePeriodId, r.CompanyJobRoleId, null)),
                    Value = r.CompanyJobRoleId.ToString(),
                }).ToList();
            return Json(roleDropDownList, JsonRequestBehavior.AllowGet);
        }

        private void PrepareColorBag(int schedulePeriodId = 0)
        {
            ViewBag.CompanyJobRoleList = _companyService.GetAllJobRolesSelectList(_workContext.CurrentAccount.CompanyId);
            ViewBag.CompanyShiftList = _companyService.GetGetAllShiftsSelectList(_workContext.CurrentAccount.CompanyId);
        }

        [HttpGet]
        public ActionResult _ScheduleEmployee(Guid employeeGuid, int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var employeeId = _candidateService.GetCandidateByGuidForClient(employeeGuid).Id;
            var model = _schedulingDemandService.GetEmployeeScheduleOfPeriod(schedulePeriodId)
                .Where(x => x.EmployeeId == employeeId).First().ToModel();
            PrepareColorBag(schedulePeriodId);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ReadEmployeeSchedulePreview([DataSourceRequest] DataSourceRequest request,
            int schedulePeriodId, Guid employeeId, int companyShiftId, int jobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var _model = _schedulingDemandService.PreviewEmployeeScheduleDetail(schedulePeriodId, candidateId, companyShiftId, jobRoleId)
                .ToArray();
            var model = _model.Select(x => new EmployeeSchedulePreviewModel(candidateId, x));
            return Json(model.ToDataSourceResult(request));
        }
        private JsonResult ReturnErrorWarningsJson(IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings)
        {
            var retObject = new
            {
                Success = !errorsAndWarnings.Any(x => x.LevelOfError == ScheduleWarningLevel.Error),
                ErrorAndWarnings = errorsAndWarnings,
                Popup = errorsAndWarnings.Any() ? RenderPartialViewToString("_ScheduleErrorWarnings", errorsAndWarnings) : null,
            };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult _EditScheduleEmployee(EmployeeScheduleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var entity = model.ToEntity();
            IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings = null;
            _schedulingDemandService.SaveEmployeeSchedule(entity, out errorsAndWarnings);
            //
            return ReturnErrorWarningsJson(errorsAndWarnings);
        }
        //
        #endregion

        #region Shift View
        public ActionResult ShiftView()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            PrepareColorBag();
            return View();
        }

        [HttpPost]
        public ActionResult _ReadShiftSchedulePreview([DataSourceRequest] DataSourceRequest request, int schedulePeriodId, int companyShiftId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var _model = _schedulingDemandService.GetDailyScheduleForShiftView(schedulePeriodId);
            if (companyShiftId > 0)
                _model = _model.Where(x => x.Shift.CompanyShiftId == companyShiftId);
            var model = _model.ToArray().SelectMany(x => EmployeeSchedulePreviewModel.FromShift(x).OrderBy(y => y.Title)).ToArray();
            var overridings = _schedulingDemandService.GetEmployeeScheduleDaily(schedulePeriodId).Select(x => x.ToModel()).ToArray();
            var result = EmployeeSchedulePreviewModel.MergeWithEmployeeScheduleDaily(model, overridings).ToArray();
            return Json(result.ToDataSourceResult(request));
        }
        [HttpPost]
        public ActionResult _ValidateShiftSchedulePreview(EmployeeSchedulePreviewModel task)
        {
            var entity = task.ToOverride().ToEntity();
            var errorsAndWarnings = _schedulingDemandService.ValidateEmployeeScheduleDaily(entity, task.SchedulePeriodId).ToArray();
            return ReturnErrorWarningsJson(errorsAndWarnings);
        }
        private JsonResult ReturnTaskJson(EmployeeSchedulePreviewModel task, EmployeeScheduleDailyModel overriding)
        {
            var model = EmployeeSchedulePreviewModel.MergeWithEmployeeScheduleDaily(new[] { task }, new[] { overriding }).ToArray();
            var result = new DataSourceResult()
            {
                Data = model,
                Total = model.Count(),
            };
            return Json(result);
        }
        [HttpPost]
        public ActionResult _CreateShiftSchedulePreview([DataSourceRequest] DataSourceRequest request, EmployeeSchedulePreviewModel task, int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            task.ScheduelDate = task.Start.Date;
            task.EmployeeScheduleId = _schedulingDemandService.AddAdhocEmployeeSchedule(schedulePeriodId, task.EmployeeId, task.Start.Date);
            var entity = task.ToOverride().ToEntity();
            var _task = _schedulingDemandService.UpdateEmployeeScheduleDaily(entity).ToModel();
            return ReturnTaskJson(task, _task);
        }

        [HttpPost]
        public ActionResult _UpdateShiftSchedulePreview([DataSourceRequest] DataSourceRequest request, EmployeeSchedulePreviewModel task)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var entity = task.ToOverride().ToEntity();
            var _task = _schedulingDemandService.UpdateEmployeeScheduleDaily(entity).ToModel();
            return ReturnTaskJson(task, _task);
        }


        [HttpPost]
        public ActionResult _UpdateEmployeeBreakTimePosition(int employeeScheduleId, DateTime scheduleDate, int breakIndex, int breakTimePosition)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var title = _schedulingDemandService.UpdateEmployeeBreakTimePosition(employeeScheduleId, scheduleDate, breakIndex, breakTimePosition);
            return Content(title);
        }

        [HttpPost]
        public ActionResult _DeleteEmployeeScheduleDaily([DataSourceRequest] DataSourceRequest request, EmployeeSchedulePreviewModel task)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            _schedulingDemandService.DeleteEmployeeScheduleDaily(task.EmployeeScheduleId, task.ScheduelDate);
            return ReturnTaskJson(task, null);
        }

        [HttpGet]
        public ActionResult _ScheduleEmployeeFromCalendar(Guid employeeId, int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var model = _schedulingDemandService.GetEmployeeScheduleOfPeriod(schedulePeriodId)
                .Where(x => x.Id == candidateId).First().ToModel();
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult _GetCompanyJobRoleIdsBySchedulePeriod(int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            return Json(new { companyJobRoles = _schedulingDemandService.GetCompanyJobRoleIdsBySchedulePeriod(schedulePeriodId) },
                JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult _GetDefaultLengthInHours(int schedulePeriodId, int companyJobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            return Json(new { defaultLengthInHours = _schedulingDemandService.GetDefaultLengthInHours(schedulePeriodId, companyJobRoleId) }
                , JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Vacancy View

        public ActionResult VacancyView()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))

                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _GetVacancyViewPanels(int schedulePeriodId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var model = _schedulingDemandService.GetScheduleVacancyView(schedulePeriodId);

            //TODO: check other conditions, like if work time closed?
            ViewBag.Populateable = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId).PeriodEndUtc < DateTime.Today;

            return PartialView("_VacancyViewPanel", model);
        }

        [HttpPost]
        public ActionResult _DeleteEmployeeSchedule(Guid employeeId, int schedulePeriodId, int companyShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            _schedulingDemandService.DeleteEmployeeSchedule(candidateId, schedulePeriodId, companyShiftId);
            return Content("done");
        }

        [HttpPost]
        public ActionResult _ScheduleEmployeeFromVacancy(int schedulePeriodId, int companyShiftId, int jobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var model = new EmployeeScheduleModel
            {
                SchedulePeriodId = schedulePeriodId,
                CompanyShiftId = companyShiftId,
                JobRoleId = jobRoleId,
            };
            return PartialView(model);
        }
        #endregion

        #region Auto Schedule

        [HttpPost]
        public ActionResult _AutoFillVacancy(int schedulePeriodId, int companyShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            IEnumerable<IScheduleDetailErrorModel> errorsAndWarnings;
            _autoScheduleService.AutoFillupVacancy(schedulePeriodId, companyShiftId, false, false, out errorsAndWarnings);

            return ReturnErrorWarningsJson(errorsAndWarnings);
        }

        [HttpPost]
        public ActionResult _ResetEmployeeSchedule(int schedulePeriodId, int companyShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            _autoScheduleService.ResetEmployeeSchedule(schedulePeriodId, companyShiftId);
            return Content("done");
        }

        [HttpPost]
        public ActionResult _PublishEmployeeSchedule(int schedulePeriodId, int companyShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            _autoScheduleService.PublishEmployeeSchedule(schedulePeriodId, companyShiftId);
            return Content("done");
        }

        [HttpPost]
        public ActionResult _MoveEmployeeSchedule(int schedulePeriodId, int fromCompanyShiftId, int fromCompanyRoleId,
            Guid employeeId, int toCompanyShiftid, int toCompanyRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var errorsAndWarnings = _autoScheduleService.MoveEmployeeSchedule(schedulePeriodId, fromCompanyShiftId, fromCompanyRoleId,
                _candidateService.GetCandidateByGuidForClient(employeeId).Id, toCompanyShiftid, toCompanyRoleId);
            return ReturnErrorWarningsJson(errorsAndWarnings);
        }
        #endregion

        #region Populate Work Time

        public ActionResult _PopulateWorkTime(int schedulePeriodId)
        {
            var schedulePeriod = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);
            if (schedulePeriod != null)
            {
                ViewBag.PeriodStartDate = schedulePeriod.PeriodStartUtc;
                ViewBag.PeriodEndDate = schedulePeriod.PeriodEndUtc;
            }

            return PartialView("_PopulateWorkTime");
        }


        [HttpPost]
        public ActionResult _PopulateWorkTime(DateTime startDate, DateTime endDate)
        {
            _CleanupWorkTimeFromScheduling(startDate, endDate);
            _PopulateWorkTimePerScheduling(startDate, endDate);

            SuccessNotification(String.Format("Time sheets from [{0}] to [{1}] populated.", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")));
            return RedirectToAction("VacancyView");
        }


        private void _CleanupWorkTimeFromScheduling(DateTime startDate, DateTime endDate)
        {
            var paras = new SqlParameter[2];
            paras[0] = new SqlParameter("StartDate", startDate);
            paras[1] = new SqlParameter("EndDate", endDate);

            //TODO: exclude those already in PayrollBatch

            _dbContext.ExecuteSqlCommand("EXEC [dbo].[CleanupWorkTimeFromScheduling] @StartDate, @EndDate", false, null, paras);
        }


        private void _PopulateWorkTimePerScheduling(DateTime startDate, DateTime endDate)
        {
            var paras = new SqlParameter[2];
            paras[0] = new SqlParameter("StartDate", startDate);
            paras[1] = new SqlParameter("EndDate", endDate);

            _dbContext.ExecuteSqlCommand("EXEC [dbo].[PopulateWorkTimeFromScheduling] @StartDate, @EndDate", false, null, paras);
        }

        #endregion

        #region Weekly demand

        public ActionResult WeeklyDemand()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult _GetWeeklyDemandPane()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;

            return PartialView("_WeeklyDemandPane");
        }


        [HttpPost]
        public ActionResult _WeeklyDemand(DataSourceRequest request, DateTime refDate, int locationId = 0, int[] deptIds = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var demands = new List<WeeklyDemandModel>();

            if (locationId > 0)
            {
                demands = _weeklyDemand_BL.GetWeeklyDemandBySchedulePperiod(locationId, refDate);

                if (deptIds != null && deptIds.Length > 0)
                    demands = demands.Where(x => deptIds.Contains(x.CompanyDepartmentId)).ToList();

                var account = _workContext.CurrentAccount;
                if (account.IsCompanyDepartmentManager())
                    demands = demands.Where(x => x.CompanyDepartmentId == account.CompanyDepartmentId).ToList();
                else if (account.IsCompanyDepartmentSupervisor())
                    demands = demands.Where(x => x.SupervisorId == account.Id).ToList();
            }

            return Json(demands.ToDataSourceResult(request));
        }


        public ActionResult _GetSchedulePeriodByLocationAndDate(int locationId, DateTime refDate)
        {
            var period = _schedulingDemandService.GetSchedulePeriodByLocationAndDate(locationId, refDate);

            return Json(new { currentId = period != null ? period.Id : 0 }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult _EditWeeklyDemand([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<WeeklyDemandModel> models, DateTime refDate)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _weeklyDemand_BL.UpdateWeeklyDemand(model, refDate);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Employee Placement

        public ActionResult EmployeePlacement(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var model = _employeePlacement_BL.GetShiftJobRoleModel(id);

            return View(model);
        }

        public ActionResult EmployeePlacementFromPlanner(int schedulePeriodId, int companyShiftId, int jobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var model = _employeePlacement_BL.GetShiftJobRoleModel(schedulePeriodId, companyShiftId, jobRoleId);

            return View("EmployeePlacement", model);
        }


        [HttpPost]
        public ActionResult _GetPlacedEmployees([DataSourceRequest] DataSourceRequest request, int companyShiftId)
        {
            var employees = _employeePlacement_BL.GetShiftEmployeesAsSelectList(companyShiftId);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _AddEmployee(int id, int schedulePeriodId, int jobRoleId, int companyShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            ViewBag.SchedulePeriodId = schedulePeriodId;
            ViewBag.CompanyJobRoleId = jobRoleId;
            ViewBag.CompanyShiftId = companyShiftId;

            return PartialView("_AddEmployeeForPlacement");
        }


        [HttpPost]
        public ActionResult _PlaceEmployee(int jobRoleId, int companyShiftId, int employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var errorsAndWarnings = _employeePlacement_BL.PlaceEmployee(jobRoleId, companyShiftId, employeeId);
            return ReturnErrorWarningsJson(errorsAndWarnings);
        }

        public ActionResult _GetAvailableEmployees(int? schedulePeriodId, int? companyShiftId, int? companyJobRoleId)
        {
            var employees = _schedulingDemandService.GetAvailableEmployees(schedulePeriodId, companyShiftId, companyJobRoleId);
            var result = employees.Select(x => new SelectListItem()
            {
                Text = string.Format("{0} {1} ({2} - {3})", x.FirstName, x.LastName, x.EmployeeType, x.EmployeeId),
                Value = x.Id.ToString()
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Employee Schedule
        [HttpPost]
        public ActionResult _ReadEmployeeSchedule([DataSourceRequest] DataSourceRequest request, Guid employeeGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            var _model = _schedulingDemandService.GetEmployeeScheduleBaseline(employeeGuid);
            var model = _model.ToArray().SelectMany(x => EmployeeSchedulePreviewModel.FromShift(x).OrderBy(y => y.Title)).ToArray();
            var overridings = _schedulingDemandService.GetEmployeeScheduleOverride(employeeGuid).Select(x => x.ToModel()).ToArray();

            var timeoffBookings = _timeoffService.GetEmployeeTimeoffBookings(employeeGuid);
            var result = EmployeeSchedulePreviewModel.MergeWithEmployeeScheduleDaily(model, overridings)
                .Union(EmployeeSchedulePreviewModel.FromEmployeeTimeoff(timeoffBookings))
                .ToArray();

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Job Order
        [HttpPost]
        public ActionResult _ScheduleJobOrder(int jobOrderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();
            var model = _autoScheduleService.GetScheduleJobOrderByJobOrderId(jobOrderId).ToModel();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _EditScheduleJobOrder(ScheduleJobOrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();

            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    _autoScheduleService.UpdateScheduleJobOrder(model.Id, model.SupervisorId, model.StartDate, model.EndDate, model.JobTitle);
                    _activityLogService.InsertActivityLog("Client.SaveScheduleJobOrder", "Schedule Job Order Id: {0}, Start: {1}, End: {2}, Title {3}", model.Id, model.StartDate, model.EndDate, model.JobTitle);
                    return Content("done");
                }
                catch (Exception ex)
                {
                    errorMessage = ex.ToString();
                    _logger.Error("_EditScheduleJobOrder()", ex, userAgent: Request.UserAgent);
                    return Content(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_EditScheduleJobOrder():" + errorMessage, userAgent: Request.UserAgent);
                return PartialView("_ScheduleJobOrder", model);
            }
        }
        [HttpPost]
        public ActionResult _JobOrderPipeline([DataSourceRequest] DataSourceRequest request, int jobOrderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling))
                return AccessDeniedView();

            var entities = _candidateJobOrderService.GetCandidateJobOrderByJobOrderId(jobOrderId);
            var model = entities.Select(x => x.ToSimpleModel());
            return Json(model.ToDataSourceResult(request));
        }
        #endregion

        #region Team Schedule
        public ActionResult TeamSchedule()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            ViewBag.EmployeeResources = _autoScheduleService.GetEmployeeResourceList();
            return View();
        }
        [HttpGet]
        public ActionResult _GetEmployeesForSchedule()
        {
            var result = _schedulingDemandService.GetEmployeeListForScheduleFilter().Distinct().ToList();
            var employeesDropDownList = new List<SelectListItem>();
            foreach (var e in result)
            {
                var item = new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", e.FirstName, e.LastName, e.EmployeeId),
                    Value = e.CandidateGuid.ToString()
                };
                employeesDropDownList.Add(item);
            }
            return Json(employeesDropDownList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult _ReadTeamSchedule([DataSourceRequest] DataSourceRequest request, Guid[] employeeGuids)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();

            if (employeeGuids == null) employeeGuids = new Guid[] { };

            var _model = _schedulingDemandService.GetTeamScheduleBaseline(employeeGuids);
            var model = _model.ToArray().SelectMany(x => EmployeeSchedulePreviewModel.FromShift(x).OrderBy(y => y.Title)).ToArray();
            var overridings = _schedulingDemandService.GetTeamScheduleOverride(employeeGuids).Select(x => x.ToModel()).ToArray();

            var timeoffBookings = _timeoffService.GetTeamTimeoffBookings(employeeGuids);
            var result = EmployeeSchedulePreviewModel.MergeWithEmployeeScheduleDaily(model, overridings)
                .Union(EmployeeSchedulePreviewModel.FromEmployeeTimeoff(timeoffBookings))
                .ToList();
            result.ForEach(x => x.Start = x.Start.AddMilliseconds(x.EmployeeId)); // trick to keep the order in same time by the employee id
            return Json(result.ToDataSourceResult(request));
        }
        #endregion
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.ClockTime;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Logging;
using Wfm.Services.Companies;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.ClockTime;
using Wfm.Services.TimeSheet;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.UI;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Candidates;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.TimeSheet;
using Kendo.Mvc.Extensions;

namespace Wfm.Admin.Controllers
{
    public class ClockTimeController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IAccountService _accountService;
        private readonly IClockTimeService _clockTimeService;
        private readonly IWorkTimeService _workTimeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly TimeClockSettings _timeClockSettings;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateService _candidateService ;
        private readonly ILogger _logger;
        private readonly ISmartCardService _smartCardService;
        #endregion

        #region Ctor

        public ClockTimeController(IActivityLogService activityLogService,
            IAccountService accountService,
            IClockTimeService clockTimeService,
            IWorkTimeService workTimeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            TimeClockSettings timeClockSettings,
            IRecruiterCompanyService recruiterCompanyService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateService candidateService,
            ILogger logger,
            ISmartCardService smartCardService
            )
        {
            _activityLogService = activityLogService;
            _accountService = accountService;
            _clockTimeService = clockTimeService;
            _workTimeService = workTimeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _workContext = workContext;
            _timeClockSettings = timeClockSettings;
            _recruiterCompanyService = recruiterCompanyService;
            _candidateJobOrderService = candidateJobOrderService;
            _candidateService = candidateService;
            _logger = logger;
            _smartCardService = smartCardService;
        }

        #endregion

        #region GET :/ClockTime/Index

        public ActionResult Index(int? candidateId, DateTime? refDate)
         {
             if (!_permissionService.Authorize(StandardPermissionProvider.ManageClockTimes))
                 return AccessDeniedView();

             ViewBag.CandidateId = candidateId;
             ViewBag.RefDate = refDate;

             return View();
         }

        #endregion

        #region POST:/ClockTime/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string status, int? candidateId, DateTime? refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClockTimes))
                return AccessDeniedView();

            if (refDate.HasValue)
                startDate = endDate = refDate.Value.ToShortDateString();

            CandidateClockTimeModel_BL model_BL = new CandidateClockTimeModel_BL();
            var result = model_BL.GetAllCandidateClockTime(request, _clockTimeService, startDate, endDate, status, candidateId);

            return Json(result);
        }

        #endregion


        #region POST :/ClockTime/Refresh
        [HttpPost]
        public ActionResult Refresh(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClockTimes))
                return AccessDeniedView();

            var clockTime = _clockTimeService.GetClockTimeById(id);
            if (clockTime == null)
                return new JsonResult() { Data = new { success = false, message = _localizationService.GetResource("Common.UnexpectedError") } };

            var error = _clockTimeService.AdvancedUpdate(clockTime);
            return new JsonResult() { Data = new { success = String.IsNullOrWhiteSpace(error), message = error } };
        }
        #endregion


        #region Mark As processed

        [HttpPost]
        public ActionResult MarkAsProcessed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClockTimes))
                return AccessDeniedView();

            var cwt = _workTimeService.GetWorkTimeById(id);
            if (cwt == null)
                return new JsonResult() { Data = new { success = false, message = _localizationService.GetResource("Common.UnexpectedError") } };

            // update clocktime status
            _workTimeService.SetClockTimeStatusByWorkTime(cwt);

            return new JsonResult() { Data = new { success = true, message = string.Empty } };
        }
        
        #endregion


        #region GET :/ClockTime/Load
        [HttpGet]
        public ActionResult Load()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClockTimes))
                return AccessDeniedView();

            // For view
            CandidateClockTimeModel model = new CandidateClockTimeModel();

            IList<TimeClockFileModel> timeClockFileModelList = new List<TimeClockFileModel>();

            try
            {
                string punchClockFilePath = _timeClockSettings.PunchClockFilesLocation;
                int count = 0;
                if (System.IO.Directory.Exists(punchClockFilePath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(punchClockFilePath);
                    foreach (FileInfo fileInfo in dirInfo.GetFiles())
                    {
                        count++;
                        TimeClockFileModel timeClockFileModel = new TimeClockFileModel();
                        timeClockFileModel.FileId = count;
                        timeClockFileModel.FileLocation = punchClockFilePath;
                        timeClockFileModel.FileName = fileInfo.Name;
                        timeClockFileModel.CreatedOn = fileInfo.CreationTimeUtc.ToLocalTime();
                        timeClockFileModel.ModifiedOn = fileInfo.LastWriteTimeUtc.ToLocalTime();

                        timeClockFileModelList.Add(timeClockFileModel);
                    }
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.TimeClocks.TimeClockFile.PathNotFound") + " " + punchClockFilePath);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification("Error occured while listing clock files : " + ex.Message);
            }

            // Hack MVC to use different model
            ViewBag.Data = timeClockFileModelList;
            ViewBag.Total = timeClockFileModelList.Count;

            return View(model);
        }
        #endregion

        #region POST:/ClockTime/Load
        [HttpPost]
        public ActionResult Load(CandidateClockTimeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClockTimes))
                return AccessDeniedView();


            // Scan the directory and load all punch clock file into database
            // --------------------------------------------------------------
            var errors = _clockTimeService.LoadPunchClockTime();


            //activity log
            _activityLogService.InsertActivityLog("LoadPunchClockTime", _localizationService.GetResource("ActivityLog.LoadPunchClockTime"), String.Join(", ", errors));

            // Display error message if any
            if (errors.Count > 0)
                foreach (var error in errors) WarningNotification(error);
            else
                SuccessNotification(_localizationService.GetResource("Admin.TimeClocks.CandidateClockTime.Loaded"));

            return RedirectToAction("Index");
        }

        #endregion

        #region Fix Daily Issues Wizard
        public ActionResult DailyIssuesProcess(int candidateClockTimeId)
        {
            var clockTime = _clockTimeService.GetClockTimeById(candidateClockTimeId);
            if (clockTime.CandidateId.HasValue)
            {
                if (clockTime.CandidateClockTimeStatusId != (int)CandidateClockTimeStatus.Processed)
                {
                    ViewBag.CandidateGuid = clockTime.Candidate.CandidateGuid;
                    ViewBag.ClockTimeId = candidateClockTimeId;
                    ViewBag.RefDate = clockTime.ClockInOut.Date;
                    bool candidateOnboarded = clockTime.Candidate.IsEmployee
                                        || clockTime.Candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started
                                        || clockTime.Candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished;
                    return PartialView("FixWizard", candidateOnboarded);
                }
                return Content("The punch record is processed!");
            }
            return Content("The punch card is not assigned to any candidates!");
            
        }
        #endregion

        #region Placement Result
        public ActionResult PlacementResult(int candidateClockTimeId)
        {
            PlacementResultModel_BL bl = new PlacementResultModel_BL(_clockTimeService, _candidateJobOrderService, _workContext, _logger, _localizationService, _workTimeService, _recruiterCompanyService);
            var result = bl.GetAllPlacementResultModels(candidateClockTimeId);
            return PartialView(result);
        }
        #endregion

        #region Related Work Time
        public ActionResult RelatedWorkTime(Guid? candidateGuid,int jobOrderId,DateTime refDate)
        {
            ViewBag.CandidateGuid = candidateGuid;
            ViewBag.JobOrderId = jobOrderId;
            ViewBag.RefDate = refDate;
            return PartialView();
        }
        [HttpPost]
        public ActionResult RelatedWorkTime(DataSourceRequest request,Guid? candidateGuid,int jobOrderId,DateTime refDate)
        {
            var workTimes = _workTimeService.GetAllWorkTimeByJobOrderAndDateAsQueryable(jobOrderId, refDate,true)
                            .Where(x => x.Candidate.CandidateGuid == candidateGuid);
            if (workTimes.Any(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved || (x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval && x.NetWorkTimeInHours > 0)))
                workTimes = workTimes.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved || (x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval && x.NetWorkTimeInHours > 0));
            //var result = workTimes.ProjectTo<CandidateWorkTimeModel>();
            return Json(workTimes.ToDataSourceResult(request,m=>m.ToModel()));
        }
        #endregion

        #region TimeSheetResult
        public ActionResult TimeSheetResult(int candidateWorkTimeId)
        {
            var workTime = _workTimeService.GetWorkTimeById(candidateWorkTimeId);
            bool result = false;
            if (workTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved)
            {
                result = true;
                ViewBag.Message = "The time sheet has been approved!";
            }
            if (workTime.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval && workTime.NetWorkTimeInHours > 0)
            {
                ViewBag.Message = "The time sheet's net hours is greater than 0!";
                result = true;
            }
            return PartialView(result);
        }
        #endregion

        #region CandidateCurrentStatus
        public ActionResult CandidateCurrentStatus(int clockTimeId)
        {
            CandidateCurrentStatusModel_BL bl = new CandidateCurrentStatusModel_BL(_candidateJobOrderService, _clockTimeService,_candidateService);
            var model = bl.GetCandidateCurrentStatus(clockTimeId);
            return PartialView(model);
        }
        #endregion

        #region ChangeCandidateUIBySmartCardUid
        [HttpPost]
        public ActionResult ChangeCandidateUIBySmartCardUid(string smartCardUid)
        {
            var any = _smartCardService.GetCandidateBySmartCardUid(smartCardUid,true);
            if (any != null)
                return Json(new { LastName = any.LastName, FirstName = any.FirstName, EmployeeId = any.EmployeeId});
            else
                return Json(new { LastName = string.Empty, FirstName = string.Empty, EmployeeId = string.Empty});
        }
        #endregion
    }
}

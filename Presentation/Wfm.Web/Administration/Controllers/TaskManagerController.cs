using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.ScheduleTask;
using Wfm.Core.Domain.Tasks;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.Tasks;
using Wfm.Web.Framework.Controllers;

namespace Wfm.Admin.Controllers
{
    public class TaskManagerController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public TaskManagerController(IActivityLogService activityLogService,
            IScheduleTaskService scheduleTaskService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _activityLogService = activityLogService;
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }
        #endregion

        #region Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduledTasks))
                return AccessDeniedView();

            IList<ScheduleTask> scheduletasks = _scheduleTaskService.GetAllTasks();
            IList<ScheduleTaskModel> scheduletaskmodels = new List<ScheduleTaskModel>();

            foreach (var item in scheduletasks)
            {
                ScheduleTaskModel i = item.ToModel();
                scheduletaskmodels.Add(i);
            }

            return View(scheduletaskmodels);
        }

        #endregion

        #region Get:/TaskManager/Create

        [HttpGet]
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduledTasks))
                return AccessDeniedView();

            ScheduleTaskModel model = new ScheduleTaskModel()
            {
                CreatedOnUtc = System.DateTime.UtcNow,
                UpdatedOnUtc = System.DateTime.UtcNow,
                IsActive = true
            };

            return View(model);

        }

        #endregion

        #region Post:/TaskManager/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ScheduleTaskModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduledTasks))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                ScheduleTask scheduletask = model.ToEntity();
                _scheduleTaskService.InsertTask(scheduletask);
                
                //activity log
                _activityLogService.InsertActivityLog("AddNewScheduleTask", _localizationService.GetResource("ActivityLog.AddNewScheduleTask"), model.Name);

                SuccessNotification(_localizationService.GetResource("Admin.ScheduleTask.Create.Success"));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("Index");
            }
            return View(model);
        }

        #endregion

        #region Get : /TaskManager/Edit

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduledTasks))
                return AccessDeniedView();

            ScheduleTask scheduletask = _scheduleTaskService.GetTaskById(Id);
            ScheduleTaskModel model = scheduletask.ToModel();
            model.UpdatedOnUtc = System.DateTime.UtcNow;

            return View(model);
        }

        #endregion

        #region Post: /TaskManager/Edit

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ScheduleTaskModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduledTasks))
                return AccessDeniedView();

            ScheduleTask scheduletask = _scheduleTaskService.GetTaskById(model.Id);
            if (ModelState.IsValid)
            {
                model.UpdatedOnUtc = System.DateTime.UtcNow;
                model.LastEndUtc = scheduletask.LastEndUtc;
                model.LastStartUtc = scheduletask.LastStartUtc;
                model.LastSuccessUtc = scheduletask.LastSuccessUtc;
                model.EnteredBy = scheduletask.EnteredBy;
                scheduletask = model.ToEntity(scheduletask);
                _scheduleTaskService.UpdateTask(scheduletask);

                //activity log
                _activityLogService.InsertActivityLog("UpdateScheduleTask", _localizationService.GetResource("ActivityLog.UpdateScheduleTask"), model.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ScheduleTask.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("Index");
            }

            return View(model);
        }

        #endregion
    }
}
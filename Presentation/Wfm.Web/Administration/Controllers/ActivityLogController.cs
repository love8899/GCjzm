using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Logging;
using Wfm.Core.Domain.Logging;
using Wfm.Core;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Web.Framework;

namespace Wfm.Admin.Controllers
{
    public class ActivityLogController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;

        #endregion 

        #region Ctor

        public ActivityLogController(IActivityLogService activityLogService,
            IWorkContext workContext,
            IPermissionService permissionService
                )
        {
            _activityLogService = activityLogService;
            _workContext = workContext;
            _permissionService = permissionService;
        }

        #endregion


        #region GET :/ActivityLog/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/ActivityLog/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //get paged list
            var activityLogs = _activityLogService.GetAllActivityLogAsQueryable(_workContext.CurrentAccount).PagedForCommand(request);

            List<ActivityLogModel> activityLogModelList = new List<ActivityLogModel>();

            foreach (var item in activityLogs)
            {
                ActivityLogModel activityLogModel = item.ToModel();

                activityLogModelList.Add(activityLogModel);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = activityLogModelList, // Process data (paging and sorting applied)
                Total = activityLogs.TotalCount // Total number of records
            };

            return Json(result);
        }
        #endregion



        #region GET :/ActivityLog/Details

        public ActionResult Details(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            ActivityLog activityLog = _activityLogService.GetActivityLogById(id);
            if (activityLog == null)
                return RedirectToAction("Index");

            ActivityLogModel model = activityLog.ToModel();
            model.ActivityLogTypeModel = activityLog.ActivityLogType.ToModel();

            return View(model);
        }

        #endregion

    }
}

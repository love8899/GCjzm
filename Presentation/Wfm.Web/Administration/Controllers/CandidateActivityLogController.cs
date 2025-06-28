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
    public class CandidateActivityLogController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;

        #endregion 

        #region Ctor

        public CandidateActivityLogController(IActivityLogService activityLogService,
            IWorkContext workContext,
            IPermissionService permissionService
                )
        {
            _activityLogService = activityLogService;
            _workContext = workContext;
            _permissionService = permissionService;
        }

        #endregion


        #region GET :/CandidateActivityLog/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateActivityLog))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/CandidateActivityLog/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateActivityLog))
                return AccessDeniedView();

            //get paged list
            var candidateActivityLogs = _activityLogService.GetAllCandidateActivityLogAsQueryable(_workContext.CurrentAccount).PagedForCommand(request);

            List<CandidateActivityLogModel> candidateActivityLogModelList = new List<CandidateActivityLogModel>();

            foreach (var item in candidateActivityLogs)
            {
                CandidateActivityLogModel candidateActivityLogModel = item.ToModel();

                candidateActivityLogModelList.Add(candidateActivityLogModel);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = candidateActivityLogModelList, // Process data (paging and sorting applied)
                Total = candidateActivityLogs.TotalCount // Total number of records
            };

            return Json(result);
        }
        #endregion



        #region GET :/CandidateActivityLog/Details

        public ActionResult Details(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateActivityLog))
                return AccessDeniedView();

            CandidateActivityLog candidateActivityLog = _activityLogService.GetCandidateActivityLogById(id);
            if (candidateActivityLog == null)
                return RedirectToAction("Index");

            CandidateActivityLogModel model = candidateActivityLog.ToModel();
            model.ActivityLogTypeModel = candidateActivityLog.ActivityLogType.ToModel();

            return View(model);
        }

        #endregion

    }
}

using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Logging;
using Wfm.Core.Domain.Logging;
using Wfm.Core;
using Wfm.Services.Logging;
using Wfm.Services.Accounts;
using Wfm.Services.Security;
using Wfm.Web.Framework;

namespace Wfm.Admin.Controllers
{
    public class AccessLogController : BaseAdminController
    {
        #region Fields

        private readonly IAccessLogService _accessLogService;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AccessLogController(IAccessLogService accessLogService,
            IAccountService accountService,
            IWorkContext workContext,
            IPermissionService permissionService
        )
        {
            _accessLogService = accessLogService;
            _accountService = accountService;
            _workContext = workContext;
            _permissionService = permissionService;
        }

        #endregion

        #region GET :/AccessLog/Index
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccessLog))
                return AccessDeniedView();

            return View();
        }
        #endregion

        #region POST:/AccessLog/Index
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccessLog))
                return AccessDeniedView();

            var accessLogs = _accessLogService.GetAllAccessLogAsQueryable(_workContext.CurrentAccount).PagedForCommand(request);

            List<AccessLogModel> accessLogModelList = new List<AccessLogModel>();

            foreach (var item in accessLogs)
            {
                AccessLogModel accessLogModel = MappingExtensions.ToModel(item);
                accessLogModelList.Add(accessLogModel);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = accessLogModelList, // Process data (paging and sorting applied)
                Total = accessLogs.TotalCount // Total number of records
            };

            return Json(result);
        }
        #endregion

        #region GET :/AccessLog/Details

        public ActionResult Details(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccessLog))
                return AccessDeniedView();

            AccessLog accessLog = _accessLogService.GetAccessLogById(id);
            if (accessLog == null)
                return RedirectToAction("Index");

            AccessLogModel accessLogModel = accessLog.ToModel();
            accessLogModel.AccountModel = _accountService.GetAccountById(accessLogModel.AccountId).ToModel();

            return View(accessLogModel);
        }

        #endregion
    }
}

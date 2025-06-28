using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.Logging;
using Wfm.Core.Domain.Logging;
using Wfm.Core;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework;

namespace Wfm.Admin.Controllers
{
    [AdminAuthorize]
    public partial class LogController : BaseAdminController
    {
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;

        public LogController(ILogger logger, IWorkContext workContext,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            this._activityLogService = activityLogService;
            this._logger = logger;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = new LogListModel();
            model.AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList();
            model.AvailableLogLevels.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult LogList([DataSourceRequest] DataSourceRequest request, LogListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            LogLevel? logLevel = model.LogLevelId > 0 ? (LogLevel?)(model.LogLevelId) : null;

            //var logItems = _logger.GetAllLogs(createdOnFromValue, createdToFromValue, model.Message,
            //    logLevel, request.Page - 1, request.PageSize);

            var logItems = _logger.GetAllLogsAsQueryable(createdOnFromValue, createdToFromValue, model.Message,
                logLevel, request.Page - 1, request.PageSize).PagedForCommand(request);

            var result = new DataSourceResult()
            {
                Data = logItems.Select(x =>
                {
                    return new LogModel()
                    {
                        
                        Id = x.Id,
                        LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                        ShortMessage = x.ShortMessage,
                        FullMessage = x.FullMessage,
                        IpAddress = x.IpAddress,
                        AccountId = x.AccountId,
                        AccountEmail = x.Account != null ? x.Account.Email : null,
                        PageUrl = x.PageUrl,
                        ReferrerUrl = x.ReferrerUrl,
                        //CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc.Value, DateTimeKind.Utc)
                        CreatedOnUtc = x.CreatedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc.Value, DateTimeKind.Utc) : DateTime.MinValue
                    };
                }),
                Total = logItems.TotalCount
            };
            return Json(result);
        }
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public ActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            _logger.ClearLog();

            //activity log
            _activityLogService.InsertActivityLog("DeleteAllLog", _localizationService.GetResource("ActivityLog.DeleteAllLog"), "ALL");


            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Cleared"));
            return RedirectToAction("List");
        }

        public ActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            var model = new LogModel()
            {
                Id = log.Id,
                LogLevel = log.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                AccountId = log.AccountId,
                AccountEmail = log.Account != null ? log.Account.Email : null,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc.Value, DateTimeKind.Utc)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            _logger.DeleteLog(log);

            //activity log
            _activityLogService.InsertActivityLog("DeleteLog", _localizationService.GetResource("ActivityLog.DeleteLog"), log.ShortMessage);


            SuccessNotification(_localizationService.GetResource("Common.IsDeleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null && selectedIds.Any())
            {
                var logItems = _logger.GetLogByIds(selectedIds.ToArray());
                foreach (var logItem in logItems)
                    _logger.DeleteLog(logItem);

                //activity log
                _activityLogService.InsertActivityLog("DeleteLog", _localizationService.GetResource("ActivityLog.DeleteLog"), "SELECTED : " + string.Join(",", selectedIds));

                SuccessNotification(_localizationService.GetResource("Common.IsDeleted"));
            }

            return Json(new { Result = true});
        }
    }
}

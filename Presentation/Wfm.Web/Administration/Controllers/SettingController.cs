using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Settings;
using Wfm.Core.Domain.Configuration;
using Wfm.Services.Logging;
using Wfm.Services.Configuration;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


namespace Wfm.Admin.Controllers
{
    public class SettingController : BaseAdminController
    {
        #region Fields/Setting

        private readonly IActivityLogService _activityLogService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public SettingController(IActivityLogService activityLogService,
            ISettingService settingService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _activityLogService = activityLogService;
            _settingService = settingService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }

        #endregion

        #region /Setting/Index
        
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var settings = _settingService.GetAllSettings();
            var result = settings.ToDataSourceResult(request, x => x.ToModel());

            return Json(result);
        }

        #endregion


        #region GET :/Setting/Create

        [HttpGet]
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new SettingModel();

            return View(model);
        }
        #endregion

        #region POST:/Setting/Create

        [HttpPost]
        public ActionResult Create(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //model.CreatedOnUtc = System.DateTime.UtcNow;
            //model.UpdatedOnUtc = System.DateTime.UtcNow;

            Setting setting = model.ToEntity();

            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _activityLogService.InsertActivityLog("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Added"));

            return RedirectToAction("Index");
        }
        #endregion

        #region GET :/Setting/Edit

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            Setting setting = _settingService.GetSettingById(Id);
            SettingModel model = setting.ToModel();

            return View(model);
        }
        #endregion

        #region GET :/Setting/Edit

        [HttpPost]
        public ActionResult Edit(Setting model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //model.UpdatedOnUtc = System.DateTime.UtcNow;

                _settingService.SetSetting(model.Name, model.Value);

                //activity log
                _activityLogService.InsertActivityLog("UpdateSetting", _localizationService.GetResource("ActivityLog.UpdateSetting"), model.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Updated"));
            }

            return RedirectToAction("Index");
        }
        #endregion

    }
}

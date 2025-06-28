using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Core.Domain.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework;
using Wfm.Shared.Models.Localization;
using Kendo.Mvc.Extensions;
using System.Linq;
namespace Wfm.Admin.Controllers
{
    public class LocaleStringResourceController : BaseAdminController
    {

        #region Fields
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILanguageService _languageService;
        #endregion

        #region Ctor
        public LocaleStringResourceController(
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ILanguageService languageService)
        {
            _activityLogService = activityLogService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _languageService = languageService;
        }
        #endregion

        #region GET :/LocaleStringResource/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLocalStringResource))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/LocaleStringResource/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLocalStringResource))
                return AccessDeniedView();
            var localStringResources = _localizationService.GetAllResourcesAsQueryable();


            return Json(localStringResources.ToDataSourceResult(request,m=>m.ToModel()));
        }

        #endregion

        #region Batch Edit
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<LocaleStringResourceModel> models)
        {
            var results = new List<LocaleStringResourceModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    _localizationService.InsertLocaleStringResource(entity);
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewLocaleStringResource", _localizationService.GetResource("ActivityLog.AddNewLocaleStringResource"), model.ResourceName);

                    model.Id = entity.Id;
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<LocaleStringResourceModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _localizationService.GetLocaleStringResourceById(model.Id);
                    model.ToEntity(entity);
                    _localizationService.UpdateLocaleStringResource(entity);
                    //activity log
                    _activityLogService.InsertActivityLog("UpdateLocaleStringResource", _localizationService.GetResource("ActivityLog.UpdateLocaleStringResource"), model.ResourceName);

                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<LocaleStringResourceModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _localizationService.GetLocaleStringResourceById(model.Id);
                    _localizationService.DeleteLocaleStringResource(entity);

                    //activity log
                    _activityLogService.InsertActivityLog("DeleteLocaleStringResource", _localizationService.GetResource("ActivityLog.DeleteLocaleStringResource"), model.ResourceName);

                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

    }
}

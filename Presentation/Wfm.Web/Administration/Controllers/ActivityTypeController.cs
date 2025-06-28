using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Services.Companies;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Wfm.Core.Domain.Companies;

namespace Wfm.Admin.Controllers
{
    public class ActivityTypeController : BaseAdminController
    {
        #region Field
        private readonly IPermissionService _permissionService;
        private readonly IActivityTypeService _activityTypeService;
        #endregion

        #region Ctor
        public ActivityTypeController(IPermissionService permissionService, IActivityTypeService activityTypeService)
        {
            _permissionService = permissionService;
            _activityTypeService = activityTypeService;
        }
        #endregion
        // GET: ActivityType
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageConfiguration))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _ActivityTypeList(DataSourceRequest request)
        {
            var result = _activityTypeService.GetAllActivityTypes();
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult _AddActivityType([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ActivityType> models)
        {
            var results = new List<ActivityType>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _activityTypeService.Create(model);
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditActivityType([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ActivityType> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _activityTypeService.Update(model);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _RemoveActivityType([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ActivityType> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _activityTypeService.Delete(model);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
    }
}
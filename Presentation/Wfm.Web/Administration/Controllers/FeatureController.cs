using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Wfm.Admin.Models.Features;
using Wfm.Services.Features;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;


namespace Wfm.Admin.Controllers
{

    public class FeatureController : BaseAdminController
    {

        #region Field

        private readonly IPermissionService _permissionService;
        private readonly IFeatureService _featureService;
        private readonly IUserFeatureService _userFeatureService;
        
        #endregion

        #region Ctor

        public FeatureController(IPermissionService permissionService, IFeatureService featureService, IUserFeatureService userFeatureService)
        {
            _permissionService = permissionService;
            _featureService = featureService;
            _userFeatureService = userFeatureService;
        }
        
        #endregion
        
        
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFeatures))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult _Index(DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFeatures))
                return AccessDeniedView();

            var features = _featureService.GetAllFeatures(activeOnly: false);
            var result = features.ProjectTo<FeatureModel>();
                
            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _AddFeature([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<FeatureModel> models)
        {
            var results = new List<FeatureModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var anyDuplicate = _featureService.GetFeaturesByAreaAndNameAndCode(model.Area, model.Code, model.Name);
                    if (anyDuplicate != null && anyDuplicate.Count > 0)
                    {
                        ModelState.AddModelError("Name", String.Format("The feature [{0}]/[{1}]/[{2}] is duplicated!", model.Area, model.Code, model.Name));
                        continue;
                    }

                    var entity = model.ToEntity();
                    entity.CreatedOnUtc = DateTime.UtcNow;
                    entity.UpdatedOnUtc = entity.CreatedOnUtc;
                    _featureService.InsertFeature(entity);

                    // enable for all users
                    _userFeatureService.EnableNewFeatureForAllUsers(entity);

                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditFeature([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<FeatureModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var anyDuplicate = _featureService.GetFeaturesByAreaAndNameAndCode(model.Area, model.Code, model.Name).Where(x => x.Id != model.Id);
                    if (anyDuplicate != null && anyDuplicate.Count() > 0)
                    {
                        ModelState.AddModelError("Name", String.Format("The feature [{0}]/[{1}]/[{2}] is duplicated!", model.Area, model.Code, model.Name));
                        continue;
                    }

                    var entity = _featureService.GetFeatureById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _featureService.UpdateFeature(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

    }
}

using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Services.Security;
using Wfm.Services.WSIBS;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.WSIBs;

namespace Wfm.Admin.Controllers
{
    public class WSIBController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWSIBService _wSIBService;

        public WSIBController(IPermissionService permissionService, IWSIBService wSIBService)
        {
            _permissionService = permissionService;
            _wSIBService = wSIBService;
        }
        // GET: WSIB
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyBillings))
                return AccessDeniedView();
            var result = _wSIBService.GetAllWSIBs();
            return Json(result.ToDataSourceResult(request,x=>x.ToModel()));
        }


        #region CRUD
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<WSIBModel> models)
        {
            var results = new List<WSIBModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    if (!_wSIBService.IsOverlappingOrNot(entity,false))
                    {
                        _wSIBService.Create(entity);
                        model.Id = entity.Id;
                        results.Add(model);
                    }
                    else
                    {
                        ModelState.AddModelError("Id", "The new WSIB has confilicts with existing WSIB!");
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<WSIBModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {

                    var entity = _wSIBService.Retrieve(model.Id);
                    model.ToEntity(entity);
                    if (!_wSIBService.IsOverlappingOrNot(entity,true))
                    {
                        _wSIBService.Update(entity);
                    }
                    else
                    {
                        ModelState.AddModelError("Id", "The new WSIB has confilicts with existing WSIB!");
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<WSIBModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _wSIBService.Retrieve(model.Id);
                    _wSIBService.Delete(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Copy WSIB
        public ActionResult _CopyWSIB(int id)
        {
            var model = _wSIBService.Retrieve(id).ToModel();
            model.StartDate = DateTime.Today;
            model.EndDate = null;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _SaveCopiedWSIB(WSIBModel model)
        {
            ModelState.Remove("Id");
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                if (!_wSIBService.IsOverlappingOrNot(entity, false))
                {
                    _wSIBService.Create(entity);
                }
                else
                {
                    message = "The new records is overlapping with an existing record. Please check the dates and try again.";
                }

            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                message = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
            }
            return Json(new { Result = String.IsNullOrEmpty(message),Message = message});
        }
        #endregion
    }
}
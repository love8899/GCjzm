using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Services.Common;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Controllers
{
    public class DNRReasonController : BaseAdminController
    {
        #region Field
        private readonly IPermissionService _permissionService;
        private readonly IDNRReasonService _dNRReasonService;
        #endregion
        #region Ctor
        public DNRReasonController(IPermissionService permissionService, IDNRReasonService dNRReasonService)
        {
            _permissionService = permissionService;
            _dNRReasonService = dNRReasonService;
        }
        #endregion

        // GET: DNRReason
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
            var result = _dNRReasonService.GetAllDNRReasons();
            return Json(result.ToDataSourceResult(request, x => x.ToModel()));
        }

        #region CRUD
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DNRReasonModel> models)
        {
            var results = new List<DNRReasonModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    _dNRReasonService.Create(entity);
                    model.Id = entity.Id;
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DNRReasonModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {

                    var entity = _dNRReasonService.Retrieve(model.Id);
                    model.ToEntity(entity);
                    _dNRReasonService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DNRReasonModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _dNRReasonService.Retrieve(model.Id);
                    entity.IsDeleted = true;
                    _dNRReasonService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}
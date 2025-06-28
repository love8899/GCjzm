using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Services.Companies;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Wfm.Core.Domain.Companies;

namespace Wfm.Admin.Controllers
{
    public class CompanyStatusController : BaseAdminController
    {
        #region Fields
        private readonly ICompanyStatusService _companyStatusService;
        private readonly IPermissionService _permissionService;
        #endregion

        #region CTOR
        public CompanyStatusController(ICompanyStatusService companyStatusService, IPermissionService permissionService)
        {
            _companyStatusService = companyStatusService;
            _permissionService = permissionService;
        }
        #endregion
        // GET: CompanyStatus
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageConfiguration))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult Index(DataSourceRequest request)
        {
            var result = _companyStatusService.GetAllCompanyStatus();
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult _AddCompanyStatus([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyStatus> models)
        {
            var results = new List<CompanyStatus>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _companyStatusService.Create(model);
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditCompanyStatus([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyStatus> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _companyStatusService.Update(model);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _RemoveCompanyStatus([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanyStatus> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _companyStatusService.Delete(model);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
    }
}
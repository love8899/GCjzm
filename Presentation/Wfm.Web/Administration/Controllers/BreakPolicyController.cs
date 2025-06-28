using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Policies;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Policies;
using Wfm.Services.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System;

namespace Wfm.Admin.Controllers
{
    public class BreakPolicyController : BaseAdminController
    {

        #region Fields

        private readonly IBreakPolicyService _breakPolicyService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BreakPolicyController(
            IBreakPolicyService BreakPolicyService,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
        
            _permissionService = permissionService;
            _breakPolicyService = BreakPolicyService;
            _workContext = workContext;
        }

        #endregion


        #region GET ://Index

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #endregion

        #region GET :// List

        [HttpGet]
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolicies))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:// List

        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolicies))
                return AccessDeniedView();

            var breakPolicies = _breakPolicyService.GetAllBreakPoliciesAsQueryable();
            if (_workContext.CurrentAccount.IsClientAdministrator())
                breakPolicies = breakPolicies.Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId);
            return Json(breakPolicies.ToDataSourceResult(request,x=>x.ToModel()));
        }

        #endregion

        #region CRUD
        [HttpPost]
        public ActionResult _AddBreakPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<BreakPolicyModel> models)
        {
            var results = new List<BreakPolicyModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.EnteredBy = _workContext.CurrentAccount.Id;
                    model.DisplayOrder = 0;
                    var entity = model.ToEntity();
                    
                    _breakPolicyService.Insert(entity);
                    model.Id = entity.Id;
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditBreakPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<BreakPolicyModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _breakPolicyService.GetBreakPolicyById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;
                    _breakPolicyService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion
    }
}

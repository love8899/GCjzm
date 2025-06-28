using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Policies;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Policies;
using Wfm.Services.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System;

namespace Wfm.Admin.Controllers
{
    public class RoundingPolicyController : BaseAdminController
    {

        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IRoundingPolicyService _roundingPolicyService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public RoundingPolicyController(IActivityLogService activityLogService,
            IRoundingPolicyService RoundingPolicyService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _roundingPolicyService = RoundingPolicyService;
            _workContext = workContext;
        }

        #endregion



        #region GET ://Index

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #endregion

        #region GET ://List

        [HttpGet]
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolicies))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST://List

        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolicies))
                return AccessDeniedView();

            var roundingPolicies = _roundingPolicyService.GetAllRoundingPoliciesAsQueryable();
            if (_workContext.CurrentAccount.IsClientAdministrator())
                roundingPolicies = roundingPolicies.Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId);
            return Json(roundingPolicies.ToDataSourceResult(request,x=>x.ToModel()));
        }

        #endregion


        #region POST://Create

        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RoundingPolicyModel> models)
        {
            var results = new List<RoundingPolicyModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    entity.EnteredBy = _workContext.CurrentAccount.Id;
                    entity.DisplayOrder = 0;
                    _roundingPolicyService.Insert(entity);
                    model.Id = entity.Id;
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewRoundingPolicy", _localizationService.GetResource("ActivityLog.AddNewRoundingPolicy"), model.Name);
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region POST://Edit

        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RoundingPolicyModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _roundingPolicyService.GetRoundingPolicyById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _roundingPolicyService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        // JsonResult
        // =============================


    }
}

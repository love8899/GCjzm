using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Policies;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Logging;
using Wfm.Services.Companies;
using Wfm.Services.Localization;
using Wfm.Services.Policies;
using Wfm.Services.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System;

namespace Wfm.Admin.Controllers
{
    public class MealPolicyController : BaseAdminController
    {

        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IMealPolicyService _mealPolicyService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public MealPolicyController(IActivityLogService activityLogService,
            IMealPolicyService MealPolicyService,
            ICompanyService CompanyService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _mealPolicyService = MealPolicyService;
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

            var mealPolicies = _mealPolicyService.GetAllMealPoliciesAsQueryable();
            if (_workContext.CurrentAccount.IsClientAdministrator())
                mealPolicies = mealPolicies.Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId);
            mealPolicies = mealPolicies.OrderBy(x => x.Company.CompanyName);

            return Json(mealPolicies.ToDataSourceResult(request,x=>x.ToModel()));
        }

        #endregion



        #region POST://Create
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MealPolicyModel> models)
        {
            var results = new List<MealPolicyModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    entity.EnteredBy = _workContext.CurrentAccount.Id;
                    entity.DisplayOrder = 0;
                    _mealPolicyService.Insert(entity);
                    model.Id = entity.Id;
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewMealPolicy", _localizationService.GetResource("ActivityLog.AddNewMealPolicy"), model.Name);
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
      

        #endregion


        #region POST://Edit
        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MealPolicyModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _mealPolicyService.GetMealPolicyById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _mealPolicyService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
      

        #endregion


        // JsonResult
        // =============================

        

    }
}

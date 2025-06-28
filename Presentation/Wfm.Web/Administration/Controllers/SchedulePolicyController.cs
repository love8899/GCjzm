using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using System.Linq;
using Wfm.Admin.Models.Policies;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Policies;
using Wfm.Services.Security;
using Wfm.Web.Framework;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Wfm.Admin.Controllers
{
    public class SchedulePolicyController : BaseAdminController
    {

        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IRoundingPolicyService _roundingPolicyService;
        private readonly IMealPolicyService _mealPolicyService;
        private readonly IBreakPolicyService _breakPolicyService;
        private readonly ISchedulePolicyService _schedulePolicyService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SchedulePolicyController(IActivityLogService activityLogService,
            IRoundingPolicyService roundingPolicyService,
            IMealPolicyService mealPolicyService,
            IBreakPolicyService breakPolicyService,
            ISchedulePolicyService SchedulePolicyService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _roundingPolicyService = roundingPolicyService;
            _mealPolicyService = mealPolicyService;
            _breakPolicyService = breakPolicyService;
            _schedulePolicyService = SchedulePolicyService;
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

            var schedulePolicies = _schedulePolicyService.GetAllSchedulePoliciesAsQueryable();
            if (_workContext.CurrentAccount.IsClientAdministrator())
                schedulePolicies = schedulePolicies.Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId);

            schedulePolicies = schedulePolicies.OrderBy(x => x.Company.CompanyName);

            var schedulePolicyModels = new List<SchedulePolicyModel>();

            foreach (var item in schedulePolicies.PagedForCommand(request))
            {
                SchedulePolicyModel schedulePolicyModel = item.ToModel();

                
                var mealPolicy = _mealPolicyService.GetMealPolicyById(item.MealPolicyId);
                var breakPolicy = _breakPolicyService.GetBreakPolicyById(item.BreakPolicyId);
                var roundingPolicy = _roundingPolicyService.GetRoundingPolicyById(item.RoundingPolicyId);


                schedulePolicyModel.MealTimeInMinutes = item.MealPolicyId == 0 ? 0 : mealPolicy.MealTimeInMinutes;
                schedulePolicyModel.BreakTimeInMinutes = item.BreakPolicyId == 0 ? 0 : breakPolicy.BreakTimeInMinutes;
                schedulePolicyModel.RoundingIntervalInMinutes = item.RoundingPolicyId == 0 ? 0 : roundingPolicy.IntervalInMinutes;
                schedulePolicyModel.RoundingGracePeriodInMinutes = item.RoundingPolicyId == 0 ? 0 : roundingPolicy.GracePeriodInMinutes;

                schedulePolicyModels.Add(schedulePolicyModel);
            }

            var gridModel = new DataSourceResult()
            {
                Data = schedulePolicyModels,
                Total = schedulePolicies.Count()
            };

            return Json(gridModel);
        }

        #endregion

        #region POST://Create
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<SchedulePolicyModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolicies))
                return AccessDeniedView();
            var results = new List<SchedulePolicyModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    entity.EnteredBy = _workContext.CurrentAccount.Id;
                    entity.DisplayOrder = 0;
                    _schedulePolicyService.Insert(entity);
                    results.Add(model);
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewSchedulePolicy", _localizationService.GetResource("ActivityLog.AddNewSchedulePolicy"), model.Name);

                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
  

        #endregion

        #region POST://Edit
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<SchedulePolicyModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePolicies))
                return AccessDeniedView();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    _schedulePolicyService.Update(model.ToEntity());
                    //activity log
                    _activityLogService.InsertActivityLog("UpdateSchedulePolicy", _localizationService.GetResource("ActivityLog.UpdateSchedulePolicy"), model.Name);

                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
  

        #endregion


    }
}

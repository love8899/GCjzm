using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.JobOrder;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Logging;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using Wfm.Core;
using System;

namespace Wfm.Admin.Controllers
{
    public class JobCategoryController : BaseAdminController  //Controller name should same as the View folder name.
    {
        #region Fields/JobCategory

        private readonly IActivityLogService _activityLogService;
        private readonly IJobOrderCategoryService _jobOrderCategoryService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public JobCategoryController(IActivityLogService activityLogService,
            IJobOrderCategoryService jobOrderCategoryService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _jobOrderCategoryService = jobOrderCategoryService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region GET :/JobCategory/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobCategories))
                return AccessDeniedView();

            return View();
        }
        #endregion

        #region POST:/JobCategory/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobCategories))
                return AccessDeniedView();

            var categories = _jobOrderCategoryService.GetAllJobOrderCategoriesAsQueryable(true);

            return Json(categories.ToDataSourceResult(request,x=>x.ToModel()));
        }

        #endregion

        //#region GET :/JobOrderCategory/Create

        //[HttpGet]
        //public ActionResult Create()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobCategories))
        //        return AccessDeniedView();

        //    JobOrderCategoryModel model = new JobOrderCategoryModel();

        //    model.UpdatedOnUtc = System.DateTime.UtcNow;
        //    model.CreatedOnUtc = System.DateTime.UtcNow;
        //    model.IsActive = true;

        //    return View(model);
        //}

        //#endregion

        //#region POST:/JobOrderCategory/Create

        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //public ActionResult Create(JobOrderCategoryModel model, bool continueEditing)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobCategories))
        //        return AccessDeniedView();


        //    if (ModelState.IsValid)
        //    {
        //        JobOrderCategory jobOrderCategory = model.ToEntity();

        //        jobOrderCategory.UpdatedOnUtc = System.DateTime.UtcNow;
        //        jobOrderCategory.CreatedOnUtc = System.DateTime.UtcNow;


        //        _jobOrderCategoryService.InsertJobOrderCategory(jobOrderCategory);


        //        //activity log
        //        _activityLogService.InsertActivityLog("AddNewJobCategory", _localizationService.GetResource("ActivityLog.AddNewJobCategory"), jobOrderCategory.CategoryName);


        //        //Notification message
        //        SuccessNotification(_localizationService.GetResource("Admin.Configuration.JobOrderCategory.Added"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = jobOrderCategory.Id }) : RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        //#endregion

        //#region GET :/JobOrderCategory/Edit
        //[HttpGet]
        //public ActionResult Edit(int id)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobCategories))
        //        return AccessDeniedView();

        //    JobOrderCategory jobOrderCategory = _jobOrderCategoryService.GetJobOrderCategoryById(id);

        //    if (jobOrderCategory == null) return RedirectToAction("Index");

        //    jobOrderCategory.UpdatedOnUtc = System.DateTime.UtcNow;
        //    JobOrderCategoryModel jobOrderCategoryModel = jobOrderCategory.ToModel();

        //    return View(jobOrderCategoryModel);
        //}
        //#endregion

        //#region POST:/JobOrderCategory/Edit
        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //public ActionResult Edit(JobOrderCategoryModel model, bool continueEditing)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobCategories))
        //        return AccessDeniedView();


        //    JobOrderCategory jobOrderCategory = _jobOrderCategoryService.GetJobOrderCategoryById(model.Id);
        //    if (jobOrderCategory == null) return RedirectToAction("Index");

        //    if (ModelState.IsValid)
        //    {
        //        jobOrderCategory = model.ToEntity(jobOrderCategory);

        //        jobOrderCategory.UpdatedOnUtc = System.DateTime.UtcNow;


        //        _jobOrderCategoryService.UpdateJobOrderCategory(jobOrderCategory);


        //        //activity log
        //        _activityLogService.InsertActivityLog("UpdateJobCategory", _localizationService.GetResource("ActivityLog.UpdateJobCategory"), jobOrderCategory.CategoryName);


        //        //Notification message
        //        SuccessNotification(_localizationService.GetResource("Admin.Configuration.JobOrderCategory.Updated"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = jobOrderCategory.Id }) : RedirectToAction("Index");
        //    }

        //    return View(model);
        //}

        //#endregion

        #region Batch Edit/Create
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<JobOrderCategoryModel> models)
        {
            var results = new List<JobOrderCategoryModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    entity.EnteredBy = _workContext.CurrentAccount.Id;
                    _jobOrderCategoryService.InsertJobOrderCategory(entity);
                    model.Id = entity.Id;
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewJobCategory", _localizationService.GetResource("ActivityLog.AddNewJobCategory"), model.CategoryName);
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<JobOrderCategoryModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _jobOrderCategoryService.GetJobOrderCategoryById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _jobOrderCategoryService.UpdateJobOrderCategory(entity);
                    //activity log
                    _activityLogService.InsertActivityLog("UpdateJobCategory", _localizationService.GetResource("ActivityLog.UpdateJobCategory"), entity.CategoryName);

                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}

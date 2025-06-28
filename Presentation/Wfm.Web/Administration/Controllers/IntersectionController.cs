using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Common;
using Wfm.Core.Domain.Common;
using Wfm.Services.Logging;
using Wfm.Services.Common;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using System;

namespace Wfm.Admin.Controllers
{
    public class IntersectionController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IIntersectionService _intersectionService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public IntersectionController(IActivityLogService activityLogService,
            IIntersectionService intersectionService,
            IPermissionService permissionService,
            ILocalizationService localizationService
                )
        {
            _activityLogService = activityLogService;
            _intersectionService = intersectionService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }

        #endregion

        #region GET/POST: /Intersection/Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            var intersections = _intersectionService.GetAllIntersections();

            List<IntersectionModel> intersectionList = new List<IntersectionModel>();
            foreach (var item in intersections)
            {
                IntersectionModel i = item.ToModel();   //MappingExtensions.ToModel(item);              
                intersectionList.Add(i);
            }

            return View(intersectionList);
        }

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            if (request.PageSize == 0)
            {
                request.PageSize = 10;
            }

            var intersections = _intersectionService.GetAllIntersections(); //it's a IPageList

            List<IntersectionModel> intersectionModelList = new List<IntersectionModel>();
            foreach (var item in intersections)
            {
                IntersectionModel i = MappingExtensions.ToModel(item);
                intersectionModelList.Add(i);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = intersectionModelList, // Process data (paging and sorting applied)
                Total = intersections.Count // Total number of records
            };

            return Json(result);
        }

        #endregion

        #region GET/POST: /Interserction/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            IntersectionModel intersectionModel = new IntersectionModel();
            intersectionModel.CreatedOnUtc = System.DateTime.UtcNow;
            intersectionModel.UpdatedOnUtc = System.DateTime.UtcNow;

            return View(intersectionModel);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(IntersectionModel intersectionModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            intersectionModel.CreatedOnUtc = System.DateTime.UtcNow;
            intersectionModel.UpdatedOnUtc = System.DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                Intersection intersection = intersectionModel.ToEntity();
                _intersectionService.InsertIntersection(intersection);

                //activity log
                _activityLogService.InsertActivityLog("AddNewIntersection", _localizationService.GetResource("ActivityLog.AddNewIntersection"), intersection.IntersectionName);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Intersection.Create.Success"));
                return continueEditing ? RedirectToAction("Create", new { id = intersection.Id }) : RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region GET/POST: /Interserction/Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            Intersection intersection = _intersectionService.GetIntersectionById(id);
            if (intersection == null) return RedirectToAction("Index");

            IntersectionModel intersectionModel = intersection.ToModel();
            intersectionModel.UpdatedOnUtc = System.DateTime.UtcNow;

            return View(intersectionModel);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(IntersectionModel intersectionModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            Intersection intersection = _intersectionService.GetIntersectionById(intersectionModel.Id);
            if (intersection == null) return RedirectToAction("Index");
         if (ModelState.IsValid)
            {
                intersectionModel.UpdatedOnUtc = System.DateTime.UtcNow;
                intersection = intersectionModel.ToEntity(intersection);
                _intersectionService.UpdateIntersection(intersection);

                //activity log
                _activityLogService.InsertActivityLog("UpdateIntersection", _localizationService.GetResource("ActivityLog.UpdateIntersection"), intersection.IntersectionName);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Intersection.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = intersection.Id }) : RedirectToAction("Index");
            }
            return View(intersectionModel);
        }
        #endregion

        #region POST:/Intersection/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIntersections))
                return AccessDeniedView();

            var intersection = _intersectionService.GetIntersectionById(id);
            if (intersection == null)              
                return RedirectToAction("Index");
            try
            {
                _intersectionService.DeleteIntersection(intersection);
                //activity log
                _activityLogService.InsertActivityLog("DeleteIntersection", _localizationService.GetResource("ActivityLog.DeleteIntersection"), intersection.IntersectionName);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Intersection.Deleted"));
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = intersection.Id });
            }
        }
        #endregion
    }
}

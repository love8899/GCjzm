using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Directory;
using Wfm.Core.Domain.Common;
using Wfm.Core;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.Extensions;
using System.Linq;

namespace Wfm.Admin.Controllers
{
    public class StateProvinceController : BaseAdminController
    {
        #region Fields
        private readonly IActivityLogService _activityLogService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public StateProvinceController(IActivityLogService activityLogService,
            IStateProvinceService stateProvinceService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _activityLogService = activityLogService;
            _stateProvinceService = stateProvinceService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }
        #endregion

        #region GET :/StateProvince/Index
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            var stateProvinces = _stateProvinceService.GetAllStateProvinces(true);          

            List<StateProvinceModel> stateProvinceModelList = new List<StateProvinceModel>();
            foreach (var item in stateProvinces)
            {
                StateProvinceModel p = MappingExtensions.ToModel(item);
                stateProvinceModelList.Add(p);
            }

            return View(stateProvinceModelList);
        }
        #endregion

        #region POST:/StateProvince/Index
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest dsRequest)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            var stateProvinceList = _stateProvinceService.GetAllStateProvinces(true);

            List<StateProvinceModel> stateProvinceModelList = new List<StateProvinceModel>();

            foreach (var item in stateProvinceList)
            {
                StateProvinceModel p = MappingExtensions.ToModel(item);
                stateProvinceModelList.Add(p);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = stateProvinceModelList, // Process data (paging and sorting applied)
                Total = stateProvinceList.Count // Total number of records
            };

            return Json(result);
        }

        #endregion

        #region  GET: /StateProvince/Create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            var model = new StateProvinceModel();

            model.IsActive = true;
            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;

            return View(model);
        }
        #endregion

        #region POST:/StateProvince/Create
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(StateProvinceModel stateProvinceModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //stateProvinceModel.IsActive = true;
            stateProvinceModel.UpdatedOnUtc = System.DateTime.UtcNow;
            stateProvinceModel.CreatedOnUtc = System.DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                StateProvince stateProvince = stateProvinceModel.ToEntity();
                _stateProvinceService.InsertStateProvince(stateProvince);


                //activity log
                _activityLogService.InsertActivityLog("AddNewStateProvince", _localizationService.GetResource("ActivityLog.AddNewStateProvince"), stateProvince.StateProvinceName);


                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.StateProvince.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = stateProvince.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(stateProvinceModel);
        }
        #endregion

        #region GET :/StateProvince/Details

            public ActionResult Details(int id)
            {
                StateProvince StateProvince = _stateProvinceService.GetStateProvinceById(id);

                StateProvinceModel StateProvinceModel = StateProvince.ToModel();

                return View(StateProvinceModel);
            }

        #endregion

        #region GET :/StateProvince/Edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            StateProvince stateProvince = _stateProvinceService.GetStateProvinceById(id);
            if (stateProvince == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            StateProvinceModel stateProvinceModel = stateProvince.ToModel();
            stateProvinceModel.UpdatedOnUtc = System.DateTime.UtcNow;
            ViewBag.StateProvinceId = id;
            return View(stateProvinceModel);
        }
        #endregion

        #region POST:/StateProvince/Edit/
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(StateProvinceModel stateProvinceModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            stateProvinceModel.UpdatedOnUtc = System.DateTime.UtcNow;

            StateProvince stateProvince = _stateProvinceService.GetStateProvinceById(stateProvinceModel.Id);
            if (stateProvince == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                stateProvince = stateProvinceModel.ToEntity(stateProvince);
                _stateProvinceService.UpdateStateProvince(stateProvince);


                //activity log
                _activityLogService.InsertActivityLog("UpdateStateProvince", _localizationService.GetResource("ActivityLog.UpdateStateProvince"), stateProvince.StateProvinceName);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.StateProvince.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = stateProvince.Id }) : RedirectToAction("Index");
            }

            return View(stateProvinceModel);
        }
        #endregion

        #region POST:/StateProvince/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            var stateProvince = _stateProvinceService.GetStateProvinceById(id);
            if (stateProvince == null)
                //No state province found with the specified id
                return RedirectToAction("Index");

            try
            {
                 throw new WfmException("The state province can't be deleted. It has associated addresses");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return new JsonResult() { Data = new { success = false, message = exc.Message } };
            }
        }
        #endregion

      
        #region Batch Edit/Create Statutory Holidays
        [HttpPost]
        public ActionResult ListOfStatutoryHolidays([DataSourceRequest]DataSourceRequest request ,int stateProvinceId)
        {
            var holidays = _stateProvinceService.GetAllStatutoryHolidaysOfStateProvince(stateProvinceId).ToList();
            return Json(holidays.ToDataSourceResult(request,x=>x.ToModel()));
        }

        [HttpPost]
        public ActionResult CreateStatutoryHolidays([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<StatutoryHolidayModel> models)
        {
            var results = new List<StatutoryHolidayModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {                    
                    var entity = model.ToEntity();
                    _stateProvinceService.AddNewStatutoryHoliday(entity);
                    model.Id = entity.Id;
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult UpdateStatutoryHolidays([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<StatutoryHolidayModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _stateProvinceService.GetStatutoryHolidayById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _stateProvinceService.UpdateStatutoryHoliday(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult DeleteStatutoryHolidays([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<StatutoryHolidayModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _stateProvinceService.DeleteStatutoryHoliday(model.Id);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion

    } //end controller
} //end namespace

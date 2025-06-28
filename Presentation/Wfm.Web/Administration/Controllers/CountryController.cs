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

namespace Wfm.Admin.Controllers
{
    public class CountryController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICountryService _countryService;

        #endregion

        #region Ctor
        public CountryController(
            IActivityLogService activityLogService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICountryService countryService)
        {
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _countryService = countryService;
        }
        #endregion

        #region GET :/Country/Index
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var countryList =  _countryService.GetAllCountries(true);

            List<CountryModel> countryModelList = new List<CountryModel>();
            foreach (var item in countryList)
            {
                CountryModel c = MappingExtensions.ToModel(item);
                countryModelList.Add(c);
            }

            return View(countryModelList);
        }
        #endregion

        #region POST:/Country/Index
  
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var countryList = _countryService.GetAllCountries(true);

            List<CountryModel> countryModelList = new List<CountryModel>();
            foreach (var item in countryList)
            {
                CountryModel c = MappingExtensions.ToModel(item);
                countryModelList.Add(c);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = countryModelList, // Process data (paging and sorting applied)
                Total = countryList.Count // Total number of records
            };

            return Json(result);
        }

        #endregion

        #region  GET:/Country/Create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            CountryModel countryModel = new CountryModel();

            countryModel.IsActive = true;
            countryModel.UpdatedOnUtc = System.DateTime.UtcNow;
            countryModel.CreatedOnUtc = System.DateTime.UtcNow;

            return View(countryModel);
        }
        #endregion

        #region POST:/Country/Create
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(CountryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                Country country = model.ToEntity();
                _countryService.InsertCountry(country);

                //activity log
                _activityLogService.InsertActivityLog("AddNewCountry", _localizationService.GetResource("ActivityLog.AddNewCountry"), model.CountryName);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Country.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = country.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region GET :/Country/Details
        public ActionResult Details(int id)
        {
            Country country= _countryService.GetCountryById(id);

            CountryModel countryModel = country.ToModel();

            ViewBag.EditMode = 0;

            return View(countryModel);
        }
        #endregion

        #region GET :/Country/Edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            Country country = _countryService.GetCountryById(id);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            CountryModel countryModel = country.ToModel();
            countryModel.UpdatedOnUtc = System.DateTime.UtcNow;

            return View(countryModel);
        }
        #endregion

        #region POST:/Country/Edit/
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CountryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            model.UpdatedOnUtc = System.DateTime.UtcNow;

            Country country= _countryService.GetCountryById(model.Id);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                // the fellowing step is mapping countryModel contents to a entity name as country
                // and assignment to Country
                country = model.ToEntity(country);
                _countryService.UpdateCountry(country);

                //activity log
                _activityLogService.InsertActivityLog("UpdateCountry", _localizationService.GetResource("ActivityLog.UpdateCountry"), model.CountryName);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Country.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = country.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region POST:/Country/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var country = _countryService.GetCountryById(id);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            try
            {
                throw new WfmException("The country can't be deleted. It has associated addresses");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return new JsonResult() { Data = new { success = false, message = exc.Message } };
            }
        }
        #endregion

    } //end controller
} //end namespace

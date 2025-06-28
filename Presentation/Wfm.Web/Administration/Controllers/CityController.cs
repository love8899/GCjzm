using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.Directory;
using Wfm.Core.Domain.Common;
using Wfm.Services.Logging;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework;
using Kendo.Mvc.UI;

namespace Wfm.Admin.Controllers
{
    public class CityController : BaseAdminController
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public CityController(
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            IActivityLogService activityLogService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }
        #endregion


        #region GET :/City/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/City/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            //var accessLogs = _accessLogService.GetAllAccessLog(_workContext.CurrentAccount, request.Page - 1, request.PageSize, true);
            var cities = _cityService.GetAllCitiesAsQueryable().PagedForCommand(request);

            List<CityModel> cityModelList = new List<CityModel>();

            foreach (var item in cities)
            {
                CityModel cityModel = MappingExtensions.ToModel(item);
                StateProvince stateProvince = _stateProvinceService.GetStateProvinceById(cityModel.StateProvinceId);
                cityModel.StateProvinceModel = stateProvince.ToModel();
                cityModel.CountryId = stateProvince.CountryId;
                Country country = _countryService.GetCountryById(stateProvince.CountryId);
                cityModel.CountryName = country.CountryName;

                cityModelList.Add(cityModel);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = cityModelList, // Process data (paging and sorting applied)
                Total = cities.TotalCount // Total number of records
            };

            return Json(result);
        }
        #endregion

        #region GET :/City/Create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            CityModel cityModel = new CityModel();

            cityModel.DisplayOrder = 1;
            cityModel.IsActive = true;
            cityModel.UpdatedOnUtc = System.DateTime.UtcNow;
            cityModel.CreatedOnUtc = System.DateTime.UtcNow;

            return View(cityModel);
        }
        #endregion

        #region POST:/City/Create
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(CityModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                City city = model.ToEntity();
                _cityService.InsertCity(city);

                //activity log
                _activityLogService.InsertActivityLog("AddNewCity", _localizationService.GetResource("ActivityLog.AddNewCity"), model.CityName);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.City.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = city.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(model);

        }
        #endregion

        #region GET :/City/Edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            City city = _cityService.GetCityById(id);
            if (city == null)
                //No city found with the specified id
                return RedirectToAction("Index");

            CityModel cityModel = city.ToModel();

            // Get country name
            StateProvince stateProvince = _stateProvinceService.GetStateProvinceById(cityModel.StateProvinceId);
            cityModel.CountryId = stateProvince.CountryId;
            Country country = _countryService.GetCountryById(stateProvince.CountryId);
            cityModel.CountryName = country.CountryName;

            return View(cityModel);
        }
        #endregion

        #region POST:/City/Edit
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CityModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            model.UpdatedOnUtc = System.DateTime.UtcNow;

            City city = _cityService.GetCityById(model.Id);
            if (city == null)
                //No city found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                city = model.ToEntity(city);
                _cityService.UpdateCity(city);

                // For UI drop down
                IList<Country> countryList = _countryService.GetAllCountries();
                List<CountryModel> countryModelList = new List<CountryModel>();
                foreach (var item in countryList)
                {
                    CountryModel c = item.ToModel();
                    countryModelList.Add(c);
                }
                ViewBag.Countries = countryModelList;

                //activity log
                _activityLogService.InsertActivityLog("UpdateCity", _localizationService.GetResource("ActivityLog.UpdateCity"), model.CityName);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Cities.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = city.Id }) : RedirectToAction("Index");
            }

            return View(model);
        }

        #endregion


        #region GET :/City/Details

        public ActionResult Details(int id)
        {
            City city = _cityService.GetCityById(id);

            CityModel cityModel = city.ToModel();

            return View(cityModel);
        }

        #endregion


        #region POST:/City/Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCities))
                return AccessDeniedView();

            var city = _cityService.GetCityById(id);
            if (city == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            try
            {
                _cityService.DeleteCity(city);

                //activity log
                _activityLogService.InsertActivityLog("DeleteCity", _localizationService.GetResource("ActivityLog.DeleteCity"), city.CityName);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.City.Deleted"));
                //return RedirectToAction("Index");
                return new JsonResult() { Data = new { success = true, url = Url.Action("Index", "City") } };
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return new JsonResult() { Data = new { success = false, message = exc.Message } };
            }
        }
        #endregion


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RecogSys.RdrAccess;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Models.ClockTime;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core;
using Wfm.Services.Logging;
using Wfm.Services.Companies;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.ClockTime;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace Wfm.Admin.Controllers
{
    public class ClockDeviceController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ClockDeviceController(IWorkContext workContext,
            IActivityLogService activityLogService,
            IClockDeviceService clockDeviceService,
            ICompanyService companyService,
            ICompanyDivisionService companyDivisionService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _workContext = workContext;
            _activityLogService = activityLogService;
            _clockDeviceService = clockDeviceService;
            _companyService = companyService;
            _companyDivisionService = companyDivisionService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }

        #endregion


        #region Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            var clockDevices = _clockDeviceService.GetAllClockDevicesAsQueryable(activeOnly: false, excludeEnrolment: false);
            var result = clockDevices.ToDataSourceResult(request, x => x.ToModel());

            return Json(result);
        }

        #endregion


        #region POST:/CompanyLocation

        [HttpPost]
        public JsonResult _GetLocation(int? CompanyId)
        {
            return _GetCompanyLocation(CompanyId);
        }

        public JsonResult _GetCompanyLocation(int? CompanyId)
        {
            int companyid = 0;
            if (CompanyId == null)
            {
                companyid = 0;
            }
            else companyid = (int)CompanyId;

            IList<CompanyLocation> locations = _companyDivisionService.GetAllCompanyLocationsByCompanyId(companyid);

            List<CompanyLocationModel> locationModelList = new List<CompanyLocationModel>();

            foreach (var item in locations)
            {
                CompanyLocationModel i = MappingExtensions.ToModel(item);
                locationModelList.Add(i);
            }

            //SelectListItem locationModelList = new SelectListItem() { Text = "MyTest", Value = "MyTest" };

            return Json(new SelectList(locationModelList, "Id", "LocationName"), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GET/POST: /ClockDevice/Create

        [HttpGet]
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            CompanyClockDeviceModel clockDeviceModel = new CompanyClockDeviceModel();

            clockDeviceModel.CreatedOnUtc = System.DateTime.UtcNow;
            clockDeviceModel.UpdatedOnUtc = System.DateTime.UtcNow;
            clockDeviceModel.AddOnEnroll = true;
            clockDeviceModel.AltIdReader = false;
            clockDeviceModel.IsActive = true;

            return View(clockDeviceModel);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(CompanyClockDeviceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            model.ClockDeviceUid = model.ClockDeviceUid.ExtractAlphanumericText();
            model.CreatedOnUtc = System.DateTime.UtcNow;
            model.UpdatedOnUtc = System.DateTime.UtcNow;

            // check if the Clock Device already exists
            if (_clockDeviceService.GetClockDeviceByClockDeviceUid(model.ClockDeviceUid) != null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.TimeClocks.CompanyClockDevice.AddNew.Fail.ClockDeviceExists"));
                return View(model);
            }

            if (ModelState.IsValid)
            {
                CompanyClockDevice companyClockDevice = model.ToEntity();
                _clockDeviceService.Insert(companyClockDevice);

                //activity log
                _activityLogService.InsertActivityLog("AddNewClockDevice", _localizationService.GetResource("ActivityLog.AddNewClockDevice"), companyClockDevice.ClockDeviceUid);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.TimeClocks.CompanyClockDevice.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = companyClockDevice.Id }) : RedirectToAction("Index");
            }

            return View(model);
        }

        #endregion

        #region GET/POST: /ClockDevice/Edit

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            CompanyClockDevice companyClockDevice = _clockDeviceService.GetClockDeviceById(id);
            if (companyClockDevice == null)
                //Not found with the specified id
                return RedirectToAction("Index");

            CompanyClockDeviceModel companyClockDeviceModel = companyClockDevice.ToModel();

            CompanyLocation companyLocation = _companyDivisionService.GetCompanyLocationById(companyClockDeviceModel.CompanyLocationId);

            companyClockDeviceModel.LocationName = companyLocation.LocationName;
            companyClockDeviceModel.CompanyId = companyLocation.CompanyId;
            companyClockDeviceModel.CompanyName = _companyService.GetCompanyById(companyLocation.CompanyId).CompanyName;

            companyClockDeviceModel.UpdatedOnUtc = System.DateTime.UtcNow;

            return View(companyClockDeviceModel);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CompanyClockDeviceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            model.UpdatedOnUtc = System.DateTime.UtcNow;

            CompanyClockDevice clockDevice = _clockDeviceService.GetClockDeviceById(model.Id);
            if (clockDevice == null)
                //Not found with the specified id
                return RedirectToAction("Index");

            // check if the Clock Device already exists
            CompanyClockDevice clockDevice2 = _clockDeviceService.GetClockDeviceByClockDeviceUid(model.ClockDeviceUid);
            if (clockDevice2 != null && (model.Id != clockDevice2.Id))
            {
                ErrorNotification(_localizationService.GetResource("Admin.TimeClocks.CompanyClockDevice.AddNew.Fail.ClockDeviceExists"));
                return View(model);
            }

            if (ModelState.IsValid)
            {
                clockDevice = model.ToEntity(clockDevice);

                _clockDeviceService.Update(clockDevice);

                //activity log
                _activityLogService.InsertActivityLog("UpdateClockDevice", _localizationService.GetResource("ActivityLog.UpdateClockDevice"), clockDevice.ClockDeviceUid);

                SuccessNotification(_localizationService.GetResource("Admin.TimeClocks.CompanyClockDevice.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = clockDevice.Id }) : RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion


        #region Backup & Restore

        [HttpPost]
        public ActionResult Backup(int clockDeviceId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            var clockDevice = _clockDeviceService.GetClockDeviceById(clockDeviceId);
            if (clockDevice != null)
            {
                using (var hr = new HandReader(clockDevice.IPAddress))
                {
                    if (hr != null && hr.TryConnect())
                    {
                        var userData = hr.GetAllUserData().Select(x => x.Get());
                        if (userData == null || !userData.Any())
                            ErrorNotification("Cannot backup the clock. Try again later.");

                        var fileName = string.Format("ClockDevice_{0}_{1}.bkp", clockDevice.ClockDeviceUid, DateTime.Now.ToString("yyyy-MM-dd"));

                        return File(CommonHelper.Combine(userData), "application/octet-stream", fileName);
                    }
                    else
                        ErrorNotification("The clock is not ready. Please check.");
                }
            }
            else
                ErrorNotification("The clock is not found. Please check.");

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Restore(int clockDeviceId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanyClockDevices))
                return AccessDeniedView();

            var clockDevice = _clockDeviceService.GetClockDeviceById(clockDeviceId);
            if (clockDevice != null)
            {
                using (var hr = new HandReader(clockDevice.IPAddress))
                {
                    if (hr != null && hr.TryConnect())
                    {
                        // clear user database
                        //var rsp = new RSI_STATUS();
                        //hr.ClearUserDatabase(rsp);

                        var file = Request.Files["restorefile"];
                        if (file == null || file.ContentLength == 0)
                            ErrorNotification(_localizationService.GetResource("Admin.Common.SelectFile"));
                        else
                        {
                            var fileName = file.FileName;
                            if (System.IO.Path.GetExtension(fileName) != ".bkp")
                                ErrorNotification("The file must be a BACKUP file.");
                            else
                            {
                                var userData = new List<CRsiDataBank>();
                                byte[] bytes;
                                using (var ms = new System.IO.MemoryStream())
                                {
                                    file.InputStream.CopyTo(ms);
                                    bytes = ms.ToArray();
                                }
                                if (bytes == null || bytes.Length == 0)
                                    ErrorNotification("Cannot get data from the backup file.");
                                else
                                {
                                    foreach (var chunk in CommonHelper.Split(bytes, 4096))
                                    {
                                        var dataBank = new CRsiDataBank();
                                        dataBank.Set(chunk);
                                        userData.Add(dataBank);
                                    }

                                    if (hr.PutUserData(userData))
                                        SuccessNotification(String.Format("The clock {0} is restored", clockDevice.ClockDeviceUid));
                                    else
                                        ErrorNotification("Cannot restore the clock.");
                                }
                            }
                        }
                    }
                    else
                        ErrorNotification("The clock is not ready. Please check.");
                }
            }
            else
                ErrorNotification("The clock is not found. Please check.");

            return RedirectToAction("Index");
        }

        #endregion
    }
}

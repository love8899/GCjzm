using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Common;
using Wfm.Core;
using Wfm.Core.Domain.Common;
using Wfm.Services.Logging;
using Wfm.Services.Common;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;


namespace Wfm.Admin.Controllers
{
    public class ShiftController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly IShiftService _shiftService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ShiftController(
            IWorkContext workContext,
            IActivityLogService activityLogService,
            IShiftService shiftService,
            IPermissionService permissionService,
            ILogger logger,
            ILocalizationService localizationService)
        {
            _workContext = workContext;
            _activityLogService = activityLogService;
            _shiftService = shiftService;
            _permissionService = permissionService;
            _logger = logger;
            _localizationService = localizationService;
        }
        #endregion

        #region GET :/Shift/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShifts))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Shift/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShifts))
                return AccessDeniedView();

            var shifts = _shiftService.GetAllShifts(showInactive: true, companyId: null);

            return Json(shifts.ToDataSourceResult(request, x => x.ToModel()));
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ShiftModel> models)
        {
            if (models != null && ModelState.IsValid) 
            {
                foreach (var model in models)
                {
                    if (_shiftService.AnyDuplicate(model.Id, model.CompanyId, model.ShiftName))
                        ModelState.AddModelError("", String.Format("Shift '{0}' exists for the company", model.ShiftName));
                    else
                    {
                        model.EnteredBy = _workContext.CurrentAccount.Id;
                        model.UpdatedOnUtc = model.CreatedOnUtc = DateTime.UtcNow;
                        var entity = model.ToEntity();
                        _shiftService.InsertShift(entity);
                        _activityLogService.InsertActivityLog("AddNewJobShift", _localizationService.GetResource("ActivityLog.AddNewJobShift"), entity.ShiftName);
                        model.Id = entity.Id;
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        #region POST:/Shift/Edit/

        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ShiftModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShifts))
                return AccessDeniedView();

            if (models != null && ModelState.IsValid)
            {
                foreach (ShiftModel shiftModel in models)
                {
                    try
                    {
                        if (_shiftService.AnyDuplicate(shiftModel.Id, shiftModel.CompanyId, shiftModel.ShiftName))
                            ModelState.AddModelError("", String.Format("Shift '{0}' exists for the company", shiftModel.ShiftName));
                        else
                        {
                            shiftModel.UpdatedOnUtc = System.DateTime.UtcNow;

                            Shift shift = _shiftService.GetShiftById(shiftModel.Id);
                            if (shift == null) return RedirectToAction("Index");
                            shift = shiftModel.ToEntity(shift);
                            _shiftService.UpdateShift(shift);
                            //activity log
                            _activityLogService.InsertActivityLog("UpdateJobShift", _localizationService.GetResource("ActivityLog.UpdateJobShift"), shift.ShiftName);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Id", _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                        _logger.Error("EditShift()", ex, userAgent: Request.UserAgent);
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        [HttpPost]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ShiftModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var shift = _shiftService.GetShiftById(model.Id);
                    if ( shift != null)
                        _shiftService.DeleteShift(shift);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

    }
}

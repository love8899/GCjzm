using System.Web.Mvc;
using System.Linq;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Services.Payroll;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Models.Payroll;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using Wfm.Core.Domain.Payroll;
using AutoMapper;
using System;
using System.Text;

namespace Wfm.Admin.Controllers
{
    public class PayGroupController : BaseAdminController
    {
        #region Fields

        private readonly IPayGroupService _payGroupService;
        private readonly IPayrollCalendarService _payCalendarService;
        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public PayGroupController(
            IPayGroupService payGroupService,
            IPayrollCalendarService payCalendarService,
            IActivityLogService activityLogService,
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _payGroupService = payGroupService;
            _payCalendarService = payCalendarService;
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }
        #endregion

        // GET: PayGroup
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            return View();
        }

        #region PayGroup

        [HttpPost]
        public ActionResult GetAllPayGroups([DataSourceRequest] DataSourceRequest request, int PayrollYear)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            var result = _payGroupService.GetAllPayGroups(PayrollYear);
            var output = Mapper.Map<List<PayGroup>, List<PayGroupModel>>(result);

            return Json(output.ToDataSourceResult(request));
        }

        public ActionResult EditPayGroup(int? paygroupId, int? payrollYear )
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            if (!payrollYear.HasValue)
                return new EmptyResult();

            var pg = paygroupId.HasValue ? _payGroupService.GetPayGroupById(paygroupId) : new PayGroup();
            if (pg == null)
            {
                pg = new PayGroup();
            }

            var model = Mapper.Map<PayGroup, PayGroupModel>(pg);
            model.Year = payrollYear.Value;

            return PartialView("_EditPayGroup", model);
        }

        [HttpPost]
        public ActionResult Create(PayGroupModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            string errorMessage = null;
            if (model != null && ModelState.IsValid)
            {
                var entity = Mapper.Map<PayGroupModel, PayGroup>(model);
                _payGroupService.InsertPayGroup(entity);
            }
            else
                errorMessage = String.Join(", ", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(x => String.Concat(key, ": ", x.ErrorMessage))));

            return Json(new { Result = ModelState.IsValid, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(PayGroupModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            string errorMessage = null;
            bool allowSave = true; 

            if (model != null && ModelState.IsValid)
            {
                // If there is at least ONE committed payroll for the year, the payfrequency do not allow any changes in the payFrequency
                var _count = _payCalendarService.GetPayrollCalendarByYearAndPayGroupIds(model.Year, model.Id.ToString(), true).Count();
                if (_count > 0)
                {
                    var _original = _payGroupService.GetPayGroupById(model.Id);
                    if (_original.PayFrequencyTypeId != model.PayFrequencyTypeId)
                    {
                        errorMessage = String.Format("There are committed payrolls for this pay group in {0}. Pay Frequency cannot be modified.", model.Year);
                        allowSave = false;
                    }
                }
                
                if (allowSave)
                {
                    var entity = Mapper.Map<PayGroupModel, PayGroup>(model);
                    _payGroupService.UpdatePayGroup(entity, null);
                }
            }
            else
                errorMessage = String.Join(", ", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(x => String.Concat(key, ": ", x.ErrorMessage))));

            return Json(new { Result = allowSave, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePayGroup(int payGroupId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            string errorMessage = null;
            var isDeleted = _payGroupService.DeletePayGroupById(payGroupId, out  errorMessage);

            return Json(new { Result = isDeleted, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult GetAllPayFrequencyTypes([DataSourceRequest] DataSourceRequest request)
        {
            var result = _payCalendarService.GetAllPayFrequencyTypesAsQueryable().ProjectTo<PayFrequencyTypeModel>();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #region Payroll Calendar

        public ActionResult GetPayrollCalendar([DataSourceRequest] DataSourceRequest request, int PayGroupId, int PayrollYear)
        {
            var result = _payCalendarService.GetPayrollCalendarAsQueryable(PayrollYear, PayGroupId).ProjectTo<PayrollCalendarModel>();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePayrollCalendar([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PayrollCalendarModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var currentData = _payCalendarService.GetPayrollCalendarByIdNoTracking(model.Id);

                    // If commit date has a value, record cannot get updated
                    if (!currentData.PayPeriodCommitDate.HasValue)
                    {
                        var entity = Mapper.Map<PayrollCalendarModel, Payroll_Calendar>(model);
                        
                        _payCalendarService.Update(entity);
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        public ActionResult PopulatePayrollCalendar(int payGroupId, int year)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            var model = new PayrollCalendarModel() { PayGroupId = payGroupId, Year = year, PayPeriodStartDate = new DateTime(year, 1, 1), PayPeriodPayDate = new DateTime(year, 1, 1) };

            return PartialView("_PopulatePayrollCalendar", model);
        }

        [HttpPost]
        public ActionResult PopulatePayrollCalendar(PayrollCalendarModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayGroups))
                return AccessDeniedView();

            ModelState.Remove("Id"); // Id is going to be zero, so remove it to avoid validation error

            StringBuilder errorMessage = new StringBuilder();
            bool result = false;

            if (model != null && ModelState.IsValid)
            {
                var _count = _payCalendarService.GetPayrollCalendarByYearAndPayGroupIds(model.Year, model.PayGroupId.ToString(), true).Count();
                if (_count > 0)
                {
                    errorMessage.AppendLine(String.Format("There are committed payrolls for this pay group in {0}. Calendar cannot be re-populated.", model.Year));
                }

                if (!(model.PayPeriodStartDate.Year == model.Year || model.PayPeriodStartDate.Year == model.Year - 1))
                {
                    errorMessage.AppendLine(String.Format("Start Date's year should be {0} or {1}",  model.Year, model.Year- 1));
                }

                if (model.PayPeriodPayDate < model.PayPeriodStartDate)
                {
                    errorMessage.AppendLine("Pay Date cannot be before Start Date.");
                }

                if (model.PayPeriodPayDate.Year != model.Year)
                {
                    errorMessage.AppendLine("Pay Date should be in the selected year.");
                }

                if (errorMessage.Length == 0)
                    result = CreateCalendarEntries(model.Year, model.PayGroupId, model.PayPeriodStartDate, model.PayPeriodPayDate);
            }

            return Json(new { Result = result, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }

        private bool CreateCalendarEntries(int year, int payGroupId, DateTime startDate, DateTime payDate)
        {
            var pg = _payGroupService.GetPayGroupById(payGroupId);
            int payFrequency = _payCalendarService.GetAllPayFrequencyTypesAsQueryable().Where(x => x.Id == pg.PayFrequencyTypeId).FirstOrDefault().Frequency;
            _payCalendarService.CreateCalendarEntries(year, payGroupId, payFrequency, startDate, payDate);

            return true;
        }

        
        #endregion
    }
}
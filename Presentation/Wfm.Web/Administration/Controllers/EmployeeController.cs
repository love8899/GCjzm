using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.Employee;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Employees;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Shared.Mapping;
using Wfm.Services.Scheduling;
using Wfm.Services.Payroll;


namespace Wfm.Admin.Controllers
{
    public class EmployeeController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateAddressService _candidateAddressService;
        private readonly ICandidateBankAccountService _candidateBankAccountService;
        private readonly IEmployeeTD1Service _employeeTD1Service;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly IPermissionService _permissionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IRepository<EmployeePayrollSetting> _employeePayrollSettings;
        private readonly IEmployeePayrollTemplateService _employeePayrollTemplateService;
        private readonly IPdfService _pdfService;
        private readonly ILogger _logger;
        private readonly ITimeoffService _timeoffService;
        private readonly Paystub_BL _paystub_BL;
        private readonly IEmployeeService _employeeService;
        private readonly ISchedulingDemandService _schedulingDemandService;
        private readonly ITaxFormService _taxFormService;
        private readonly ICandidateWSIBCommonRateService _candidateWSIBCommonRateService;
        #endregion

        #region Ctor

        public EmployeeController(
            IWorkContext workContext,
            ICandidateService candidateService,
            ICandidateAddressService candidateAddressService,
            ICandidateBankAccountService candidateBankAccountService,
            IEmployeeTD1Service employeeTD1Service,
            IPaymentHistoryService paymentHistoryService,
            IPermissionService permissionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            IRepository<EmployeePayrollSetting> employeePayrollSettings,
            IEmployeePayrollTemplateService employeePayrollTemplateService,
            IPdfService pdfService,
            ILogger logger,
            ITimeoffService timeoffService,
            IEmployeeService employeeService,
            ISchedulingDemandService schedulingDemandService,
            ITaxFormService taxFormService,
            ICandidateWSIBCommonRateService candidateWSIBCommonRateService
        )
        {
            _workContext = workContext;
            _candidateService = candidateService;
            _candidateAddressService = candidateAddressService;
            _candidateBankAccountService = candidateBankAccountService;
            _employeeTD1Service = employeeTD1Service;
            _paymentHistoryService = paymentHistoryService;
            _permissionService = permissionService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _employeePayrollSettings = employeePayrollSettings;
            _employeePayrollTemplateService = employeePayrollTemplateService;
            _pdfService = pdfService;
            _logger = logger;
            _timeoffService = timeoffService;
            _employeeService = employeeService;
            _schedulingDemandService = schedulingDemandService;
            _taxFormService = taxFormService;
            _candidateWSIBCommonRateService = candidateWSIBCommonRateService;
            _paystub_BL = new Paystub_BL(_candidateService, _employeePayrollSettings, _pdfService, _workflowMessageService, _logger);
        }

        #endregion


        #region Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            return View("List");
        }


        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var result = _candidateService.GetAllEmployeesAsQueryable(_workContext.CurrentAccount);

            return Json(result.ToDataSourceResult(request, m => m.ToEmployeeListModel()));
        }

        #endregion


        #region Details

        private EmployeeModel _GetEmployeeModelByGuid(Guid guid)
        {
            var candidate = _candidateService.GetCandidateByGuidForClient(guid);
            ViewBag.EmployeeTypeId = candidate != null ? candidate.EmployeeTypeId : (int)EmployeeTypeEnum.TEMP;

            return candidate != null ? candidate.ToEmployeeModel() : null; 
        }


        public ActionResult Details(Guid guid, string tabId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees) || guid == null)
                return AccessDeniedView();

            var model = _GetEmployeeModelByGuid(guid);
            if (model == null)
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(tabId))
                ViewBag.TabId = tabId;

            return View(model);
        }

        #endregion


        #region Basic Info

        public ActionResult _BasicInfo(Guid guid)
        {
            return PartialView(_GetEmployeeModelByGuid(guid));
        }


        [HttpPost]
        public ActionResult _UpdateBasicInfo(EmployeeModel model)
        {
            var errorMessage = String.Empty;

            if (ModelState.IsValid)
            {
                var candidate = _candidateService.GetCandidateByIdForClient(model.Id);
                if (candidate == null)
                    errorMessage = _localizationService.GetResource("Admin.Candidate.Candidate.CandidateDoesntExist");
                else
                {
                    var newCandidate = model.ToCandidateEntity();
                    if (_candidateService.IsDuplicateWhenUpdate(newCandidate))
                        errorMessage = _localizationService.GetResource("Admin.Candidate.Candidate.Added.Fail.DuplicateCandidate");
                    else
                    {
                        candidate = model.ToCandidateEntity(candidate);
                        candidate.UpdatedOnUtc = DateTime.UtcNow;
                        _candidateService.UpdateCandidate(candidate);
                    }
                }
            }
            else
                errorMessage = String.Join(", ", ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)));

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Contact Info
        public ActionResult _TabContactInfo(Guid guid)
        {
            ViewBag.CandidateGuid = guid;

            return PartialView();
        }

        private ContactInfoModel _GetContactInfoModelByGuid(Guid guid)
        {
            var candidate = _candidateService.GetCandidateByGuidForClient(guid);

            return candidate != null ? candidate.ToContactInfoModel() : null;
        }

        public ActionResult _ContactInfo(Guid guid)
        {
            return PartialView(_GetContactInfoModelByGuid(guid));
        }


        [HttpPost]
        public ActionResult _UpdateContactInfo(ContactInfoModel model)
        {
            var errorMessage = String.Empty;

            if (ModelState.IsValid)
            {
                var candidate = _candidateService.GetCandidateByGuidForClient(model.CandidateGuid);
                if (candidate == null)
                    errorMessage = _localizationService.GetResource("Admin.Candidate.Candidate.CandidateDoesntExist");
                else
                {
                    candidate.Email = model.Email;
                    candidate.HomePhone = model.HomePhone;
                    candidate.MobilePhone = model.MobilePhone;
                    candidate.UpdatedOnUtc = DateTime.UtcNow;
                    _candidateService.UpdateCandidate(candidate);
                }
            }
            else
            {
                errorMessage = String.Join(", ", ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)));
            }

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _AddressList(Guid? candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();
            //var candidate = _candidateService.GetCandidateByGuidForClient(candidateGuid);
            //if (candidate == null)
            //{
            //    ErrorNotification("The candidate does not exist!");
            //    return new EmptyResult();
            //}
            //ViewBag.CandidateId = candidate.Id;
            ViewBag.CandidateGuid = candidateGuid;

            return View();
        }

        #endregion


        #region Payroll Info

        public ActionResult _PayrollInfo(Guid guid)
        {
            var model = _GetEmployeePayrollSettingModelByCandidateGuid(guid);
            ViewBag.CandidateGuid = guid;

            return PartialView(model);
        }


        private EmployeePayrollSettingModel _GetEmployeePayrollSettingModelByCandidateGuid(Guid guid)
        {
            int employeeTypeId;
            var setting = _candidateService.GetEmployeePayrollSettingByCandidateGuid(guid, out employeeTypeId);

            if (setting != null)
            {
                return setting.ToModel(employeeTypeId);
            }
            else
            {
                var candiate = _candidateService.GetCandidateByGuidForClient(guid);
                return new EmployeePayrollSettingModel()
                {
                    EmployeeId = candiate.Id,
                    EmployeeTypeId = candiate.EmployeeTypeId.Value,
                    PayStubPassword = String.Format("{0}{1}", candiate.BirthDate.Value.ToString("MMdd"), candiate.SocialInsuranceNumber.Substring(6)),
                    VacationRate = 0.04m,
                    FranchiseId = candiate.FranchiseId
                };
            }
        }


        public ActionResult _TabPayrollInfo(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var model = _GetEmployeePayrollSettingModelByCandidateGuid(guid);
            ViewBag.CandidateGuid = guid;

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _UpdatePayrollInfo(EmployeePayrollSettingModel model)
        {
            var errorMessage = String.Empty;

            if (ModelState.IsValid)
            {
                int employeeTypeId;
                var setting = _candidateService.GetEmployeePayrollSettingByEmployeeId(model.EmployeeId, out employeeTypeId);
                if (setting == null)
                    errorMessage = "Cannot find the payroll setting";
                else
                {
                    setting = model.ToEntity(setting);
                    setting.UpdatedOnUtc = DateTime.UtcNow;
                    setting.UpdatedBy = _workContext.CurrentAccount.Id;

                    _candidateService.UpdateEmployeePayrollSetting(setting);

                    // update employee type
                    var candidate = _candidateService.GetCandidateByIdForClient(model.EmployeeId);
                    if (candidate != null)
                    {
                        candidate.EmployeeTypeId = model.EmployeeTypeId;
                        candidate.UpdatedOnUtc = DateTime.UtcNow;
                        _candidateService.UpdateCandidate(candidate);
                    }
                    // anything further to do if employee type changes ???
                }
            }
            else
                errorMessage = String.Join(", ", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(x => String.Concat(key, ": ", x.ErrorMessage))));

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult _EmployeePayrollTemplates([DataSourceRequest]DataSourceRequest request, Guid candidateGuid)
        {
            var candidate = _candidateService.GetCandidateByGuidForClient(candidateGuid);
            var templates = _employeePayrollTemplateService.PrepareEmployeePayrollTemplates(candidate.FranchiseId, candidate != null ? candidate.Id : 0);

            return Json(templates.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _UpdateEmployeePayrollTemplates([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<EmployeePayrollTemplate> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _employeePayrollTemplateService.GetEmployeePayrollTemplateById(model.Id);
                    if (entity != null)
                    {
                        entity.Hours = model.Hours;
                        entity.Rate = model.Rate;
                        _employeePayrollTemplateService.UpdateEmployeePayrollTemplate(entity);
                    }
                    else if (model.Hours > 0 && model.Rate > 0)
                        _employeePayrollTemplateService.InsertEmployeePayrollTemplate(model);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region TD1

        public ActionResult _TabTD1(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            ViewBag.CandidateGuid = guid;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _EmployeeTD1Overview([DataSourceRequest]DataSourceRequest request, Guid candidateGuid, int? year)
        {
            var candidate = _candidateService.GetCandidateByGuidForClient(candidateGuid);
            var creditAmounts = _employeeTD1Service.GetAllEmployeeTD1sByEmployeeId(candidate != null ? candidate.Id : 0, year.GetValueOrDefault(DateTime.Today.Year));

            return Json(creditAmounts.ProjectTo<EmployeeTD1OverviewModel>().ToDataSourceResult(request));
        }


        public ActionResult _TD1Details(int id, string provinceName)
        {
            ViewBag.ProvinceName = provinceName;
            var model = _employeeTD1Service.GetEmployeeTD1ById(id).ToModel();

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _UpdateTD1(EmployeeTD1Model model)
        {
            var errorMessage = String.Empty;

            if (ModelState.IsValid)
            {
                var entity = _employeeTD1Service.GetEmployeeTD1ById(model.EmployeeTD1_Id);
                model.ToEntity(entity);
                _employeeTD1Service.UpdateEmployeeTD1(entity);
            }
            else
                errorMessage = String.Join(", ", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(x => String.Concat(key, ": ", x.ErrorMessage))));

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Bank Account

        public ActionResult _TabBankAccount(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuidForClient(guid);
            ViewBag.EmployeeId = candidate != null ? candidate.Id : 0;
            ViewBag.CandidateGuid = guid;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _EmployeeBankAccounts([DataSourceRequest]DataSourceRequest request, Guid candidateGuid)
        {
            var candidate = _candidateService.GetCandidateByGuidForClient(candidateGuid);
            var accounts = _candidateBankAccountService.GetAllCandidateBankAccountsByCandidateId(candidate != null ? candidate.Id : 0).Where(x => !x.IsDeleted);

            return Json(accounts.ProjectTo<CandidateBankAccountModel>().ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _EditEmployeeBankAccount([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateBankAccountModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _candidateBankAccountService.GetCandidateBankAccountById(model.Id);
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;
                    _candidateBankAccountService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _CreateEmployeeBankAccount([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateBankAccountModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.UpdatedOnUtc = model.CreatedOnUtc = DateTime.UtcNow;
                    model.EnteredBy = _workContext.CurrentAccount.Id;
                    var entity = model.ToEntity();
                    _candidateBankAccountService.Insert(entity);
                    model.Id = entity.Id;
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        
        [HttpPost]
        public ActionResult _DeleteEmployeeBankAccount([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateBankAccountModel> models)
        {
            foreach (var model in models)
            {
                model.UpdatedOnUtc = DateTime.UtcNow;
                model.IsActive = false;
                model.IsDeleted = true;
                var entity = _candidateBankAccountService.GetCandidateBankAccountById(model.Id);
                model.ToEntity(entity);
                _candidateBankAccountService.Update(entity);
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region Payroll History

        public ActionResult _TabPayrollHistory(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            ViewBag.CandidateGuid = guid;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _EmployeePayrollHistory([DataSourceRequest] DataSourceRequest request, Guid guid, int? year)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByGuidForClient(guid);
            var result = _paymentHistoryService.GetPaymentHistoryByCandidatIdAndYear(candidate.Id, candidate.FranchiseId, year ?? DateTime.Today.Year);

            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _ShowPaymentDetails(PaymentHistory paymentHistory)
        {
            return PartialView(paymentHistory);
        }


        [HttpPost]
        public ActionResult _PaymentDetails([DataSourceRequest] DataSourceRequest request, int historyId)
        {
            var result = _paymentHistoryService.GetPaymentDetails(historyId);

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _AddPaymentNote(string note)
        {
            ViewBag.PaymentNote = note != "null" ? note : null;
            return PartialView();
        }


        [HttpPost]
        public ActionResult _UpdatePaymentNote(int historyId, string note)
        {
            var errorMessage = String.Empty;

            _paymentHistoryService.UdpatePaymentHistory(historyId, note: note ?? String.Empty);

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _PaystubPdf(int id)
        {
            // Force the pdf document to be displayed in the browser
            Response.AppendHeader("Content-Disposition", "inline; filename=paystub.pdf;");

            var paystub = _paymentHistoryService.GetPaystub(id);
            if (paystub == null)
                return Content("Cannot find the pay stub");

            return File(paystub, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }


        public ActionResult _PaystubPdfPrint(int id)
        {
            // Force the pdf document to be displayed in the browser
            Response.AppendHeader("Content-Disposition", "inline; filename=paystub.pdf;");

            var paystub = _paymentHistoryService.GetPaystub(id);
            if (paystub == null)
                return Content("Cannot find the pay stub");

            // set IsPrinted flag
            // TODO: determine if print finish???
            _paymentHistoryService.UdpatePaymentHistory(id, isPrinted: true);

            return File(paystub, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }


        [HttpPost]
        public ActionResult _SendPaystub(int historyId, int employeeId, DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            var errorMessage = String.Empty;

            var paystub = _paymentHistoryService.GetPaystub(historyId);
            if (paystub == null)
                errorMessage = "Cannot find the pay stub";

            else
            {
                errorMessage = _paystub_BL.SendPaystub(employeeId, paystub, payPeriodStart, payPeriodEnd);
                
                // set IsEmailed flag
                if (String.IsNullOrWhiteSpace(errorMessage))
                    _paymentHistoryService.UdpatePaymentHistory(historyId, isEmailed: true);
            }

            return Json(new { Result = String.IsNullOrEmpty(errorMessage), ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Entitlements
        public ActionResult _TabEntitlements(Guid? guid)
        {
            ViewBag.EmployeeId = guid;
            return PartialView();
        }
        [HttpPost]
        public ActionResult _GetEmployeeTimeoffEntitlement([DataSourceRequest] DataSourceRequest request, Guid employeeId, int? year = null)
        {
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var entities = _timeoffService.GetEntitlement(candidateId, year.GetValueOrDefault(DateTime.Today.Year));
            return Json(entities.ToDataSourceResult(request, m => m.ToModel()));
        }
        public ActionResult _UpdateEmployeeTimeoffEntitlement([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<Wfm.Shared.Models.Employees.EmployeeTimeoffBalanceModel> entitlements)
        {
            if (entitlements != null && ModelState.IsValid)
            {
                var entities = entitlements.Select(x => x.ToEntity()).ToList();
                entities = _timeoffService.UpdateEntitlements(entities).ToList();
            }
            return Json(entitlements.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult _GetEmployeeTimeoffBookHistory([DataSourceRequest] DataSourceRequest request, Guid employeeId, int? year = null)
        {
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var entities = _timeoffService.GetBookHistoryByEmployee(candidateId, year.GetValueOrDefault(DateTime.Today.Year));

            return Json(entities.ToDataSourceResult(request, m => m.ToModel()));
        }

        [HttpPost]
        public ContentResult GetHoursBetweenDates(DateTime start, DateTime end, int employeeId, int thisBookingId, bool startHalf, bool endHalf)
        {
            start = start.Date.AddHours(startHalf ? 12 : 0);
            end = end.Date.AddHours(endHalf ? 12 : 23);
            return Content(_timeoffService.GetHoursBetweenDates(employeeId, start, end, thisBookingId).ToString());
        }

        public ActionResult _BookNewTimeoffPopup(Guid employeeId, int timeoffTypeId)
        {
            Wfm.Shared.Models.Employees.EmployeeTimeOff_BL bl 
                = new Wfm.Shared.Models.Employees.EmployeeTimeOff_BL(_candidateService, _timeoffService, _employeeService, _schedulingDemandService);
            var model = bl.BookNewTimeoffPopup(employeeId, timeoffTypeId);
            return PartialView("../Account/_BookTimeOffPopup", model);
        }

        [HttpPost]
        public ActionResult _GetEmployeeScheduleForTimeoffBooking(Wfm.Shared.Models.Employees.EmployeeTimeoffBookingModel model)
        {
            Wfm.Shared.Models.Employees.EmployeeTimeOff_BL bl 
                = new Wfm.Shared.Models.Employees.EmployeeTimeOff_BL(_candidateService, _timeoffService, _employeeService, _schedulingDemandService);
            var result = bl.GetEmployeeScheduleForTimeoffBooking(model);
            return Json(new { Schedule = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _SaveTimeoffBooking(Wfm.Shared.Models.Employees.EmployeeTimeoffBookingModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    if (entity.Id > 0)
                    {
                        _timeoffService.UpdateTimeoffBooking(entity);
                    }
                    else
                    {
                        _timeoffService.BookNewtTimeoff(entity);
                    }
                    return Content("done");
                }
                catch (Exception ex)
                {
                    _logger.Error("_SaveTimeoffBooking()", ex, userAgent: Request.UserAgent);
                    return Content(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            else
            {
                model.TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == model.EmployeeTimeoffTypeId }).ToArray();
                return PartialView("../Account/_BookTimeOffPopup", model);
            }
        }

        public ActionResult _EditTimeoffPopup(int timeoffBookingId)
        {
            var model = _timeoffService.GetTimeoffBookingById(timeoffBookingId).ToEditModel();
            model.TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == model.EmployeeTimeoffTypeId }).ToArray();
            return PartialView("../Account/_BookTimeOffPopup", model);
        }
        #endregion

        #region Tax Form
        public ActionResult _TabTaxForm(Guid? guid)
        {
            ViewBag.CandidateGuid = guid;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabTaxForm(DataSourceRequest request,Guid? guid, int year)
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                _logger.Error("_TabTaxForm():Cannot load data!");
                return Json(null);
            }
            var result = _taxFormService.GetAllTaxFormsByCandidateIdAndYear(candidate.Id, year);
            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region Candidate WSIB Common Rate 
        public ActionResult _TabWCBRate(Guid? guid)
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                _logger.Error("_TabWCBRate():Cannot load data!");
                return RedirectToAction("Index");
            }
            ViewBag.CandidateGuid = guid;
            ViewBag.CandidateId = candidate.Id;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabWCBRate(DataSourceRequest request,Guid? guid)
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                _logger.Error("_TabWCBRate():Cannot load data!");
                return Json(null);
            }
            var result = _candidateWSIBCommonRateService.GetAllWSIBCommonRateByCandidateId(candidate.Id);
            return Json(result.ToDataSourceResult(request,m=>m.ToModel()));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateWCBRate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateWSIBCommonRateModel> models,Guid? guid)
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                _logger.Error("_TabWCBRate():Cannot load data!");
                return Json(null);
            }
            var results = new List<CandidateWSIBCommonRateModel>();
            if (models != null && ModelState.IsValid)
            {
                string error = string.Empty;
                var total = _candidateWSIBCommonRateService.GetAllWSIBCommonRateByCandidateId(candidate.Id).ToList();
                var added = models.Select(x => x.ToEntity()).ToList();
                total.AddRange(added);
                if (_candidateWSIBCommonRateService.ValidateWSIBCommonRates(total, out error))
                {
                    foreach (var model in models)
                    {
                        _candidateWSIBCommonRateService.Create(model.ToEntity());
                        results.Add(model);
                    }
                }
                else
                {
                    _logger.Error(error);
                    ModelState.AddModelError("Id", error);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
            

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateWCBRate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateWSIBCommonRateModel> models,Guid? guid)
        {
            var candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                _logger.Error("_TabWCBRate():Cannot load data!");
                return Json(null);
            }
            if (models != null && ModelState.IsValid)
            {
                string error = string.Empty;
                var ids = models.Select(x => x.Id);
                var total = _candidateWSIBCommonRateService.GetAllWSIBCommonRateByCandidateId(candidate.Id).Where(x=>!ids.Contains(x.Id)).ToList();
                var added = models.Select(x => x.ToEntity()).ToList();
                total.AddRange(added);
                if (_candidateWSIBCommonRateService.ValidateWSIBCommonRates(total, out error))
                {
                    foreach (var model in models)
                    {
                        _candidateWSIBCommonRateService.Update(model.ToEntity());
                    }
                }
                else
                {
                    _logger.Error(error);
                    ModelState.AddModelError("Id", error);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DestroyWCBRate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateWSIBCommonRateModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _candidateWSIBCommonRateService.Delete(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}

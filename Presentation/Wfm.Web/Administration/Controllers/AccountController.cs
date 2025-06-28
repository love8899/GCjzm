using AutoMapper.QueryableExtensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Core;
using Wfm.Shared.Models.Accounts;
using Wfm.Shared.Models.Employees;
using Wfm.Shared.Models.Scheduling;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Accounts;
using Wfm.Services.Authentication;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.Employees;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Shared.Mapping;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework.Mvc;
using Wfm.Services.Messages;
using Wfm.Services.Franchises;


namespace Wfm.Admin.Controllers
{
    public class AccountController : BaseAdminController
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly IAccessLogService _accessLogService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly ICandidateService _candidateService;
        private readonly ITimeoffService _timeoffService;
        private readonly ILogger _logger;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IFranchiseService _franchiseService;
        #endregion

        #region Ctor

        public AccountController(IAccountService accountService,
            AccountSettings accountSettings,
            IAccessLogService accessLogService,
            IWorkContext workContext,
            IAuthenticationService authenticationService,
            IPermissionService permissionService,
            ICompanyService companyService,
            ILocalizationService localizationService,
            IActivityLogService activityLogService,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            ICandidateService candidateService,
            ITimeoffService timeoffService,
            ILogger logger,
            IWorkflowMessageService workflowMessageService,
            IFranchiseService franchiseService
            )
        {
            _accountService = accountService;
            _accessLogService = accessLogService;
            _workContext = workContext;
            _authenticationService = authenticationService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _candidateService = candidateService;
            _timeoffService = timeoffService;
            _logger = logger;
            _workflowMessageService = workflowMessageService;
            _franchiseService = franchiseService;
        }

        #endregion

        #region Helper
        private AccountModel getCurrentUserAccountModel()
        {
            Account currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return null;

            Wfm.Shared.Models.Accounts.AccountModel accountModel = currentAccount.ToModel();
            accountModel.AccountRoleSystemName = currentAccount.AccountRoles.First().AccountRoleName;

            return accountModel;
        }

        #endregion


        #region GET :/Account/Sign Out

        public ActionResult SignOut()
        {
            //access log
            _accessLogService.InsertAccessLog(
                _workContext.CurrentAccount.Username,
                true,
                Request.UserHostAddress,
                Request.UserAgent,
                _localizationService.GetResource("Account.AccountLogin.Success.Logout"), _workContext.CurrentAccount.Username);

            _authenticationService.SignOut();

            //return RedirectToAction("Index", "Home");
            return RedirectToRoute("AccountLogin");
        }

        #endregion


        #region GET :/Account/Details

        public ActionResult Details()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var currentAccount =  this.getCurrentUserAccountModel();
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            return View("MainProfile", currentAccount);
        }

        #endregion


        #region POST:/Account/Edit

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(AccountModel model, bool continueEditing)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                Wfm.Shared.Models.Accounts.Account_BL bl = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                if (bl.EditProfile(model))
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Accounts.Account.Updated"));
                    return continueEditing ? RedirectToAction("Edit") : RedirectToAction("Details");
                }
                else
                    return RedirectToRoute("HomePage");
            }

            return View("MainProfile", model);
        }
        #endregion //end at edit post


        #region GET :/Account/ResetPassword
        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            Account currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            AccountResetPasswordModel_BL model_BL = new AccountResetPasswordModel_BL(_accountPasswordPolicyService, _accountService);
            var model = model_BL.GetResetPasswordModel(currentAccount);
            return View(model);
        }

        #endregion

        #region POST:/Account/ResetPassword

        [HttpPost]
        public ActionResult ResetPassword(AccountResetPasswordModel model)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                Wfm.Shared.Models.Accounts.Account_BL bl = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                string errorMessage = bl.ResetPassword(model);

                if (errorMessage == null)
                {
                    SuccessNotification(_localizationService.GetResource("Common.PasswordHasBeenChanged"));
                    return RedirectToAction("Details");
                }
                else if (!String.IsNullOrWhiteSpace(errorMessage))
                {
                    ErrorNotification(errorMessage);
                }
            }

            ViewBag.TabId = "password";

            var currentAccount = this.getCurrentUserAccountModel();
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            return View("MainProfile", currentAccount);
        }

        #endregion

        #region POST:/Account/_ChangeSecurityQuestions

        [HttpPost]
        public ActionResult _ChangeSecurityQuestions(AccountChangeSecuirtyQuestionsModel model)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                string successMessage = string.Empty;
                string errorMessage = string.Empty;
                Wfm.Shared.Models.Accounts.Account_BL bl = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                bool result = bl.ChangeSecurityQuestion(model, out errorMessage, out successMessage);

                if (result)
                    SuccessNotification(successMessage);
                else 
                    ErrorNotification(errorMessage);
            }

            ViewBag.TabId = "securityQuestions";

            var currentAccount = this.getCurrentUserAccountModel();
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            return View("MainProfile", currentAccount);
        }

        #endregion


        #region Time Off

        [HttpPost]
        public ActionResult _GetEmployeeTimeoffEntitlement([DataSourceRequest] DataSourceRequest request, int? employeeId, int? year)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var entities = employeeId.HasValue ? _timeoffService.GetEntitlement(employeeId.Value, year.GetValueOrDefault(DateTime.Today.Year)) :
                                                 Enumerable.Empty<EmployeeTimeoffBalance>();

            return Json(entities.ToDataSourceResult(request, x => x.ToModel()));
        }


        [HttpPost]
        public ActionResult _GetEmployeeTimeoffBookHistory([DataSourceRequest] DataSourceRequest request, int? employeeId, int? year)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var entities = employeeId.HasValue ? _timeoffService.GetBookHistoryByEmployee(employeeId.Value, year.GetValueOrDefault(DateTime.Today.Year)) :
                                                 Enumerable.Empty<EmployeeTimeoffBooking>();

            return Json(entities.AsQueryable().ProjectTo<EmployeeTimeoffBookingHistoryModel>().ToDataSourceResult(request));
        }


        public ActionResult _BookNewTimeoffPopup(int employeeId, int timeoffTypeId)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var candidate = _candidateService.GetCandidateByIdForClient(employeeId);
            var model = new EmployeeTimeoffBookingModel()
            {
                EmployeeIntId = employeeId,
                EmployeeName = candidate.GetFullName(),
                TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true).Where(x => x.EmployeeTypeId == candidate.EmployeeTypeId)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name, 
                        Value = x.Id.ToString(), Selected = x.Id == timeoffTypeId 
                    }).ToArray(),
                TimeOffStartDateTime = DateTime.Today,
                TimeOffEndDateTime = DateTime.Today,
            };

            return PartialView("_BookTimeoffPopup", model);
        }


        [HttpPost]
        public ContentResult GetHoursBetweenDates(DateTime start, DateTime end, int employeeId, int thisBookingId)
        {
            return Content(_timeoffService.GetHoursBetweenDates(employeeId, start, end, thisBookingId).ToString());
        }


        [HttpPost]
        public ActionResult _GetEmployeeScheduleForTimeoffBooking(EmployeeTimeoffBookingModel model)
        {
            // TODO: check employee schedule
            var schedules = Enumerable.Empty<EmployeeSchedulePreviewModel>();
            return Json(new { Schedule = schedules.ToArray() }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult _SaveTimeoffBooking(EmployeeTimeoffBookingModel model)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                
                try
                {
                    if (model.Id > 0)
                    {
                        var entity = _timeoffService.GetTimeoffBookingById(model.Id);
                        entity = model.ToEntity(entity);
                        _timeoffService.UpdateTimeoffBooking(entity);
                        _workflowMessageService.SendTimeOffRequestToManager(_workContext.CurrentAccount, entity, "updated");
                    }
                    else
                    {
                        var entity = model.ToEntity();
                        _timeoffService.BookNewtTimeoff(entity);
                        _workflowMessageService.SendTimeOffRequestToManager(_workContext.CurrentAccount, entity, "submitted");
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
                return PartialView("_BookTimeoffPopup", model);
            }
        }


        public ActionResult _EditTimeoffPopup(int timeoffBookingId)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var model = _timeoffService.GetTimeoffBookingById(timeoffBookingId).ToEditModel();
            model.TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == model.EmployeeTimeoffTypeId }).ToArray();
            
            return PartialView("_BookTimeoffPopup", model);
        }


        [HttpPost]
        public ActionResult _DeleteEmployeeTimeoffBook([DataSourceRequest] DataSourceRequest request, EmployeeTimeoffBookingHistoryModel model)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return new NullJsonResult();

            if (model != null)
            {
                var timeoff = _timeoffService.GetTimeoffBookingById(model.Id);
                if (timeoff == null)
                {
                    _logger.Error("_DeleteEmployeeTimeoffBook(): Cannot find timeoff book data");
                    return new NullJsonResult();
                }
                // approved already?
                if (model.ApprovedTimeoffInHours > 0 && model.ApprovedByAccountId > 0)
                    _workflowMessageService.SendTimeOffRequestFeedBackToAccount(_workContext.CurrentAccount, timeoff, result: "cancelled");
                else
                    _workflowMessageService.SendTimeOffRequestToManager(_workContext.CurrentAccount, timeoff, "cancelled");
                _timeoffService.DeleteTimeOff(timeoff);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult _TabTimeOffRequest()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            return PartialView();
        }


        public ActionResult _TabTimeOffHistory()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            return PartialView();
        }


        [HttpPost]
        public ActionResult _GetEmployeeTimeoffRequest([DataSourceRequest] DataSourceRequest request)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var entities = _timeoffService.GetBookHistoryByManager(_workContext.CurrentAccount.Id)
                .Where(x => x.ApprovedTimeoffInHours == 0 && !x.ApprovedByAccountId.HasValue && (!x.IsRejected.HasValue || !x.IsRejected.Value));

            return Json(entities.AsQueryable().ProjectTo<EmployeeTimeoffBookingHistoryModel>().ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _GetEmployeeTimeoffHistory([DataSourceRequest] DataSourceRequest request, int? year)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var entities = _timeoffService.GetBookHistoryByManager(_workContext.CurrentAccount.Id)
                .Where(x => x.Year == year.GetValueOrDefault(DateTime.Today.Year))
                .Where(x => (x.ApprovedTimeoffInHours > 0 && x.ApprovedByAccountId.HasValue) || (x.IsRejected.HasValue && x.IsRejected.Value));

            return Json(entities.AsQueryable().ProjectTo<EmployeeTimeoffBookingHistoryModel>().ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _ApproveTimeOffRequest(int id)
        {
            var error = string.Empty;

            if (_workContext.OriginalAccountIfImpersonate != null)
                error = "Access denied";
            else
            {
                var timeoff = _timeoffService.GetTimeoffBookingById(id);
                if (timeoff == null)
                    error = "Cannot find the timeoff request";

                timeoff.ApprovedTimeoffInHours = timeoff.BookedTimeoffInHours;
                timeoff.ApprovedByAccountId = _workContext.CurrentAccount.Id;
                timeoff.UpdatedOnUtc = DateTime.UtcNow;
                _timeoffService.UpdateTimeoffBooking(timeoff);
                _workflowMessageService.SendTimeOffRequestFeedBackToAccount(_workContext.CurrentAccount, timeoff);
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error });
        }


        public PartialViewResult _TimeOffRejectionNote()
        {
            return PartialView("_TimeOffRejectionNote");
        }


        [HttpPost]
        public ActionResult _RejectTimeOffRequest(int id, string reason)
        {
            var error = string.Empty;

            if (_workContext.OriginalAccountIfImpersonate != null)
                error = "Access denied";
            else
            {
                var timeoff = _timeoffService.GetTimeoffBookingById(id);
                if (timeoff == null)
                    error = "Cannot find the timeoff request";

                timeoff.IsRejected = true;
                timeoff.Note = reason;
                timeoff.UpdatedOnUtc = DateTime.UtcNow;
                _timeoffService.UpdateTimeoffBooking(timeoff);
                _workflowMessageService.SendTimeOffRequestFeedBackToAccount(_workContext.CurrentAccount, timeoff);
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error });
        }

        #endregion
    }
}


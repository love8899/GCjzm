using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Accounts;
using Wfm.Services.Authentication;
using Wfm.Services.Common;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Shared.Models.Accounts;
using Wfm.Web.Extensions;
using Wfm.Web.Framework.Filters;
using Wfm.Web.Models.Accounts;


namespace Wfm.Web.Controllers
{
    public class AccountController : BasePublicController
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly AccountSettings _accountSettings;
        private readonly IAccessLogService _accessLogService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISecurityQuestionService _securityQuestionService;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;

        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        public AccountController(IAccountService accountService,
            AccountSettings accountSettings,
            IAccessLogService accessLogService,
            IWorkContext workContext,
            IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            IMessageHistoryService messageHistoryService,
            IActivityLogService activityLogService,
            IWorkflowMessageService workflowMessageService,
            IGenericAttributeService genericAttributeService,
            ISecurityQuestionService securityQuestionService,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            ILogger logger,
            IWebHelper webHelper
            )
        {
            _accountService = accountService;
            _accountSettings = accountSettings;
            _accessLogService = accessLogService;
            _workContext = workContext;
            _authenticationService = authenticationService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _workflowMessageService = workflowMessageService;
            _genericAttributeService = genericAttributeService;
            _securityQuestionService = securityQuestionService;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _logger = logger;
            _webHelper = webHelper;
        }

        #endregion

        #region Helper

        #endregion


        #region Login

        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Login(string returnUrl, bool? alert = false)
        {
            if (String.IsNullOrWhiteSpace(returnUrl))
                returnUrl = Request.QueryString["returnUrl"];

            ViewBag.ReturnUrl = returnUrl;
            var model = new AccountLoginModel();
            if (alert.HasValue && alert.Value)
                ModelState.AddModelError("", "The session has expired or security tokens do not match");

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult Login(AccountLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                TimeSpan time = new TimeSpan();
                bool passwordIsExpired;
                bool showPasswordExpiryWarning;

                var loginResult = _accountService.AuthenticateUser(model.Username, model.Password, out time, out passwordIsExpired, out showPasswordExpiryWarning);
                switch (loginResult)
                {
                    case AccountLoginResults.Successful:
                        {
                            var account = _accountService.GetAccountByUsername(model.Username);
                            var delegates = _accountService.GetActiveDelegatesOf(account.Id);

                            if (delegates.Any())
                            {
                                ViewBag.CurrentAccountGuid = account.AccountGuid;
                                var delegateModel = delegates.Select(x => x.ToModel()).ToList();
                                delegateModel.Insert(0, new AccountDelegateModel()
                                {
                                    Id = -account.Id,
                                    AccountId = account.Id,
                                    DelegateAccountId = account.Id,
                                    AccountName = _accountService.GetAccountById(account.Id).Username,
                                    Remark = "Yourself",
                                });
                                return View("LoginAs", delegateModel);
                            }
                            _authenticationService.SignIn(account, false);

                            //access log
                            _accessLogService.InsertAccessLog(
                                model.Username,
                                true,
                                Request.UserHostAddress,
                                Request.UserAgent,
                                _localizationService.GetResource("Account.AccountLogin.Success.Login"), account.FullName);

                            if (passwordIsExpired)
                                return RedirectToAction("ChangePasswordIfPasswordExpired", new { guid = account.AccountGuid });

                            if (showPasswordExpiryWarning)
                                WarningNotification(String.Format(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.PasswordWillExpire"), time.Days, time.Hours, time.Minutes));

                            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);

                            // check if it's client account
                            if (account.IsClient())
                                return Redirect("~/Client/Home/News");
                            else
                                return Redirect("~/Admin/Home/News");
                        }

                    case AccountLoginResults.AccountNotExist:
                        //access log
                        _accessLogService.InsertAccessLog(
                            model.Username,
                            false,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            _localizationService.GetResource("Account.AccountLogin.WrongCredentials.AccountNotExist"), model.Username);

                        ModelState.AddModelError("", _localizationService.GetResource("Account.AccountLogin.WrongCredentials"));
                        break;

                    case AccountLoginResults.Deleted:
                        //access log
                        _accessLogService.InsertAccessLog(
                            model.Username,
                            false,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            _localizationService.GetResource("Account.AccountLogin.WrongCredentials.Deleted"), model.Username);

                        ModelState.AddModelError("", _localizationService.GetResource("Account.AccountLogin.WrongCredentials"));
                        break;

                    case AccountLoginResults.NotActive:
                        //access log
                        _accessLogService.InsertAccessLog(
                            model.Username,
                            false,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            _localizationService.GetResource("Account.AccountLogin.WrongCredentials.NotActive"), model.Username);

                        ModelState.AddModelError("", _localizationService.GetResource("Account.AccountLogin.WrongCredentials"));
                        break;

                    case AccountLoginResults.NotRegistered:
                        //access log
                        _accessLogService.InsertAccessLog(
                            model.Username,
                            false,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            _localizationService.GetResource("Account.AccountLogin.WrongCredentials.NotRegistered"), model.Username);

                        ModelState.AddModelError("", _localizationService.GetResource("Account.AccountLogin.WrongCredentials"));
                        break;

                    case AccountLoginResults.WrongPassword:
                    default:
                        //access log
                        _accessLogService.InsertAccessLog(
                            model.Username,
                            false,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            _localizationService.GetResource("Account.AccountLogin.WrongCredentials.WrongPassword"), model.Username);

                        ModelState.AddModelError("", _localizationService.GetResource("Account.AccountLogin.WrongCredentials"));
                        break;
                }
            }

            return View(model);
        }

        #endregion


        #region ChangePasswordIfPasswordExpired

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ChangePasswordIfPasswordExpired(Guid? guid)
        {
            if (!guid.HasValue)
                RedirectToAction("LogIn");

            var account = _accountService.GetAccountByGuid(guid);
            if (account == null)
            {
                _logger.Error(String.Format("ChangePasswordIfPasswordExpired(): Invalid account GUID is passed. This is a possible attack. Guid= {0}  and the IP address is {1}.", guid, _webHelper.GetCurrentIpAddress()));
                RedirectToAction("LogIn");
            }

            // ViewBag.LogInfo = _localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.PasswordExpired");
            Password_BL model_BL = new Password_BL(_accountPasswordPolicyService, _securityQuestionService);
            var model = model_BL.GetPasswordResetModel(account);
            //ViewBag.ShowOldPassword = true;
            return View("ChangePasswordIfExpired", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult ChangePasswordIfPasswordExpired(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var account = _accountService.GetAccountById(_workContext.CurrentAccount.Id); // User has already logged in

                var _request = new ChangePasswordRequest()
                {
                    UserAccount = account,
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword,
                    ConfirmNewPassword = model.ConfirmNewPassword,
                    NewPasswordFormat = _accountSettings.DefaultPasswordFormat,
                    ValidateOldPassword = true,
                    RequestEnteredBy = account.Id
                };

                var response = _accountService.ChangePassword(_request);
                if (response.IsSuccess)
                {
                    //model.SuccessfullyChanged = true;
                    model.Result = _localizationService.GetResource("Common.PasswordHasBeenChanged");
                    _activityLogService.InsertActivityLog("Account.UpdatePassword", _localizationService.GetResource("ActivityLog.UpdateAccountPassword"), account.Username);

                    SuccessNotification(_localizationService.GetResource("Common.PasswordHasBeenChanged"));
                    if (account.IsClient())
                        return Redirect("~/Client/Home/News");
                    else
                        return Redirect("~/Admin/Home/News");
                }
                else
                {
                    model.Result = response.ErrorsAsString();
                    ViewBag.LogInfo = model.Result;
                }

            }
            else
                ViewBag.LogInfo = String.Join(", ",
                    ModelState.Values.Where(E => E.Errors.Count > 0)
                    .SelectMany(E => E.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray());


            //If we got this far, something failed, redisplay form
            //ViewBag.ShowOldPassword = true;
            return View("ChangePasswordIfExpired", model);
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

            return RedirectToRoute("HomePage");
        }

        #endregion


        #region GET :/Account/PasswordRecovery

        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }

        #endregion

        #region POST:/Account/PasswordRecoverySend

        [HttpPost, ActionName("PasswordRecovery")]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var account = _accountService.GetAccountByEmail(model.Email);
                if (account != null && account.IsActive && !account.IsDeleted)
                {
                    // check if security questions are present for the account
                    if (account.SecurityQuestion1Id == null || account.SecurityQuestion2Id == null)
                    {
                        ModelState.AddModelError("", _localizationService.GetResource("Common.PasswordRecovery.SecurityQuestionsNotPresent"));
                        return View(model);
                    }


                    var passwordRecoveryToken = System.Guid.NewGuid().ToString("N") + System.Guid.NewGuid().ToString("N");

                    var tokenExpiryDate = DateTime.UtcNow.AddDays(3).ToString("u"); // Token will expire after three days

                    _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.TokenExpiryDate, tokenExpiryDate.ToString());

                    //send notification
                    _workflowMessageService.SendAccountPasswordRecoveryMessage(account, _workContext.WorkingLanguage.Id);

                    //activity log
                    _activityLogService.InsertActivityLog("RetrieveAccountPassword", _localizationService.GetResource("ActivityLog.RetrieveAccountPassword"), account, account.FullName);

                    model.Email = string.Empty;
                    account.FailedSecurityQuestionAttempts = 0;
                    _accountService.Update(account);

                }

                ModelState.AddModelError("", _localizationService.GetResource("Common.ResetPassword.EmailSent"));
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion


        #region GET :/Account/PasswordRecoveryConfirm
        private bool ValidateParametersForPasswordRecovery(string token, string email)
        {
            var account = _accountService.GetAccountByEmail(email);
            if (account == null)
                return false;

            var eDate = account.GetAttribute<string>(SystemAccountAttributeNames.TokenExpiryDate);
            if (String.IsNullOrEmpty(eDate))
                return false;

            if (DateTime.Parse(eDate) < DateTime.UtcNow)
                return false;

            var cPrt = account.GetAttribute<string>(SystemAccountAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }
        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            if (!ValidateParametersForPasswordRecovery(token, email))
                return RedirectToRoute("HomePage");
            ViewBag.LogInfo = _localizationService.GetResource("Account.PasswordRecovery.AccountResetPassword");
            var account = _accountService.GetAccountByEmail(email);
            Password_BL model_BL = new Password_BL(_accountPasswordPolicyService, _securityQuestionService);
            var model = model_BL.GetPasswordRecoveryModel(account);
            ViewBag.ShowOldPassword = false;
            return View(model);
        }

        #endregion

        #region POST:/Account/PasswordRecoveryConfirmPOST

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        //[FormValueRequired("set-password")]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            if (!ValidateParametersForPasswordRecovery(token, email))
                return RedirectToRoute("HomePage");

            //ModelState.Remove("OldPassword");
            ViewBag.ShowOldPassword = false;

            if (ModelState.IsValid)
            {
                var account = _accountService.GetAccountByEmail(email);

                if (_accountSettings.MaxFailedSecurityQuestionAttempts >= account.FailedSecurityQuestionAttempts)
                {
                    if (_accountService.ValidateSecurityQuestions(account, model.SecurityQuestion1Answer, model.SecurityQuestion2Answer))
                    {
                        var response = _accountService.ChangePassword(new ChangePasswordRequest()
                        {
                            NewPassword = model.NewPassword,
                            ConfirmNewPassword = model.ConfirmNewPassword,
                            NewPasswordFormat = _accountSettings.DefaultPasswordFormat,
                            ValidateOldPassword = false,
                            UserAccount = account,
                            RequestEnteredBy = account.Id// user is changing his/her own password
                        });

                        if (response.IsSuccess)
                        {
                            _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.PasswordRecoveryToken, "");
                            _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.TokenExpiryDate, "");

                            model.SuccessfullyChanged = true;
                            model.Result = _localizationService.GetResource("Common.PasswordHasBeenChanged");
                        }
                        else
                        {
                            model.Result = _localizationService.GetResource(response.ErrorsAsString());
                        }
                    }
                    else
                    {
                        model.Result = _localizationService.GetResource("Common.PasswordRecovery.InvalidSecurityAnswer");
                    }
                }
                else
                {
                    model.Result = _localizationService.GetResource("Common.PasswordRecovery.ReachedMaxSecurityQuestionLimit");
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delegates
        [HttpPost]
        public JsonResult SignInAs(int delegateId, Guid currentAccountGuid)
        {
            Account account;
            if (delegateId < 0)
            {
                account = _accountService.GetAccountById(-delegateId);
                _authenticationService.SignIn(account, false);

                //access log
                _accessLogService.InsertAccessLog(
                    account.Username,
                    true,
                    Request.UserHostAddress,
                    Request.UserAgent,
                    _localizationService.GetResource("Account.AccountLogin.Success.Login"), account.FullName);
            }
            else
            {
                var model = _accountService.GetDelegateById(delegateId);
                account = _accountService.GetAccountByGuid(currentAccountGuid);
                if (account == null || model.DelegateAccountId != account.Id)// check if current account is assigned delegate.
                {
                    return Json(new { url = "Account/Login" });
                }
                var impersonAccount = _accountService.GetAccountById(model.AccountId);
                _authenticationService.SignInAs(impersonAccount, account, false);

                //access log
                _accountService.AddDelegateHistory(delegateId, account.Id);
                _accessLogService.InsertAccessLog(
                    impersonAccount.Username,
                    true,
                    Request.UserHostAddress,
                    Request.UserAgent,
                    _localizationService.GetResource("Account.AccountLogin.Success.LoginAs"), impersonAccount.FullName, account.Username);
            }

            // check if it's client account
            if (account.IsClient())
                return Json(new { url = "Client/Home/News" });
            else
                return Json(new { url = "Admin/Home/News" });
        }
        #endregion
    }
}


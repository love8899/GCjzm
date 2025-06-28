using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Client.Extensions;
using Wfm.Client.Models.Accounts;
using Wfm.Shared.Models.Accounts;
using Wfm.Services.Accounts;
using Wfm.Services.Authentication;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Web.Framework.Controllers;
using System.Web;
using System.IO;

namespace Wfm.Client.Controllers
{
    public class AccountController : BaseClientController
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly IAccessLogService _accessLogService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicy;
        private readonly IFranchiseService _franchiseService;
        #endregion

        #region Ctor

        public AccountController(IAccountService accountService,
            IAccessLogService accessLogService,
            IWorkContext workContext,
            IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            IActivityLogService activityLogService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            IAccountPasswordPolicyService accountPasswordPolicy,
            IFranchiseService franchiseService
            )
        {
            _accountService = accountService;
            _accessLogService = accessLogService;
            _workContext = workContext;
            _authenticationService = authenticationService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _accountPasswordPolicy = accountPasswordPolicy;
            _franchiseService = franchiseService;
        }

        #endregion

        #region Helper

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

            Account currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            AccountModel accountModel = currentAccount.ToModel();

            if (accountModel.CompanyLocationId != 0)
            {
                var location = _companyDivisionService.GetCompanyLocationById(accountModel.CompanyLocationId);
                accountModel.CompanyLocationName = location == null ? string.Empty : location.LocationName;
            }

            if (accountModel.CompanyDepartmentId != 0)
            {
                var department = _companyDepartmentService.GetCompanyDepartmentById(accountModel.CompanyDepartmentId);
                accountModel.CompanyDepartmentName = department == null ? string.Empty : department.DepartmentName;
            }
            if (accountModel.ShiftId!=null && accountModel.ShiftId > 0)
            {
                accountModel.ShiftName = currentAccount.Shift.ShiftName;
            }

            accountModel.AccountRoleSystemName = currentAccount.AccountRoles.First().AccountRoleName;

            return View(accountModel);
        }

        #endregion


        #region Edit

        public ActionResult Edit()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            Account currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            AccountModel model = currentAccount.ToModel();

            model.AccountRoleSystemName = currentAccount.AccountRoles.First().AccountRoleName; // to pass validation

            return View(model);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(AccountModel model, bool continueEditing, FormCollection form)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                Account_BL _bl = new Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                if (_bl.EditProfile(model))
                {
                    SuccessNotification(_localizationService.GetResource("Admin.Accounts.Account.Updated"));

                    if (continueEditing)
                    {
                        SaveSelectedTabName();
                        return RedirectToAction("Edit");
                    }
                    return RedirectToAction("Details");
                }
                else
                    return RedirectToRoute("HomePage");
            }

            return View(model);
        }
        
        #endregion //end at edit post


        #region ResetPassword

        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            Account currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            AccountResetPasswordModel_BL model_BL = new AccountResetPasswordModel_BL(_accountPasswordPolicy, _accountService);

            var model = model_BL.GetResetPasswordModel(currentAccount);

            return View(model);
        }


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

            return View(model);
        }

        #endregion


        #region Delegate

        public ActionResult Delegate()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var bl = new Wfm.Client.Models.Accounts.Accounts_BL(_accountService, _workContext);
            var model = bl.GetAvailableAccounts(longName: false);

            return View("AccountDelegate", model);
        }


        [HttpPost]
        public ActionResult GetDelegates([DataSourceRequest] DataSourceRequest request)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var delegates = _accountService.GetDelegates(_workContext.CurrentAccount.Id).ToList();
            var modelList = new List<AccountDelegateModel>();
            foreach (var x in delegates)
            {
                var model = x.ToModel();
                modelList.Add(model);
            } 
            var result = new DataSourceResult()
            {
                Data = modelList,
                Total = modelList.Count
            };
            return Json(result);
        }


        public PartialViewResult _DelegatePopup(int id, string formName, 
            string viewName = "_AccountDelegatePopup", string panelName = "addDelegate")
        {
            var bl = new Wfm.Client.Models.Accounts.Accounts_BL(_accountService, _workContext);
            var model = bl.GetAccountDelegateModelForPopUp(id);

            if (!String.IsNullOrWhiteSpace(formName))
                ViewBag.FormName = formName;
            if (!String.IsNullOrWhiteSpace(panelName))
                ViewBag.PanelName = panelName;

            return PartialView(viewName, model);
        }


        [HttpGet]
        public ActionResult DelegateWindow(int id = 0)
        {
            var bl = new Wfm.Client.Models.Accounts.Accounts_BL(_accountService, _workContext);
            var model = bl.GetAccountDelegateModelForPopUp(id);

            return View(model);
        }


        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult DelegateWindow(AccountDelegateModel model, string refreshBtnId)
        {
            var result = _UpdateDelegate(model, out string errorMessage);
            if (result)
            {
                _activityLogService.InsertActivityLog("SaveAccountDelegate", "Update Account Delegate " + model.AccountId + " on " + DateTime.UtcNow);
                ViewBag.RefreshBtnId = refreshBtnId;    // indication to close window and refesh grid
            }
            else
            {
                ErrorNotification(errorMessage);
                var bl = new Wfm.Client.Models.Accounts.Accounts_BL(_accountService, _workContext);
                model.AvaliableAccounts = bl.GetAvailableAccounts();
            }

            return View(model);
        }


        private bool _UpdateDelegate(AccountDelegateModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var result = false;

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    var instance = model.ToEntity();
                    var bl = new Wfm.Client.Models.Accounts.Accounts_BL(_accountService, _workContext);
                    if (!bl.AnyDelegateConflict(instance))
                    {
                        if (instance.Id == 0)
                            _accountService.AddDelegate(instance);
                        else
                            _accountService.UpdateDelegate(instance);
                        result = true;
                    }
                    else
                        errorMessage = "There are conflicts with existing delegates. Please double check!";
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
            }

            return result;
        }


        [HttpPost]
        public ActionResult UpdateDelegate(AccountDelegateModel model, string panelName, bool returnViewOnErr = false)
        {
            var result = _UpdateDelegate(model, out string errorMessage);
            if (result)
            {
                _activityLogService.InsertActivityLog("SaveAccountDelegate", "Update Account Delegate " + model.AccountId + " on " + DateTime.UtcNow);
                return Json(new { Succeed = result, Error = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            if (!returnViewOnErr)
                return Json(new { Succeed = result, Error = errorMessage }, JsonRequestBehavior.AllowGet);
            else
            {
                ModelState.AddModelError("", errorMessage);
                var bl = new Wfm.Client.Models.Accounts.Accounts_BL(_accountService, _workContext);
                model.AvaliableAccounts = bl.GetAvailableAccounts();
                ViewBag.PanelName = panelName;

                return PartialView("_EditDelegate", model);
            }
        }


        [HttpPost]
        public JsonResult _DelegateHistory([DataSourceRequest] DataSourceRequest request, int id)
        {
            var history = _accountService.GetDelegateHistories(id).Select(m => m.ToModel());

            return Json(history.ToDataSourceResult(request));
        }

        #endregion


        #region ChangeSecurityQuestions

        [HttpGet]
        public ActionResult ChangeSecurityQuestions()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            var model = new AccountChangeSecuirtyQuestionsModel()
            {
                SecurityQuestion1Id = currentAccount.SecurityQuestion1Id,
                SecurityQuestion2Id = currentAccount.SecurityQuestion2Id
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult ChangeSecurityQuestions(AccountChangeSecuirtyQuestionsModel model)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return new EmptyResult();

            if (ModelState.IsValid)
            {
                string successMessage = string.Empty;
                string errorMessage = string.Empty;

                var bl = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                var result = bl.ChangeSecurityQuestion(model, out errorMessage, out successMessage);
                if (result)
                {
                    SuccessNotification(successMessage);
                    return RedirectToAction("Details");
                }
                else
                {
                    ErrorNotification(errorMessage);
                }
            }

            return View("ChangeSecurityQuestions", model);
        }

        #endregion

        [HttpGet]
        public ActionResult SignaturePad()
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return AccessDeniedView();

            var currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            string filePath = GetSignatureFilePathName(currentAccount);
            var model = GetSinaturePadModel(filePath);

            return View(model);
        }

        [HttpPost]
        public ActionResult SignaturePad(SignaturePadModel model)
        {
            if (_workContext.OriginalAccountIfImpersonate != null)
                return new EmptyResult();

            var base64png = model.Base64png;  //"data:image/png;base64,iVBORw0K...";

            var currentAccount = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
            if (currentAccount == null)
                return RedirectToRoute("HomePage");

            string filePath = GetSignatureFilePathName(currentAccount);

            if (!string.IsNullOrEmpty(base64png))
            {
                try
                {
                    var encodedImage = base64png.Split(',')[1];
                    var decodedImage = Convert.FromBase64String(encodedImage);
                    System.IO.File.WriteAllBytes(filePath, decodedImage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            var modelSaved = GetSinaturePadModel(filePath);

            return View("SignaturePad", modelSaved);
        }

        private string GetSignatureFilePathName(Account currentAccount)
        {
            var appPath = HttpRuntime.AppDomainAppPath;
            var folder =  Path.Combine(appPath, "Signatures");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, $"{_workContext.CurrentAccount.Id}.png");
        }

        private static SignaturePadModel GetSinaturePadModel(string filePath)
        {
            string base64png = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
            if (System.IO.File.Exists(filePath))
            {
                var decodedImage = System.IO.File.ReadAllBytes(filePath);
                base64png = "data:image/png;base64," + Convert.ToBase64String(decodedImage);
            }
            return new SignaturePadModel()
            {
                Base64png = base64png
            };
        }
    }
}


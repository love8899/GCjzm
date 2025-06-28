using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.Accounts;
using Wfm.Core.Domain.Accounts;
using Wfm.Core;
using Wfm.Services.Accounts;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Shared.Models.Accounts;
using System.Text;
using Wfm.Services.Common;
using System.Collections.Generic;
using Wfm.Services.Companies;
using Wfm.Admin.Models.Companies;
using Wfm.Services.JobOrders;


namespace Wfm.Admin.Controllers
{
    public class AccountManagerController : BaseAdminController
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IAccountService _accountService;
        private readonly IAccountRoleService _accountRoleService;
        private readonly IFranchiseService _franchiseService;
        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IJobOrderService _jobOrderService;
        #endregion

        #region Ctor

        public AccountManagerController(ILogger logger,
                                        IAccountService accountService,
                                        IAccountRoleService accountRoleService,
                                        IFranchiseService franchiseService,
                                        IWorkContext workContext,
                                        IActivityLogService activityLogService,
                                        ILocalizationService localizationService,
                                        IPermissionService permissionService,
                                        IAccountPasswordPolicyService accountPasswordPolicyService,
                                        IRecruiterCompanyService recruiterCompanyService,
                                        IJobOrderService jobOrderService
                                       )
        {
            _logger = logger;
            _accountService = accountService;
            _accountRoleService = accountRoleService;
            _franchiseService = franchiseService;
            _activityLogService = activityLogService;
            _workContext = workContext;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _recruiterCompanyService = recruiterCompanyService;
            _jobOrderService = jobOrderService;
        }

        #endregion

        #region GET :/AccountManager/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/AccountManager/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request, Guid? vendorGuId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            if (vendorGuId == null) vendorGuId = _workContext.CurrentFranchise.FranchiseGuid;
            var accounts = _accountService.GetAllAccountsAsQueryable(_workContext.CurrentAccount, true).Where(x => x.Franchise.FranchiseGuid == vendorGuId);

            // exclude Client Admin
            accounts = accounts.Where(x => !x.AccountRoles.Any(r => r.SystemName == AccountRoleSystemNames.ClientAdministrators));

            var result = accounts.ProjectTo<AccountsViewModel>();

            return Json(result.ToDataSourceResult(request));
        }

        #endregion

        #region GET :/AccountManager/Create

        public ActionResult Create(string returnPath = null, Guid? vendorGuId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            ViewBag.ReturnPath = returnPath;

            AccountFullModel accountModel = new AccountFullModel()
            {
                IsClientAccount = returnPath != null && returnPath.Contains("/Company/") ? true : false,
                CreatedOnUtc = System.DateTime.UtcNow,
                UpdatedOnUtc = System.DateTime.UtcNow
            };


            if (_workContext.CurrentAccount.IsLimitedToFranchises) 
                accountModel.FranchiseId = _workContext.CurrentAccount.FranchiseId;
            else
            {
                var franchise = _franchiseService.GetFranchiseByGuid(vendorGuId);
                if (franchise != null)
                    accountModel.FranchiseId = franchise.Id;
            }

            return View(accountModel);
        }

        #endregion

        #region POST:/AccountManager/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(AccountFullModel model, bool continueEditing, string returnPath = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.Id != model.FranchiseId)
                return AccessDeniedView();


            StringBuilder errors = new StringBuilder();
            bool valid = _accountPasswordPolicyService.ApplyPasswordPolicy(0, "Admin", model.Password, String.Empty, Core.Domain.Accounts.PasswordFormat.Clear, String.Empty, out errors);
            if (valid)
            {
                var businessLogic = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);

                Account account = model.ToEntity();
                businessLogic.prepareAccountForRegistration(account, model.AccountRoleSystemName);

                // non MSP account
                if (account.FranchiseId != _franchiseService.GetDefaultMSPId())
                    account.IsLimitedToFranchises = true;
                account.IsClientAccount = false;
                account.IsActive = true;

                string error = _accountService.RegisterAccount(account);

                if (error == null) // there was no error in registration
                {
                    var accountEntity = _accountService.GetAccountByUsername(model.Username);

                    SuccessNotification(_localizationService.GetResource("Admin.Accounts.Account.Added"));

                    //activity log
                    _activityLogService.InsertActivityLog("AddNewAccount", _localizationService.GetResource("ActivityLog.AddNewAccount"), model.Username);

                    return RedirectToAction(continueEditing ? "Edit" : "Details", new { guid = accountEntity.AccountGuid, returnPath = @returnPath });
                }
                else
                {
                    ErrorNotification(error);
                    return View(model);
                }
            }
            else
            {
                ErrorNotification(errors.ToString());
            }

            return View(model);
        }

       
        #endregion


        #region GET :/AccountManager/Edit

        public ActionResult Edit(Guid? guid, string returnPath = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            ViewBag.ReturnPath = returnPath;
            if (guid == null)
            {
                WarningNotification("The account does not exist!");
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }

            Account account = _accountService.GetAccountByGuid(guid);
            if (account == null)
                return RedirectToAction(Request.UrlReferrer.PathAndQuery);

            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.Id != account.FranchiseId)
                return AccessDeniedView();

            var result = account.ToModel();

            if (result.EmployeeId == null)  result.EmployeeId = "";
            if (result.ManagerId == "0") result.ManagerId = "";

            return View(result);
        }

        #endregion

        #region POST:/AccountManager/Edit

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(AccountModel model, bool continueEditing, string returnPath = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var businessLogic = new Wfm.Shared.Models.Accounts.Account_BL(_accountService, _franchiseService, _workContext, _localizationService, _activityLogService);
                var id = businessLogic.UpdateVendorAccountFromModel(model);

                if (id == 0) return Redirect(Request.UrlReferrer.PathAndQuery);

                if (id <= -2) return AccessDeniedView();

                if (id == -1)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Accounts.Account.Added.Failed.AccountExists"));
                    return View(model);
                }

                //activity log
                _activityLogService.InsertActivityLog("UpdateAccountProfile", _localizationService.GetResource("ActivityLog.UpdateAccountProfile"), model.Username);

                SuccessNotification(_localizationService.GetResource("Admin.Accounts.Account.Updated"));

                return RedirectToAction(continueEditing ? "Edit" : "Details", new { guid = model.AccountGuid, returnPath = @returnPath });
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                var exceps = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                _logger.Warning(string.Format("Error occurred during updating account to database: {0} - {1}", errors, exceps), userAgent: Request.UserAgent);
            }

            return View(model);
        }

        #endregion //end at edit post

        #region GET :/AccountManager/Details

        public ActionResult Details(Guid? guid, string returnPath = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            ViewBag.ReturnPath = returnPath;
            if (guid == null)
            {
                WarningNotification("The account does not exist!");
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            Account account = _accountService.GetAccountByGuid(guid);
            if (account == null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.Id != account.FranchiseId)
                return AccessDeniedView();

            return View(account.ToModel());
        }

        #endregion

        #region GET :/AccountManager/ResetPassword
        [HttpGet]
        public ActionResult ResetPassword(Guid? guid, string returnPath = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            ViewBag.ReturnPath = returnPath;
            if (guid == null)
            {
                _logger.Error("ResetPassword():Failed to read data!");
                ErrorNotification("Failed to read data!");
                return RedirectToAction("Index");
            }
            Account account = _accountService.GetAccountByGuid(guid);
            if (account == null)
            {
                _logger.Error("ResetPassword():Failed to read data!");
                ErrorNotification("Failed to read data!");
                return RedirectToAction("Index");
            }
            AccountResetPasswordModel_BL model_BL = new AccountResetPasswordModel_BL(_accountPasswordPolicyService, _accountService);
            var model = model_BL.GetResetPasswordModel(account);
            return View(model);
        }

        #endregion

        #region POST:/AccountManager/ResetPassword

        [HttpPost]
        public ActionResult ResetPassword(AccountResetPasswordModel model, string returnPath = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ErrorNotification(_localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));
                return View(model);
            }

            Account account = _accountService.GetAccountByGuid(model.AccountGuid);
            if (account == null)
            {
                ErrorNotification(_localizationService.GetResource("Account.ResetPassword.Updated.Fail"));
                return RedirectToAction("Index");  //return View(model);
            }

            if (ModelState.IsValid)
            {
                //reset password
                var result = _accountService.ChangePassword(account, model.NewPassword, _workContext.CurrentAccount.Id);
                if (!result.IsSuccess)
                {
                    ErrorNotification(_localizationService.GetResource("Account.ResetPassword.Updated.Fail"));
                    WarningNotification(result.ErrorsAsString());
                    _logger.Warning(string.Format("ResetPassword():AccountResetPasswordfailed: {0}", result.ErrorsAsString()), userAgent: Request.UserAgent);
                }
                else
                {
                    //activity log
                    _activityLogService.InsertActivityLog("ResetAccountPassword", _localizationService.GetResource("ActivityLog.ResetAccountPassword"), account.Username);
                    SuccessNotification(_localizationService.GetResource("Common.PasswordHasBeenChanged"));
                    returnPath = !String.IsNullOrEmpty(returnPath) ? returnPath : "/Admin/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/Details?guid=" + model.AccountGuid;
                    return Redirect(returnPath);
                }
            }
            else
            {
                ErrorNotification(_localizationService.GetResource("Account.ResetPassword.Updated.Fail"));

                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                var exceps = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                _logger.Warning(string.Format("ResetPassword():AccountResetPasswordModel is invalid: {0} - {1}", errors, exceps), userAgent: Request.UserAgent);
            }

            return View(model);
        }

        #endregion


        #region // JsonResult: GetCascadeAccountRoles
        [HttpGet]
        public JsonResult GetCascadeAccountRoles(int franchiseId)
        {
            IList<SelectListItem> result = new List<SelectListItem>();
            var targetFranchise = _franchiseService.GetFranchiseById(franchiseId);

            if (targetFranchise != null && (_workContext.CurrentAccount.FranchiseId == franchiseId || !_workContext.CurrentAccount.IsLimitedToFranchises))
            {
                var query = _accountRoleService.GetAllAccountRolesForDropDownList(false);
                query = query.Where(x => x.IsVendorRole == !targetFranchise.IsDefaultManagedServiceProvider);

                result = query.Select(x => new SelectListItem() { Text = x.AccountRoleName, Value = x.SystemName }).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCascadeAccounts
        private IQueryable<SelectListItem> _getAdminAccounts(int franchiseId, bool activeOnly)
        {
            var accounts = _accountService.GetAllAccountsAsQueryable(_workContext.CurrentAccount, !activeOnly, false, false)
                                          .Where(x => x.FranchiseId == franchiseId);

            // exclude Client Admin
            accounts = accounts.Where(x => !x.AccountRoles.Any(r => r.SystemName == AccountRoleSystemNames.ClientAdministrators));

            var result = accounts.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = String.Concat(x.FirstName, " ", x.LastName)
            });

            return result;
        }

        public ActionResult GetCascadeActiveAccounts(int franchiseId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            var result = _getAdminAccounts(franchiseId, true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCascadeAccounts(int franchiseId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            var result = _getAdminAccounts(franchiseId, false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _TabCompanyRecruiterList
        public ActionResult _TabCompanyRecruiterList(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();
            Account account = _accountService.GetAccountByGuid(guid);
            if (account == null)
            {
                _logger.Error("_TabCompanyRecruiterList():Failed to read data!");
                ErrorNotification("Failed to read data!");
                return RedirectToAction("Index");
            }
            ViewBag.AccountGuid = guid;
            ViewBag.AccountId = account.Id;
            ViewBag.Companies = _recruiterCompanyService.GetAllCompaniesByRecruiter(account.Id);
            ViewBag.Accounts = _accountService.GetAllRecruitersAsQueryable(account).Select(x => new SelectListItem() { Text = String.Concat(x.FirstName, " ", x.LastName), Value = x.Id.ToString() }).ToList();
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabCompanyRecruiterList(DataSourceRequest request, Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();
            Account account = _accountService.GetAccountByGuid(guid);
            if (account == null)
            {
                _logger.Error("_TabCompanyRecruiterList():Failed to read data!");
                ErrorNotification("Failed to read data!");
                return Json(null);
            }
            var result = _recruiterCompanyService.GetAllRecruiterCompaniesByRecruiterId(account.Id);
            return Json(result.ToDataSourceResult(request, x => x.ToSimpleModel()));
        }

        [HttpPost]
        public ActionResult _UpdateRecruiterCompany([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RecruiterCompanySimpleModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _recruiterCompanyService.GetRecruiterCompanyById(model.Id);
                    int prevAccountId = entity.AccountId;
                    model.ToEntity(entity);
                    entity.UpdatedOnUtc = DateTime.UtcNow;

                    _recruiterCompanyService.UpdateCompanyRecruiter(entity);
                    //all job order under company will move to new account
                    _jobOrderService.MoveAllJobOrdersToNewAccount(model.CompanyId, model.AccountId, prevAccountId);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    } // End of class
} //end namespace

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using Kendo.Mvc.UI;
using Wfm.Admin.Models.Accounts;
using Wfm.Admin.Models.Security;
using Wfm.Core;
using Wfm.Services.Accounts;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;


namespace Wfm.Admin.Controllers
{
    public partial class SecurityController : BaseAdminController
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IAccountService _accountService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public SecurityController(ILogger logger, IWorkContext workContext,
            IPermissionService permissionService,
            IAccountService accountService, ILocalizationService localizationService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._accountService = accountService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        public ActionResult AccessDenied(string pageUrl)
        {
            var currentAccount = _workContext.CurrentAccount;
            if (currentAccount == null)
            {
                _logger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl), userAgent: Request.UserAgent);
                return View();
            }

            _logger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentAccount.Email, currentAccount.Email, pageUrl), userAgent: Request.UserAgent);
            return View();
        }


        public ActionResult Permissions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var permissionRoleMapping_BL = new PermissionRoleMapping_BL(_permissionService, _accountService);
            var model = permissionRoleMapping_BL.GetPermissionRoleMapping();

            return View(model);
        }


        [HttpPost]
        public ActionResult _GetPermissionRoleMaps()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();

            var permissionRoleMapping_BL = new PermissionRoleMapping_BL(_permissionService, _accountService);
            var maps = permissionRoleMapping_BL.GetPermissionRoleMaps();
            var result = new DataSourceResult()
            {
                Data = maps,
                Total = maps.Count
            };

            return Json(result);
        }


        [HttpPost]
        public ActionResult _SavePermissionMapping(string permissionNames, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCompanies))
                return AccessDeniedView();

            var permissionRoleMapping_BL = new PermissionRoleMapping_BL(_permissionService, _accountService);
            permissionRoleMapping_BL.SavePermissionMapping(permissionNames, form);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));

            return RedirectToAction("Permissions");
        }

        #endregion
    } 
} 

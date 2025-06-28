using System;
using System.Linq;
using System.Web;
using Wfm.Core;
using Wfm.Core.Domain.Localization;
using Wfm.Core.Domain.Franchises;
using Wfm.Services.Localization;
using Wfm.Web.Framework.Localization;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Accounts;
using Wfm.Services.Authentication;
using Wfm.Services.Franchises;

namespace Wfm.Web.Framework
{
    /// <summary>
    /// Working context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Const

        private const string AccountCookieName = "Wfm.account";
        private const string OriginalAccountCookieName = "Wfm.original.account";

        #endregion
        
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWebHelper _webHelper;
        private readonly IFranchiseService _franchiseService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        private readonly IAccountService _accountService;

        private Account _cachedAccount;
        private Account _cachedOriginalAccount;
        private AccountRole _cachedAccountRole;
        private Franchise _cachedFranchise;

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            LocalizationSettings localizationSettings,
            IAuthenticationService authenticationService,
            IFranchiseService franchiseService,
            ILanguageService languageService,
            IAccountService accountService,
            IWebHelper webHelper)
        {
            this._httpContext = httpContext;
            this._localizationSettings = localizationSettings;
            this._franchiseService = franchiseService;
            this._authenticationService = authenticationService;
            this._languageService = languageService;
            this._accountService = accountService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities
        private HttpCookie GetCookie(string cookieName)
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[cookieName];
        }

        private void SetCookie<T>(T value, string cookieName)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(cookieName);
                cookie.HttpOnly = true;
                
                #if DEBUG
                  cookie.Secure = false;
                #else
                 cookie.Secure = true;
                #endif
                cookie.Value = value.ToString();
                if (value.Equals(default(T)))
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(cookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        protected virtual HttpCookie GetUserCookie()
        {
            return GetCookie(AccountCookieName);
        }

        protected virtual void SetUserCookie(Guid customerGuid)
        {
            SetCookie(customerGuid, AccountCookieName);
        }
        protected virtual HttpCookie GetOriginalUserCookie()
        {
            return GetCookie(OriginalAccountCookieName);
        }

        protected virtual void SetOriginalUserCookie(Guid customerGuid)
        {
            SetCookie(customerGuid, OriginalAccountCookieName);
        }

        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            string virtualPath = _httpContext.Request.AppRelativeCurrentExecutionFilePath;
            string applicationPath = _httpContext.Request.ApplicationPath;
            if (!virtualPath.IsLocalizedUrl(applicationPath, false))
                return null;

            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
            if (String.IsNullOrEmpty(seoCode))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.IsActive)
            {
                return language;
            }

            return null;
        }

        protected virtual Language GetLanguageFromBrowserSettings()
        {
            if (_httpContext == null ||
                _httpContext.Request == null ||
                _httpContext.Request.UserLanguages == null)
                return null;

            var userLanguage = _httpContext.Request.UserLanguages.FirstOrDefault();
            if (String.IsNullOrEmpty(userLanguage))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => userLanguage.Equals(l.LanguageCulture, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.IsActive)
            {
                return language;
            }

            return null;
        }

        #endregion
               
        #region Properties

        /// <summary>
        /// Gets or sets the current account
        /// </summary>
        public virtual Account CurrentAccount
        {
            get
            {
                if (_cachedAccount != null) 
                    return _cachedAccount;

                Account account = _authenticationService.GetAuthenticatedUser();

                if (account != null)
                {
                    _cachedAccount = account;
                }
                //else
                //{

                //    //validation
                //    if (account != null && !account.IsDeleted && account.IsActive)
                //    {
                //        SetUserCookie(account.AccountGuid);
                //        _cachedAccount = account;
                //    }

                //}

                return _cachedAccount;
            }

            set
            {
                SetUserCookie(value.AccountGuid);
                _cachedAccount = value;
            }

        }

        public virtual AccountRole CurrentAccountRole
        {
            get
            {
                if (this.CurrentAccount == null)
                    return null;

                if (_cachedAccountRole != null)
                    return _cachedAccountRole;

                var accountRole = this.CurrentAccount.AccountRoles.FirstOrDefault();

                //validation
                if (accountRole != null && accountRole.IsActive)
                    _cachedAccountRole = accountRole;

                return _cachedAccountRole;
            }
        }

        /// <summary>
        /// Gets or sets the current Franchise (logged-in manager)
        /// </summary>
        public virtual Franchise CurrentFranchise
        {
            get
            {
                if (this.CurrentAccount == null)
                    return null;

                if (_cachedFranchise != null)
                    return _cachedFranchise;

                var franchise = _franchiseService.GetFranchiseById(this.CurrentAccount.FranchiseId);

                //validation
                if (franchise != null && !franchise.IsDeleted && franchise.IsActive)
                    _cachedFranchise = franchise;

                return _cachedFranchise;
            }
        }

        public virtual Language WorkingLanguage
        {
            get
            {
                return _languageService.GetAllLanguages().FirstOrDefault();
            }
            set
            {
            }
        }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in client area
        /// </summary>
        public virtual bool IsCompanyAdministrator { get; set; }
        /// <summary>
        /// If the system is accessed as impersonate way, which account is used from the origin
        /// </summary>
        public Account OriginalAccountIfImpersonate
        {
            get
            {
                if (_cachedOriginalAccount != null)
                    return _cachedOriginalAccount;

                Account account = _authenticationService.GetOriginalUserIfImpersonate();

                if (account != null && !account.IsDeleted && account.IsActive)
                {
                    _cachedOriginalAccount = account;
                }
                else
                {
                    if (account != null && !account.IsDeleted && account.IsActive)
                    {
                        SetOriginalUserCookie(account.AccountGuid);
                        _cachedOriginalAccount = account;
                    }
                }
                return _cachedOriginalAccount;
            }
            set
            {
                SetOriginalUserCookie(value.AccountGuid);
                _cachedOriginalAccount = value;
            }
        }
        #endregion
    }
}

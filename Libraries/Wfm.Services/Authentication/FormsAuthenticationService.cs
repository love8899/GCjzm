using System;
using System.Web;
using Wfm.Services.Accounts;
using Wfm.Core.Domain.Accounts;
using System.Web.Security;

namespace Wfm.Services.Authentication
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IAccountService _accountService;
        private readonly AccountSettings _accountSettings;
        private readonly TimeSpan _expirationTimeSpan;

        private Account _cachedAccount;
        private Account _cachedOriginalAccount;

        private const string ImpersonateCookieName = "_FORM_IMPERSONATE";

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="accountService">Account service</param>
        /// <param name="accountSettings">Account settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            IAccountService accountService, AccountSettings accountSettings)
        {
            this._httpContext = httpContext;
            this._accountService = accountService;
            this._accountSettings = accountSettings;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
        }

        public virtual void SignIn(Account account, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1, /*version*/
                account.Username,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                account.Username,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            _httpContext.Response.Cookies.Add(cookie);
            _cachedAccount = account;
        }
        
        public virtual void SignInAs(Account account, Account original, bool createPersistentCookie)
        {
            SignIn(account, createPersistentCookie);

            var now = DateTime.UtcNow.ToLocalTime();
            var ticket = new FormsAuthenticationTicket(
                1, /*version*/
                original.Username,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                original.Username,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(ImpersonateCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            _httpContext.Response.Cookies.Add(cookie);
            _cachedOriginalAccount = account;
        }
        public virtual void SignOut()
        {
            _cachedAccount = null;
            _cachedOriginalAccount = null;
            // clear impersonate cookie
            var cookie = new HttpCookie(ImpersonateCookieName, string.Empty);
            cookie.Expires = DateTime.UtcNow.ToLocalTime().AddYears(-1);
            _httpContext.Response.Cookies.Add(cookie);
            FormsAuthentication.SignOut();

            //// clear authentication cookie
            //HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            //cookie1.Expires = DateTime.UtcNow.ToLocalTime().AddYears(-1);
            //_httpContext.Response.Cookies.Add(cookie1);

            //// clear session cookie (not necessary, but recommend to do it anyway)
            //HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            //cookie2.Expires = DateTime.UtcNow.ToLocalTime().AddYears(-1);
            //_httpContext.Response.Cookies.Add(cookie2);
        }

        public virtual Account GetAuthenticatedUser()
        {
            if (_cachedAccount != null) return _cachedAccount;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var account = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
            if (account != null && account.IsActive && !account.IsDeleted)
                _cachedAccount = account;
            return _cachedAccount;
        }

        public virtual Account GetOriginalUserIfImpersonate()
        {
            if (_cachedOriginalAccount != null) return _cachedOriginalAccount;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity)
                || _httpContext.Request.Cookies[ImpersonateCookieName] == null
                || string.IsNullOrEmpty(_httpContext.Request.Cookies[ImpersonateCookieName].Value))
            {
                return null;
            }

            var ticket = FormsAuthentication.Decrypt(_httpContext.Request.Cookies[ImpersonateCookieName].Value);
            var account = GetAuthenticatedCustomerFromTicket(ticket);
            if (account != null && account.IsActive && !account.IsDeleted)
                _cachedOriginalAccount = account;
            return _cachedOriginalAccount;
        }

        public virtual Account GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
                return null;
            var account = _accountService.GetAccountByUsername(usernameOrEmail);
            return account;
        }

    }
}

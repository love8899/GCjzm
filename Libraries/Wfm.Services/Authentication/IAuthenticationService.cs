using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService
    {
        void SignIn(Account account, bool createPersistentCookie);
        void SignInAs(Account account, Account original, bool createPersistentCookie);
        void SignOut();
        Account GetAuthenticatedUser();
        Account GetOriginalUserIfImpersonate();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Security;

namespace Wfm.Services.Accounts
{
    public partial interface IAccountService
    {

        #region CRUD
        void Insert(Account account);

        void Update(Account account);
        void Update(Account account, bool trackChangeTimestamp);

        void Delete(Account account);
        #endregion

        #region Account

        Account GetAccountById(int id);

        Account GetAccountByGuid(Guid? guid);
        Account[] GetMSPAccount();
        string GetAllMSPEmails();
        Account GetAccountByUsername(string username, bool showInactive = false, bool showHidden = false);

        Account GetAccountByEmail(string email, bool showInactive = false, bool showHidden = false);

       
        void InsertAccountRole(AccountRole accountRole);

        string GetFranchiseNameByAccountId(int? id);

        AccountLoginResults AuthenticateUser(string username, string password, out TimeSpan time, out bool passwordIsExpired, out bool showPasswordExpiryWarning);

        //bool IsDuplicate(AccountRegisterRequest request);
        bool IsDuplicate(Account account);

        string RegisterAccount(Account request);

        AccountRole GetAccountRoleBySystemName(string systemName);

        AccountRole GetAccountRoleById(int id);
        PasswordChangeResult ChangePassword(ChangePasswordRequest request);

        PasswordChangeResult ChangePassword(Account account, string newPassword, int enteredBy);

        bool ChangeSecurityQuestions(Account account);

        bool ValidateSecurityQuestions(Account account, string answer1, string answer2);

        #endregion

        #region List

        IList<AccountRole> GetAllAccountRoles(bool showHidden = false);

        IQueryable<Account> GetAllAccountsAsQueryable(Account account = null, bool showInactive = false, bool showClient = false, bool showHidden = false);

        IQueryable<Account> GetAllRecruitersAsQueryable(Account account, bool showInactive = false, bool showHidden = false);

        IList<Account> GetAllRecruitersByCompanyIdAndVendorId(int companyId, int vendorId, bool includeAdmin = false);

        IList<Account> GetAllRecruitersByCompanyId(int companyId, bool includeAdmin = false);

        IQueryable<Account> GetAllClientAccountForTask();

        IQueryable<Account> GetAllCompanyHrAccounts(IList<int> companyIds = null);

        IQueryable<Account> GetAllCompanyHrAccountsByCompany(int companyId);

        Account GetClientCompanyHRAccount(int companyId);

        #endregion

        #region Delegate
        IQueryable<AccountDelegate> GetDelegates(int accountId);
        AccountDelegate GetDelegateById(int id);
        void AddDelegate(AccountDelegate accountDelegate);
        void UpdateDelegate(AccountDelegate accountDelegate);
        IEnumerable<AccountDelegate> GetActiveDelegatesOf(int accountId, System.DateTime? refDate = null);
        void AddDelegateHistory(int id, int delegateAccountId);
        IEnumerable<AccountDelegateHistory> GetDelegateHistories(int delegateId);
        #endregion


        #region Hierarchy

        bool isMangaer(int accountId, bool showInactive = false);

        #endregion
    }
}

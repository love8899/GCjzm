using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Security;
using System.Web.Mvc;
using Wfm.Services.Companies;
using System.Text;
using Wfm.Data;
using Wfm.Core.Domain.Policies;
using Wfm.Core.Domain.Localization;

namespace Wfm.Services.Accounts
{
    public partial class AccountService : IAccountService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string CUSTOMERROLES_ALL_KEY = "Wfm.accountrole.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Wfm.accountrole.systemname-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERROLES_PATTERN_KEY = "Wfm.accountrole.";

        #endregion

        #region Fields

        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<AccountRole> _accountRoleRepository;
        private readonly IRepository<AccountDelegate> _accountDelegateRepository;
        private readonly IRepository<Franchise> _franchiseRepository;
        private readonly IRecruiterCompanyService _companyRecruiterService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICacheManager _cacheManager;
        private readonly AccountSettings _accountSettings;
        private readonly IWebHelper _webHelper;
        private readonly IDbContext _dbContext;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly IRepository<LocaleStringResource> _lsrRepository;
        #endregion

        #region Ctor

        public AccountService(ICacheManager cacheManager,
            IRepository<Account> accountRepository,
            IRepository<AccountRole> accountRoleRepository,
            IRepository<AccountDelegate> accountDelegateRepository,
            IRepository<Franchise> franchiseRepository,
            IEncryptionService encryptionService,
            AccountSettings accountSettings,
            IWebHelper webHelper,
            IRecruiterCompanyService companyRecruiterService,
            IDbContext dbContext,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            IRepository<LocaleStringResource> lsrRepository
            )
        {
            this._cacheManager = cacheManager;
            this._accountRepository = accountRepository;
            this._accountRoleRepository = accountRoleRepository;
            this._accountDelegateRepository = accountDelegateRepository;
            this._franchiseRepository = franchiseRepository;
            this._encryptionService = encryptionService;
            this._accountSettings = accountSettings;
            this._webHelper = webHelper;
            this._companyRecruiterService = companyRecruiterService;
            this._dbContext = dbContext;
            this._accountPasswordPolicyService = accountPasswordPolicyService;
            this._lsrRepository = lsrRepository;
        }

        #endregion

        #region CRUD

        public void Insert(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            account.CreatedOnUtc = System.DateTime.UtcNow;
            account.UpdatedOnUtc = account.CreatedOnUtc;

            //insert
            _accountRepository.Insert(account);
        }

        public void Update(Account account)
        {
            this.Update(account, true);
        }
        public void Update(Account account, bool trackChangeTimestamp)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            account.UpdatedOnUtc = System.DateTime.UtcNow;
            _accountRepository.Update(account);
        }

        public void Delete(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            account.IsActive = false;
            account.IsDeleted = true;
            account.UpdatedOnUtc = System.DateTime.UtcNow;

            _accountRepository.Update(account);
        }

        #endregion


        #region Account
        public Account GetAccountByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            var entity = _accountRepository.Table.Where(x => x.AccountGuid == guid).FirstOrDefault();
            return entity;
        }
        public Account GetAccountById(int id)
        {
            if (id == 0)
                return null;

            var query = _accountRepository.Table;

            query = from c in _accountRepository.Table
                    where c.Id == id
                    select c;

            return query.FirstOrDefault();
        }

        public Account[] GetMSPAccount()
        {
            var query = _accountRepository.TableNoTracking.Where(x => x.Franchise.IsDefaultManagedServiceProvider && x.IsActive && !x.IsDeleted && x.AccountRoles.FirstOrDefault().SystemName == "Administrators");
            return query.ToArray();
        }

        public string GetAllMSPEmails()
        {
            Account[] accounts = GetMSPAccount();
            var result = string.Join(";", accounts.Select(x => x.Email).Distinct());
            return result;
        }

        public Account GetAccountByUsername(string username, bool showInactive = false, bool showHidden = false)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = _accountRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = from a in _accountRepository.Table
                    where a.Username == username
                    select a;

            return query.FirstOrDefault();
        }

        public Account GetAccountByEmail(string email, bool showInactive = false, bool showHidden = false)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = _accountRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = from a in query
                    where a.Email == email
                    select a;

            return query.FirstOrDefault();
        }



        public virtual AccountLoginResults AuthenticateUser(string username, string password,out TimeSpan time, out bool passwordIsExpired, out bool showPasswordExpiryWarning )
        {
            time = new TimeSpan();
            passwordIsExpired = false;
            showPasswordExpiryWarning = false;

            if (String.IsNullOrWhiteSpace(username))
                return AccountLoginResults.AccountNotExist;

            Account account = GetAccountByUsername(username);

            if (account == null) return AccountLoginResults.AccountNotExist;
            if (!account.IsActive) return AccountLoginResults.NotActive;
            if (account.IsDeleted) return AccountLoginResults.Deleted;

            string pwd = _encryptionService.ConvertPassword(account.PasswordFormat, password, account.PasswordSalt);
            bool isValid = pwd == account.Password;

            if (isValid)
            {
                PasswordPolicy policy = new PasswordPolicy();
                if(account.IsClientAccount)
                    policy = _accountPasswordPolicyService.Retrieve("Client").PasswordPolicy;
                else
                    policy = _accountPasswordPolicyService.Retrieve("Admin").PasswordPolicy;

                if (policy.PasswordLifeTime != 0)
                {
                    if (account.LastPasswordUpdateDate.AddDays(policy.PasswordLifeTime) <= DateTime.UtcNow)
                    {
                        passwordIsExpired = true;
                    }
                    else if (account.LastPasswordUpdateDate.AddDays(policy.PasswordLifeTime - 10) < DateTime.UtcNow)
                    {
                        time = account.LastPasswordUpdateDate.AddDays(policy.PasswordLifeTime) - DateTime.UtcNow;
                        showPasswordExpiryWarning = true;
                    }
                }

                account.LastIpAddress = _webHelper.GetCurrentIpAddress();
                account.LastLoginDateUtc = DateTime.UtcNow;
                _accountRepository.Update(account);

                return AccountLoginResults.Successful;
            }
            else
                return AccountLoginResults.WrongPassword;
        }



        public bool IsDuplicate(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            var query = _accountRepository.TableNoTracking;

            // check duplication
            query = from a in query
                    where a.Username == account.Username ||
                    a.Email == account.Email ||
                    (a.FirstName == account.FirstName && a.LastName == account.LastName && a.WorkPhone == account.WorkPhone)
                    select a;

            // TO DO: other validation
            return query.Count() > 0;
        }


        /// <summary>
        /// Registers the account.
        /// </summary>
        /// <param name="account">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request is null</exception>
        /// <exception cref="System.ArgumentException">Current account is null</exception>
        /// <exception cref="WfmException">'Registered' role could not be loaded</exception>
        public string RegisterAccount(Account account)
        {
            if (account == null) throw new ArgumentNullException("Request is null");
            if (account.AccountRoles == null || account.AccountRoles.Count == 0) throw new WfmException("'Registered' role could not be loaded");

            var table = _lsrRepository.TableNoTracking;
            if (account.IsSystemAccount) return table.Where(x => x.ResourceName == "Common.AtLeastOneAccountPropertyIsInvalid").FirstOrDefault().ResourceValue;

            if (IsDuplicate(account)) return table.Where(x => x.ResourceName == "Admin.Accounts.Account.Added.Failed.AccountExists").FirstOrDefault().ResourceValue; 

            //at this point request is valid
            account.LastPasswordUpdateDate = DateTime.UtcNow;

            var _pwRequest = new ChangePasswordRequest()
            {
                UserAccount = account,
                NewPassword = account.Password,
                ConfirmNewPassword = account.Password,
                NewPasswordFormat = _accountSettings.DefaultPasswordFormat,
                ValidateOldPassword = false,
                RequestEnteredBy = account.EnteredBy
            };

            var result = GenerateNewPassword(_pwRequest);

            // Add new account
            if (result.IsSuccess)
            {
                this.Insert(account);

                var newAccount = GetAccountById(account.Id);
                if (newAccount == null)
                    return table.Where(x => x.ResourceName == "Admin.Accounts.Account.Added.Failed").FirstOrDefault().ResourceValue;
                else
                    return null;
            }
            else
                return String.Join(" | ", result.Errors.ToArray());
            
        }


        public string GetFranchiseNameByAccountId(int? id)
        {
            if (id == null) return null;

            Account account = _accountRepository.GetById(id);

            int FranchiseId = account.FranchiseId;

            string FranchiseName = _franchiseRepository.GetById(FranchiseId).FranchiseName;

            return FranchiseName;
        }

        public void InsertAccountRole(AccountRole accountRole)
        {
            if (accountRole == null)
                throw new ArgumentNullException("accountRole");

            _accountRoleRepository.Insert(accountRole);

            _cacheManager.RemoveByPattern(CUSTOMERROLES_PATTERN_KEY);
        }

        // <summary>
        /// Gets a account role
        /// </summary>
        /// <param name="systemName">Account role system name</param>
        /// <returns>Account role</returns>
        public AccountRole GetAccountRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(CUSTOMERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _accountRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var accountRole = query.FirstOrDefault();
                return accountRole;
            });
        }
        public AccountRole GetAccountRoleById(int id)
        {
            var query = from cr in _accountRoleRepository.Table
                        where cr.Id==id
                        select cr;
            var accountRole = query.FirstOrDefault();
            return accountRole;
        }
        public PasswordChangeResult GenerateNewPassword(ChangePasswordRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (request.UserAccount == null) throw new ArgumentNullException("request.UserAccount");

            var table = _lsrRepository.TableNoTracking;
            
            // We can't use the localization Service here because it will create a circular dependency at run time. 
            var result = new PasswordChangeResult();

            // setp 1 - Make sure password meets all the requirements of password policy
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(table.Where(x => x.ResourceName == "Admin.Accounts.Account.Fields.Password.Required").FirstOrDefault().ResourceValue);
                return result;
            }
            request.NewPassword = request.NewPassword.Trim();

            if (String.IsNullOrWhiteSpace(request.ConfirmNewPassword) || request.NewPassword != request.ConfirmNewPassword.Trim())
            {
                result.AddError(table.Where(x => x.ResourceName == "Common.EnteredPasswordsDoNotMatch").FirstOrDefault().ResourceValue);
                return result;
            }
            request.ConfirmNewPassword = request.ConfirmNewPassword.Trim();


            if (request.ValidateOldPassword)
            {
                if (String.IsNullOrWhiteSpace(request.OldPassword))
                {
                    result.AddError(table.Where(x => x.ResourceName == "Common.OldPassword.Required").FirstOrDefault().ResourceValue);
                    return result;
                }

                //password
                string oldPwd = _encryptionService.ConvertPassword(request.UserAccount.PasswordFormat, request.OldPassword, request.UserAccount.PasswordSalt);

                bool oldPasswordIsValid = oldPwd == request.UserAccount.Password;
                if (!oldPasswordIsValid)
                {
                    result.AddError(table.Where(x => x.ResourceName.Equals("Account.ResetPassword.Fields.OldPassword.Incorrect", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().ResourceValue);
                    return result;
                }
            }

            string _accountType = request.UserAccount.IsClientAccount ? "Client" : "Admin";
            StringBuilder errors;
            if (!_accountPasswordPolicyService.ApplyPasswordPolicy(request.UserAccount.Id, _accountType, request.NewPassword, request.UserAccount.Password, request.UserAccount.PasswordFormat, request.UserAccount.PasswordSalt, out errors))
            {
                result.AddError(errors.ToString()); 
                return result;
            }

            // New password shouldn't be same as the current password
            var _pwd = _encryptionService.ConvertPassword(request.UserAccount.PasswordFormat, request.NewPassword, request.UserAccount.PasswordSalt);
            if (_pwd == request.UserAccount.Password)
            {
                result.AddError(table.Where(x => x.ResourceName.Equals("Common.PasswordIsUsed", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().ResourceValue);
                return result;
            }

            // step 2 - update the properties because at this point request is valid
            request.UserAccount.PasswordSalt = _encryptionService.CreateSaltKey(5);
            request.UserAccount.PasswordFormat = request.NewPasswordFormat;
            request.UserAccount.Password = _encryptionService.ConvertPassword(request.NewPasswordFormat, request.NewPassword, request.UserAccount.PasswordSalt);
            request.UserAccount.LastPasswordUpdateDate = DateTime.UtcNow;
            request.UserAccount.FailedSecurityQuestionAttempts = 0;
            request.UserAccount.EnteredBy = request.RequestEnteredBy;

            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // We can't use the localization Service here because it will create a circular dependency at run time. That's why we have to send back the
            // resource name to the controller and let controller to localize it
            var result =  GenerateNewPassword(request);

            if (result.IsSuccess)
            {
                this.Update(request.UserAccount);
            }

            return result;
        }

        public PasswordChangeResult ChangePassword(Account account, string newPassword, int enteredBy)
        {
            var _request = new ChangePasswordRequest()
            {
                UserAccount = account,
                NewPassword = newPassword,
                ConfirmNewPassword = newPassword,
                NewPasswordFormat = _accountSettings.DefaultPasswordFormat,
                ValidateOldPassword = false,
                RequestEnteredBy = enteredBy
            };

            return this.ChangePassword(_request);
        }


        public bool ChangeSecurityQuestions(Account account)
        {
            if (account == null || String.IsNullOrWhiteSpace(account.SecurityQuestion1Answer) || String.IsNullOrWhiteSpace(account.SecurityQuestion2Answer))
                return false;

            string saltKey = _encryptionService.CreateSaltKey(5);
            account.SecurityQuestionSalt = saltKey;
            account.FailedSecurityQuestionAttempts = 0;
            account.UpdatedOnUtc = System.DateTime.UtcNow;

            string cleanedString = System.Text.RegularExpressions.Regex.Replace(account.SecurityQuestion1Answer, @"\s+", "");
            account.SecurityQuestion1Answer = cleanedString.ToLower();
            account.SecurityQuestion1Answer = _encryptionService.CreatePasswordHash(account.SecurityQuestion1Answer, saltKey, _accountSettings.HashedPasswordFormat);

            cleanedString = System.Text.RegularExpressions.Regex.Replace(account.SecurityQuestion2Answer, @"\s+", "");
            account.SecurityQuestion2Answer = cleanedString.ToLower();
            account.SecurityQuestion2Answer = _encryptionService.CreatePasswordHash(account.SecurityQuestion2Answer, saltKey, _accountSettings.HashedPasswordFormat);

            _accountRepository.Update(account);
            return true;
        }

        public bool ValidateSecurityQuestions(Account account, string answer1, string answer2)
        {
            answer1 = System.Text.RegularExpressions.Regex.Replace(answer1, @"\s+", "").ToLower();
            answer1 = _encryptionService.CreatePasswordHash(answer1, account.SecurityQuestionSalt, _accountSettings.HashedPasswordFormat);

            answer2 = System.Text.RegularExpressions.Regex.Replace(answer2, @"\s+", "").ToLower();
            answer2 = _encryptionService.CreatePasswordHash(answer2, account.SecurityQuestionSalt, _accountSettings.HashedPasswordFormat);

            if (answer1 == account.SecurityQuestion1Answer && answer2 == account.SecurityQuestion2Answer)
            {
                return true;
            }
            else
            {
                account.FailedSecurityQuestionAttempts = account.FailedSecurityQuestionAttempts + 1;
                account.UpdatedOnUtc = System.DateTime.UtcNow;
                _accountRepository.Update(account);
            }
            return false;
        }

        #endregion

        #region List

        public IList<AccountRole> GetAllAccountRoles(bool showHidden = false)
        {
            var query = _accountRoleRepository.Table;

            query = from c in query
                    where c.IsActive == true
                    orderby c.DisplayOrder, c.SystemName
                    select c;

            return query.ToList();
        }

        
        /// <summary>
        /// Gets all accounts asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public IQueryable<Account> GetAllAccountsAsQueryable(Account account = null, bool showInactive = false, bool showClient = false, bool showHidden = false)
        {
            var query = _accountRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // client
            if (!showClient)
                query = query.Where(c => c.IsClientAccount == false);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            // IsLimitedToFranchises -- it is vendor             
            if (account != null && !account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(c => c.FranchiseId == account.FranchiseId);

            query = from c in query
                    orderby c.LastName, c.FirstName
                    select c;

            return query.AsQueryable();
        }

        public IQueryable<Account> GetAllClientAccountForTask()
        {
            var query = _accountRepository.Table;

            query = query.Where(c => c.IsActive == true);

            query = query.Where(c => c.IsClientAccount == true);

            query = query.Where(c => c.IsDeleted == false);

            query = from c in query
                    orderby c.LastName, c.FirstName
                    select c;

            return query.AsQueryable();
        }

        public IQueryable<int> GetAllCompaniesByAccount(Account account)
        {
            StringBuilder query = new StringBuilder(@"Select c.Id FROM Company c");

            bool onlyMappedUsers = false;
            if (account.IsAdministrator(true) == false && account.IsPayrollAdministrator(true) == false && account.IsCompanyHrManager(true) == false && account.IsVendorAdmin(true) == false)
            {
                query.AppendLine(" inner join Account_Company_Mapping mappedUsers on c.Id = mappedUsers.CompanyId ");
                onlyMappedUsers = true;
            }
            query = query.Append(" Where c.IsDeleted = 0 ");

            // active
            query = query.Append(" and c.IsActive = 1 ");


            // vendor only
            if (account.IsLimitedToFranchises)
            {
                query = query.Append(String.Format(" and c.Id in (Select distinct CompanyId from JobOrder Where jobOrder.FranchiseId = {0}) ", account.FranchiseId));
            }

            if (account.IsClientAccount)
                query = query.Append(String.Format(" and c.Id = {0} ", account.CompanyId));

            if (onlyMappedUsers)
                query.Append(String.Format(" and mappedUsers.AccountId = {0} ", account.Id));

            query.Append(" Order by c.IsAdminCompany desc, c.UpdatedOnUtc desc");

            var data = _dbContext.SqlQuery<int>(query.ToString());
            var result = data.AsQueryable<int>();

            return result;
        }

        public IQueryable<Account> GetAllRecruitersAsQueryable(Account account, bool showInactive = false, bool showHidden = false)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            var query = _accountRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            // IsLimitedToFranchises
            if (account.IsLimitedToFranchises)
                query = query.Where(c => c.FranchiseId == account.FranchiseId);

            query = query.Where(x => x.AccountRoles.Where(y => y.SystemName == "Recruiters" || y.SystemName == "RecruiterSupervisors" ||
                                                         y.SystemName == "VendorRecruiters" || y.SystemName == "VendorRecruiterSupervisors").Count() > 0);
            

            query = from c in query
                    where c.IsClientAccount == false
                    orderby c.LastName, c.FirstName
                    select c;

            return query.AsQueryable();
        }

        public IList<Account> GetAllRecruitersByCompanyIdAndVendorId(int companyId, int vendorId, bool includeAdmin = false)
        {
            var recruiters = _companyRecruiterService.GetAllRecruitersByCompanyId(companyId)
                             .Where(x => x.Account.FranchiseId == vendorId&&x.Account.IsActive&&!x.Account.IsDeleted)
                             .Select(x => x.Account);

            if (includeAdmin)
            {
                var admins = GetAllAccountsAsQueryable().Where(x => x.FranchiseId == vendorId && x.AccountRoles.Where(y => y.SystemName == AccountRoleSystemNames.VendorAdministrators).Any());
                recruiters = recruiters.Union(admins);
            }

            return recruiters.ToList();
        }

        public IList<Account> GetAllRecruitersByCompanyId(int companyId, bool includeAdmin = false)
        {
            var recruiters = _companyRecruiterService.GetAllRecruitersByCompanyId(companyId)
                             .Select(x => x.Account);

            if (includeAdmin)
            {
                var admins = GetAllAccountsAsQueryable().Where(x => x.AccountRoles.Where(y => y.SystemName == AccountRoleSystemNames.VendorAdministrators).Any());
                recruiters = recruiters.Union(admins);
            }

            return recruiters.ToList();
        }

        public IQueryable<Account> GetAllCompanyHrAccounts(IList<int> companyIds = null)
        {
            var result = GetAllClientAccountForTask();
            if (companyIds != null && companyIds.Any())
                result = result.Where(x => companyIds.Contains(x.CompanyId));

            return result.Where(x => x.AccountRoles.Any(r => r.SystemName == AccountRoleSystemNames.CompanyHrManagers));
        }


        public IQueryable<Account> GetAllCompanyHrAccountsByCompany(int companyId)
        {
            return GetAllCompanyHrAccounts().Where(x => x.CompanyId == companyId);
        }


        public Account GetClientCompanyHRAccount(int companyId)
        {
            return GetAllCompanyHrAccountsByCompany(companyId).FirstOrDefault();
        }

        #endregion

        #region Delegate
        public IQueryable<AccountDelegate> GetDelegates(int accountId)
        {
            return _accountDelegateRepository.Table.Where(d => d.AccountId == accountId)
                .OrderByDescending(d => d.EndDate);
        }
        public AccountDelegate GetDelegateById(int id)
        {
            return _accountDelegateRepository.Table.Where(d => d.Id == id).FirstOrDefault();
        }
        public void AddDelegate(AccountDelegate accountDelegate)
        {
            accountDelegate.UpdatedOnUtc = accountDelegate.CreatedOnUtc = DateTime.UtcNow;
            _accountDelegateRepository.Insert(accountDelegate);
        }
        public void UpdateDelegate(AccountDelegate accountDelegate)
        {
            accountDelegate.UpdatedOnUtc = DateTime.UtcNow;
            _accountDelegateRepository.Update(accountDelegate);
        }
        public IEnumerable<AccountDelegate> GetActiveDelegatesOf(int accountId, DateTime? refDate = null)
        {
            var _refDate = refDate.GetValueOrDefault(DateTime.Today);
            return _accountDelegateRepository.Table.Where(d => d.DelegateAccountId == accountId && d.StartDate <= _refDate
                && d.EndDate >= _refDate)
                .Include(x => x.Account).OrderByDescending(d => d.EndDate);
        }
        public void AddDelegateHistory(int id, int delegateAccountId)
        {
            var delegateInstance = GetDelegateById(id);
            delegateInstance.Histories.Add(new AccountDelegateHistory()
            {
                AccountDelegateId = id,
                DelegateAccountId = delegateAccountId,
                LoginDateTime = DateTime.Now,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            });
            UpdateDelegate(delegateInstance);
        }
        public IEnumerable<AccountDelegateHistory> GetDelegateHistories(int delegateId)
        {
            var graph = _accountDelegateRepository.Table.Where(x => x.Id == delegateId).Include(x => x.Histories.Select(y => y.DelegateAccount)).First();
            return graph.Histories;
        }
        #endregion


        #region Hierarchy

        public bool isMangaer(int accountId, bool showInactive = false)
        {
            var employees = this.GetAllAccountsAsQueryable(account: null, showInactive: showInactive);

            return employees.Any(x => x.ManagerId == accountId);
        }

        #endregion
    }
}

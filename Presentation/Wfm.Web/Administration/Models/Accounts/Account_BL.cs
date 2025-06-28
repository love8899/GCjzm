using Wfm.Core.Domain.Accounts;
using Wfm.Services.Accounts;
using Wfm.Services.Localization;
using System;
using Wfm.Admin.Models.Companies;
using Wfm.Web.Framework;
using System.Linq;
using Wfm.Services.Logging;
using Wfm.Shared.Models.Accounts;
namespace Wfm.Admin.Models.Accounts
{
    public class Account_BL
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        #endregion

        public Account_BL(
            IAccountService accountService,
            ILocalizationService localizationService,
            IActivityLogService activityLogService
            )
        {
            _accountService = accountService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
        }

        public bool AuthenticateAdmin(AccountLoginModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool result = false;
            TimeSpan time = new TimeSpan();
            bool passwordIsExpired;
            bool showPasswordExpiryWarning;

            var loginResult = _accountService.AuthenticateUser(model.Username, model.Password, out time, out passwordIsExpired, out showPasswordExpiryWarning);

            switch (loginResult)
            {
                case AccountLoginResults.Successful:
                    var account = _accountService.GetAccountByUsername(model.Username);
                   
                    // check if it's Admin account or recruiter Supervisor
                    if (account.IsAdminOrRecruiterSupervisor())
                        result = true;
                    else
                        errorMessage = "Please login as Admin/Recruiter supervisor";
                    break;

                case AccountLoginResults.AccountNotExist:
                case AccountLoginResults.Deleted:
                case AccountLoginResults.NotActive:
                case AccountLoginResults.NotRegistered:
                case AccountLoginResults.WrongPassword:
                default:
                    errorMessage = _localizationService.GetResource("Account.AccountLogin.WrongCredentials");
                    break;
            }

            return result;
        }
        public bool EditCompanyContactProfile(CompanyContactModel model, out string error)
        {
            error = String.Empty;

            Account currentAccount = _accountService.GetAccountById(model.Id);
            if (currentAccount == null)
                return false;

            // Check for duplication
            Account tmpAccount = _accountService.GetAccountByEmail(model.Email, true, true);
            if (tmpAccount != null&&tmpAccount.Id!=currentAccount.Id)
            {
                error = _localizationService.GetResource("Admin.Accounts.Account.Added.Failed.AccountExists");
                return false;
            }

            // IMPORTANT: Client User Name = Email address
            // --------------------------------------------
            currentAccount.Username = model.Email;
            currentAccount.Email = model.Email;
            // --------------------------------------------

            // Data cleanup
            currentAccount.FirstName = model.FirstName.ToPrettyName();
            currentAccount.LastName = model.LastName.ToPrettyName();

            currentAccount.CompanyLocationId = model.CompanyLocationId;
            currentAccount.CompanyDepartmentId = model.CompanyDepartmentId;
            currentAccount.ShiftId = model.ShiftId;

            currentAccount.WorkPhone = model.WorkPhone.ExtractNumericText();
            currentAccount.HomePhone = model.HomePhone.ExtractNumericText();
            currentAccount.MobilePhone = model.MobilePhone.ExtractNumericText();

            currentAccount.IsActive = model.IsActive;
            currentAccount.IsClientAccount = model.AccountRoleSystemName != AccountRoleSystemNames.ClientAdministrators;
            currentAccount.IsLimitedToFranchises = model.AccountRoleSystemName != AccountRoleSystemNames.ClientAdministrators;

            currentAccount.LastActivityDateUtc = DateTime.UtcNow;

            // Update account
            _accountService.Update(currentAccount);

            // Update role
            if (currentAccount.AccountRoles.First().SystemName != model.AccountRoleSystemName)
            {
                var previousRole = currentAccount.AccountRoles.FirstOrDefault();
                if (previousRole != null)
                {
                    currentAccount.AccountRoles.Remove(previousRole);
                }
                var NewRole = _accountService.GetAccountRoleBySystemName(model.AccountRoleSystemName);
                currentAccount.AccountRoles.Add(NewRole);
                _accountService.Update(currentAccount);
            }

            //activity log
            _activityLogService.InsertActivityLog("UpdateContactProfile", _localizationService.GetResource("ActivityLog.UpdateContactProfile"), currentAccount.FullName);

            return true;
        }

    }
}
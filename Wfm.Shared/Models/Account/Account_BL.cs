using System;
using System.Linq;
using Wfm.Core;
using Wfm.Services.Accounts;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Web.Framework;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Franchises;

namespace Wfm.Shared.Models.Accounts
{
    public class Account_BL
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly IFranchiseService _franchiseService;
        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;

        #endregion

        public Account_BL(
            IAccountService accountService,
            IFranchiseService franchiseService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IActivityLogService activityLogService
            )
        {
            _accountService = accountService;
            _franchiseService = franchiseService;
            _workContext = workContext;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
        }

        private void CleanUpProfileData(AccountModel srcAccount, Account trgAccount)
        {
            trgAccount.Email = srcAccount.Email.Trim();
            trgAccount.FirstName = srcAccount.FirstName.ToPrettyName();
            trgAccount.LastName = srcAccount.LastName.ToPrettyName();
            trgAccount.WorkPhone = srcAccount.WorkPhone.ExtractNumericText();
            trgAccount.HomePhone = srcAccount.HomePhone.ExtractNumericText();
            trgAccount.MobilePhone = srcAccount.MobilePhone.ExtractNumericText();
        }

        private void CleanUpProfileData(Account account)
        {
            account.Email = account.Email.Trim();
            account.FirstName = account.FirstName.ToPrettyName();
            account.LastName = account.LastName.ToPrettyName();
            account.WorkPhone = account.WorkPhone.ExtractNumericText();
            account.HomePhone = account.HomePhone.ExtractNumericText();
            account.MobilePhone = account.MobilePhone.ExtractNumericText();
        }

        public bool EditProfile(AccountModel model)
        {
            Account currentAccount = _accountService.GetAccountById(model.Id);
            if (currentAccount == null || model.CompanyId != _workContext.CurrentAccount.CompanyId || currentAccount.AccountGuid != model.AccountGuid)
                return false;

            // Data cleanup
            this.CleanUpProfileData(model, currentAccount);

            currentAccount.LastActivityDateUtc = DateTime.UtcNow;
            currentAccount.ShiftId = model.ShiftId;

            // Update account
            _accountService.Update(currentAccount);

            //activity log
            _activityLogService.InsertActivityLog("UpdateAccountProfile", _localizationService.GetResource("ActivityLog.UpdateAccountProfile"), currentAccount.FullName);

            return true;
        }

        public bool EditCompanyContactProfile(AccountModel model, out string error)
        {
            error = String.Empty;

            Account currentAccount = _accountService.GetAccountById(model.Id);
            if (currentAccount == null || currentAccount.AccountGuid != model.AccountGuid)
                return false;

            // Check for duplication
            Account tmpAccount = _accountService.GetAccountByEmail(model.Email, true, true);
            if (tmpAccount != null && (model.AccountGuid != tmpAccount.AccountGuid))
            {
                error = _localizationService.GetResource("Admin.Accounts.Account.Added.Failed.AccountExists");
                return false;
            }

            // IMPORTANT: Client User Name = Email address
            // --------------------------------------------
            currentAccount.Username = model.Username;
            currentAccount.Email = model.Email;
            // --------------------------------------------

            // Data cleanup
            this.CleanUpProfileData(model, currentAccount);

            currentAccount.CompanyLocationId = model.CompanyLocationId;
            currentAccount.CompanyDepartmentId = model.CompanyDepartmentId;
            currentAccount.ShiftId = model.ShiftId;

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

        public string ResetPassword(AccountResetPasswordModel model, bool validateOldPassword = true)
        {
            Account account = _accountService.GetAccountByGuid(model.AccountGuid);

            if (account != null)
            {
                var _request = new ChangePasswordRequest()
                {
                    UserAccount = account,
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword,
                    ConfirmNewPassword = model.ConfirmNewPassword,
                    NewPasswordFormat = PasswordFormat.Hashed, //Force using hashed password
                    ValidateOldPassword = validateOldPassword,
                    RequestEnteredBy = _workContext.CurrentAccount.Id
                };

                var response = _accountService.ChangePassword(_request);
                if (!response.IsSuccess)
                {
                    return response.ErrorsAsString();  
                }

                //activity log
                if (account.IsClientAccount)
                    _activityLogService.InsertActivityLog("ResetContactPassword", _localizationService.GetResource("ActivityLog.ResetContactPassword"), account.FullName);
                else
                    _activityLogService.InsertActivityLog("ResetAccountPassword", _localizationService.GetResource("ActivityLog.ResetAccountPassword"), account.FullName);
            }
            else
                return _localizationService.GetResource("Common.AtLeastOneAccountPropertyIsInvalid");

            return null;

        }

        public bool ChangeSecurityQuestion(AccountChangeSecuirtyQuestionsModel model, out string errorMessage, out string successMessage)
        {
            bool result = false;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            if (model.SecurityQuestion1Id == null || model.SecurityQuestion1Id <= 0)
            {
                errorMessage = _localizationService.GetResource("SecurityQuestion1.Required");
                result = false;
            }
            else if (model.SecurityQuestion2Id == null || model.SecurityQuestion1Id <= 0)
            {
                errorMessage = _localizationService.GetResource("SecurityQuestion2.Required");
                result = false;
            }
            else if (string.IsNullOrEmpty(model.SecurityQuestion1Answer))
            {
                errorMessage = _localizationService.GetResource("SecurityQuestion1Answer.Required");
                result = false;
            }
            else if (string.IsNullOrEmpty(model.SecurityQuestion2Answer))
            {
                errorMessage = _localizationService.GetResource("SecurityQuestion2Answer.Required");
                result = false;
            }
            else
            {
                Account account = _accountService.GetAccountById(_workContext.CurrentAccount.Id);
                account.SecurityQuestion1Id = model.SecurityQuestion1Id;
                account.SecurityQuestion2Id = model.SecurityQuestion2Id;
                account.SecurityQuestion1Answer = model.SecurityQuestion1Answer;
                account.SecurityQuestion2Answer = model.SecurityQuestion2Answer;
                if (_accountService.ChangeSecurityQuestions(account))
                {
                    result = true;
                    successMessage = _localizationService.GetResource("Candidate.SecurityQuestionsUpdated");
                }
                else
                {
                    result = false;
                    errorMessage = "Security questions not updated.";
                }
            }
            return result;
        }

        public void prepareAccountForRegistration(Account model, string accountRoleSystemName)
        {
            // Data cleanup
            this.CleanUpProfileData(model);
            
            model.EnteredBy = _workContext.CurrentAccount.Id;
            model.IsLimitedToFranchises = false;
            model.IsSystemAccount = false;
            model.IsActive = true;
            model.IsDeleted = false;
            model.PasswordFormat = PasswordFormat.Hashed;

            var NewRole = _accountService.GetAccountRoleBySystemName(accountRoleSystemName);
            model.AccountRoles.Add(NewRole);
        }

        public int UpdateVendorAccountFromModel(AccountModel model)
        {
            Account account = _accountService.GetAccountById(model.Id);
            if (account == null || account.AccountGuid != model.AccountGuid)
                return 0;

            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.Id != account.FranchiseId)
                return -2;

            if (_workContext.CurrentAccount.IsLimitedToFranchises && !model.AccountRoleSystemName.Contains("Vendor"))
                return -3;

            // Check duplication
            Account account2 = _accountService.GetAccountByUsername(model.Username, true, true);
            Account account3 = _accountService.GetAccountByEmail(model.Email, true, true);
            if ((account2 != null && (model.Id != account2.Id)) || (account3 != null && (model.Id != account3.Id)))
                return -1;

            // Data cleanup
            this.CleanUpProfileData(model, account);

            account.IsActive = model.IsActive;
            account.Username = model.Username;
            account.LastActivityDateUtc = DateTime.UtcNow;

            // Employee number
            int empId = 0;
            if (!String.IsNullOrWhiteSpace(model.EmployeeId))
            {
                Int32.TryParse(model.EmployeeId, out empId);
            }

            if (empId > 0)
                account.EmployeeId = empId;
            else
                account.EmployeeId = null;

            // Manager 
            int managerId = 0;
            if (!String.IsNullOrWhiteSpace(model.ManagerId))
            {
                Int32.TryParse(model.ManagerId, out managerId);
            }

            //if (ManagerId > 0)
            account.ManagerId = managerId;
            //else
            //    account.ManagerId = null;


            //account.ReportTo = model.ReportTo;



            // non MSP account
            if (account.FranchiseId != _franchiseService.GetDefaultMSPId())
                account.IsLimitedToFranchises = true;

            // Update account
            _accountService.Update(account);

            if (account.AccountRoles != null && account.AccountRoles.FirstOrDefault() != null)
            {
                // Update role
                if (account.AccountRoles.First().SystemName != model.AccountRoleSystemName)
                {
                    var previousRole = account.AccountRoles.FirstOrDefault();
                    if (previousRole != null)
                    {
                        // Remove old role
                        account.AccountRoles.Remove(previousRole);
                    }
                    // Add new role
                    var newRole = _accountService.GetAccountRoleBySystemName(model.AccountRoleSystemName);
                    account.AccountRoles.Add(newRole);
                }
            }
            else
            {
                // Add new role if not exists
                var newRole = _accountService.GetAccountRoleBySystemName(model.AccountRoleSystemName);
                account.AccountRoles.Add(newRole);
            }

            //activity log
            _activityLogService.InsertActivityLog("UpdateVendorAccount", _localizationService.GetResource("ActivityLog.UpdateVendorAccount"), account.FullName);

            return account.Id;
        }
    }
}

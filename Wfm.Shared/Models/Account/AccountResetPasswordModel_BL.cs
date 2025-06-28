using Wfm.Core.Domain.Policies;
using Wfm.Services.Accounts;
using Wfm.Shared.Extensions;
using System;

namespace Wfm.Shared.Models.Accounts
{
    public class AccountResetPasswordModel_BL
    {
        #region Fields
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly IAccountService _accountService;
	    #endregion

        #region Ctor
		public AccountResetPasswordModel_BL (IAccountPasswordPolicyService accountPasswordPolicyService, IAccountService accountService)
	    {
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _accountService = accountService;
	    }
	    #endregion

        #region Method
        public AccountResetPasswordModel GetResetPasswordModel(Wfm.Core.Domain.Accounts.Account account)
        {
            if (account == null)
                return null;

            AccountResetPasswordModel model = new AccountResetPasswordModel();
            model.Id = account.Id;
            model.Username = account.Username;
            model.AccountGuid = account.AccountGuid;
            PasswordPolicy policy = new PasswordPolicy();
            if (account.IsClientAccount)
                policy = _accountPasswordPolicyService.Retrieve("Client").PasswordPolicy;
            else
                policy = _accountPasswordPolicyService.Retrieve("Admin").PasswordPolicy;

            model.PasswordPolicyModel = policy.ToModel();
            return model;
        }

        public AccountResetPasswordModel GetResetPasswordModel(Guid accountGuid)
        {
            var account = _accountService.GetAccountByGuid(accountGuid);
            return this.GetResetPasswordModel(account);
        }
        #endregion
    }
}

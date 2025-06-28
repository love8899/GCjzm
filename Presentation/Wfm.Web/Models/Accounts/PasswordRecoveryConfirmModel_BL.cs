using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Policies;
using Wfm.Services.Accounts;
using Wfm.Services.Common;
using Wfm.Shared.Extensions;
using Wfm.Shared.Models.Accounts;
using Wfm.Shared.Models.Policies;

namespace Wfm.Web.Models.Accounts
{
    public class Password_BL
    {
        #region Fields
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly ISecurityQuestionService _securityQuestionService;
        #endregion

        #region Ctor
        public Password_BL(IAccountPasswordPolicyService accountPasswordPolicyService,ISecurityQuestionService securityQuestionService)
        {
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _securityQuestionService = securityQuestionService;
        }
        #endregion

        private PasswordPolicyModel getPasswordPolicyModel(Account account)
        {
            PasswordPolicy policy = new PasswordPolicy();
            if (account.IsClientAccount)
                policy = _accountPasswordPolicyService.Retrieve("Client").PasswordPolicy;
            else
                policy = _accountPasswordPolicyService.Retrieve("Admin").PasswordPolicy;

            return policy.ToModel();
        }

        #region Methods
        public PasswordRecoveryConfirmModel GetPasswordRecoveryModel(Account account)
        {
            if (account == null)
                return null;

            PasswordRecoveryConfirmModel model = new PasswordRecoveryConfirmModel();
            model.PasswordPolicyModel = this.getPasswordPolicyModel(account);

            if (account.SecurityQuestion1Id.HasValue)
                model.SecurityQuestion1 = _securityQuestionService.GetSecurityQuestionById(Convert.ToInt32(account.SecurityQuestion1Id)).Question;

            if (account.SecurityQuestion2Id.HasValue)
                model.SecurityQuestion2 = _securityQuestionService.GetSecurityQuestionById(Convert.ToInt32(account.SecurityQuestion2Id)).Question;

            return model;
        }

        public ChangePasswordModel GetPasswordResetModel(Account account)
        {
            if (account == null)
                return null;

            ChangePasswordModel model = new ChangePasswordModel();
            model.PasswordPolicyModel = this.getPasswordPolicyModel(account);

            return model;
        }
        #endregion
    }
}
using FluentValidation;
using Wfm.Admin.Models.Accounts;

namespace Wfm.Admin.Validators.Accounts
{
    public class AccountPasswordPolicyValidator : AbstractValidator<AccountPasswordPolicyModel>
    {
        public AccountPasswordPolicyValidator()
        {
            RuleFor(x => x.AccountType).NotEmpty();
            RuleFor(x => x.PasswordPolicyId).NotEmpty();
        }
    }
}
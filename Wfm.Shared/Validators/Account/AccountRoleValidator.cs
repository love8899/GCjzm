using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Accounts;

namespace Wfm.Shared.Validators.Accounts
{
    public class AccountRoleValidator : AbstractValidator<AccountRoleModel>
    {
        public AccountRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Accounts.AccountRoles.Fields.Name.Required"));
        }
    }
}
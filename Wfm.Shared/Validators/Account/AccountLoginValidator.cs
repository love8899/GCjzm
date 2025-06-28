using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Accounts;

namespace Wfm.Shared.Validators.Accounts
{
    public class AccountLoginValidator : AbstractValidator<AccountLoginModel>
    {
        public AccountLoginValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithMessage(localizationService.GetResource("Account.AccountLogin.Fields.Username.Required"));

            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage(localizationService.GetResource("Account.AccountLogin.Fields.Password.Required"));
        }
    }
}
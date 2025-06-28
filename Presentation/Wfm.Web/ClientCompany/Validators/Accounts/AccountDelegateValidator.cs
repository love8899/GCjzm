using System;
using FluentValidation;
using Wfm.Client.Models.Accounts;
using Wfm.Services.Localization;

namespace Wfm.Client.Validators.Accounts
{
    public class AccountDelegateValidator : AbstractValidator<AccountDelegateModel>
    {
        public AccountDelegateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DelegateAccountId).Must(d => d > 0).WithMessage(localizationService.GetResource("Admin.Accounts.Account.Delegate.IsRequired"));
            RuleFor(x => x.StartDate).Must(d => d != default(DateTime)).WithMessage(localizationService.GetResource("Admin.Accounts.Account.StartDate.IsRequired"));
            RuleFor(x => x.EndDate).Must(d => d != default(DateTime)).WithMessage(localizationService.GetResource("Admin.Accounts.Account.EndDate.IsRequired"));
        }
    }
}

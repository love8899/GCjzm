using FluentValidation;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Accounts;

namespace Wfm.Shared.Validators.Accounts
{
    public class AccountValidator : AbstractValidator<AccountModel>
    {
        public AccountValidator(ILocalizationService localizationService, AccountSettings customerSettings)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.Username.Required"));
            RuleFor(x => x.Username).Length(customerSettings.UsernameMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Admin.Accounts.Account.Fields.Username.LengthValidation"), customerSettings.UsernameMinLength));

            RuleFor(x => x.FranchiseId)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.FranchiseId.IsRequired"));

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .Unless(x=>x.IsClientAccount == false)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.AccountRoleSystemName)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.AccountRoleSystemName.Required"));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.FirstName.IsRequired"));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.WorkPhone)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.WorkPhone.Required"));
            

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));

        }
    }
}
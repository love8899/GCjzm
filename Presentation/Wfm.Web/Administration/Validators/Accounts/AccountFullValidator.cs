using FluentValidation;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Accounts;

namespace Wfm.Admin.Validators.Accounts
{
    public class AccountFullValidator : AbstractValidator<AccountFullModel>
    {
        public AccountFullValidator(ILocalizationService localizationService, AccountSettings accountSettings)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.Username.Required"));
            RuleFor(x => x.Username).Length(accountSettings.UsernameMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Admin.Accounts.Account.Fields.Username.LengthValidation"), accountSettings.UsernameMinLength));

            RuleFor(x => x.FranchiseId)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.FranchiseId.IsRequired"));

            RuleFor(x => x.CompanyId)
                .NotEmpty().When(x => x.IsClientAccount).WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.AccountRoleSystemName)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.AccountRoleSystemName.Required"));

            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Admin.Accounts.Account.Fields.Password.Required"));
            //RuleFor(x => x.Password).Length(accountSettings.PasswordMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Common.Password.LengthValidation"), accountSettings.PasswordMinLength));
            RuleFor(x => x.RePassword).NotEmpty().WithMessage(localizationService.GetResource("Common.ConfirmNewPassword"));
            RuleFor(x => x.RePassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));

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
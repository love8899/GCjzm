using FluentValidation;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Localization;
using Wfm.Web.Models.Accounts;
using System.Linq;

namespace Wfm.Web.Validators.Accounts
{
    public class PasswordRecoveryConfirmValidator : AbstractValidator<PasswordRecoveryConfirmModel>
    {
        public PasswordRecoveryConfirmValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.OldPassword).NotEmpty().WithMessage(localizationService.GetResource("Common.OldPassword.Required"));
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(localizationService.GetResource("Common.NewPassword.Required"));

            When(x => x.NewPassword != null, () =>
            {
                RuleFor(x => x.NewPassword).Must((e, pass) => { return pass.Length >= e.PasswordPolicyModel.MinLength; }).WithMessage(localizationService.GetResource("Common.PasswordPolicy.MinLength"), x => x.PasswordPolicyModel.MinLength);
                RuleFor(x => x.NewPassword).Must((e, pass) => { return pass.Length <= e.PasswordPolicyModel.MaxLength; }).WithMessage(localizationService.GetResource("Common.PasswordPolicy.MaxLength"), x => x.PasswordPolicyModel.MaxLength);
                RuleFor(x => x.NewPassword)
                    .Must((e, pass) =>
                    {
                        if (e.PasswordPolicyModel.RequireUpperCase)
                            return pass.Any(x => char.IsUpper(x));
                        else
                            return true;
                    })
                    .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireUpperCase"));
                RuleFor(x => x.NewPassword)
                    .Must((e, pass) =>
                    {
                        if (e.PasswordPolicyModel.RequireLowerCase)
                            return pass.Any(x => char.IsLower(x));
                        else
                            return true;
                    })
                    .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireLowerCase"));
                RuleFor(x => x.NewPassword)
                    .Must((e, pass) =>
                    {
                        if (e.PasswordPolicyModel.RequireNumber)
                            return pass.Any(x => char.IsNumber(x));
                        else
                            return true;
                    })
                    .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireNumber"));
                RuleFor(x => x.NewPassword)
                    .Must((e, pass) =>
                    {
                        if (e.PasswordPolicyModel.RequireSymbol)
                            return pass.Any(x => char.IsSymbol(x));
                        else
                            return true;
                    })
                    .WithMessage(localizationService.GetResource("Common.PasswordPolicy.RequireSymbol"));
            });

            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(localizationService.GetResource("Common.ConfirmNewPassword.Required"));
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));

            RuleFor(x => x.SecurityQuestion1Answer).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion1Answer.Required"));
            RuleFor(x => x.SecurityQuestion2Answer).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion2Answer.Required"));
        }
    }
}
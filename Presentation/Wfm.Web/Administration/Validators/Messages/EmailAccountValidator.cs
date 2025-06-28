using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Messages;

namespace Wfm.Admin.Validators.Messages
{
    public class EmailAccountValidator : AbstractValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.EmailAccount.Fields.Email.Required"));

            RuleFor(x => x.Host)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Configuration.EmailAccount.Fields.Host.Required"));
        }
    }
}
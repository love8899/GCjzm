using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Localization;

namespace Wfm.Shared.Validators.Localization
{
    public class LocaleStringResourceValidtor : AbstractValidator<LocaleStringResourceModel>
    {
        public LocaleStringResourceValidtor(ILocalizationService localizationService)
        {
            RuleFor(x => x.LanguageId)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.LanguageId.Required"));

            RuleFor(x => x.ResourceName)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.ResourceName.Required"));

            RuleFor(x => x.ResourceValue)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.ResourceValue.Required"));

        }
    }
}
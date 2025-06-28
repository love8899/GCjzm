using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Settings;

namespace Wfm.Admin.Validators.Settings
{
    public class SettingValidator : AbstractValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                 .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Fields.Name.Required"));

            RuleFor(x => x.Value)
                .NotNull()
                 .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Fields.Value.Required"));

        }
    }
}
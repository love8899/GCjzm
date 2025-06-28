using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Franchises;


namespace Wfm.Admin.Validators.Franchise
{
    public class FranchiseSettingValidator : AbstractValidator<FranchiseSettingModel>
    {
        public FranchiseSettingValidator(ILocalizationService localizationService)
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
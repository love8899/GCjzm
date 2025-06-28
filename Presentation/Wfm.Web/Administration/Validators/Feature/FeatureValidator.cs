using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Features;


namespace Wfm.Admin.Validators.Feature
{
    public class FeatureValidator : AbstractValidator<FeatureModel>
    {
        public FeatureValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Area)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Features.Feature.Fields.Area.Required"));

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Common.Fields.Code.Required"));

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Common.Fields.Name.IsRequired"));
        }
    }
}

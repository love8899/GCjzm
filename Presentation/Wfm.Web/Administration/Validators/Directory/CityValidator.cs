using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Directory;

namespace Wfm.Admin.Validators.Directory
{
    public class CityValidator : AbstractValidator<CityModel>
    {
        public CityValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CountryId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.CountryIsRequired"));

            RuleFor(x => x.StateProvinceId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.StateProvinceIsRequired"));

            RuleFor(x => x.CityName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.CityIsRequired"));
        }
    }
}

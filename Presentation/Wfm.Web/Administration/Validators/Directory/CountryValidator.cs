using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Directory;

namespace Wfm.Admin.Validators.Directory
{
    public class CountryValidator : AbstractValidator<CountryModel>
    {
        public CountryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CountryName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.CountryIsRequired"));

            //RuleFor(x => x.TwoLetterIsoCode)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.Country.Fields.TwoLetterIsoCode.Required"));
            //RuleFor(x => x.TwoLetterIsoCode)
            //    .Length(2)
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.Country.Fields.TwoLetterIsoCode.Length"));

            //RuleFor(x => x.ThreeLetterIsoCode)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.Country.Fields.ThreeLetterIsoCode.Required"));
            //RuleFor(x => x.ThreeLetterIsoCode)
            //    .Length(3)
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.Country.Fields.ThreeLetterIsoCode.Length"));
        }
    }
}
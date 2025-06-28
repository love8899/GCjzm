using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Common
{
    public class AddressModelValidator<T> : AbstractValidator<T> where T : AddressModel
    {
        public AddressModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddressLine1)
             .NotEmpty()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateAddress.Fields.AddressLine1.Required"));

            RuleFor(x => x.CountryId)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Common.CountryIsRequired"));

            RuleFor(x => x.StateProvinceId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage(localizationService.GetResource("Common.StateProvinceIsRequired"));

            RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Common.CityIsRequired"));

            RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Common.PostalCodeIsRequired"));
        }
    }
}

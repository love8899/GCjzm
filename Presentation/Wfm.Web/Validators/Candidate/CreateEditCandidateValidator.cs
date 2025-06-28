using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Web.Models.Candidate;

namespace Wfm.Web.Validators.Candidate
{
    public class CreateEditCandidateValidator : AbstractValidator<CreateEditCandidateModel>
    {
        public CreateEditCandidateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddressLine1).NotEmpty().WithMessage(localizationService.GetResource("Web.Common.Address.Fields.AddressLine1.Required"));
            RuleFor(x => x.CountryId).NotNull().WithMessage(localizationService.GetResource("Common.CountryIsRequired"));
            RuleFor(x => x.StateProvinceId).NotNull().WithMessage(localizationService.GetResource("Common.StateProvinceIsRequired"));
            RuleFor(x => x.CityId).NotNull().WithMessage(localizationService.GetResource("Common.CityIsRequired"));
            RuleFor(x => x.PostalCode).NotNull().WithMessage(localizationService.GetResource("Common.PostalCodeIsRequired"));


            RuleFor(x => x.KeySkill1).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.KeySkill.Required"));

            RuleFor(x => x.YearsOfExperience1).NotNull().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.YearsOfExperience.Required"));
                                              //.ExclusiveBetween(0, 99).WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.YearsOfExperience.GreaterThanZero"));

            RuleFor(x => x.LastUsedDate1).NotNull().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.LastUsedDate.Required"));

        }
    }
}
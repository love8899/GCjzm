using FluentValidation;
using Wfm.Admin.Models.Candidate;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Candidate
{
    public class CreateEditCandidateValidator : AbstractValidator<CreateEditCandidateModel>
    {
        public CreateEditCandidateValidator(ILocalizationService localizationService)
        {
            Include(new CandidateValidator<CreateEditCandidateModel>(localizationService)); // this applies all the rules that are defined in CandidateValidator

            ////

            RuleFor(x => x.KeySkill1)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.KeySkill.Required"));

            RuleFor(x => x.YearsOfExperience1)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.YearsOfExperience.Required"))
                .ExclusiveBetween(0, 99).WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.YearsOfExperience.GreaterThanZero"));

            RuleFor(x => x.LastUsedDate1)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.LastUsedDate.Required"));

        }
    }
}
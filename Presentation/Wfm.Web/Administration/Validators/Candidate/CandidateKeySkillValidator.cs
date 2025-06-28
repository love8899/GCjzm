using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateKeySkillValidator : AbstractValidator<CandidateKeySkillModel>
    {
        public CandidateKeySkillValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KeySkill)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateKeySkill.Fields.KeySkill.Required"));

            RuleFor(x => x.YearsOfExperience)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateKeySkill.Fields.YearsOfExperience.Required"));

            RuleFor(x => x.LastUsedDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateKeySkill.Fields.LastUsedDate.Required"));

            RuleFor(x => x.LastUsedDate).LessThanOrEqualTo(System.DateTime.Today);
        }
    }
}
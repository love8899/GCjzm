using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Services.Localization;
using Wfm.Web.Models.Candidate;

namespace Wfm.Web.Validators.Candidate
{
    public class CandidateKeySkillValidator : AbstractValidator<CandidateKeySkillModel>
    {
        public CandidateKeySkillValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KeySkill)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Web.Candidate.CandidateKeySkill.Fields.KeySkill.Required"));

            RuleFor(x => x.YearsOfExperience)
                .NotNull().WithMessage(localizationService.GetResource("Web.Candidate.CandidateKeySkill.Fields.YearsOfExperience.Required"))
                .ExclusiveBetween(0, 99).WithMessage(localizationService.GetResource("Web.Candidate.CandidateKeySkill.Fields.YearsOfExperience.GreaterThanZero"));

            RuleFor(x => x.LastUsedDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Web.Candidate.CandidateKeySkill.Fields.LastUsedDate.Required"));
        }

    }
}
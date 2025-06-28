using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.Candidate;

namespace Wfm.Client.Validators.Candidate
{
    public class SkillValidator : AbstractValidator<SkillModel>
    {
        public SkillValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SkillName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Skill.Fields.SkillName.Required"));
        }
    }
}
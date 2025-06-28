using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Common
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
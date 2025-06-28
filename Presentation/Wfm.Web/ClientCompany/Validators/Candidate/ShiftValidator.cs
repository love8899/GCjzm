using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.Candidate;

namespace Wfm.Client.Validators.Candidate
{
    public class ShiftValidator : AbstractValidator<ShiftModel>
    {
        public ShiftValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ShiftName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.Skill.Fields.SkillName.Required"));
        }
    }
}
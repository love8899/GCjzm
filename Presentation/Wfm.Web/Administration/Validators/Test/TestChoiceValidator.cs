using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Test;

namespace Wfm.Admin.Validators.Test
{
    public class TestChoiceValidator : AbstractValidator<TestChoiceModel>
    {
        public TestChoiceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TestQuestionId)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestChoice.Fields.TestQuestionId.Required"));

            RuleFor(x => x.TestChoiceText)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestChoice.Fields.TestChoiceText.Required"));

            RuleFor(x => x.TestChoiceValue)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestChoice.Fields.TestChoiceValue.Required"));
        }
    }
}
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Test;

namespace Wfm.Admin.Validators.Test
{
    public class TestQuestionValidator : AbstractValidator<TestQuestionModel>
    {
        public TestQuestionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TestCategoryId)
               .NotEmpty()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestQuestion.Fields.TestCategoryId.Required"));

            RuleFor(x => x.Question)
               .NotEmpty()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestQuestion.Fields.TestQuestion.Required"));

            RuleFor(x => x.Answers)
               .NotEmpty()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestQuestion.Fields.Answers.Required"));

            RuleFor(x => x.Score)
               .NotEmpty()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestQuestion.Fields.Score.Required"))
               .InclusiveBetween(1, 10)
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestQuestion.Fields.Score.Valid"));
        }
    }
}
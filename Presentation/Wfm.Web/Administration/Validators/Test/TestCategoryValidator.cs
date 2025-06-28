using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Test;

namespace Wfm.Admin.Validators.Test
{
    public class TestCategoryValidator : AbstractValidator<TestCategoryModel>
    {
        public TestCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TestCategoryName)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestCategory.Fields.TestCategoryName.Required"));

            RuleFor(x => x.PassScore)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.TestCategory.Fields.PassScore.Required"));
        }
    }
}
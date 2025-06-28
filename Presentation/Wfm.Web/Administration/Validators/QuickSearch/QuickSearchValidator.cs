using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.QuickSearch;

namespace Wfm.Admin.Validators.Common
{
    public class QuickSearchValidator : AbstractValidator<QuickSearchModel>
    {
        public QuickSearchValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KeyWord)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.QuickSearch.Fields.KeyWord.Required"));
        }
    }
}

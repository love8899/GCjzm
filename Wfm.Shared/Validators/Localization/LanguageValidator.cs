using System.Globalization;
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Shared.Models.Localization;

namespace Wfm.Shared.Validators.Localization
{
    public class LanguageValidator : AbstractValidator<LanguageModel>
    {
        public LanguageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Language.Fields.Name.Required"));
            RuleFor(x => x.LanguageCulture)
                .Must(x =>
                          {
                              try
                              {
                                  var culture = new CultureInfo(x);
                                  return culture != null;
                              }
                              catch
                              {
                                  return false;
                              }
                          })
                .WithMessage(localizationService.GetResource("Admin.Configuration.Language.Fields.LanguageCulture.Validation"));

            RuleFor(x => x.UniqueSeoCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Language.Fields.UniqueSeoCode.Required"));
            RuleFor(x => x.UniqueSeoCode)
                .Length(2)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Language.Fields.UniqueSeoCode.Length"));

        }
    }
}
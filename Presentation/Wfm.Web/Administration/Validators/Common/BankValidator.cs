using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Common
{
    public class BankValidator : AbstractValidator<BankModel>
    {
        public BankValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BankName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.Bank.Fields.BankName.Required"));

        }
    }
}

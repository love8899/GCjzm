using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.CompanyBilling;

namespace Wfm.Client.Validators.Company
{
    public class CompanyBillingPositionValidator : AbstractValidator<CompanyBillingPositionModel>
    {
        public CompanyBillingPositionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BillingPositionCode)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingPosition.Fields.BillingPositionCode.Required"));

            RuleFor(x => x.BillingPositionName)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingPosition.Fields.BillingPositionName.Required"));
        }
    }
}
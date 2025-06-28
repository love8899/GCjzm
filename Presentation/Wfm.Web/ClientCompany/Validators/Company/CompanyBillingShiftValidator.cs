using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.CompanyBilling;

namespace Wfm.Client.Validators.Company
{
    public class CompanyBillingShiftValidator : AbstractValidator<CompanyBillingShiftModel>
    {
        public CompanyBillingShiftValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BillingShiftCode)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingShift.Fields.BillingShiftCode.Required"));

            RuleFor(x => x.BillingShiftName)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingShift.Fields.BillingShiftName.Required"));
        }
    }
}
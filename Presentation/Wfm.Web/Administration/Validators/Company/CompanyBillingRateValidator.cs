using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.CompanyBilling;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyBillingRateValidator : AbstractValidator<CompanyBillingRateModel>
    {
        public CompanyBillingRateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PositionId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.PositionCode.Required"));

            RuleFor(x => x.ShiftCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.ShiftCode.Required"));

            RuleFor(x => x.RegularBillingRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.RegularBillingRate.Required"));

            RuleFor(x => x.RegularPayRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.RegularPayRate.Required"));

            RuleFor(x => x.OvertimeBillingRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.OvertimeBillingRate.Required"));

            RuleFor(x => x.OvertimePayRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.OvertimePayRate.Required"));

            RuleFor(x => x.BillingTaxRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.BillingTaxRate.Required"));

            RuleFor(x => x.EffectiveDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.EffectiveDate.Required"));
            RuleFor(x => x.WSIBCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyBillingRate.Fields.WSIBCode.Required"));


        }
    }
}
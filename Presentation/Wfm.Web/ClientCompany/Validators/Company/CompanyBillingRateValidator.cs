using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.CompanyBilling;

namespace Wfm.Client.Validators.Company
{
    public class CompanyBillingRateValidator : AbstractValidator<CompanyBillingRateModel>
    {
        public CompanyBillingRateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PositionId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingRate.Fields.PositionCode.Required"));

            RuleFor(x => x.ShiftCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingRate.Fields.ShiftCode.Required"));

            RuleFor(x => x.RegularBillingRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingRate.Fields.RegularBillingRate.Required"));

            //RuleFor(x => x.RegularPayRate)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingRate.Fields.RegularPayRate.Required"));

            RuleFor(x => x.OvertimeBillingRate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingRate.Fields.OvertimeBillingRate.Required"));

            //RuleFor(x => x.OvertimePayRate)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.CompanyBillingRate.Fields.OvertimePayRate.Required"));

        }
    }
}
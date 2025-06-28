using FluentValidation;
using Wfm.Admin.Models.Payroll;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Payroll
{
    public class PayrollItemValidator : AbstractValidator<PayrollItemDetailModel>
    {
        public PayrollItemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Common.Fields.Code.Required"))
                .Length(1,4);
            RuleFor(x => x.Description).Length(0, 30);
            RuleFor(x => x.DebitAccount).Length(0, 18);
            RuleFor(x => x.CreditAccount).Length(0, 18);

        }
    }
}
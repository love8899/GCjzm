using FluentValidation;
using Wfm.Admin.Models.Payroll;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Payroll
{
    public class PayGroupValidator : AbstractValidator<PayGroupModel>
    {
        public PayGroupValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Common.Fields.Code.Required"));

            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Common.Fields.Name.IsRequired"));

            RuleFor(x => x.PayFrequencyTypeId)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Admin.Payroll.PayFrequencyType.Required"));

            RuleFor(x => x.FranchiseId)
            .NotEmpty()
            .WithMessage(localizationService.GetResource("Common.FranchiseId.IsRequired"));
        }
    }
}
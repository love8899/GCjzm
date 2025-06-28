using FluentValidation;
using Wfm.Admin.Models.Payroll;
using Wfm.Services.Localization;


namespace Wfm.Admin.Validators.Payroll
{
    public class OvertimeRuleSettingValidator : AbstractValidator<OvertimeRuleSettingModel>
    {
        public OvertimeRuleSettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Code)
            .NotNull()
            .WithMessage(localizationService.GetResource("Common.Fields.Code.Required"));

            RuleFor(x => x.TypeId)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Payroll.OvertimeRuleSetting.Fields.TypeId.Required"));

            RuleFor(x => x.ApplyAfter)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.Payroll.OvertimeRuleSetting.Fields.ApplyAfter.Required"));
        }
    }
}

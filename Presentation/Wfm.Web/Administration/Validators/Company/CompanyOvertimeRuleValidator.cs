
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Companies;


namespace Wfm.Admin.Validators.Company
{
    public class CompanyOvertimeRuleValidator : AbstractValidator<CompanyOvertimeRuleModel>
    {
        public CompanyOvertimeRuleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.OvertimeRuleSettingId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyOvertimeRule.Fields.OvertimeRuleSettingId.Required"));
        }
    }
}
 
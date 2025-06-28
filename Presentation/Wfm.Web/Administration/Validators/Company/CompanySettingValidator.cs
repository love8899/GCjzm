using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Companies;


namespace Wfm.Admin.Validators.Comany
{
    public class CompanySettingValidator : AbstractValidator<CompanySettingModel>
    {
        public CompanySettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Fields.Name.Required"));

            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Fields.Value.Required"));

        }
    }
}
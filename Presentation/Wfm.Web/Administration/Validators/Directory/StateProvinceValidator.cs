using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Directory;

namespace Wfm.Admin.Validators.Directory
{
    public class StateProvinceValidator : AbstractValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CountryId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.CountryIsRequired"));

            RuleFor(x => x.StateProvinceName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.StateProvince.Fields.StateProvinceName.Required"));

            //RuleFor(x => x.StateProvinceCode)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Configuration.StateProvinces.Fields.Code.Required"));

        }
    }
}
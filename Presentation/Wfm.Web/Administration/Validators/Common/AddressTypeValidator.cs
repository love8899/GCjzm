using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Common
{
    public class AddressTypeValidator : AbstractValidator<AddressTypeModel>
    {
        public AddressTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddressTypeName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.AddressTypes.Fields.AddressTypeName.Required"));
        }
    }
}

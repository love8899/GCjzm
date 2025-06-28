using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Franchises;
using Wfm.Admin.Validators.Common;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Franchise
{
    public class FranchiseAddressValidator : AbstractValidator<FranchiseAddressModel>
    {
        public FranchiseAddressValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FranchiseId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Common.FranchiseId.IsRequired"));
            RuleFor(x => x.PrimaryPhone)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Companies.CompanyLocation.Fields.PrimaryPhone.Required"));
            Include(new AddressModelValidator<AddressModel>(localizationService));
            RuleFor(x => x.LocationName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyLocation.Fields.LocationName.Required"));
        }
    }
}

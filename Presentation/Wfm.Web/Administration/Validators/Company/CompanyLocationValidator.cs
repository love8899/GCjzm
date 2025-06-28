using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Validators.Common;
using Wfm.Admin.Models.Common;

namespace Wfm.Admin.Validators.Company
{
    public class CompanyLocationValidator : AbstractValidator<CompanyLocationListModel>
    {
        public CompanyLocationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LocationName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyLocation.Fields.LocationName.Required"));

            //RuleFor(x => x.PrimaryPhone)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Companies.CompanyLocation.Fields.PrimaryPhone.Required"));

            Include(new AddressModelValidator<AddressModel>(localizationService)); 

        }
    }
}
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.Common;
using Wfm.Admin.Validators.Common;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateAddressValidator : AbstractValidator<CandidateAddressModel>
    {
        public CandidateAddressValidator(ILocalizationService localizationService)
        {        
            RuleFor(x => x.AddressTypeId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateAddress.Fields.AddressTypeId.Required"));

            Include(new AddressModelValidator<AddressModel>(localizationService)); 
        }
    }
}
using FluentValidation;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Web.Models.Candidate;
using Wfm.Services.Localization;


namespace Wfm.Web.Validators.Candidate
{
    public class CandidateUpdateProfileValidator : AbstractValidator<CandidateUpdateProfileModel>
    {
        public CandidateUpdateProfileValidator(ILocalizationService localizationService, CandidateSettings candidateSettings)
        {
            //RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.Email.Invalid"));

            //RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Common.FirstName.IsRequired"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.SalutationId).NotEmpty().WithMessage(localizationService.GetResource("Common.SalutationId.Required"));

            RuleFor(x => x.HomePhone).NotEmpty().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.HomePhone.Required"));
            
            RuleFor(x => x.TransportationId).NotNull().WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.TransportationId.Required"));
            When(x => x.TransportationId == (int)TransportationEnum.Car, () =>
            {
                RuleFor(x => x.LicencePlate).NotNull().NotEmpty();
            });

            RuleFor(x => x.SecurityQuestion1Id).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion1.Required"));
            RuleFor(x => x.SecurityQuestion2Id).NotNull().WithMessage(localizationService.GetResource("SecurityQuestion2.Required"));
        }
    }

}

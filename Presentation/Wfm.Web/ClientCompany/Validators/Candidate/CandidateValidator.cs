using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.Candidate;

namespace Wfm.Client.Validators.Candidate
{
    public class CandidateValidator : AbstractValidator<CandidateModel>
    {
        public CandidateValidator(ILocalizationService localizationService)
        {
            RuleFor(x =>x.Email)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Required"));

            RuleFor(x =>x.Password)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Password.Required"));
                
            //RuleFor(x => x.FirstName)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.FirstName.Required"));

            RuleFor(x => x.LastName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.LastName.IsRequired"));

            RuleFor(x => x.SalutationId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Common.SalutationId.Required"));

            RuleFor(x => x.GenderId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.GenderId.Required"));

            RuleFor(x => x.HomePhone)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.HomePhone.Required"));

            //RuleFor(x => x.SocialInsuranceNumber)
            //   .NotNull()
            //   .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber.Required"));

            RuleFor(x => x.ShiftId)
                 .NotNull()
                 .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.ShiftId.Required"));

            RuleFor(x => x.TransportationId)
                 .NotNull()
                 .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.TransportationId.Required"));

            RuleFor(x => x.Entitled)
                 .Equal(true)
                 .WithMessage(localizationService.GetResource("Admin.Candidate.Candidate.Fields.Entitled.Required"));

        }
    }
}
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateBankAccountValidator : AbstractValidator<CandidateBankAccountModel>
    {
        public CandidateBankAccountValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Companies.CompanyCandidate.Fields.CandidateId.Required"));

            RuleFor(x => x.InstitutionNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateBankAccount.Fields.InstitutionNumber.Required"));

            RuleFor(x => x.TransitNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateBankAccount.Fields.TransitNumber.Required"));

            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateBankAccount.Fields.AccountNumber.Required"));

        }
    }
}
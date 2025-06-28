using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Company
{
    public class CandidateWorkHistoryValidator : AbstractValidator<CandidateWorkHistoryModel>
    {
        public CandidateWorkHistoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateWorkHistory.Fields.Title.Required"));

            RuleFor(x => x.CompanyName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateWorkHistory.Fields.CompanyName.Required"));

            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateWorkHistory.Fields.StartDate.Required"));

            //RuleFor(x => x.EndDate)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("AdminAdmin.Candidate.CandidateWorkHistory.Fields.EndDate.Required"));
        }

    }
}
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.Candidate;

namespace Wfm.Client.Validators.Candidate
{
    public class CandidateJobOrderValidator : AbstractValidator<CandidateJobOrderModel>
    {
        public CandidateJobOrderValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateJobOrder.Fields.CandidateId.Required"));

            RuleFor(x => x.JobOrderId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateJobOrder.Fields.JobOrderId.Required"));
        }
    }
}
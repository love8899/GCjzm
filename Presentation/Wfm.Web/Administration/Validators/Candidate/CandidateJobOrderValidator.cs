using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateJobOrderValidator : AbstractValidator<CandidateJobOrderModel>
    {
        public CandidateJobOrderValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateJobOrderStatusId)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateJobOrder.Fields.CandidateJobOrderStatusId.Required"));

        }
    }
}
using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Client.Models.Candidate;

namespace Wfm.Client.Validators.Candidate
{
    public class CandidateJobOrderStatusValidator : AbstractValidator<CandidateJobOrderStatusModel>
    {
        public CandidateJobOrderStatusValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.StatusName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Configuration.JobOrderStatus.Fields.StatusName.Required"));
        }
    }
}
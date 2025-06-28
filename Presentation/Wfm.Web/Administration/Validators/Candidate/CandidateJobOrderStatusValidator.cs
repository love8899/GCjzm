using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
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
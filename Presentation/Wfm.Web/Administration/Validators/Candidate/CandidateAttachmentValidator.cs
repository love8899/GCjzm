using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateAttachmentValidator : AbstractValidator<CandidateAttachmentModel>
    {
        public CandidateAttachmentValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.OriginalFileName)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateAttachment.Fields.OriginalFileName.Required"));
        }
    }
}

using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateResetPasswordValidator : AbstractValidator<CandidateResetPasswordModel>
    {
        public CandidateResetPasswordValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email)
             .NotNull()
             .WithMessage(localizationService.GetResource("Admin.Candidate.CandidateResetPassword.Fields.Email.Required"));
        }
    }
}
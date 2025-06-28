using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Messages;


namespace Wfm.Admin.Validators.Messages
{
    public class ResumeHistoryValidator : AbstractValidator<ResumeHistoryModel>
    {
        public ResumeHistoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ContactedOn).NotEmpty();

            RuleFor(x => x.Result).NotEmpty();
        }
    }
}

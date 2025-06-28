using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.TimeSheet;


namespace Wfm.Admin.Validators.TimeSheet
{
    public class CandidateMissingHourValidator : AbstractValidator<CandidateMissingHourModel>
    {
        public CandidateMissingHourValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CandidateWorkTime.Fields.CandidateId.Required"));

            RuleFor(x => x.JobOrderId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CandidateWorkTime.Fields.JobOrderId.Required"));

            RuleFor(x => x.Note)
                .NotEmpty();
        }
    }
}
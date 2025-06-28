using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.TimeSheet;

namespace Wfm.Admin.Validators.TimeSheet
{
    public class CandidateWorkTimeValidator : AbstractValidator<CandidateWorkTimeModel>
    {
        public CandidateWorkTimeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CandidateWorkTime.Fields.CandidateId.Required"));

            RuleFor(x => x.JobOrderId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CandidateWorkTime.Fields.JobOrderId.Required"));

            RuleFor(x => x.AdjustmentInMinutes)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.AdjustmentInMinutes.Required"));

            RuleFor(x => x.Note)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.Note.Required"));
        }
    }
}
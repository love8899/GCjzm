using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.TimeSheet;

namespace Wfm.Admin.Validators.TimeSheet
{
    public class EditCandidateWorkTimeAdjustmentValidator : AbstractValidator<EditCandidateWorkTimeAdjustmentModel>
    {
        public EditCandidateWorkTimeAdjustmentValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AdjustmentInMinutes)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.AdjustmentInMinutes.Required"));

            RuleFor(x => x.Note)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.Note.Required"));

        }
    }
}
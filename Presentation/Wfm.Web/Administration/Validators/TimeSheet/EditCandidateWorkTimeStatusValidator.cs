using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.TimeSheet;

namespace Wfm.Admin.Validators.TimeSheet
{
    public class EditCandidateWorkTimeStatusValidator : AbstractValidator<EditCandidateWorkTimeStatusModel>
    {
        public EditCandidateWorkTimeStatusValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CandidateWorkTimeStatusId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeStatus.Fields.CandidateWorkTimeStatusId.Required"));

            RuleFor(x => x.Note)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.EditCandidateWorkTimeStatusModel.Fields.Note.Required"));
        }
    }
}
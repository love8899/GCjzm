using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.TimeSheet;

namespace Wfm.Admin.Validators.TimeSheet
{
    public class CalculateCandidateWorkTimeValidator : AbstractValidator<CalculateCandidateWorkTimeModel>
    {
        public CalculateCandidateWorkTimeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FromDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CalculateCandidateWorkTime.Fields.FromDate.Required"));

            RuleFor(x => x.ToDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeSheet.CalculateCandidateWorkTime.Fields.ToDate.Required"));

        }
    }
}
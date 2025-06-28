using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.ClockTime;

namespace Wfm.Admin.Validators.ClockTime
{
    public class CandidateClockTimeValidator : AbstractValidator<CandidateClockTimeModel>
    {
        public CandidateClockTimeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ClockDeviceUid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateClockTime.Fields.ClockDeviceUid.Required"));

            RuleFor(x => x.SmartCardUid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateClockTime.Fields.SmartCardUid.Required"));

            RuleFor(x => x.ClockInOut)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateClockTime.Fields.ClockInOut.Required"));

        }
    }
}
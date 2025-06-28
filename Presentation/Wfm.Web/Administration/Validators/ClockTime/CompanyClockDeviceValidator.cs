using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.ClockTime;

namespace Wfm.Admin.Validators.ClockTime
{
    public class CompanyClockDeviceValidator : AbstractValidator<CompanyClockDeviceModel>
    {
        public CompanyClockDeviceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyLocationId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CompanyClockDevice.Fields.CompanyLocationId.Required"));

            RuleFor(x => x.ClockDeviceUid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CompanyClockDevice.Fields.ClockDeviceUid.Required"));
        }
    }
}
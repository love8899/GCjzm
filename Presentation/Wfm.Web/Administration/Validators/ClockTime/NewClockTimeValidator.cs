using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.ClockTime;
using System.Linq;

namespace Wfm.Admin.Validators.ClockTime
{
    public class NewClockTimeValidator : AbstractValidator<NewClockTimeModel>
    {
        public NewClockTimeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));
            RuleFor(x => x.CompanyLocationId)
                .NotNull().GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.LocationId.IsRequired"));
            RuleFor(x => x.ClockDeviceUid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.NewClockTime.Fields.ClockDeviceUid.Required"));
            RuleFor(x => x.SmartCardUid)
                .Must((e, pass) =>
                {
                    return pass.Replace("_", "").Trim().Length == 10;

                })
                .WithMessage(localizationService.GetResource("Common.SmartCardUid.RequiredLength"), 10);
                        RuleFor(x => x.SmartCardUid)
                            .Must((e, pass) =>
                            {
                                return pass.Replace("_", "").Trim().All(x => char.IsNumber(x));

                            })
                .WithMessage(localizationService.GetResource("Common.SmartCardUid.RequireNumber"));
            RuleFor(x => x.SmartCardUid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.NewClockTime.Fields.SmartCardUid.Required"));

            RuleFor(x => x.ClockInOut)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateClockTime.Fields.ClockInOut.Required"));

        }
    }
}
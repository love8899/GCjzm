using FluentValidation;
using Wfm.Services.Localization;
using Wfm.Admin.Models.ClockTime;
using System.Linq;

namespace Wfm.Admin.Validators.ClockTime
{
    public class CandidateSmartCardValidator : AbstractValidator<CandidateSmartCardModel>
    {
        public CandidateSmartCardValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SmartCardUid)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.Fields.SmartCardUid.Required"));

            RuleFor(x => x.SmartCardUid)
                .Must((e, pass) =>
                {
                    return pass.Replace("_","").Trim().Length==10;

                })
                .WithMessage(localizationService.GetResource("Common.SmartCardUid.RequiredLength"),10);

            RuleFor(x => x.SmartCardUid)
                .Must((e, pass) =>
                {
                    return pass.Replace("_","").Trim().All(x => char.IsNumber(x));

                })
                .WithMessage(localizationService.GetResource("Common.SmartCardUid.RequireNumber"));

            RuleFor(x => x.CandidateId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.Fields.CandidateId.Required"));

            RuleFor(x => x.ActivatedDate)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.Fields.ActivatedDate.Required"));

            When(x => !x.IsActive, () =>
            {
                RuleFor(x => x.ReasonForDeactivation).NotEmpty();
            });
        }
    }
}
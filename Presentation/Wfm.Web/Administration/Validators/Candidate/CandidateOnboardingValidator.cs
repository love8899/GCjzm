using FluentValidation;
using System;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Localization;
using Wfm.Admin.Models.Candidate;
using Wfm.Shared.Validators;
using System.Linq;

namespace Wfm.Admin.Validators.Candidate
{
    public class CandidateOnboardingValidator : AbstractValidator<CandidateOnboardingModel>
    {
        public CandidateOnboardingValidator(ILocalizationService localizationService, CandidateSettings candidateSettings)
        {
            RuleFor(x => x.SocialInsuranceNumber)
                .NotNull()
                .SetValidator(new SinValidator<string>())
                .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid"));
            When(x => !String.IsNullOrWhiteSpace(x.SocialInsuranceNumber) && x.SocialInsuranceNumber.Trim().StartsWith("9"), () =>
            {
                RuleFor(x => x.SINExpiryDate)
                    .NotNull().NotEmpty()
                    .WithMessage(localizationService.GetResource("Web.Candidate.Candidate.Fields.SINExpiryDate.Required"));
            });
            When(x => !String.IsNullOrEmpty(x.SmartCardUid), () =>
            {
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
            });
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Policies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Policies
{

    public class RoundingPolicyValidator : AbstractValidator<RoundingPolicyModel>
    {
        public RoundingPolicyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
               .NotNull()
               .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.Name)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Policy.RoundingPolicy.Fields.Name.Required"));

            RuleFor(x => x.IntervalInMinutes)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Policy.RoundingPolicy.Fields.IntervalInMinutes.Required"));
        }
    }
}
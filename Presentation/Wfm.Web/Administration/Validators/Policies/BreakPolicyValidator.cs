using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Policies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Policies
{
    public class BreakPolicyValidator : AbstractValidator<BreakPolicyModel>
    {
        public BreakPolicyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.Name)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Policy.BreakPolicy.Fields.Name.Required"));

            RuleFor(x => x.BreakTimeInMinutes)
               .NotNull()
               .WithMessage(localizationService.GetResource("Admin.Policy.BreakPolicy.Fields.BreakTimeInMinutes.Required"));
        }
    }
}
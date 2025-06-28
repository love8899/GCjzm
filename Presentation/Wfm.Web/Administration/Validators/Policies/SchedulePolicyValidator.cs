using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Policies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Policies
{
    public class SchedulePolicyValidator : AbstractValidator<SchedulePolicyModel>
    {
        public SchedulePolicyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull().WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.Name)
               .NotEmpty().WithMessage(localizationService.GetResource("Admin.Policy.SchedulePolicy.Fields.Name.Required"));

            //RuleFor(x => x.MealPolicyId)
            //   .NotNull().WithMessage(localizationService.GetResource("Admin.Policy.SchedulePolicy.Fields.MealPolicyId.Required"));

            //RuleFor(x => x.BreakPolicyId)
            //   .NotNull().WithMessage(localizationService.GetResource("Admin.Policy.SchedulePolicy.Fields.BreakPolicyId.Required"));

            //RuleFor(x => x.RoundingPolicyId)
            //   .NotNull().WithMessage(localizationService.GetResource("Admin.Policy.SchedulePolicy.Fields.RoundingPolicyId.Required"));

        }
    }
}
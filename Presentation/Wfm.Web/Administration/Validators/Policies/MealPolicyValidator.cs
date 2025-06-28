using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Policies;
using Wfm.Services.Localization;

namespace Wfm.Admin.Validators.Policies
{
    public class MealPolicyValidator : AbstractValidator<MealPolicyModel>
    {
        public MealPolicyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CompanyId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(localizationService.GetResource("Common.CompanyId.IsRequired"));

            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Policy.MealPolicy.Fields.Name.Required"));

            RuleFor(x => x.MealTimeInMinutes)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Policy.MealPolicy.Fields.MealTimeInMinutes.Required"));

            RuleFor(x => x.MinWorkHours)
                .NotNull().WithMessage(localizationService.GetResource("Admin.Policy.MealPolicy.Fields.MinWorkHours.Required"))
                .GreaterThan(0m).WithMessage(localizationService.GetResource("Admin.Policy.MealPolicy.Fields.MinWorkHours.Positive"));
        }
    }
}
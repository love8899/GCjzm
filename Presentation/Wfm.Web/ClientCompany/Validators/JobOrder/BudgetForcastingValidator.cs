using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Client.Models.JobOrder;
using Wfm.Services.Localization;

namespace Wfm.Client.Validators.JobOrder
{
    public class BudgetForcastingValidator : AbstractValidator<BudgetForcastingModel>
    {
        public BudgetForcastingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartDate.Required"));

            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.EndDate.Required"))
                .Must((x, d) => (d >= x.StartDate))
                .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodEndDate.GreaterThan"));
        }
    }
}
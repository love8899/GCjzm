using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Infrastructure;
using Wfm.Services.Localization;
using Wfm.Services.Scheduling;
using Wfm.Shared.Models.Scheduling;

namespace Wfm.Shared.Validators
{
    public class ScheduleJobOrderValidator : AbstractValidator<ScheduleJobOrderModel>
    {
        public ScheduleJobOrderValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.JobTitle)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.JobTitle.Required"));
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.JobOrder.JobOrder.Fields.StartDate.Required"));
            RuleFor(x => x.EndDate)
                .Must((x, d) => (!d.HasValue || d.Value > x.StartDate))
                .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodEndDate.GreaterThan"));
        }
    }
}

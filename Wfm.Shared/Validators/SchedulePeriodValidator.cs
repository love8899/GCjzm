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
    public class SchedulePeriodValidator : AbstractValidator<SchedulePeriodModel>
    {
        public SchedulePeriodValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PeriodStartDate)
              .Must((e, r) =>
              {
                  var svc = EngineContext.Current.Resolve<ISchedulingDemandService>();
                  var startUtc = e.PeriodStartDate.ToUniversalTime();
                  var endtUtc = e.PeriodEndDate.HasValue ? e.PeriodEndDate.Value.ToUniversalTime() : DateTime.MaxValue;
                  return !svc.GetAllSchedulePeriods(e.CompanyId).Any(x => x.Id != e.Id &&
                    ((x.PeriodStartUtc <= startUtc && x.PeriodEndUtc >= startUtc) ||
                    (x.PeriodStartUtc <= endtUtc && x.PeriodEndUtc >= endtUtc) ||
                    (x.PeriodStartUtc <= startUtc && x.PeriodEndUtc >= endtUtc) ||
                    (x.PeriodStartUtc >= startUtc && x.PeriodEndUtc <= endtUtc)) &&
                    ((!e.CompanyDepartmentId.HasValue && !x.CompanyDepartmentId.HasValue) || e.CompanyDepartmentId == x.CompanyDepartmentId));
              })
              .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodStartDate.NoOverlap"));
            RuleFor(x => x.PeriodEndDate)
                .NotNull()
                .WithMessage("End date is required");
            RuleFor(x => x.CompanyLocationId)
                .NotNull()
                .WithMessage("Please select location.");
            RuleFor(x => x.CompanyDepartmentId)
                .NotNull()
                .When(x => x.CompanyLocationId.HasValue)
                .WithMessage("Please select department when you select location.");
            RuleFor(x => x.CompanyDepartmentId)
                .GreaterThan(0)
                .When(x => x.CompanyLocationId.HasValue)
                .WithMessage("Please select department when you select location.");
            RuleFor(x => x.PeriodEndDate)
              .GreaterThan(x => x.PeriodStartDate)
              .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodEndDate.GreaterThan"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Wfm.Client.Models.Employees;
using Wfm.Core.Infrastructure;
using Wfm.Services.Employees;

namespace Wfm.Client.Validators.Employees
{
    public class EmployeeAvailabilityValidator : AbstractValidator<EmployeeAvailabilityModel>
    {
        public EmployeeAvailabilityValidator()
        {
            RuleFor(x => x.IsAllDay)
                .Must((m, x) =>
                {
                    var svc = EngineContext.Current.Resolve<IEmployeeService>();
                    return !svc.CheckOverlaping(m.EmployeeIntId, m.DayOfWeek, 0, 863400000000, m.Id, m.EmployeeAvailabilityType,
                        m.StartDate, m.EndDate);
                })
                .When(x => x.IsAllDay)
                .WithMessage("Overlaping with existing settings");
            RuleFor(x => x.StartTimeOfDay)
                .Must((m, x) =>
                {
                    var svc = EngineContext.Current.Resolve<IEmployeeService>();
                    return !svc.CheckOverlaping(m.EmployeeIntId, m.DayOfWeek, x.GetValueOrDefault().TimeOfDay.Ticks,
                        m.EndTimeOfDay.GetValueOrDefault().TimeOfDay.Ticks, m.Id, m.EmployeeAvailabilityType,
                        m.StartDate, m.EndDate);
                })
                .WithMessage("Overlaping with existing settings");
            //RuleFor(x => x.EndTimeOfDay)
            //    .GreaterThanOrEqualTo(x => x.StartTimeOfDay)
            //    .WithMessage("End time must be greater than or equal to start time");
            RuleFor(x => x.StartTimeOfDay)
                .NotNull()
                .When(x => !x.IsAllDay)
                .WithMessage("Start time cannot be empty");
            RuleFor(x => x.EndTimeOfDay)
                .NotNull()
                .When(x => !x.IsAllDay)
                .WithMessage("End time cannot be empty");
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue && x.StartDate.HasValue)
                .WithMessage("End date must be greater than start date");
        }
    }
}

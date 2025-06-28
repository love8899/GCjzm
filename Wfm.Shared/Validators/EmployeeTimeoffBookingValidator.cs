using FluentValidation;
using System.Linq;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Infrastructure;
using Wfm.Services.Employees;
using Wfm.Services.Localization;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Models.Employees;

namespace Wfm.Shared.Validators
{
    public class EmployeeTimeoffBookingValidator : AbstractValidator<EmployeeTimeoffBookingModel>
    {
        public EmployeeTimeoffBookingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.EmployeeTimeoffTypeId)
                .NotEqual(default(int))
                .WithMessage(localizationService.GetResource("Admin.Employee.TimeoffEntitlement.Fields.EmployeeTimeoffTypeId.IsRequired"));
            RuleFor(x => x.TimeOffEndDateTime)
                .GreaterThanOrEqualTo(e => e.TimeOffStartDateTime)
                .WithMessage(localizationService.GetResource("Web.SchedulePeriod.Fields.PeriodEndDate.GreaterThan"));
            RuleFor(x => x.EmployeeTimeoffTypeId)
                .Must((e, typeId) =>
                {
                    var svc = EngineContext.Current.Resolve<ITimeoffService>();
                    var timeOffType = svc.GetAllTimeoffTypes().Where(x => x.Id == e.EmployeeTimeoffTypeId).FirstOrDefault();
                    if (timeOffType != null && timeOffType.AllowNegative) return true;
                    //
                    var balance = svc.GetEntitlement(e.EmployeeIntId, e.TimeOffStartDateTime.Year)
                        .Where(x => x.EmployeeTimeoffTypeId == e.EmployeeTimeoffTypeId)
                        .FirstOrDefault().LatestBalanceInHours.GetValueOrDefault();
                    var hours = svc.GetHoursBetweenDates(e.EmployeeIntId, e.TimeOffStartDateTime, e.TimeOffEndDateTime, e.Id);
                    return balance - hours >= 0;
                })
                .WithMessage(localizationService.GetResource("Admin.Employee.TimeoffEntitlement.Fields.AllowNegative"));
            
            RuleFor(x => x.TimeOffStartDateTime)
                .Must((e, startDate) =>
                {
                    var svc = EngineContext.Current.Resolve<IWorkTimeService>();
                    return !svc.GetAllCandidateWorkTimeAsQueryable()
                        .Any(x => (x.CandidateId == e.EmployeeIntId) &&
                            (x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved ||
                            x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Processed) &&
                            ((x.JobStartDateTime >= e.TimeOffStartDateTime && x.JobStartDateTime <= e.TimeOffEndDateTime) ||
                            (x.JobEndDateTime >= e.TimeOffStartDateTime && x.JobEndDateTime <= e.TimeOffEndDateTime)));
                }).WithMessage(localizationService.GetResource("Admin.EmployeeTimeoff.AlreadyWorktimeApproved"))
                .Must((e, startDate) =>
                {
                    var timeoffService = EngineContext.Current.Resolve<ITimeoffService>();
                    return !timeoffService.GetBookHistoryByEmployee(e.EmployeeIntId).Where(x => x.Id != e.Id)
                        .Any(x => x.TimeOffStartDateTime <= e.TimeOffStartDateTime && x.TimeOffEndDateTime >= e.TimeOffStartDateTime);
                }).WithMessage(localizationService.GetResource("Admin.EmployeeTimeoff.StartOverlap"));

            RuleFor(x => x.TimeOffEndDateTime)
                .Must((e, endDate) =>
                {
                    var timeoffService = EngineContext.Current.Resolve<ITimeoffService>();
                    return !timeoffService.GetBookHistoryByEmployee(e.EmployeeIntId).Where(x => x.Id != e.Id)
                        .Any(x => x.TimeOffStartDateTime <= e.TimeOffEndDateTime && x.TimeOffEndDateTime >= e.TimeOffEndDateTime);
                }).WithMessage(localizationService.GetResource("Admin.EmployeeTimeoff.EndOverlap"));
        }
    }
}

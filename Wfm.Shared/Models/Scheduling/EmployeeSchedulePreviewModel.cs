using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Scheduling;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class EmployeeSchedulePreviewModel : BaseWfmEntityModel, ISchedulerEvent
    {
        const int UiBreakSlots = 4;
        public EmployeeSchedulePreviewModel()
        {
            this.UniqueId = Guid.NewGuid();
            // reserve 4 breaks consistent to UI
            this._breaksTimeOfDay = new List<TimeSpan>();
            this._breakLengthInMinutes = new List<decimal?>();
        }
        public EmployeeSchedulePreviewModel(int employeeId, IEmployeeScheduleDetailModel tuple)
            : this()
        {
            this.EmployeeId = employeeId;
            //
            this.Start = tuple.ScheduleDate.AddTicks(tuple.StartTimeTicks);
            this.End = tuple.ScheduleDate.AddTicks(tuple.EndTimeTicks);
            this.Title = tuple.EntryTitle;
            this.CompanyJobRoleId = tuple.CompanyJobRoleId;
        }

        public static IEnumerable<EmployeeSchedulePreviewModel> FromShift(IShiftViewDay tuple)
        {
            foreach (var entry in tuple.EmployeeSchedules)
            {
                yield return new EmployeeSchedulePreviewModel
                {
                    EmployeeId = entry.Employee.Id,
                    ScheduelDate = tuple.Shift.ScheduelDate,
                    Start = tuple.Shift.ScheduelDate.AddTicks(tuple.Shift.StartTimeOfDayTicks),
                    End = tuple.Shift.ScheduelDate.AddTicks(tuple.Shift.StartTimeOfDayTicks + (long)(TimeSpan.TicksPerHour * tuple.Shift.LengthInHours)),
                    Title = entry.Title,
                    Description = entry.Employee.ToString(),
                    CompanyJobRoleId = entry.JobRoleId,
                    CompanyShiftId = entry.CompanyShiftId,
                    EmployeeScheduleId = entry.EmployeeScheduleId,
                    SchedulePeriodId = tuple.Shift.SchedulePeriodId,
                    BreakTimeOfDay = new TimeSpan[] {
                        TimeSpan.FromTicks(tuple.Shift.StartTimeOfDayTicks + (long)(TimeSpan.TicksPerHour * tuple.Shift.LengthInHours) / 2)
                    },
                    BreakLengthInMinutes = new List<decimal?> { 30m },
                    ForDailyAdhoc = entry.ForDailyAdhoc,
                };
            }
        }

        public static IEnumerable<EmployeeSchedulePreviewModel> MergeWithEmployeeScheduleDaily(IEnumerable<EmployeeSchedulePreviewModel> baseline, IEnumerable<EmployeeScheduleDailyModel> dailyModel)
        {
            var baselineDates = new List<Tuple<int, DateTime>>();
            foreach (var entry in baseline)
            {
                var overriding = dailyModel.FirstOrDefault(x => x != null && x.EmployeeScheduleId == entry.EmployeeScheduleId && x.ScheduelDate.Date == entry.ScheduelDate.Date);
                if (overriding != null)
                {
                    entry.Start = overriding.Start;
                    entry.End = overriding.End;
                    entry.BreakTimeOfDay = overriding.Breaks.Any() ? overriding.Breaks.OrderBy(x => x.BreakTimeOfDay).Select(x => x.BreakTimeOfDay).ToList() :
                        new List<TimeSpan> {
                            TimeSpan.FromTicks(overriding.StartTimeOfDay.Ticks + (overriding.End - overriding.Start).Ticks / 2)
                        };
                    entry.BreakLengthInMinutes = overriding.Breaks.Any() ? overriding.Breaks.OrderBy(x => x.BreakTimeOfDay).Select(x => new decimal?(x.BreakLengthInMinutes)).ToList() :
                        new List<decimal?> {
                            30m,
                        };
                    entry.Description = !string.IsNullOrEmpty(overriding.Description) ? overriding.Description : entry.Description;
                    entry.Title = !string.IsNullOrEmpty(overriding.Title) ? overriding.Title : entry.Title;
                    entry.CompanyJobRoleId = overriding.ReplacementCompanyJobRoleId.GetValueOrDefault(entry.CompanyJobRoleId);
                    if (overriding.IsAdhoc)
                    {
                        entry.Description += " (Ad hoc)";
                    }
                    else if (overriding.ReplacementEmployeeId.HasValue && overriding.ReplacementEmployeeId != entry.EmployeeId)
                    {
                        entry.Description += " (Substitution)";
                    }
                    entry.EmployeeId = overriding.ReplacementEmployeeId.GetValueOrDefault(entry.EmployeeId);
                }
                if ((overriding == null && !entry.ForDailyAdhoc) || (overriding != null && !overriding.IsDeleted))
                {
                    yield return entry;
                }
                baselineDates.Add(Tuple.Create(entry.EmployeeScheduleId, entry.ScheduelDate));
            }
            foreach (var overriding in dailyModel.Where(x => x != null && !x.IsDeleted && x.ReplacementEmployeeId.HasValue &&
                !baselineDates.Contains(Tuple.Create(x.EmployeeScheduleId, x.ScheduelDate))))
            {
                yield return new EmployeeSchedulePreviewModel
                {
                    EmployeeScheduleId = overriding.EmployeeScheduleId,
                    EmployeeId = overriding.ReplacementEmployeeId.Value,
                    ScheduelDate = overriding.ScheduelDate,
                    Start = overriding.ScheduelDate.Add(overriding.Start.TimeOfDay),
                    End = overriding.End,
                    BreakTimeOfDay = overriding.Breaks.Any() ? overriding.Breaks.OrderBy(x => x.BreakTimeOfDay).Select(x => x.BreakTimeOfDay).ToList() :
                        new List<TimeSpan> {
                            TimeSpan.FromTicks(overriding.StartTimeOfDay.Ticks + (overriding.End - overriding.Start).Ticks / 2)
                        },
                    BreakLengthInMinutes = overriding.Breaks.Any() ? overriding.Breaks.OrderBy(x => x.BreakTimeOfDay).Select(x => new decimal?(x.BreakLengthInMinutes)).ToList() :
                        new List<decimal?> {
                            30m,
                        },
                    Description = overriding.Description + " (Ad hoc)",
                    Title = overriding.Title,
                    CompanyJobRoleId = overriding.ReplacementCompanyJobRoleId.Value,
                };
            }
        }
        public static IEnumerable<EmployeeSchedulePreviewModel> FromEmployeeTimeoff(IEnumerable<EmployeeTimeoffBooking> timeOffBookings)
        {
            foreach (var booking in timeOffBookings)
            {
                var date = booking.TimeOffStartDateTime;
                while (date <= booking.TimeOffEndDateTime)
                {
                    var end = (date.Date != booking.TimeOffEndDateTime.Date) ? date.Date.AddDays(1).AddMinutes(-1) : booking.TimeOffEndDateTime;
                    yield return new EmployeeSchedulePreviewModel
                    {
                        EmployeeScheduleId = 0,
                        EmployeeId = booking.EmployeeId,
                        ScheduelDate = date,
                        Start = date,
                        End = end,
                        Description = booking.Employee.ToString(),
                        Title = "Time off - " + booking.EmployeeTimeoffType.Description,
                        CompanyJobRoleId = 0,
                    };
                    date = date.AddDays(1).Date;
                }
            }
        }
        public Guid UniqueId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public bool IsAllDay { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        private List<TimeSpan> _breaksTimeOfDay { get; set; }
        public IList<DateTime> BreakAt
        {
            get
            {
                return BreakTimeOfDay.Select(x => ScheduelDate.Add(x)).OrderBy(x => x.Ticks).ToList();
            }
            set
            {
                BreakTimeOfDay = value.Select(x => x - ScheduelDate).ToList();
            }
        }
        public IEnumerable<EmployeeScheduleDailyBreakModel> BreakModels
        {
            get
            {
                for (int i = 0; i < BreakTimeOfDay.Count(); i++)
                {
                    yield return new EmployeeScheduleDailyBreakModel
                    {
                        BreakTimeOfDay = BreakTimeOfDay[i],
                        BreakLengthInMinutes = BreakLengthInMinutes[i].GetValueOrDefault(),
                    };
                }
            }
        }
        public IList<TimeSpan> BreakTimeOfDay
        {
            get
            {
                return _breaksTimeOfDay.Any() ? _breaksTimeOfDay.ToList() :
                    new List<TimeSpan> {
                        TimeSpan.FromTicks(Start.Ticks + (End - Start).Ticks / 2 - ScheduelDate.Ticks)
                        };
            }
            set
            {
                _breaksTimeOfDay = value.ToList();
            }
        }
        private IList<decimal?> _breakLengthInMinutes;
        public IList<decimal?> BreakLengthInMinutes
        {
            get
            {
                while (_breakLengthInMinutes.Count < UiBreakSlots) _breakLengthInMinutes.Add(null);
                return _breakLengthInMinutes;
            }
            set
            {
                _breakLengthInMinutes = value;
            }
        }
        public IList<long> BreakTimePosition
        {
            get
            {
                return BreakTimeOfDay.OrderBy(x => x).Select(x =>
                    (End.Ticks - Start.Ticks) != 0 ? (x.Ticks + ScheduelDate.Ticks - Start.Ticks) * 100 / (End - Start).Ticks : 50).ToList();
            }
            set
            {
                BreakTimeOfDay = value.Select(x => TimeSpan.FromTicks(Start.Ticks + (End - Start).Ticks * x / 100 - ScheduelDate.Ticks)).ToList();
            }
        }
        public IList<string> BreakTimeDisplayNotNull
        {
            get
            {
                return BreakAt.OrderBy(x => x).Select(x => x.ToString("hh:mm tt")).ToList();
            }
        }
        public IList<string> BreakTimeDisplay
        {
            get
            {
                var result = BreakTimeDisplayNotNull;
                while (result.Count < UiBreakSlots) result.Add(null);
                return result;
            }
            set
            {
                BreakAt = ToDateTimeFromDisplayString(value).ToList();
            }
        }

        private IEnumerable<DateTime> ToDateTimeFromDisplayString(IEnumerable<string> value)
        {
            foreach (var x in value)
            {
                if (!string.IsNullOrEmpty(x))
                {
                    var token = x.IndexOf("GMT") > 0 ? x.Substring(0, x.IndexOf("GMT")) : x;
                    yield return ScheduelDate + DateTime.Parse(token).TimeOfDay;
                }
            }
        }

        public string StartTimezone { get; set; }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }

        public string RecurrenceException { get; set; }

        [WfmResourceDisplayName("Common.Employee")]
        public int EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.ScheduledJobRoleName")]
        public int CompanyJobRoleId { get; set; }
        public int CompanyShiftId { get; set; }
        public int EmployeeScheduleId { get; set; }
        public int SchedulePeriodId { get; set; }
        public bool ForDailyAdhoc { get; set; }

        public decimal TotalHours { get { return (decimal)(End - Start).TotalHours; } }

        public bool ReadOnly { get; set; }
        public DateTime ScheduelDate { get; set; }
    }
}
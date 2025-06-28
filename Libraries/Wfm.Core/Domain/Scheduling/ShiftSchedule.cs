using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Scheduling
{
    public class ShiftSchedule : BaseEntity
    {
        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public bool MondaySwitch { get; set; }
        public bool TuesdaySwitch { get; set; }
        public bool WednesdaySwitch { get; set; }
        public bool ThursdaySwitch { get; set; }
        public bool FridaySwitch { get; set; }
        public bool SaturdaySwitch { get; set; }
        public bool SundaySwitch { get; set; }
        // for biwe
        public bool? AlternativeMondaySwitch { get; set; }
        public bool? AlternativeTuesdaySwitch { get; set; }
        public bool? AlternativeWednesdaySwitch { get; set; }
        public bool? AlternativeThursdaySwitch { get; set; }
        public bool? AlternativeFridaySwitch { get; set; }
        public bool? AlternativeSaturdaySwitch { get; set; }
        public bool? AlternativeSundaySwitch { get; set; }
        //
        public long StartTimeOfDayTicks { get; set; }

        [NotMapped]
        public TimeSpan StartTimeOfDay
        {
            get { return TimeSpan.FromTicks(StartTimeOfDayTicks); }
            set { StartTimeOfDayTicks = value.Ticks; }
        }
        [NotMapped]
        public TimeSpan EndTimeOfDay
        {
            get { return TimeSpan.FromTicks(StartTimeOfDayTicks).Add(new TimeSpan((int)LengthInHours,
                (int)((LengthInHours % 1) * 60), 0)); }
        }
        public decimal LengthInHours { get; set; }

        public virtual SchedulePeriod SchedulePeriod { get; set; }
        public virtual CompanyShift CompanyShift { get; set; }

        /// <summary>
        /// Test if the shift is scheduled at day of week
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public bool IsShiftScheduled(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return this.SundaySwitch;
                case DayOfWeek.Monday:
                    return this.MondaySwitch;
                case DayOfWeek.Tuesday:
                    return this.TuesdaySwitch;
                case DayOfWeek.Wednesday:
                    return this.WednesdaySwitch;
                case DayOfWeek.Thursday:
                    return this.ThursdaySwitch;
                case DayOfWeek.Friday:
                    return this.FridaySwitch;
                case DayOfWeek.Saturday:
                    return this.SaturdaySwitch;
                default:
                    return false;
            }
        }
        public decimal TotalHours
        {
            get
            {
                var result = 0m;
                for (var date = SchedulePeriod.PeriodStartUtc.ToLocalTime().Date; date <= SchedulePeriod.PeriodEndUtc.ToLocalTime().Date; date = date.AddDays(1))
                {
                    var dayOfWeek = date.DayOfWeek;
                    if (this.IsShiftScheduled(dayOfWeek))
                    {
                        result += LengthInHours;
                    }
                }
                return result;
            }
        }
    }
}

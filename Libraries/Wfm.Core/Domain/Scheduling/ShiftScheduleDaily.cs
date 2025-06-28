using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Scheduling
{
    public class ShiftScheduleDaily : BaseEntity
    {
        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public DateTime ScheduelDate { get; set; }
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
            get
            {
                return TimeSpan.FromTicks(StartTimeOfDayTicks).Add(new TimeSpan((int)LengthInHours,
                    (int)((LengthInHours % 1) * 60), 0));
            }
        }
        public decimal LengthInHours { get; set; }

        public virtual SchedulePeriod SchedulePeriod { get; set; }
        public virtual CompanyShift CompanyShift { get; set; }
        public virtual ICollection<ShiftScheduleDailyDemandAdjustment> Adjustments { get; set; }
    }
}

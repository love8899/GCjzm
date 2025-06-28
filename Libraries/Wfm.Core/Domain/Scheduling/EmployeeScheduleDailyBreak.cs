using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Scheduling
{
    public class EmployeeScheduleDailyBreak : BaseEntity
    {
        public int EmployeeScheduleDailyId { get; set; }
        public long BreakTimeOfDayTicks { get; set; }

        [NotMapped]
        public TimeSpan BreakTimeOfDay
        {
            get { return TimeSpan.FromTicks(BreakTimeOfDayTicks); }
            set { BreakTimeOfDayTicks = value.Ticks; }
        }
        public decimal BreakLengthInMinutes { get; set; }

        public virtual EmployeeScheduleDaily EmployeeScheduleDaily { get; set; }
    }
}

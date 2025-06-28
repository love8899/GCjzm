using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wfm.Core.Domain.Common
{
    public class Shift : BaseEntity
    {
        public string ShiftName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
        public Int64? StartTimeOfDayTicks { get; set; }

        [NotMapped]
        public TimeSpan StartTimeOfDay
        {
            get { return TimeSpan.FromTicks(StartTimeOfDayTicks.GetValueOrDefault()); }
            set { StartTimeOfDayTicks = value.Ticks; }
        }
        public decimal? LengthInHours { get; set; }

        public override string ToString()
        {
            return ShiftName;
        }

        public bool EnableInRegistration { get; set; }

        public bool EnableInSchedule { get; set; }

        public int CompanyId { get; set; }
        public TimeSpan? MinStartTime { get; set; }
        public TimeSpan? MaxEndTime { get; set; }
        
    }
}

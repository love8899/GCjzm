using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Employees
{
    public class EmployeeAvailability : BaseEntity
    {
        public int EmployeeId { get; set; }
        public EmployeeAvailabilityType EmployeeAvailabilityType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool WholeDay { get; set; }
        public long StartTimeOfDayTicks { get; set; }

        [NotMapped]
        public TimeSpan StartTimeOfDay
        {
            get { return TimeSpan.FromTicks(StartTimeOfDayTicks); }
            set { StartTimeOfDayTicks = value.Ticks; }
        }
        public long EndTimeOfDayTicks { get; set; }

        [NotMapped]
        public TimeSpan EndTimeOfDay
        {
            get { return TimeSpan.FromTicks(EndTimeOfDayTicks); }
            set { EndTimeOfDayTicks = value.Ticks; }
        }
        public virtual Employee Employee { get; set; }
    }

    public enum EmployeeAvailabilityType
    {
        /// <summary>
        /// Indicate the selection of preferred work hours
        /// </summary>
        Available,
        /// <summary>
        /// Indicate the selection of unavailable hours
        /// </summary>
        Unavailable,
        ///// <summary>
        ///// Indicate the employee is neutral for the hours
        ///// </summary>
        //Neutral,
    }
}

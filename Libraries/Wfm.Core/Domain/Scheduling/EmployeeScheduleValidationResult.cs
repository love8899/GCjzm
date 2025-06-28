using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Scheduling
{
    public class EmployeeScheduleValidationResult : BaseEntity
    {
        public int EmployeeScheduleId { get; set; }
        public int? EmployeeScheduleDailyId { get; set; }
        public ScheduleWarningLevel ScheduleWarningLevel { get; set; }
        public ScheduleWarningScope ScheduleWarningScope { get; set; }
        public string ValidationResultMessage { get; set; }
        public bool Overriden { get; set; }
        public int? OverridenById { get; set; }
        public DateTime? OverridenDateTime { get; set; }

        public virtual EmployeeSchedule EmployeeSchedule { get; set; }
        public virtual EmployeeScheduleDaily EmployeeScheduleDaily { get; set; }
        public virtual Account OverridenBy { get; set; }
    }
    public enum ScheduleWarningLevel
    {
        /// <summary>
        /// Such as schedule conflicts of resource
        /// </summary>
        Error,
        /// <summary>
        /// Such as not preferred work hours of employee
        /// </summary>
        Major,
        /// <summary>
        /// Such as not employee doesn't have the job role
        /// </summary>
        Minor,
    }

    public enum ScheduleWarningScope
    {
        /// <summary>
        /// Issue is related to shift
        /// </summary>
        Shift,
        /// <summary>
        /// Issue is related to employee
        /// </summary>
        Employee,
    }
}

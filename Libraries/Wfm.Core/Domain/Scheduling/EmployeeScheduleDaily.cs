using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;

namespace Wfm.Core.Domain.Scheduling
{
    public class EmployeeScheduleDaily : BaseEntity
    {
        public int EmployeeScheduleId { get; set; }
        public DateTime ScheduelDate { get; set; }
        public long StartTimeOfDayTicks { get; set; }
        public int? ReplacementEmployeeId { get; set; }
        public int? ReplacementCompanyJobRoleId { get; set; }

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
            set
            {
                LengthInHours = (decimal)((value - StartTimeOfDay).TotalHours);
            }
        }
        public decimal LengthInHours { get; set; }

        public bool IsDeleted { get; set; }
        public virtual EmployeeSchedule EmployeeSchedule { get; set; }
        public virtual Employee ReplacementEmployee { get; set; }
        public virtual CompanyJobRole ReplacementCompanyJobRole { get; set; }
        public virtual ICollection<EmployeeScheduleValidationResult> ValidationResults { get; set; }
        public virtual ICollection<EmployeeScheduleDailyBreak> Breaks { get; set; }
    }
}

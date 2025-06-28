using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Scheduling
{
    public class SchedulePeriod : BaseEntity
    {
        public int CompanyId { get; set; }
        public int? CompanyLocationId { get; set; }
        public int? CompanyDepartmentId { get; set; }
        public DateTime PeriodStartUtc { get; set; }
        public DateTime PeriodEndUtc { get; set; }

        public int Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual CompanyLocation CompanyLocation { get; set; }
        public virtual CompanyDepartment CompanyDepartment { get; set; }
        public ICollection<ShiftSchedule> ShiftSchedules { get; set; }
        public virtual ICollection<ShiftScheduleDaily> ShiftScheduleDays { get; set; }
        public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; }
    }
}

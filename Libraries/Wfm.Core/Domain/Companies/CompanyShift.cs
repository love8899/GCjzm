using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Policies;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyShift : BaseEntity
    {
        public int CompanyId { get; set; }
        public int ShiftId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Note { get; set; } 
        public int? SchedulePolicyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual CompanyDepartment CompanyDepartment { get; set; }
        public virtual SchedulePolicy SchedulePolicy { get; set; }
        public virtual ICollection<CompanyShiftJobRole> CompanyShiftJobRoles { get; set; }
        public virtual ICollection<ShiftSchedule> ShiftSchedules { get; set; }
        public virtual ICollection<ShiftScheduleDaily> ShiftScheduleDays { get; set; }
        public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; }
    }
}

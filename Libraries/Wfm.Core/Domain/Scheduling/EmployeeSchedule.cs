using System.Collections.Generic;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Scheduling
{
    public class EmployeeSchedule : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public int JobRoleId { get; set; }
        public string Note { get; set; }
        public int? PublishedJobOrderId { get; set; }
        public bool ForDailyAdhoc { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual SchedulePeriod SchedulePeriod { get; set; }
        public virtual CompanyShift CompanyShift { get; set; }
        public virtual CompanyJobRole JobRole { get; set; }
        public virtual JobOrder PublishedJobOrder { get; set; }
        public virtual ICollection<EmployeeScheduleDaily> EmployeeScheduleDays { get; set; }
        public virtual ICollection<EmployeeScheduleValidationResult> ValidationResults { get; set; }
    }
}

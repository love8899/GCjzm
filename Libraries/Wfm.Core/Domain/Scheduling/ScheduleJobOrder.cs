using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Scheduling
{
    public class ScheduleJobOrder : BaseEntity
    {
        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public int JobRoleId { get; set; }
        public int JobOrderId { get; set; }
        public int? SupervisorId { get; set; }
        public virtual SchedulePeriod SchedulePeriod { get; set; }
        public virtual CompanyShift CompanyShift { get; set; }
        public virtual CompanyJobRole JobRole { get; set; }
        public virtual JobOrder JobOrder { get; set; }
        public virtual Account Supervisor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Scheduling
{
    public class ScheduleStatusHistory : BaseEntity
    {
        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public int JobRoleId { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int? SubmittedById { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? RevisionRequestedDate { get; set; }
        public int? RevisionRequestedById { get; set; }
        public DateTime? ResubmittedDate { get;set; }
        public int? ResubmittedById { get; set; }
        //
        public virtual SchedulePeriod SchedulePeriod { get; set; }
        public virtual CompanyShift CompanyShift { get; set; }
        public virtual CompanyJobRole JobRole { get; set; }
        public virtual Account SubmittedBy { get; set; }
        public virtual Account ApprovedBy { get; set; }
        public virtual Account RevisionRequestedBy { get; set; }
        public virtual Account ResubmittedBy { get; set; }
    }
}


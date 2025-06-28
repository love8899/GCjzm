using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.TimeSheet
{
    public class CandidateWorkOverTime : BaseEntity
    {
        public int CandidateId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int JobOrderId { get; set; }
        public int OvertimeTypeId { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal OvertimePayRate { get; set; }
        public bool IsPaid { get; set; }
        public int? Payroll_BatchId { get; set; }

        public int? Year { get; set; }
        public int? WeekOfYear { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
        public virtual OvertimeType OvertimeType { get; set; }
    }
}

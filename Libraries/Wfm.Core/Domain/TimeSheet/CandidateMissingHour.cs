using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Core.Domain.TimeSheet
{
    public class CandidateMissingHour : BaseEntity
    {
        public CandidateMissingHour()
        {
            MissingHourDocuments = new List<MissingHourDocument>();
        }

        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public int CandidateMissingHourStatusId { get; set; }
        public DateTime WorkDate { get; set; }
        public decimal OrigHours { get; set; }
        public decimal NewHours { get; set; }
        public string Note { get; set; }
        public int EnteredBy { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime? ApprovedOnUtc { get; set; }
        public int ProcessedBy { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }

        public DateTime? LastAskForApprovalOnUtc { get; set; }

        public string PayrollNote { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
        public virtual ICollection<MissingHourDocument> MissingHourDocuments { get; set; }
    }

}
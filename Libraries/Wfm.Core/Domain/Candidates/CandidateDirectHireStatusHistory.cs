using System;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateDirectHireStatusHistory : BaseEntity
    {
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public int? StatusFrom { get; set; } 
        public int StatusTo { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string Notes { get; set; } 
        public int EnteredBy { get; set; }
        public DateTime? HiredDate { get; set; }
        public decimal? Salary { get; set; }
        public bool IssueInvoice { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; } 
    }
}
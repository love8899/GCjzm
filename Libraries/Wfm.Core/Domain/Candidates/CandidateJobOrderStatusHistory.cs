using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateJobOrderStatusHistory : BaseEntity
    {
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public int? Status { get; set; }
        //public int StatusTo { get; set; }
        public int RatingValue { get; set; }
        public string RatingComment { get; set; }
        public int EnteredBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
        public virtual Account Account { get; set; }
        public virtual CandidateJobOrderStatus CandidateJobOrderStatus { get; set; }
    }
}
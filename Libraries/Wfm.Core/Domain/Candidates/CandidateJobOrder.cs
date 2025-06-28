using System;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateJobOrder : BaseEntity
    {
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public int CandidateJobOrderStatusId { get; set; }
        public int RatingValue { get; set; }
        public string RatingComment { get; set; }
        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }
        public string RatedBy { get; set; }
        public int EnteredBy { get; set; }
        public string Note { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } 

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
        public virtual CandidateJobOrderStatus CandidateJobOrderStatus { get; set; }
    }

    public class CandidateAvailableDays
    {
        public int CandidateId { get; set; }
        public int AvailableDays { get; set; }
    }


    public class DailyPlacement : BaseEntity
    {
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CandidateJobOrderStatusId { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }
    }

}

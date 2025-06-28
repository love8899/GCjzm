using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateJobHistory 

    {
        public int Id { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }


        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }
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
     

        public string CompanyName { get; set; }

        public string JobOrderStatusName { get; set; }

        public string JobTitle { get;set; }

        public DateTime JobOrderStartDate { get; set; }
        public DateTime? JobOrderEndDate { get; set; } 

        public virtual CandidateJobOrderStatus CandidateJobOrderStatus { get; set; }
        
    }
}

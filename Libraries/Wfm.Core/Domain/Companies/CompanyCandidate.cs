using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyCandidate : BaseEntity
    {
        public int CompanyId { get; set; }
        public int CandidateId { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ReasonForLeave { get; set; }
        public string Note { get; set; }
        public int? RatingValue { get; set; }
        public string RatingComment { get; set; }
        public string RatedBy { get; set; }
        public int? PreferredLocationId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Candidate Candidate { get; set; }
        public virtual CompanyLocation PreferredLocation { get; set; }
    }
}

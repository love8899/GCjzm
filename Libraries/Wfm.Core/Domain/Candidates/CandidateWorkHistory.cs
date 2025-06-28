using System;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateWorkHistory : BaseEntity
    {
        public int CandidateId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string CompanyUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Note { get; set; }

        public virtual Candidate Candidate { get; set; }
    }
}
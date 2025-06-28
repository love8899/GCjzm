using System;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateBlacklist : BaseEntity
    {
        public int CandidateId { get; set; }

        public int? ClientId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string BannedReason { get; set; }
        public int EnteredBy { get; set; }
        public string Note { get; set; }
        public string ClientName { get; set; }

        public int? JobOrderId { get; set; }

        public virtual Candidate Candidate { get; set; }
    }

}

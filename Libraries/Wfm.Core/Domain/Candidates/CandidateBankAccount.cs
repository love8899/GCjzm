using Wfm.Core.Domain.Common;


namespace Wfm.Core.Domain.Candidates
{
    public class CandidateBankAccount : BaseEntity
    {
        public int CandidateId { get; set; }
        public string InstitutionNumber { get; set; }
        public string TransitNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Note { get; set; }
        public int EnteredBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Candidate Candidate { get; set; }
    }
}
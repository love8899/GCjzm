namespace Wfm.Core.Domain.Candidates
{
    public class CandidateJobOrderStatus : BaseEntity
    {
        public string StatusName { get; set; }
        public bool CanBeScheduled { get; set; }
        public bool TriggersEmail { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool ForDirectHire { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}
namespace Wfm.Core.Domain.Logging
{
    public partial class CandidateActivityLog : BaseEntity
    {
        public int ActivityLogTypeId { get; set; }
        public int? CandidateId { get; set; }
        public string CandidateName { get; set; }

        public int? FranchiseId { get; set; }
        public string FranchiseName { get; set; }

        public string ActivityLogDetail { get; set; }
        

        
        public virtual ActivityLogType ActivityLogType { get; set; }
    }
}
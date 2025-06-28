using Wfm.Core.Domain.Messages;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateMessageNotification : BaseEntity
    {
        public int CandidateId { get; set; }
        public int MessageTemplateId { get; set; }
        public bool IsSubscribed { get; set; }
        public string Note { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual MessageTemplate MessageTemplate { get; set; }
    }
}

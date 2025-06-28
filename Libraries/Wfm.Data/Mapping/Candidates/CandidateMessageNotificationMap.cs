using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateMessageNotificationMap : EntityTypeConfiguration<CandidateMessageNotification>
    {

        public CandidateMessageNotificationMap()
        {
            this.ToTable("CandidateMessageNotification");
            this.HasKey(c => c.Id);

        }
    }
}

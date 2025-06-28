using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;

namespace Wfm.Data.Mapping.Messages
{
    public class ConfirmationEmailLinkMap : EntityTypeConfiguration<ConfirmationEmailLink>
    {
        public ConfirmationEmailLinkMap()
        {
            this.ToTable("ConfirmationEmailLink");
            this.HasKey(ea => ea.Id);
            this.HasRequired(x => x.Candidate).WithMany().HasForeignKey(x => x.CandidateId);
            this.HasRequired(x => x.JobOrder).WithMany().HasForeignKey(x => x.JobOrderId);
        }
    }
}

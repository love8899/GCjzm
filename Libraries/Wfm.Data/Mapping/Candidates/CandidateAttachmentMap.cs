using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Media
{
    public partial class CandidateAttachmentMap : EntityTypeConfiguration<CandidateAttachment>
    {
        public CandidateAttachmentMap()
        {
            this.ToTable("CandidateAttachment");
            this.HasKey(ca => ca.Id);

            this.Property(ca => ca.AttachmentName).HasMaxLength(255);

            this.HasRequired(ca => ca.AttachmentType)
                .WithMany()
                .HasForeignKey(ca => ca.AttachmentTypeId)
                .WillCascadeOnDelete(false);

            this.HasRequired(ca => ca.DocumentType)
                .WithMany()
                .HasForeignKey(ca => ca.DocumentTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Messages;

namespace Wfm.Data.Mapping.Messages
{
    public partial class MessageHistoryMap : EntityTypeConfiguration<MessageHistory>
    {
        public MessageHistoryMap()
        {
            this.ToTable("MessageHistory");
            this.HasKey(qe => qe.Id);

            this.Property(qe => qe.MailFrom).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.FromName).HasMaxLength(500);
            this.Property(qe => qe.MailTo).IsRequired();
            this.Property(qe => qe.ToName).HasMaxLength(500);
            this.Property(qe => qe.CC).HasMaxLength(500);
            this.Property(qe => qe.Bcc).HasMaxLength(4000);
            this.Property(qe => qe.Subject).HasMaxLength(1600);
            this.HasOptional(qe => qe.ConfirmationEmailLink).WithMany().HasForeignKey(qe => qe.ConfirmationEmailLinkId);
        }
    }
}
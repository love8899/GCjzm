using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Forums;

namespace Wfm.Data.Mapping.Forums
{
    public partial class PrivateMessageMap : EntityTypeConfiguration<PrivateMessage>
    {
        public PrivateMessageMap()
        {
            this.ToTable("Forums_PrivateMessage");
            this.HasKey(pm => pm.Id);
            this.Property(pm => pm.Subject).IsRequired().HasMaxLength(450);
            this.Property(pm => pm.Text).IsRequired();

            this.HasRequired(pm => pm.FromAccount)
               .WithMany()
               .HasForeignKey(pm => pm.FromAccountId)
               .WillCascadeOnDelete(false);

            this.HasRequired(pm => pm.ToAccount)
               .WithMany()
               .HasForeignKey(pm => pm.ToAccountId)
               .WillCascadeOnDelete(false);
        }
    }
}

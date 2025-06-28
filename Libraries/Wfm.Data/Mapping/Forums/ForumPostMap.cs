using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Forums;

namespace Wfm.Data.Mapping.Forums
{
    public partial class ForumPostMap : EntityTypeConfiguration<ForumPost>
    {
        public ForumPostMap()
        {
            this.ToTable("Forums_Post");
            this.HasKey(fp => fp.Id);
            this.Property(fp => fp.Text).IsRequired();
            this.Property(fp => fp.IPAddress).HasMaxLength(100);

            this.HasRequired(fp => fp.ForumTopic)
                .WithMany()
                .HasForeignKey(fp => fp.TopicId);

            this.HasRequired(fp => fp.Account)
               .WithMany()
               .HasForeignKey(fp => fp.AccountId)
               .WillCascadeOnDelete(false);
        }
    }
}

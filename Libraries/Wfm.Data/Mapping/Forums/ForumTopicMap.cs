using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Forums;

namespace Wfm.Data.Mapping.Forums
{
    public partial class ForumTopicMap : EntityTypeConfiguration<ForumTopic>
    {
        public ForumTopicMap()
        {
            this.ToTable("Forums_Topic");
            this.HasKey(ft => ft.Id);
            this.Property(ft => ft.Subject).IsRequired().HasMaxLength(450);
            this.Ignore(ft => ft.ForumTopicType);

            this.HasRequired(ft => ft.Forum)
                .WithMany()
                .HasForeignKey(ft => ft.ForumId);

            this.HasRequired(ft => ft.Account)
               .WithMany()
               .HasForeignKey(ft => ft.AccountId)
               .WillCascadeOnDelete(false);
        }
    }
}

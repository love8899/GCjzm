using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Forums;

namespace Wfm.Data.Mapping.Forums
{
    public partial class ForumSubscriptionMap : EntityTypeConfiguration<ForumSubscription>
    {
        public ForumSubscriptionMap()
        {
            this.ToTable("Forums_Subscription");
            this.HasKey(fs => fs.Id);

            this.HasRequired(fs => fs.Account)
                .WithMany()
                .HasForeignKey(fs => fs.AccountId)
                .WillCascadeOnDelete(false);
        }
    }
}

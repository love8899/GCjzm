using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public class AccountPasswordHistoryMap: EntityTypeConfiguration<AccountPasswordHistory>
    {
        public AccountPasswordHistoryMap()
        {
            this.ToTable("AccountPasswordHistory");
            this.HasKey(x => x.Id);
            this.Property(x => x.AccountId).IsRequired();
        }
    }
}

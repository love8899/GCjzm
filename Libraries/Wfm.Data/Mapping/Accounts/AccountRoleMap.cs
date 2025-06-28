using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public partial class AccountRoleMap : EntityTypeConfiguration<AccountRole>
    {
        public AccountRoleMap()
        {
            this.ToTable("AccountRole");
            this.HasKey(c => c.Id);

            this.Property(c => c.AccountRoleName).IsRequired().HasMaxLength(255);
            this.Property(c => c.SystemName).HasMaxLength(255);

            this.Property(c => c.ClientName).HasMaxLength(255);
        }
    }
}


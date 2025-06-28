using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public class AccountPasswordPolicyMap : EntityTypeConfiguration<AccountPasswordPolicy>
    {
        public AccountPasswordPolicyMap()
        {
            this.ToTable("AccountType_PasswordPolicy_Mapping");
            this.HasKey(x => x.Id);
            this.Property(x => x.AccountType).IsRequired();
            this.Property(x => x.PasswordPolicyId).IsRequired();
            this.HasRequired(x => x.PasswordPolicy).WithMany().HasForeignKey(x => x.PasswordPolicyId);
        }
    }
}

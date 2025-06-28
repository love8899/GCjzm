using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public class AccountDelegateMap : EntityTypeConfiguration<AccountDelegate>
    {
        public AccountDelegateMap()
        {
            this.ToTable("AccountDelegate");
            this.HasKey(c => c.Id);

            this.Property(c => c.Remark).HasMaxLength(1024);

            this.HasRequired(c => c.Account)
                .WithMany()
                .HasForeignKey(c => c.AccountId);
            this.HasRequired(c => c.DelegateAccount)
                .WithMany()
                .HasForeignKey(c => c.DelegateAccountId);
        }
    }
}

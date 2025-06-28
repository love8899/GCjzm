using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public class AccountDelegateHistoryMap : EntityTypeConfiguration<AccountDelegateHistory>
    {
        public AccountDelegateHistoryMap()
        {
            this.ToTable("AccountDelegateHistory");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.AccountDelegate)
                .WithMany(d => d.Histories)
                .HasForeignKey(c => c.AccountDelegateId);
            this.HasRequired(c => c.DelegateAccount)
                .WithMany()
                .HasForeignKey(c => c.DelegateAccountId);
        }
    }
}

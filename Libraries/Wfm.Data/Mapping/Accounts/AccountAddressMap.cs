using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public partial class AccountAddressMap : EntityTypeConfiguration<AccountAddress>
    {

        public AccountAddressMap()
        {
            this.ToTable("AccountAddress");
            this.HasKey(c => c.Id);

            this.HasRequired(aa => aa.AddressType)
                .WithMany()
                .HasForeignKey(aa => aa.AddressTypeId).WillCascadeOnDelete(false);
        }
    }
}

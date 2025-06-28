using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Data.Mapping.Accounts
{
    public partial class AccountMap : EntityTypeConfiguration<Account>
    {
        public AccountMap()
        {
            this.ToTable("Account");
            this.HasKey(u => u.Id);

            this.Property(u => u.AccountGuid).IsRequired();
            this.Property(u => u.Username).HasMaxLength(255);
            this.Property(u => u.Email).HasMaxLength(255);

            this.Property(u => u.FirstName).HasMaxLength(255);
            this.Property(u => u.LastName).HasMaxLength(255);

            this.Ignore(u => u.PasswordFormat);

            this.HasMany(u => u.AccountRoles)
                .WithMany()
                .Map(m => m.ToTable("Account_AccountRole_Mapping"));

        }
    }
}


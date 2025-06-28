using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class BankMap : EntityTypeConfiguration<Bank>
    {
        public BankMap()
        {
            this.ToTable("Bank");
            this.HasKey(c => c.Id);

            this.Property(c => c.BankName).IsRequired().HasMaxLength(255);
        }
    }
}

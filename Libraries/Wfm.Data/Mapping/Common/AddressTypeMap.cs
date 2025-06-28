using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public class AddressTypeMap :EntityTypeConfiguration<AddressType>
    {
        public AddressTypeMap()
        {
            this.ToTable("AddressType");
            this.HasKey(c => c.Id);

            this.Property(c => c.AddressTypeName).IsRequired().HasMaxLength(255);
        }
    }
}

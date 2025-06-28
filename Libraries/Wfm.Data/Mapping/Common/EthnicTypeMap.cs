using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class EthnicTypeMap :EntityTypeConfiguration<EthnicType>
    {
        public EthnicTypeMap()
        {
            this.ToTable("EthnicType");
            this.HasKey(c=> c.Id);

            this.Property(c => c.EthnicTypeName).IsRequired().HasMaxLength(255);
        }
    }
}

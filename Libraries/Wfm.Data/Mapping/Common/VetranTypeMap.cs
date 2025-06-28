using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;


namespace Wfm.Data.Mapping.Common
{
    public partial class VetranTypeMap : EntityTypeConfiguration<VetranType>
    {
        public VetranTypeMap()
        {
            this.ToTable("VetranType");
            this.HasKey(c => c.Id);

            this.Property(c => c.VetranTypeName).IsRequired().HasMaxLength(255);
        }
    }
}

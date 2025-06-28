using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Data.Mapping.JobOrders
{
    public partial class FeeTypeMap : EntityTypeConfiguration<FeeType>
    {
        public FeeTypeMap()
        {
            this.ToTable("FeeType");
            this.HasKey(ft => ft.Id);

            this.Property(ft => ft.FeeTypeName).IsRequired().HasMaxLength(255);
        }
    }
}

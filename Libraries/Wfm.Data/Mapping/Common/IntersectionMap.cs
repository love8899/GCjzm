using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class IntersectionMap : EntityTypeConfiguration<Intersection>
    {
        public IntersectionMap()
        {
            this.ToTable("Intersection");
            this.HasKey(c => c.Id);

            this.Property(c => c.IntersectionName).IsRequired().HasMaxLength(255);
        }
    }
}

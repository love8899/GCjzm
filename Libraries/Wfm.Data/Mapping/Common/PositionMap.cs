using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class PositionMap : EntityTypeConfiguration<Position>
    {
        public PositionMap()
        {
            this.ToTable("Position");
            this.HasKey(c=> c.Id);

            this.Property(c => c.Code).IsRequired().HasMaxLength(20);
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);

            this.HasRequired(c => c.Company).WithMany(x => x.Positions).HasForeignKey(x => x.CompanyId);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class ShiftMap : EntityTypeConfiguration<Shift>
    {
        public ShiftMap()
        {
            this.ToTable("Shift");
            this.HasKey(c=> c.Id);

            this.Property(c => c.ShiftName).IsRequired().HasMaxLength(255);
        }
    }
}

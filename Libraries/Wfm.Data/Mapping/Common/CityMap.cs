using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial  class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            this.ToTable("City");
            this.HasKey(c => c.Id);

            this.Property(c => c.CityName).IsRequired().HasMaxLength(255);

            this.HasRequired(c => c.StateProvince)
                .WithMany(sp => sp.Cities)
                .HasForeignKey(c => c.StateProvinceId);
        }
    }
}

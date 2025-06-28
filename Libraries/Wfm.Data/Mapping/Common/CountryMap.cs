using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class CountryMap : EntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            this.ToTable("Country");
            this.HasKey(c => c.Id);
            
            this.Property(c => c.CountryName).IsRequired().HasMaxLength(255);

            this.Property(c => c.TwoLetterIsoCode).HasMaxLength(2);
            this.Property(c => c.ThreeLetterIsoCode).HasMaxLength(3);
        }
    }
}

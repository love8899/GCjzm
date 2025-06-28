using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public class StatutoryHolidayMap : EntityTypeConfiguration<StatutoryHoliday>
    {
        public StatutoryHolidayMap()
        {
            this.ToTable("StatutoryHoliday");
            this.HasKey(c => c.Id);

            this.Property(c => c.StatutoryHolidayName).IsRequired().HasMaxLength(50);
            this.Property(c => c.Note).IsOptional().HasMaxLength(512);

            this.HasRequired(c => c.StateProvince)
                .WithMany(x => x.StatutoryHolidays)
                .HasForeignKey(c => c.StateProvinceId);
        }
    }
}


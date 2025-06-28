using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.ClockTime;

namespace Wfm.Data.Mapping.TimeClocks
{
    public partial class CompanyClockDeviceMap : EntityTypeConfiguration<CompanyClockDevice>
    {
        public CompanyClockDeviceMap()
        {
            this.ToTable("CompanyClockDevice");
            this.HasKey(ccd => ccd.Id);

            this.Property(ccd => ccd.ClockDeviceUid).IsRequired().HasMaxLength(255);
        }
    }
}

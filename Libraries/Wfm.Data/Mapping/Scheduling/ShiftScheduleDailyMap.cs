using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class ShiftScheduleDailyMap : EntityTypeConfiguration<ShiftScheduleDaily>
    {
        public ShiftScheduleDailyMap()
        {
            this.ToTable("ShiftScheduleDaily");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.SchedulePeriod)
                .WithMany(c => c.ShiftScheduleDays)
                .HasForeignKey(c => c.SchedulePeriodId);
            this.HasRequired(c => c.CompanyShift)
                .WithMany(c => c.ShiftScheduleDays)
                .HasForeignKey(c => c.CompanyShiftId);
        }
    }
}

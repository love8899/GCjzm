using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class ShiftScheduleMap : EntityTypeConfiguration<ShiftSchedule>
    {
        public ShiftScheduleMap()
        {
            this.ToTable("ShiftSchedule");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.SchedulePeriod)
                .WithMany(c => c.ShiftSchedules)
                .HasForeignKey(c => c.SchedulePeriodId);
            this.HasRequired(c => c.CompanyShift)
                .WithMany(c => c.ShiftSchedules)
                .HasForeignKey(c => c.CompanyShiftId);
        }
    }
}

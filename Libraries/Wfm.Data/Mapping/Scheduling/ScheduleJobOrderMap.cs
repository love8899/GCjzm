using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class ScheduleJobOrderMap : EntityTypeConfiguration<ScheduleJobOrder>
    {
        public ScheduleJobOrderMap()
        {
            this.ToTable("ScheduleJobOrder");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.SchedulePeriod)
                .WithMany()
                .HasForeignKey(c => c.SchedulePeriodId);
            this.HasRequired(c => c.CompanyShift)
                .WithMany()
                .HasForeignKey(c => c.CompanyShiftId);
            this.HasRequired(c => c.JobRole)
                .WithMany()
                .HasForeignKey(c => c.JobRoleId);
            this.HasRequired(c => c.JobOrder)
                .WithMany()
                .HasForeignKey(c => c.JobOrderId);
            this.HasOptional(c => c.Supervisor)
                .WithMany()
                .HasForeignKey(c => c.SupervisorId);
        }
    }
}

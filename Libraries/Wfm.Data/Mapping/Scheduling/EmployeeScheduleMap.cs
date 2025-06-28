using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;


namespace Wfm.Data.Mapping.Scheduling
{
    public class EmployeeScheduleMap : EntityTypeConfiguration<EmployeeSchedule>
    {
        public EmployeeScheduleMap()
        {
            this.ToTable("EmployeeSchedule");
            this.HasKey(a => a.Id);
            this.Property(a => a.Note).HasMaxLength(1024);

            this.HasRequired(c => c.Employee)
                .WithMany(x => x.EmployeeSchedules)
                .HasForeignKey(c => c.EmployeeId);
            this.HasRequired(c => c.SchedulePeriod)
                .WithMany(x => x.EmployeeSchedules)
                .HasForeignKey(c => c.SchedulePeriodId);
            this.HasRequired(c => c.JobRole)
                .WithMany(x => x.EmployeeSchedules)
                .HasForeignKey(c => c.JobRoleId);
            this.HasRequired(c => c.CompanyShift)
                .WithMany(x => x.EmployeeSchedules)
                .HasForeignKey(c => c.CompanyShiftId);
            this.HasOptional(c => c.PublishedJobOrder)
                .WithMany(x => x.EmployeeSchedules)
                .HasForeignKey(c => c.PublishedJobOrderId);
        }
    }
}

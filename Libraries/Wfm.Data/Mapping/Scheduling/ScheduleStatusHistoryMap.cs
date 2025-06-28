using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class ScheduleStatusHistoryMap : EntityTypeConfiguration<ScheduleStatusHistory>
    {
        public ScheduleStatusHistoryMap()
        {
            this.ToTable("ScheduleStatusHistory");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.SchedulePeriod)
                .WithMany()
                .HasForeignKey(c => c.SchedulePeriodId);
            this.HasRequired(c => c.JobRole)
                .WithMany()
                .HasForeignKey(c => c.JobRoleId);
            this.HasRequired(c => c.CompanyShift)
                .WithMany()
                .HasForeignKey(c => c.CompanyShiftId);
            this.HasOptional(c => c.SubmittedBy)
                .WithMany()
                .HasForeignKey(c => c.SubmittedById);
            this.HasOptional(c => c.ApprovedBy)
                .WithMany()
                .HasForeignKey(c => c.ApprovedById);
            this.HasOptional(c => c.RevisionRequestedBy)
                .WithMany()
                .HasForeignKey(c => c.RevisionRequestedById);
            this.HasOptional(c => c.ResubmittedBy)
                .WithMany()
                .HasForeignKey(c => c.ResubmittedById);
        }
    }
}

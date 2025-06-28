using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class EmployeeScheduleDailyMap : EntityTypeConfiguration<EmployeeScheduleDaily>
    {
        public EmployeeScheduleDailyMap()
        {
            this.ToTable("EmployeeScheduleDaily");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.EmployeeSchedule)
                .WithMany(x => x.EmployeeScheduleDays)
                .HasForeignKey(c => c.EmployeeScheduleId);
            this.HasOptional(c => c.ReplacementEmployee)
                .WithMany()
                .HasForeignKey(c => c.ReplacementEmployeeId);
            this.HasOptional(c => c.ReplacementCompanyJobRole)
                .WithMany()
                .HasForeignKey(c => c.ReplacementCompanyJobRoleId);
        }
    }
}

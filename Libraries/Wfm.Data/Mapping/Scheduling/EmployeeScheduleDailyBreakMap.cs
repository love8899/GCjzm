using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class EmployeeScheduleDailyBreakMap : EntityTypeConfiguration<EmployeeScheduleDailyBreak>
    {
        public EmployeeScheduleDailyBreakMap()
        {
            this.ToTable("EmployeeScheduleDailyBreak");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.EmployeeScheduleDaily)
                .WithMany(x => x.Breaks)
                .HasForeignKey(c => c.EmployeeScheduleDailyId);
        }
    }
}

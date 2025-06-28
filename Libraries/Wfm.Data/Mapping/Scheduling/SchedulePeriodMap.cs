using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Data.Mapping.Scheduling
{
    public class SchedulePeriodMap : EntityTypeConfiguration<SchedulePeriod>
    {
        public SchedulePeriodMap()
        {
            this.ToTable("SchedulePeriod");
            this.HasKey(a => a.Id);

            this.HasRequired(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId);

            this.HasOptional(c => c.CompanyLocation)
                .WithMany()
                .HasForeignKey(c => c.CompanyLocationId);

            this.HasOptional(c => c.CompanyDepartment)
                .WithMany()
                .HasForeignKey(c => c.CompanyDepartmentId);
        }
    }
}

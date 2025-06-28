using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyShiftMap : EntityTypeConfiguration<CompanyShift>
    {
        public CompanyShiftMap()
        {
            this.ToTable("CompanyShift");
            this.HasKey(cjr => cjr.Id);

            this.Property(cjr => cjr.Note).HasMaxLength(2048);

            this.HasRequired(cjr => cjr.Company)
                .WithMany(c => c.CompanyShifts)
                .HasForeignKey(cjr => cjr.CompanyId);
            this.HasRequired(cjr => cjr.Shift)
                .WithMany()
                .HasForeignKey(cjr => cjr.ShiftId);
            this.HasRequired(cjr => cjr.CompanyDepartment)
                .WithMany(l => l.CompanyShifts)
                .HasForeignKey(cjr => cjr.CompanyDepartmentId);

            this.HasOptional(cjr => cjr.SchedulePolicy)
                .WithMany()
                .HasForeignKey(cjr => cjr.SchedulePolicyId);
        }
    }
}

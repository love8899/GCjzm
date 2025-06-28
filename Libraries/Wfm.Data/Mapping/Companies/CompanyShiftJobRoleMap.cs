using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyShiftJobRoleMap : EntityTypeConfiguration<CompanyShiftJobRole>
    {
        public CompanyShiftJobRoleMap()
        {
            this.ToTable("CompanyShiftJobRole");
            this.HasKey(cjr => cjr.Id);

            this.HasRequired(cjr => cjr.CompanyShift)
                .WithMany(c => c.CompanyShiftJobRoles)
                .HasForeignKey(cjr => cjr.CompanyShiftId);
            this.HasRequired(cjr => cjr.CompanyJobRole)
                .WithMany()
                .HasForeignKey(cjr => cjr.CompanyJobRoleId);
            this.HasOptional(cjr => cjr.Supervisor)
                .WithMany()
                .HasForeignKey(cjr => cjr.SupervisorId);
        }
    }
}

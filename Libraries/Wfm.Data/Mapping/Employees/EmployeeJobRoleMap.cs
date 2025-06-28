using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeJobRoleMap : EntityTypeConfiguration<EmployeeJobRole>
    {
        public EmployeeJobRoleMap()
        {
            this.ToTable("EmployeeJobRole");
            this.HasKey(e => e.Id);
            //
            this.HasRequired(x => x.Employee)
                .WithMany(y => y.EmployeeJobRoles)
                .HasForeignKey(x => x.EmployeeId);
            this.HasRequired(x => x.CompanyJobRole)
                .WithMany(y => y.EmployeeJobRoles)
                .HasForeignKey(x => x.CompanyJobRoleId);
        }
    }
}

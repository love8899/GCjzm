using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public partial class CompanyDepartmentMap : EntityTypeConfiguration<CompanyDepartment>
    {
        public CompanyDepartmentMap()
        {
            this.ToTable("CompanyDepartment");
            this.HasKey(cl => cl.Id);

            this.Property(cl => cl.DepartmentName).IsRequired().HasMaxLength(255);

            this.HasRequired(cl => cl.Company)
                .WithMany(c => c.CompanyDepartments)
                .HasForeignKey(cl => cl.CompanyId);
            this.HasRequired(cl => cl.CompanyLocation)
                .WithMany()
                .HasForeignKey(cl => cl.CompanyLocationId);
        }
    }
}

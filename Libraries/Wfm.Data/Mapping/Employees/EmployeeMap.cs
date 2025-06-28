using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            this.ToTable("Employee");
            this.HasKey(e => e.Id);
            //
            this.HasRequired(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId);
            this.HasOptional(x => x.CompanyLocation)
                .WithMany()
                .HasForeignKey(x => x.CompanyLocationId);
            this.HasOptional(x => x.EthnicType)
                .WithMany()
                .HasForeignKey(x => x.EthnicTypeId);
            this.HasOptional(x => x.Gender)
                .WithMany()
                .HasForeignKey(x => x.GenderId);
            this.HasRequired(x => x.Salutation)
                .WithMany()
                .HasForeignKey(x => x.SalutationId);
            this.HasRequired(x => x.EnteredByAccount)
                .WithMany()
                .HasForeignKey(x => x.EnteredBy);
            this.HasOptional(x => x.VetranType)
                .WithMany()
                .HasForeignKey(x => x.VetranTypeId);
        }
    }
}

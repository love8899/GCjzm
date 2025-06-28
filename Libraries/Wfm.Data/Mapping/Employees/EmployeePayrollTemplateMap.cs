using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Employees;


namespace Wfm.Data.Mapping.Employees
{
    public class EmployeePayrollTemplateMap : EntityTypeConfiguration<EmployeePayrollTemplate>
    {
        public EmployeePayrollTemplateMap()
        {
            this.ToTable("Employee_Payroll_Template");
            this.HasKey(x => x.Id);

            this.Property(x => x.Hours).HasPrecision(10, 4);
            this.Property(x => x.Rate).HasPrecision(10, 4);

            //this.HasRequired(x => x.Employee)
            //    .WithMany()
            //    .HasForeignKey(x => x.EmployeeId);

            //this.HasRequired(x => x.PayrollItem)
            //    .WithMany()
            //    .HasForeignKey(x => x.PayrollItemId);
        }
    }
}

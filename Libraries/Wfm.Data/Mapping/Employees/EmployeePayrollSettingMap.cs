using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Employees;

namespace Wfm.Data.Mapping.Employees
{
    public class EmployeePayrollSettingMap : EntityTypeConfiguration<EmployeePayrollSetting>
    {
        public EmployeePayrollSettingMap()
        {
            this.ToTable("EmployeePayrollSetting");
            this.HasKey(e => e.Id);

            //
            this.HasRequired(x => x.Employee)
                .WithMany(x => x.EmployeePayrollSettings)
                .HasForeignKey(x => x.EmployeeId);
            //this.HasOptional(x => x.PayGroup)
            //    .WithMany()
            //    .HasForeignKey(x => x.PayGroupId);
            this.HasOptional(x => x.TaxProvince)
                .WithMany()
                .HasForeignKey(x => x.TaxProvinceId);
        }
    }
}

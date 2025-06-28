using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class EmployeePayrollSettingMap : EntityTypeConfiguration<EmployeePayrollSetting>
    {
        public EmployeePayrollSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PayStubPassword)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("EmployeePayrollSetting");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.FirstHireDate).HasColumnName("FirstHireDate");
            this.Property(t => t.LastHireDate).HasColumnName("LastHireDate");
            this.Property(t => t.TerminationDate).HasColumnName("TerminationDate");
            this.Property(t => t.Tax_Exempt).HasColumnName("Tax_Exempt");
            this.Property(t => t.CPP_Exempt).HasColumnName("CPP_Exempt");
            this.Property(t => t.EI_Exempt).HasColumnName("EI_Exempt");
            this.Property(t => t.QPIP_Exempt).HasColumnName("QPIP_Exempt");
            this.Property(t => t.PayGroupId).HasColumnName("PayGroupId");
            this.Property(t => t.TaxProvinceId).HasColumnName("TaxProvinceId");
            this.Property(t => t.PayStubPassword).HasColumnName("PayStubPassword");
            this.Property(t => t.AccrueVacation).HasColumnName("AccrueVacation");
            this.Property(t => t.VacationRate).HasColumnName("VacationRate");

            // Relationships
            this.HasOptional(t => t.PayGroup)
                .WithMany(t => t.EmployeePayrollSettings)
                .HasForeignKey(d => d.PayGroupId);

        }
    }
}

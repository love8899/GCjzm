using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class Payroll_CalendarMap : EntityTypeConfiguration<Payroll_Calendar>
    {
        public Payroll_CalendarMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Payroll_Calendar");
           // this.Property(t => t.Id).HasColumnName("Id");
            //this.Property(t => t.PayGroupId).HasColumnName("PayGroupId");
            //this.Property(t => t.PayPeriodNumber).HasColumnName("PayPeriodNumber");
            //this.Property(t => t.PayPeriodStartDate).HasColumnName("PayPeriodStartDate");
            //this.Property(t => t.PayPeriodEndDate).HasColumnName("PayPeriodEndDate");
            //this.Property(t => t.PayPeriodPayDate).HasColumnName("PayPeriodPayDate");
            //this.Property(t => t.PayPeriodCommitDate).HasColumnName("PayPeriodCommitDate");

            // Relationships
            this.HasRequired(t => t.PayGroup)
                .WithMany(t => t.Payroll_Calendar)
                .HasForeignKey(d => d.PayGroupId);

        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class Payroll_BatchMap : EntityTypeConfiguration<Payroll_Batch>
    {
        public Payroll_BatchMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Status)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Payroll_Batch");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.LastUpdateDate).HasColumnName("LastUpdateDate");
            this.Property(t => t.Payroll_CalendarId).HasColumnName("Payroll_CalendarId");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.PayDate).HasColumnName("PayDate");

            // Relationships
            this.HasRequired(t => t.Payroll_Calendar)
                .WithMany(t => t.Payroll_Batch)
                .HasForeignKey(d => d.Payroll_CalendarId);

        }
    }
}

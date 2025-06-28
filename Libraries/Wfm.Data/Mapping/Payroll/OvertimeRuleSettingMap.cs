using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class OvertimeRuleSettingMap : EntityTypeConfiguration<OvertimeRuleSetting>
    {
        public OvertimeRuleSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("OvertimeRuleSetting");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.TypeId).HasColumnName("TypeId");
            this.Property(t => t.ApplyAfter).HasColumnName("ApplyAfter");
            this.Property(t => t.PayrollItemId).HasColumnName("PayrollItemId");
            this.Property(t => t.Rate).HasColumnName("Rate");

            // Relationships
            this.HasRequired(t => t.Payroll_Item)
                .WithMany(t => t.OvertimeRuleSettings)
                .HasForeignKey(d => d.PayrollItemId);
            this.HasRequired(t => t.OvertimeType)
                .WithMany()
                .HasForeignKey(d => d.TypeId);

        }
    }
}

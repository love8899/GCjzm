using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class Payroll_Item_SubTypeMap : EntityTypeConfiguration<Payroll_Item_SubType>
    {
        public Payroll_Item_SubTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.UnitName)
                .HasMaxLength(20);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Payroll_Item_SubType");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.UnitName).HasColumnName("UnitName");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.IsInternal).HasColumnName("IsInternal");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class Payroll_Item_TypeMap : EntityTypeConfiguration<Payroll_Item_Type>
    {
        public Payroll_Item_TypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Description)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Payroll_Item_Type");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.IsInternal).HasColumnName("IsInternal");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Sort_Order).HasColumnName("Sort_Order");
        }
    }
}

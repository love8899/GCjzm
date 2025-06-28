using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class Payroll_ItemMap : EntityTypeConfiguration<Payroll_Item>
    {
        public Payroll_ItemMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(4);

            this.Property(t => t.Description)
                .HasMaxLength(30);

            this.Property(t => t.State_Code)
                .HasMaxLength(3);


            // Table & Column Mappings
            this.ToTable("Payroll_Item");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.TypeID).HasColumnName("TypeID");
            this.Property(t => t.SubTypeId).HasColumnName("SubTypeId");
            this.Property(t => t.State_Code).HasColumnName("State_Code");
            this.Property(t => t.PrintOnPayStub).HasColumnName("PrintOnPayStub");
            this.Property(t => t.IsReadOnly).HasColumnName("IsReadOnly");
            this.Property(t => t.IsTaxable).HasColumnName("IsTaxable");
            this.Property(t => t.IsPensionable).HasColumnName("IsPensionable");
            this.Property(t => t.IsInsurable).HasColumnName("IsInsurable");
            this.Property(t => t.PayOutItemId).HasColumnName("PayOutItemId");
            this.Property(t => t.BalanceItemId).HasColumnName("BalanceItemId");
            this.Property(t => t.Rate).HasColumnName("Rate");

            // Relationships
            this.HasRequired(t => t.Payroll_Item_SubType)
                .WithMany(t => t.Payroll_Item)
                .HasForeignKey(d => d.SubTypeId);
            this.HasRequired(t => t.Payroll_Item_Type)
                .WithMany(t => t.Payroll_Item)
                .HasForeignKey(d => d.TypeID);

            this.HasMany(x => x.TaxForms).WithMany(x => x.PayrollItems).Map(x => x.ToTable("PayrollItem_TaxFormBoxes_Mapping").MapLeftKey("PayrollItemId").MapRightKey("TaxFormBoxId"));

        }
    }
}

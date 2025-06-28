using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class PayGroupMap : EntityTypeConfiguration<PayGroup>
    {
        public PayGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("PayGroup");


            // Relationships
            this.HasRequired(t => t.PayFrequencyType)
                .WithMany(t => t.PayGroups)
                .HasForeignKey(d => d.PayFrequencyTypeId);

        }
    }
}

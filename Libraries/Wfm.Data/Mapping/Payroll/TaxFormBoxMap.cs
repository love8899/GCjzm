using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Data.Mapping.Payroll
{
    public class TaxFormBoxMap : EntityTypeConfiguration<TaxFormBox>
    {
        public TaxFormBoxMap()
        {
            // Table & Column Mappings
            this.ToTable("TaxFormBoxes");
            this.HasKey(t => t.Id);

            this.HasMany(x => x.PayrollItems).WithMany(x => x.TaxForms).Map(x => x.ToTable("PayrollItem_TaxFormBoxes_Mapping").MapLeftKey("TaxFormBoxId").MapRightKey("PayrollItemId"));

        }
    }
}

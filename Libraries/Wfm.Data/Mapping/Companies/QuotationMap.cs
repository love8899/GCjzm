using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;


namespace Wfm.Data.Mapping.Companies
{
    public class QuotationMap : EntityTypeConfiguration<Quotation>
    {
        public QuotationMap()
        {
            this.ToTable("Quotation");
            this.HasKey(x => x.Id);

            this.Property(x => x.FileName).HasMaxLength(255);

            this.HasRequired(x => x.CompanyBillingRate)
                .WithMany(y => y.Quotations)
                .HasForeignKey(x => x.CompanyBillingRateId)
                .WillCascadeOnDelete(false);
        }
    }
}

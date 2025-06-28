using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public partial class CompanyBillingRatesMap : EntityTypeConfiguration<CompanyBillingRate>
    {
        public CompanyBillingRatesMap()
        {
            this.ToTable("CompanyBillingRates");
            this.HasKey(cbr => cbr.Id);

            this.Property(cbr => cbr.RegularBillingRate).HasPrecision(18, 4);
            this.Property(cbr => cbr.RegularPayRate).HasPrecision(18, 4);
            this.Property(cbr => cbr.OvertimePayRate).HasPrecision(18, 4);
            this.Property(cbr => cbr.OvertimeBillingRate).HasPrecision(18, 4);

            this.Property(cbr => cbr.BillingTaxRate).HasPrecision(18, 5);

            this.Property(cbr => cbr.MaxRate).HasPrecision(18, 4);
            this.Property(cbr => cbr.WeeklyWorkHours).HasPrecision(18, 2);

            this.HasRequired(x => x.Franchise).WithMany().HasForeignKey(x => x.FranchiseId);
            this.HasRequired(c => c.Company)
                .WithMany(c => c.CompanyBillingRates)
                .HasForeignKey(c => c.CompanyId);

        }
    }
}

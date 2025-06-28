using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public partial class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            this.ToTable("Company");
            this.HasKey(c => c.Id);

            this.Property(c => c.CompanyName).IsRequired().HasMaxLength(255);

            this.Property(c => c.AdminName).HasMaxLength(255);
            this.HasRequired(x => x.CompanyStatus).WithMany().HasForeignKey(x => x.CompanyStatusId);
        }
    }
}


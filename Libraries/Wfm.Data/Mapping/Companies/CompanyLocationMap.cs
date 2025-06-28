using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public partial class CompanyLocationMap : EntityTypeConfiguration<CompanyLocation>
    {
        public CompanyLocationMap()
        {
            this.ToTable("CompanyLocation");
            this.HasKey(cl => cl.Id);

            this.Property(cl => cl.LocationName).IsRequired().HasMaxLength(255);

            this.HasRequired(cl => cl.Company)
                .WithMany(c => c.CompanyLocations)
                .HasForeignKey(cl => cl.CompanyId);
        }
    }
}

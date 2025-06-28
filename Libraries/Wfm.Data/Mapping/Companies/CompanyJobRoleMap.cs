using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;


namespace Wfm.Data.Mapping.Companies
{
    public class CompanyJobRoleMap : EntityTypeConfiguration<CompanyJobRole>
    {
        public CompanyJobRoleMap()
        {
            this.ToTable("CompanyJobRole");
            this.HasKey(cjr => cjr.Id);

            this.Property(cjr => cjr.Name).IsRequired().HasMaxLength(50);
            this.Property(cjr => cjr.Description).HasMaxLength(2048);

            this.HasRequired(cjr => cjr.Position)
                .WithMany()
                .HasForeignKey(cjr => cjr.PositionId);
            this.HasRequired(cjr => cjr.Company)
                .WithMany(c => c.CompanyJobRoles)
                .HasForeignKey(cjr => cjr.CompanyId);
            this.HasOptional(cjr => cjr.CompanyLocation)
                .WithMany()
                .HasForeignKey(cjr => cjr.CompanyLocationId);
        }
    }
}

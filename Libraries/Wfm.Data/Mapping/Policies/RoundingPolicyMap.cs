using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Policies;

namespace Wfm.Data.Mapping.Policies
{
    public partial class RoundingPolicyMap : EntityTypeConfiguration<RoundingPolicy>
    {
        public RoundingPolicyMap()
        {
            this.ToTable("RoundingPolicy");
            this.HasKey(a => a.Id);

            this.Property(a => a.Name).HasMaxLength(255);


            this.HasRequired(c => c.Company)
                .WithMany(c => c.RoundingPolicies)
                .HasForeignKey(c => c.CompanyId);
        }
    }
}

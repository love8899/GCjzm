using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Policies;

namespace Wfm.Data.Mapping.Policies
{
    public partial class BreakPolicyMap : EntityTypeConfiguration<BreakPolicy>
    {
        public BreakPolicyMap()
        {
            this.ToTable("BreakPolicy");
            this.HasKey(a => a.Id);

            this.Property(a => a.Name).HasMaxLength(255);


            this.HasRequired(c => c.Company)
                .WithMany(c => c.BreakPolicies)
                .HasForeignKey(c => c.CompanyId);
        }
    }
}

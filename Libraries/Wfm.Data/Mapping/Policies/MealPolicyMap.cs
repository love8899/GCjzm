using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Policies;

namespace Wfm.Data.Mapping.Policies
{
    public partial class MealPolicyMap : EntityTypeConfiguration<MealPolicy>
    {
        public MealPolicyMap()
        {
            this.ToTable("MealPolicy");
            this.HasKey(m => m.Id);

            this.Property(m => m.Name).HasMaxLength(255);


            this.HasRequired(c => c.Company)
                .WithMany(c => c.MealPolicies)
                .HasForeignKey(c => c.CompanyId);
        }
    }
}

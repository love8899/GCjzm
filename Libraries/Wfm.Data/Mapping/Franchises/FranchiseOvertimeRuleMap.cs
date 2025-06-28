using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Data.Mapping.Franchises
{
    public partial class FranchiseOvertimeRuleMap : EntityTypeConfiguration<FranchiseOvertimeRule>
    {

        public FranchiseOvertimeRuleMap()
        {
            this.ToTable("FranchiseOvertimeRule");
            this.HasKey(fo => fo.Id);

            this.HasRequired(fo => fo.Franchise)
                .WithMany(f => f.FranchiseOvertimeRules)
                .HasForeignKey(fo => fo.FranchiseId);
        }
    }
}

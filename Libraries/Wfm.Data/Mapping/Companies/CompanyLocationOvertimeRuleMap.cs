using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;


namespace Wfm.Data.Mapping.Companies
{
    public partial class CompanyLocationOvertimeRuleMap : EntityTypeConfiguration<CompanyLocationOvertimeRule>
    {
        public CompanyLocationOvertimeRuleMap()
        {
            this.ToTable("CompanyLocationOvertimeRule");
            this.HasKey(clo => clo.Id);

            this.HasRequired(clo => clo.CompanyLocation)
                .WithMany(cl => cl.CompanyLocationOvertimeRules)
                .HasForeignKey(clo => clo.CompanyLocationId);
        }
    }
}

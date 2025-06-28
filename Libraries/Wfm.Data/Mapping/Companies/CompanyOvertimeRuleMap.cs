using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;


namespace Wfm.Data.Mapping.Companies
{
    public partial class CompanyOvertimeRuleMap : EntityTypeConfiguration<CompanyOvertimeRule>
    {
        public CompanyOvertimeRuleMap()
        {
            this.ToTable("CompanyOvertimeRule");
            this.HasKey(co => co.Id);

            this.HasRequired(co => co.Company)
                .WithMany(c => c.CompanyOvertimeRules)
                .HasForeignKey(co => co.CompanyId);
        }
    }
}

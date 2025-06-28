using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Companies;

namespace Wfm.Data.Mapping.Companies
{
    public class CompanyJobRoleSkillMap : EntityTypeConfiguration<CompanyJobRoleSkill>
    {
        public CompanyJobRoleSkillMap()
        {
            this.ToTable("CompanyJobRoleSkill");
            this.HasKey(cjrs => cjrs.Id);

            this.HasRequired(cjrs => cjrs.CompanyJobRole)
                .WithMany(c => c.RequiredSkills)
                .HasForeignKey(cjrs => cjrs.CompanyJobRoleId);
            this.HasRequired(cjrs => cjrs.Skill)
                .WithMany()
                .HasForeignKey(cjrs => cjrs.SkillId);
        }
    }
}

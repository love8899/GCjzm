using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Data.Mapping.Candidates
{
    public partial class CandidateKeySkillsMap : EntityTypeConfiguration<CandidateKeySkill>
    {
        public CandidateKeySkillsMap()
        {
            this.ToTable("CandidateKeySkills");
            this.HasKey(c => c.Id);

            this.Property(c => c.YearsOfExperience).HasPrecision(10, 1);

            this.Property(c => c.KeySkill).HasMaxLength(255);
        }
    }
}

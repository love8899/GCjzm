using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class SkillMap : EntityTypeConfiguration<Skill>
    {
        public SkillMap()
        {
            this.ToTable("Skill");
            this.HasKey(c=> c.Id);

            this.Property(c => c.SkillName).IsRequired().HasMaxLength(255);
        }
    }
}

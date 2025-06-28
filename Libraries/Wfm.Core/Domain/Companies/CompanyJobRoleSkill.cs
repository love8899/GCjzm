using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyJobRoleSkill : BaseEntity
    {
        public int CompanyJobRoleId { get; set; }
        public int SkillId { get; set; }
        public SkillProficiencyLevelEnum? RequiredSkillProficiencyLevel { get; set; }
        public virtual CompanyJobRole CompanyJobRole { get; set; }
        public virtual Skill Skill { get; set; }
    }

    public enum SkillProficiencyLevelEnum
    {
        Beginner = 1,
        Elementary,
        Independent,
        Intermediate,
        Advanced,
        Mastery,
    }
}

using Wfm.Core.Domain.Companies;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Companies
{
    public class CompanyJobRoleSkillModel : BaseWfmEntityModel
    {
        public int CompanyJobRoleId { get; set; }
        [WfmResourceDisplayName("Web.JobRole.Fields.CompanyJobRoleName")]
        public string CompanyJobRoleName { get; set; }
        public int SkillId { get; set; }
        [WfmResourceDisplayName("Web.JobRole.Fields.SkillName")]
        public string SkillName { get; set; }
        [WfmResourceDisplayName("Web.JobRole.Fields.RequiredSkillProficiencyLevel")]
        public SkillProficiencyLevelEnum? RequiredSkillProficiencyLevel { get; set; }
    }
}

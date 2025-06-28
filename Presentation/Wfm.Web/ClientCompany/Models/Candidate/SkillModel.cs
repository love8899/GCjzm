using FluentValidation.Attributes;
using Wfm.Client.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Candidate
{
    [Validator(typeof(SkillValidator))]
    public partial class SkillModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Skill.Fields.SkillName")]
        public string SkillName { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
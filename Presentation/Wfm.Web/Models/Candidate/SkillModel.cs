using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Candidate
{
    public partial class SkillModel : BaseWfmEntityModel
    {
        public string SkillName { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public bool IsActive { get; set; }

        public int EnteredBy { get; set; }

    }
}
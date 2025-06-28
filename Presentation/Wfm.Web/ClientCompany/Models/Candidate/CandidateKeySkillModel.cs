using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Candidate
{
    /// <summary>
    /// Candidate registes key skills.
    /// </summary>
    public partial class CandidateKeySkillModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.KeySkill")]
        public string KeySkill { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateKeySkill.Fields.YearsOfExperience")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        public decimal? YearsOfExperience { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateKeySkill.Fields.LastUsedDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastUsedDate { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }


        public virtual CandidateModel CandidateModel { get; set; }

        //public IList<SkillModel> SkillModels { get; set; }

    }
}
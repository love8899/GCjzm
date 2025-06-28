using FluentValidation.Attributes;
using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Validators.Candidate;

namespace Wfm.Web.Models.Candidate
{
    [Validator(typeof(CandidateKeySkillValidator))]
    public partial class CandidateKeySkillModel : BaseWfmEntityModel
    {
        public Guid CandidateKeySkillGuid { get; set; }
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.KeySkill")]
        public string KeySkill { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateKeySkill.Fields.YearsOfExperience")]
        public decimal? YearsOfExperience { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateKeySkill.Fields.LastUsedDate")]
        public DateTime? LastUsedDate { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        public bool IsDeleted { get; set; }

    }
}
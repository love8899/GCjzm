using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    /// <summary>
    /// Candidate registes key skills.
    /// </summary>
    [Validator(typeof(CandidateKeySkillValidator))]
    public partial class CandidateKeySkillModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.KeySkill")]
        public string KeySkill { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateKeySkill.Fields.YearsOfExperience")]
        //[DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        public decimal? YearsOfExperience { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateKeySkill.Fields.LastUsedDate")]
        [UIHint("Date")]
        public DateTime? LastUsedDate { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        public string CandidateName { get; set; }
    }
}
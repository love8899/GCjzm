using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;


namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CreateEditCandidateValidator))]
    public class CreateEditCandidateModel : CandidateModel 
    {
        // key skill 1
        
        [WfmResourceDisplayName("Common.KeySkill")]
        [DataType(DataType.MultilineText)]
        public string KeySkill1 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.YearsOfExperience")]
        public decimal YearsOfExperience1 { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.LastUsedDate")]
        public DateTime? LastUsedDate1 { get; set; }
    }
}
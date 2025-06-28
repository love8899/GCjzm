using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
  
    public partial class DirectHireCandidatePoolListModel : BaseWfmEntityModel
    {

        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.KeySkill")]
        public string KeySkill { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateKeySkill.Fields.YearsOfExperience")]       
        public decimal? YearsOfExperience { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateKeySkill.Fields.LastUsedDate")]
        [UIHint("Date")]      
        public DateTime? LastUsedDate { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        public string EmployeeId { get; set; }
        
    }
}
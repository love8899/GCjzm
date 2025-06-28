using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateBankAccountValidator))]
    public class CandidateBankAccountModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }
        
        [WfmResourceDisplayName("Admin.Candidate.CandidateBankAccount.Fields.InstitutionNumber")]
        [StringLength(4)]
        public string InstitutionNumber { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateBankAccount.Fields.TransitNumber")]
        [StringLength(5)]
        public string TransitNumber { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateBankAccount.Fields.AccountNumber")]
        [StringLength(17)]
        public string AccountNumber { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }
        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public virtual CandidateModel CandidateModel { get; set; }

    }
}
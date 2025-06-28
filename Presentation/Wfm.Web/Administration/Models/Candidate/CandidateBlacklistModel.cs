using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    /// <summary>
    /// Candidate basic information
    /// </summary>
    public partial class CandidateBlacklistModel : BaseWfmEntityModel
    {
        public int CandidateId { get; set; }

        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public virtual string EmployeeId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        public int GenderId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BirthDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? BirthDate { get; set; }

        //[WfmResourceDisplayName("Common.Note")]
        //public string Note { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public int? ClientId { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.EffectiveDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EffectiveDate { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.BannedReason")]
        public string BannedReason { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
    }

}

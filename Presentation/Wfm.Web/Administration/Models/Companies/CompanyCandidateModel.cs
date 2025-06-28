using System;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Company;
using Wfm.Admin.Models.Candidate;
using Wfm.Web.Framework.Mvc;
using Wfm.Services.Companies;


namespace Wfm.Admin.Models.Companies
{
    /// <summary>
    /// Company Candidate
    /// </summary>
    [Validator(typeof(CompanyCandidateValidator))]
    public class CompanyCandidateModel : BaseWfmEntityModel, ICompanyCandidatePriorityInfo
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.Position")]
        public string Position { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime? StartDate { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? EndDate { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyCandidate.Fields.ReasonForLeave")]
        [MaxLength(255)]
        public string ReasonForLeave { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        [MaxLength(255)]
        public string Note { get; set; }

        public int? RatingValue { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatedBy")]
        public string RatedBy { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.RatingComment")]
        public string RatingComment { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.LastWorkingDate")]
        public DateTime? LastWorkingDate { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.LastWorkingShift")]
        public string LastWorkingShift { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.CandidateJobOrder.Fields.LastWorkingLocation")]
        public string LastWorkingLocation { get; set; }
        [WfmResourceDisplayName("Common.TotalHours")]
        public decimal? TotalWorkingHours { get; set; }

        public string Status 
        {
            get
            {
                if (EndDate == null || EndDate>=DateTime.Today)
                {
                    return "Active";
                }
                else
                {
                    return "Inactive";
                }
            }
        }
        public virtual CandidateModel CandidateModel { get; set; }
    }

}

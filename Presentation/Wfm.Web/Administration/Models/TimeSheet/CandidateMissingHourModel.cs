using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.TimeSheet;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.TimeSheet
{
    [Validator(typeof(CandidateMissingHourValidator))]
    public class CandidateMissingHourModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateMissingHourStatusId { get; set; }

        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string DepartmentName { get; set; }
        [WfmResourceDisplayName("Common.Contact")]
        public string ContactName { get; set; }

        public DateTime WorkDate { get; set; }

        public decimal OrigHours { get; set; }
        
        public decimal NewHours { get; set; }

        public decimal BillableHours { get { return NewHours - OrigHours; } }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.ApprovedBy")]
        public int ApprovedBy { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ApprovedOnUtc")]
        public DateTime? ApprovedOnUtc { get; set; }
        public DateTime? ApprovedOn { get { return ApprovedOnUtc.HasValue ? ApprovedOnUtc.Value.ToLocalTime() : (DateTime?)null; } }

        public int ProcessedBy { get; set; }
        
        public DateTime? ProcessedOnUtc { get; set; }
        public DateTime? ProcessedOn { get { return ProcessedOnUtc.HasValue ? ProcessedOnUtc.Value.ToLocalTime() : (DateTime?)null; } }

        public IList<MissingHourDocumentModel> MissingHourDocuments { get; set; }

        public string PayrollNote { get; set; }
    }
}
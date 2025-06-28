using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.ClockTime;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.ClockTime
{
    /// <summary>
    /// Record daily Candidate punch in and out time.
    /// </summary>
    [Validator(typeof(CandidateClockTimeValidator))]
    public class CandidateClockTimeModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.ClockDeviceUid")]
        public string ClockDeviceUid { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public int? CompanyId { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int? CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.RecordNumber")]
        public int RecordNumber { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.SmartCardUid")]
        public string SmartCardUid { get; set; }
        [WfmResourceDisplayName("Common.Id")]
        public int? CandidateId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.ClockInOut")]
        public DateTime? ClockInOut { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.Source")]
        public string Source { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.PunchClockFileName")]
        [AllowHtml]
        public string PunchClockFileName { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateClockTimeStatusId { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        [WfmResourceDisplayName("Common.EnteredBy")]
        public string EnteredByName { get; set; }

        [WfmResourceDisplayName("Common.UpdatedBy")]
        public int UpdatedBy { get; set; }
        [WfmResourceDisplayName("Common.UpdatedBy")]
        public string UpdatedByName { get; set; }

        public int FranchiseId { get; set; }
    }
}
using System;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.ClockTime;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.ClockTime
{
    /// <summary>
    /// Record daily Candidate punch in and out time.
    /// </summary>
    [Validator(typeof(NewClockTimeValidator))]
    public class NewClockTimeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.ClockDeviceUid")]
        public string ClockDeviceUid { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.RecordNumber")]
        public int RecordNumber { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.SmartCardUid")]
        public string SmartCardUid { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut")]
        public DateTime? ClockInOut { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut2")]
        public DateTime? ClockInOut2 { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut3")]
        public DateTime? ClockInOut3 { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut4")]
        public DateTime? ClockInOut4 { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut5")]
        public DateTime? ClockInOut5 { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut6")]
        public DateTime? ClockInOut6 { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut7")]
        public DateTime? ClockInOut7 { get; set; }
        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.ClockInOut8")]
        public DateTime? ClockInOut8 { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.Source")]
        public string Source { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.NewClockTime.Fields.PunchClockFileName")]
        public string PunchClockFileName { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateClockTimeStatusId { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.UpdatedBy")]
        public int UpdatedBy { get; set; }

    }
}
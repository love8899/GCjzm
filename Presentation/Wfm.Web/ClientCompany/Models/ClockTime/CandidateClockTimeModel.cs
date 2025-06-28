using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Client.Models.ClockTime
{
    /// <summary>
    /// Record daily Candidate punch in and out time.
    /// </summary>
    public class CandidateClockTimeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.ClockDeviceUid")]
        public string ClockDeviceUid { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int? CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.SmartCardUid")]
        public string SmartCardUid { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string CandidateLastName { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string CandidateFirstName { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateClockTime.Fields.ClockInOut")]
        public DateTime? ClockInOut { get; set; }

        public int FranchiseId { get; set; }
    }
}
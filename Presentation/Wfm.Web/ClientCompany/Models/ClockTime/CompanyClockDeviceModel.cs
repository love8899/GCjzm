using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Client.Models.Companies;

namespace Wfm.Client.Models.ClockTime
{
    public class CompanyClockDeviceModel : BaseWfmEntityModel 
    {
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.ClockDeviceUid")]
        public string ClockDeviceUid { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.MacAddress")]
        public string MacAddress { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.DongleName")]

        public string DongleName { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.DongleModel")]

        public string DongleModel { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.SIMCardCarrier")]

        public string SIMCardCarrier { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CandidateSmartCard.Fields.ActivatedDate")]
        public DateTime? ActivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.DeactivatedDate")]
        public DateTime? DeactivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.ReasonForDeactivation")]
        public string ReasonForDeactivation { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }


        public virtual CompanyLocationModel CompanyLocationModel { get; set; }
    }
}
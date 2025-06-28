using System;
using System.ComponentModel;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.ClockTime;
using Wfm.Admin.Models.Companies;


namespace Wfm.Admin.Models.ClockTime
{
    [Validator(typeof(CompanyClockDeviceValidator))]
    public class CompanyClockDeviceModel : BaseWfmEntityModel 
    {
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.ClockDeviceUid")]
        public string ClockDeviceUid { get; set; }

        [WfmResourceDisplayName("Common.IpAddress")]
        public string IPAddress { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.DongleName")]
        public string DongleName { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.DongleModel")]
        public string DongleModel { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.CompanyClockDevice.Fields.SIMCardCarrier")]
        public string SIMCardCarrier { get; set; }

        [DisplayName("ID Length")]
        public int? IDLength { get; set; }

        [DisplayName("Manual ID")]
        public bool ManualID { get; set; }

        [DisplayName("Refresh Hour")]
        public int? RefreshHour { get; set; }

        [DisplayName("Expiry Days")]
        public int? ExpiryDays { get; set; }

        [DisplayName("Add on Enroll")]
        public bool AddOnEnroll { get; set; }

        [DisplayName("Alternative Id Reader")]
        public bool AltIdReader { get; set; }

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


        // For UI
        [WfmResourceDisplayName("Common.Location")]
        public virtual string LocationName { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public virtual int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public virtual string CompanyName { get; set; }


        public virtual CompanyLocationModel CompanyLocationModel { get; set; }
    }
}
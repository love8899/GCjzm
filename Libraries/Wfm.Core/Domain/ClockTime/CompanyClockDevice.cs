using System;
using Wfm.Core.Domain.Companies;


namespace Wfm.Core.Domain.ClockTime
{
    /// <summary>
    /// One Company location may have many Punch Clocks.
    /// One Punch clock has one CompanyLocation
    /// </summary>
    public class CompanyClockDevice : BaseEntity
    {
        public int CompanyLocationId { get; set; }
        public string ClockDeviceUid { get; set; }
        public string IPAddress { get; set; }
        public string DongleName { get; set; }
        public string DongleModel { get; set; }
        public string SIMCardCarrier { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string ReasonForDeactivation { get; set; }
        public string Note { get; set; }
        public int EnteredBy { get; set; }

        public int? IDLength { get; set; }
        public bool ManualID { get; set; }

        public int? RefreshHour { get; set; }
        public int? ExpiryDays { get; set; }
        public bool AddOnEnroll { get; set; }

        // alt. ID reader, like barcode scanner
        public bool AltIdReader { get; set; }

        public virtual CompanyLocation CompanyLocation { get; set; }
    }

}
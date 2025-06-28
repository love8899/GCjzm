using System;


namespace Wfm.Core.Domain.TimeSheet
{
    public class InOutTimes
    {
        public int CandidateId { get; set; }
        public string SmartCardUid { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime ClockOutTime { get; set; }
        public string ClockDeviceUid { get; set; }
        public string Source { get; set; }
    }
}

using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.ClockTime
{
    public class TimeClockSettings : ISettings
    {
        /// <summary>
        /// Gets a sets a value indicating whether to retrive punch clock files
        /// </summary>
        public string PunchClockFilesLocation { get; set; }


        /// <summary>
        /// Gets a sets a breadcrumb delimiter used in time record
        /// </summary>
        public string TimeRecordDelimiter { get; set; }

        // sync time, for hand punch
        public int SyncHour { get; set; }
        public int MinDrift { get; set; }
        public int MaxDrift { get; set; }
    }
}

using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.TimeSheet
{
    public class CandidateMissingHourSettings : ISettings
    {
        public int FollowUpIntervalInDays { get; set; }
    }

}

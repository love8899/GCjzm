using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateMassEmailSettings : ISettings
    {
        public int FromEmailAccountId { get; set; }
        public int PriorityId { get; set; }
        public int BatchSize { get; set; }
        public int BccMaxLength { get; set; }
    }

}

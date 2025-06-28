using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Franchises
{
    public class VendorMassEmailSettings : ISettings
    {
        public int FromEmailAccountId { get; set; }
        public int PriorityId { get; set; }
        public int BatchSize { get; set; }
        public int BccMaxLength { get; set; }
    }

}

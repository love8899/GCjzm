using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Messages
{
    public class EmailAccountSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a franchise default email account identifier
        /// </summary>
        public int DefaultEmailAccountId { get; set; }

        public string DefaultToEmailAddress { get; set; }
        public string DefaultCCEmailAddress { get; set; }
        public string DefaultBccEmailAddress { get; set; }

        // system wide
        public int? TotalHourlyLimit { get; set; }
        public int? PageSize { get; set; }
    }

}

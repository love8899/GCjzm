using Wfm.Core.Configuration;


namespace Wfm.Core.Domain.Messages
{
    public class SMSSettings : ISettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromNumber { get; set; }
    }

}

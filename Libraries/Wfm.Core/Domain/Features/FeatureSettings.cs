using Wfm.Core.Configuration;


namespace Wfm.Core.Domain.Features
{
    public class FeatureSettings : ISettings
    {
        public string ClientFeaturesNotEnabledByDefault { get; set; }
    }
}

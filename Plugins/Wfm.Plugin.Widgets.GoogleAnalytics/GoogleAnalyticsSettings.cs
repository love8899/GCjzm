﻿
using Wfm.Core.Configuration;

namespace Wfm.Plugin.Widgets.GoogleAnalytics
{
    public class GoogleAnalyticsSettings : ISettings
    {
        public string GoogleId { get; set; }
        public string TrackingScript { get; set; }
        public string EcommerceScript { get; set; }
        public string EcommerceDetailScript { get; set; }
    }
}
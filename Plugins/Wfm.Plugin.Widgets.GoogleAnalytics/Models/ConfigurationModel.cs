using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Plugin.Widgets.GoogleAnalytics.Models
{
    public class ConfigurationModel : BaseWfmModel
    {
        public int ActiveFranchiseScopeConfiguration { get; set; }
        
        [WfmResourceDisplayName("Plugins.Widgets.GoogleAnalytics.GoogleId")]
        [AllowHtml]
        public string GoogleId { get; set; }
        public bool GoogleId_OverrideForFranchise { get; set; }

        [WfmResourceDisplayName("Plugins.Widgets.GoogleAnalytics.TrackingScript")]
        [AllowHtml]
        //tracking code
        public string TrackingScript { get; set; }
        public bool TrackingScript_OverrideForFranchise { get; set; }

        [WfmResourceDisplayName("Plugins.Widgets.GoogleAnalytics.EcommerceScript")]
        [AllowHtml]
        public string EcommerceScript { get; set; }
        public bool EcommerceScript_OverrideForFranchise { get; set; }

        [WfmResourceDisplayName("Plugins.Widgets.GoogleAnalytics.EcommerceDetailScript")]
        [AllowHtml]
        public string EcommerceDetailScript { get; set; }
        public bool EcommerceDetailScript_OverrideForFranchise { get; set; }

    }
}
using Wfm.Core.Configuration;

namespace Wfm.Core.Domain
{
    public class FranchiseInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether franchise is closed
        /// </summary>
        public bool WebClosed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether administrators can visit a closed franchise
        /// </summary>
        public bool WebClosedAllowForAdmins { get; set; }

        /// <summary>
        /// Gets or sets a default franchise theme
        /// </summary>
        public string DefaultWebTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select a theme
        /// </summary>
        public bool AllowUserToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether responsive design supported (a graphical theme should also support it)
        /// </summary>
        public bool ResponsiveDesignSupported { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public franchise (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInPublicWeb { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        /// Gets or sets a value of Facebook page URL of the site
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Twitter page URL of the site
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Gets or sets a value of YouTube channel URL of the site
        /// </summary>
        public string YoutubeLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Google+ page URL of the site
        /// </summary>
        public string GooglePlusLink { get; set; }
    }
}

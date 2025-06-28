using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        public bool UseSystemEmailForContactUsForm { get; set; }

        public bool UseStoredProceduresIfSupported { get; set; }

        public bool HideAdvertisementsOnAdminArea { get; set; }

        /// <summary>
        /// Gets a sets a value indicating whether to display a warning if java-script is disabled
        /// </summary>
        public bool DisplayJavaScriptDisabledWarning { get; set; }

        /// <summary>
        /// Gets a sets a value indicating whether full-text search is supported
        /// </summary>
        public bool UseFullTextSearch { get; set; }

        /// <summary>
        /// Gets a sets a Full-Text search mode
        /// </summary>
        public FulltextSearchMode FullTextMode { get; set; }

        /// <summary>
        /// Gets a sets a value indicating whether 404 errors (page or file not found) should be logged
        /// </summary>
        public bool Log404Errors { get; set; }

        /// <summary>
        /// Gets a sets a breadcrumb delimiter used on the site
        /// </summary>
        public string BreadcrumbDelimiter { get; set; }


        /// <summary>
        /// Gets a sets a value indicating whether we should render <meta http-equiv="X-UA-Compatible" content="IE=edge"/> tag
        /// </summary>
        public bool RenderXuaCompatible { get; set; }

        /// <summary>
        /// Gets a sets a value of "X-UA-Compatible" META tag
        /// </summary>
        public string XuaCompatibleValue { get; set; }

        /// <summary>
        /// Gets a sets a host url for the web site
        /// </summary>
        public string HostUrl { get; set; }

        public int AllowedIPAddressHistory { get; set; }

        // if display vendor field on UI (detail pages and grids)
        public bool DisplayVendor { get; set; }

        public string SiteTitle { get; set; }
        public string FacebookLink { get; set; }
        public string LinkedinLink { get; set; }
        public string TwitterLink { get; set; }
        public bool IsISO9001Certified { get; set; }
        public int WorkDayHours { get; set; }
    }
}
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Plugin.Widgets.GoogleAnalytics.Models;
using Wfm.Services.Configuration;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.JobOrders;
using Wfm.Services.Franchises;
using Wfm.Web.Framework.Controllers;

namespace Wfm.Plugin.Widgets.GoogleAnalytics.Controllers
{
    public class WidgetsGoogleAnalyticsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IFranchiseContext _franchiseContext;
        private readonly IFranchiseService _franchiseService;
        private readonly ISettingService _settingService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        public WidgetsGoogleAnalyticsController(IWorkContext workContext,
            IFranchiseContext franchiseContext, 
            IFranchiseService franchiseService,
            ISettingService settingService, 
            IJobOrderService jobOrderService, 
            ILogger logger, 
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._franchiseContext = franchiseContext;
            this._franchiseService = franchiseService;
            this._settingService = settingService;
            this._jobOrderService = jobOrderService;
            this._logger = logger;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen franchise scope
            var franchiseScope = this.GetActiveFranchiseScopeConfiguration(_franchiseService, _workContext);
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(franchiseScope);
            var model = new ConfigurationModel();
            model.GoogleId = googleAnalyticsSettings.GoogleId;
            model.TrackingScript = googleAnalyticsSettings.TrackingScript;
            model.EcommerceScript = googleAnalyticsSettings.EcommerceScript;
            model.EcommerceDetailScript = googleAnalyticsSettings.EcommerceDetailScript;

            model.ActiveFranchiseScopeConfiguration = franchiseScope;
            if (franchiseScope > 0)
            {
                model.GoogleId_OverrideForFranchise = _settingService.SettingExists(googleAnalyticsSettings, x => x.GoogleId, franchiseScope);
                model.TrackingScript_OverrideForFranchise = _settingService.SettingExists(googleAnalyticsSettings, x => x.TrackingScript, franchiseScope);
                model.EcommerceScript_OverrideForFranchise = _settingService.SettingExists(googleAnalyticsSettings, x => x.EcommerceScript, franchiseScope);
                model.EcommerceDetailScript_OverrideForFranchise = _settingService.SettingExists(googleAnalyticsSettings, x => x.EcommerceDetailScript, franchiseScope);
            }

            return View("~/Plugins/Widgets.GoogleAnalytics/Views/WidgetsGoogleAnalytics/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen franchise scope
            var franchiseScope = this.GetActiveFranchiseScopeConfiguration(_franchiseService, _workContext);
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(franchiseScope);
            googleAnalyticsSettings.GoogleId = model.GoogleId;
            googleAnalyticsSettings.TrackingScript = model.TrackingScript;
            googleAnalyticsSettings.EcommerceScript = model.EcommerceScript;
            googleAnalyticsSettings.EcommerceDetailScript = model.EcommerceDetailScript;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.GoogleId_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(googleAnalyticsSettings, x => x.GoogleId, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(googleAnalyticsSettings, x => x.GoogleId, franchiseScope);
            
            if (model.TrackingScript_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(googleAnalyticsSettings, x => x.TrackingScript, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(googleAnalyticsSettings, x => x.TrackingScript, franchiseScope);
            
            if (model.EcommerceScript_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(googleAnalyticsSettings, x => x.EcommerceScript, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(googleAnalyticsSettings, x => x.EcommerceScript, franchiseScope);
            
            if (model.EcommerceDetailScript_OverrideForFranchise || franchiseScope == 0)
                _settingService.SaveSetting(googleAnalyticsSettings, x => x.EcommerceDetailScript, franchiseScope, false);
            else if (franchiseScope > 0)
                _settingService.DeleteSetting(googleAnalyticsSettings, x => x.EcommerceDetailScript, franchiseScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            string globalScript = "";
            var routeData = ((System.Web.UI.Page)this.HttpContext.CurrentHandler).RouteData;

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                //Special case, if we are in last step of checkout, we can use jobOrder total for conversion value
                if (controller.ToString().Equals("checkout", StringComparison.InvariantCultureIgnoreCase) &&
                    action.ToString().Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    var lastJobOrder = GetLastJobOrder();
                    globalScript += GetEcommerceScript(lastJobOrder);
                }
                else
                {
                    globalScript += GetTrackingScript();
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for google ecommerce tracking", ex.ToString());
            }
            return Content(globalScript);
        }

        private JobOrder GetLastJobOrder()
        {
            var jobOrder = _jobOrderService.GetAllJobOrders(0, 1, false).FirstOrDefault();
            return jobOrder;
        }
        
        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetTrackingScript()
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(_franchiseContext.CurrentFranchise.Id);
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            return analyticsTrackingScript;
        }
        
        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 
        //_gaq.push(['_addTrans', 
        //'1234',           // jobOrder ID - required 
        //'Acme Clothing',  // affiliation or franchise name 
        //'11.99',          // total - required 
        //'1.29',           // tax 
        //'5',              // shipping 
        //'San Jose',       // city 
        //'California',     // state or province 
        //'USA'             // country 
        //]); 

        //// add item might be called for every item in the shopping cart 
        //// where your ecommerce engine loops through each item in the cart and 
        //// prints out _addItem for each 
        //_gaq.push(['_addItem', 
        //'1234',           // jobOrder ID - required 
        //'DD44',           // SKU/code - required 
        //'T-Shirt',        // product name 
        //'Green Medium',   // category or variation 
        //'11.99',          // unit price - required 
        //'1'               // quantity - required 
        //]); 
        //_gaq.push(['_trackTrans']); //submits transaction to the Analytics servers 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetEcommerceScript(JobOrder jobOrder)
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(_franchiseContext.CurrentFranchise.Id);
            var usCulture = new CultureInfo("en-US");
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);

            if (jobOrder != null)
            {
                var analyticsEcommerceScript = googleAnalyticsSettings.EcommerceScript + "\n";
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{ORDERID}", jobOrder.Id.ToString());
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SITE}", _franchiseContext.CurrentFranchise.WebSite.Replace("http://", "").Replace("/", ""));

                var sb = new StringBuilder();
                {
                    string analyticsEcommerceDetailScript = googleAnalyticsSettings.EcommerceDetailScript;

                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{ORDERID}", jobOrder.Id.ToString());
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTNAME}", FixIllegalJavaScriptChars(jobOrder.JobTitle));
                    sb.AppendLine(analyticsEcommerceDetailScript);
                }

                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{DETAILS}", sb.ToString());

                analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", analyticsEcommerceScript);
            }

            return analyticsTrackingScript;
        }

        private string FixIllegalJavaScriptChars(string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            return text;
        }
    }
}
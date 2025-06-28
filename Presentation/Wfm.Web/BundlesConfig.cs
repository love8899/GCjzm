using System.Web.Optimization;

namespace Wfm.Web
{
    public class BundlesConfig
    {
        // replace "2014.3.1119" with the Kendo UI version that you are using
        //public static readonly string kendoVersion = "2014.3.1119";
        public static readonly string kendoVersion = "2014.3.1314";

        public static void RegisterBundles(BundleCollection bundles)
        {
            // The jQuery bundle
            // --------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-1.*"));


            // The Kendo JavaScript bundle
            // --------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/kendo")
                .Include(string.Format("~/Scripts/kendo/{0}/kendo.all.min.js", kendoVersion))
                //.Include(string.Format("~/Scripts/kendo/{0}/kendo.timezones.min.js", kendoVersion)) // uncomment if using the Scheduler
                .Include(string.Format("~/Scripts/kendo/{0}/kendo.aspnetmvc.min.js", kendoVersion))
            );


            // The Kendo CSS bundle
            // --------------------------------------------
            bundles.Add(new StyleBundle("~/Content/kendo/css")
                .Include(string.Format("~/Content/kendo/{0}/kendo.common.*", kendoVersion))
                .Include(string.Format("~/Content/kendo/{0}/kendo.default.*", kendoVersion))
            );


            // Clear all items from the ignore list to allow minified CSS and JavaScript files in debug mode
            bundles.IgnoreList.Clear();


            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore("*.intellisense.js");
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);

        }
    }
}
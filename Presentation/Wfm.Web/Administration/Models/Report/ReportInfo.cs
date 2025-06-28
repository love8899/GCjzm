using System.Web.Routing;

namespace Wfm.Admin.Models.Report
{
    public class ReportInfo
    {
        public string ReportInfoActionName { get; set; }
        public string ReportInfoControllerName { get; set; }
        public int ReportInfoId { get; set; }
        public RouteValueDictionary ReportInfoRouterValues { get; set; }
    }
}
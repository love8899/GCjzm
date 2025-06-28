using System.Web.Routing;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Cms
{
    public partial class RenderWidgetModel : BaseWfmModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}
using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Web.Framework.Mvc.Routes;

namespace Wfm.Client.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("ClientNews",
                "client/home/news",
                new { controller = "Home", action = "News" },
                new[] { "Wfm.Client.Controllers" });
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}

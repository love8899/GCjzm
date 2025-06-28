using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Web.Framework.Mvc.Routes;

namespace Wfm.Admin.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //We reordered our routes so the most used ones are on top. It can improve performance.

            #region Account

            routes.MapRoute("AdminNews",
                "admin/home/news",
                new { controller = "Home", action = "News" },
                new[] { "Wfm.Admin.Controllers" });

            #endregion

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

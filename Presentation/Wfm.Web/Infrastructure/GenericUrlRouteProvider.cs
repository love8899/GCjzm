using System.Web.Routing;
using Wfm.Web.Framework.Localization;
using Wfm.Web.Framework.Mvc.Routes;
using Wfm.Web.Framework.Seo;

namespace Wfm.Web.Infrastructure
{
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                                       "{generic_se_name}",
                                       new {controller = "Common", action = "GenericUrl"},
                                       new[] {"Wfm.Web.Controllers"});

            routes.MapLocalizedRoute("BlogPost",
                            "{SeName}",
                            new { controller = "Blog", action = "BlogPost" },
                            new[] { "Wfm.Web.Controllers" });


            //routes.MapLocalizedRoute("Candidate",
            //                 "{SeName}",
            //                  new { controller = "Candidate", action = "Index" },
            //                  new[] { "Wfm.Web.Controllers" });
                             
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                //we do not set it to -int.MaxValue so it could be overriden (if required)
                return -1000000;
            }
        }
    }
}

using System.Web.Mvc;

namespace Wfm.Client
{
    public class ClientAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Client";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Client_default",
                "Client/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Client", id = "" },
                new[] { "Wfm.Client.Controllers" }
            );
        }
    }
}

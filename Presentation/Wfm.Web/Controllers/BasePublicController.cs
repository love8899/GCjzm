using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Core.Infrastructure;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework.Security;
using Wfm.Web.Framework.Seo;

namespace Wfm.Web.Controllers
{
    [WfmHttpsRequirement(SslRequirement.NoMatter)]
    [WwwRequirement]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual ActionResult InvokeHttp404()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<Wfm.Web.Controllers.CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "PageNotFound");

            errorController.Execute(new RequestContext(this.HttpContext, routeData));

            return new EmptyResult();
        }

    }
}

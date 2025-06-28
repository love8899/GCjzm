using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Core;
using Wfm.Core.Infrastructure;
using Wfm.Services.Features;


namespace Wfm.Web.Framework.Feature
{
    public class FeatureAuthorizeAttribute : ActionFilterAttribute
    {
        public FeatureAuthorizeAttribute(string featureName)
        {
            this.featureName = featureName;
        }
        private readonly string featureName;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var result = false;
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            if (workContext.CurrentAccount.IsClientAccount)
            {
                var userFeatureService = EngineContext.Current.Resolve<IUserFeatureService>();
                result = userFeatureService.GetAllUserFeatureNamesByUserId(workContext.CurrentAccount.CompanyId, "Client").Contains(featureName);
            }
            else
            {
                var featureService = EngineContext.Current.Resolve<IFeatureService>();
                result = featureService.IsFeatureEnabled("Admin", featureName);
            }

            if (!result)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Security", action = "AccessDenied" }));

            base.OnActionExecuting(filterContext);
        }
    }
}

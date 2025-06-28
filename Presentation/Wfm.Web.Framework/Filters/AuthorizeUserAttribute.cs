using System;
using System.Web.Mvc;
using System.Web.Routing;


namespace Wfm.Web.Framework.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var username = filterContext.HttpContext.User.Identity.Name;

            if (!String.IsNullOrWhiteSpace(username))
                base.HandleUnauthorizedRequest(filterContext);

            else
            {
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                //{
                //    controller = "Candidate",
                //    action = "SignIn",
                //    returnUrl = returnUrl
                //}));

                RouteValueDictionary routeValues = null;
                if (!filterContext.IsChildAction)
                    routeValues = new RouteValueDictionary(new { returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery });
                filterContext.Result = new RedirectToRouteResult("CandidateSignIn", routeValues);
            }

        }
    }
}

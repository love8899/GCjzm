using System;
using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Core;
using Wfm.Core.Infrastructure;


namespace Wfm.Web.Framework.Filters
{
    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {

            //base.OnException(filterContext);    // no longer need, as below implemented a custom error handling

            if (filterContext.Exception is HttpAntiForgeryException)
            {
                var area = filterContext.RequestContext.RouteData.Values["area"] as string;
                var action = filterContext.RequestContext.RouteData.Values["action"] as string;
                var controller = filterContext.RequestContext.RouteData.Values["controller"] as string;
                
                //var username = filterContext.RequestContext.HttpContext.Request.Form["Username"] as string;
                var returnUrl = filterContext.RequestContext.HttpContext.Request.QueryString["returnUrl"];
                // remove meaningless return url (TODO: try to aviod from source)
                if (!String.IsNullOrWhiteSpace(returnUrl))
                    if (returnUrl.Contains("login") || returnUrl.Contains("logout") || returnUrl.Contains("signin") || returnUrl.Contains("signout"))
                        returnUrl = null;

                if (controller == "Account" && action == "Login")
                {
                    filterContext.Result = new RedirectToRouteResult("AccountLogin", 
                        new RouteValueDictionary(new { returnUrl, alert = true }));
                    filterContext.ExceptionHandled = true;
                }

                else if (controller == "Candidate" && action == "SignIn")
                {
                    filterContext.Result = new RedirectToRouteResult("CandidateSignIn", 
                        new RouteValueDictionary(new { returnUrl, alert = true }));
                    filterContext.ExceptionHandled = true;
                }

                else
                {
                    // assuming GET / POST action names are the same, and GET action accepts null parameters
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                        new { area, action, controller }));
                    filterContext.ExceptionHandled = true;
                }
            }

            return;
        }


        private bool _IsAccountSignedIn(ExceptionContext filterContext, string username, out bool isClientAccount)
        {
            isClientAccount = false;

            var identityUsername = string.Empty;
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            if (workContext.CurrentAccount != null)
            {
                identityUsername = workContext.CurrentAccount.Username;
                isClientAccount = workContext.CurrentAccount.IsClientAccount;
            }

            return !String.IsNullOrWhiteSpace(identityUsername)
                && String.Equals(identityUsername, username, StringComparison.InvariantCultureIgnoreCase);
        }


        private bool _IsCandidateSignedIn(ExceptionContext filterContext, string username)
        {
            var identityUsername = string.Empty;
            if (filterContext.RequestContext.HttpContext.User != null 
                && filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                identityUsername = filterContext.RequestContext.HttpContext.User.Identity.Name;
            }

            return !String.IsNullOrWhiteSpace(identityUsername)
                && String.Equals(identityUsername, username, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

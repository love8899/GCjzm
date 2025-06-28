using System;
using System.Web;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Infrastructure;

namespace Wfm.Web.Framework
{
    public class FranchiseClosedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var franchiseInformationSettings = EngineContext.Current.Resolve<FranchiseInformationSettings>();
            if (!franchiseInformationSettings.WebClosed)
                return;

            if (//ensure it's not the Login page
                !(controllerName.Equals("Wfm.Web.Controllers.AccountController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the Logout page
                !(controllerName.Equals("Wfm.Web.Controllers.AccountController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Logout", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the method (AJAX) for accepting EU Cookie law
                !(controllerName.Equals("Wfm.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("EuCookieLawAccept", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the franchise closed page
                !(controllerName.Equals("Wfm.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("FranchiseClosed", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the change language page (request)
                !(controllerName.Equals("Wfm.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("SetLanguage", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (franchiseInformationSettings.WebClosedAllowForAdmins && EngineContext.Current.Resolve<IWorkContext>().CurrentAccount.IsAdministrator())
                {
                    //do nothing - allow admin access
                }
                else
                {
                    var franchiseClosedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl("FranchiseClosed");
                    filterContext.Result = new RedirectResult(franchiseClosedUrl);
                }
            }
        }
    }
}

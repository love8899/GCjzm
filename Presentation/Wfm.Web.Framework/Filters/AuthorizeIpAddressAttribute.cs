using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Core.Infrastructure;
using Wfm.Core.Domain.Common;
using Wfm.Services.Logging;
using System.Web.Routing;


namespace Wfm.Web.Framework
{
    public class AuthorizeIpAddressAttribute : ActionFilterAttribute
    {
        public bool SkipIpCheck { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!SkipIpCheck)
            {
                var ipAddress = HttpContext.Current.Request.UserHostAddress;

                if (!IsIpAddressAllowed(ipAddress.Trim()))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Common", action = "PageNotFound" }));
                }
            }
            base.OnActionExecuting(context);
        }

        private bool IsIpAddressAllowed(string ipAddress)
        {
            if (!HttpContext.Current.Request.IsLocal)
            {
                if (!string.IsNullOrWhiteSpace(ipAddress))
                {
                    var commonSettings = EngineContext.Current.Resolve<CommonSettings>();
                    var accessService = EngineContext.Current.Resolve<IAccessLogService>();
                    var days = commonSettings.AllowedIPAddressHistory;
                    var latestAddresses = accessService.GetAccessIpAddressesWithinDateRange(DateTime.UtcNow.AddDays(0 - (days != 0 ? days : 3)));

                    return latestAddresses.Where(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase)).Any();
                }
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Infrastructure;
using Wfm.Services.Accounts;

namespace Wfm.Web.Framework
{
    public class FranchiseIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            //update IP address
            string currentIpAddress = webHelper.GetCurrentIpAddress();
            if (!String.IsNullOrEmpty(currentIpAddress))
            {
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var account = workContext.CurrentAccount;
                if (!currentIpAddress.Equals(account.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    var accountService = EngineContext.Current.Resolve<IAccountService>();
                    account.LastIpAddress = currentIpAddress;
                    accountService.Update(account, false);
                }
            }
        }
    }
}

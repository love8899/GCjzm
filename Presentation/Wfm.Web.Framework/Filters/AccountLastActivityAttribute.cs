using System;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Infrastructure;
using Wfm.Services.Accounts;

namespace Wfm.Web.Framework
{
    public class AccountLastActivityAttribute : ActionFilterAttribute
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

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var account = workContext.CurrentAccount;

            //update last activity date
            if (!account.LastActivityDateUtc.HasValue)
                account.LastActivityDateUtc = DateTime.UtcNow;
            if (account.LastActivityDateUtc.Value.AddMinutes(1.0) < DateTime.UtcNow)
            {
                var accountService = EngineContext.Current.Resolve<IAccountService>();
                account.LastActivityDateUtc = DateTime.UtcNow;
                accountService.Update(account, false);
            }
        }
    }
}

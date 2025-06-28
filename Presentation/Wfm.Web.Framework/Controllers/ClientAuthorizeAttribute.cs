using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Infrastructure;
using Wfm.Services.Accounts;
using Wfm.Services.Security;

namespace Wfm.Web.Framework.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
    public class ClientAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool _dontValidate = false;

        public string Roles { get; set; }

        public Account currentUser;

        public ClientAuthorizeAttribute()
            : this(false)
        {
        }

        public ClientAuthorizeAttribute(bool dontValidate)
        {
            this._dontValidate = dontValidate;
        }

        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }


        private IEnumerable<ClientAuthorizeAttribute> GetClientAuthorizeAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(ClientAuthorizeAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(ClientAuthorizeAttribute), true))
                .OfType<ClientAuthorizeAttribute>();
        }

        private bool IsClientPageRequested(AuthorizationContext filterContext)
        {
            var clientAttributes = GetClientAuthorizeAttributes(filterContext.ActionDescriptor);
            if (clientAttributes != null && clientAttributes.Any())
                return true;
            return false;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [ClientAuthorize] attribute when a child action cache is active");

            if (IsClientPageRequested(filterContext))
            {
                if (!this.HasClientAccess(filterContext))
                    this.HandleUnauthorizedRequest(filterContext);
            }
        }

        public virtual bool HasClientAccess(AuthorizationContext filterContext)
        {
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            bool result = permissionService.Authorize(StandardPermissionProvider.AccessClientPanel);
            return result;

           // User user = Session["CurrentUser"];

            //var roleAccessService = EngineContext.Current.Resolve<IRoleAccessService>();

            //string[] RoleList = Roles.Split(',');

            //bool result = roleAccessService.Authorize(RoleList);

            //return result;
        }
    }
}

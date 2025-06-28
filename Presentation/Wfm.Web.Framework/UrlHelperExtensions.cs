using System;
using System.Web;
using System.Web.Mvc;

namespace Wfm.Web.Framework
{
    public static class UrlHelperExtensions
    {
        public static string LogOn(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Login", "Account", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Login", "Account");
        }

        public static string LogOff(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Logout", "Account", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Logout", "Account");
        }


        public static string AbsoluteAction(this UrlHelper urlHelper, string action, string controller)
        {
            Uri requestUrl = urlHelper.RequestContext.HttpContext.Request.Url;

            string absoluteAction = string.Format("{0}://{1}{2}",
                                        requestUrl.Scheme,
                                        requestUrl.Authority,
                                        urlHelper.Action(action, controller));

            return absoluteAction;
        }

        public static string AbsoluteAction(this UrlHelper url, string action, string controller, object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;

            string absoluteAction = string.Format("{0}{1}",
                requestUrl.GetLeftPart(UriPartial.Authority),
                url.Action(action, controller, routeValues));

            return absoluteAction;
        }

        public static string Absolute(this UrlHelper urlHelper, string relativeUrl)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;

            return string.Format("{0}://{1}{2}",
                (request.IsSecureConnection) ? "https" : "http",
                request.Headers["Host"],
                VirtualPathUtility.ToAbsolute(relativeUrl));
        }

    }
}

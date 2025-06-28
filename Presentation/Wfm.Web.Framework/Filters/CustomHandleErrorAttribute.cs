using System;
using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Core.Infrastructure;
using Wfm.Services.Logging;


namespace Wfm.Web.Framework.Filters
{
    public class HandleChildActionAccessErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //_LogException(filterContext.Exception);

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            if (!filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

                //var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                //filterContext.Result = new ViewResult
                //{
                //    ViewName = "_ChildActionOnlyError",
                //    MasterName = Master,
                //    ViewData = new ViewDataDictionary(model),
                //    TempData = filterContext.Controller.TempData
                //};

                filterContext.Result = new RedirectToRouteResult("AccessDenied",
                    new RouteValueDictionary(new { pageUrl = filterContext.HttpContext.Request.RawUrl }));
            }
        }


        private void _LogException(Exception exc)
        {
            var logger = EngineContext.Current.Resolve<ILogger>();
            logger.Error(exc.Message, exc);
        }
    }
}

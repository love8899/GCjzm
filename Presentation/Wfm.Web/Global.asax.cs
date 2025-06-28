using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain;
using Wfm.Core.Domain.Common;
using Wfm.Core.Infrastructure;
using Wfm.Services.Logging;
using Wfm.Web.Controllers;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Framework.Mvc.Routes;
using Wfm.Web.Framework.Themes;
using StackExchange.Profiling;

namespace Wfm.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Wfm.Web.Controllers" }
            );
            
        }

        protected void Application_Start()
        {
            // Remove X-AspNet-Version
            MvcHandler.DisableMvcResponseHeader = true;
            
            //initialize engine context
            EngineContext.Initialize(false);

            ////model binders
            //ModelBinders.Binders.Add(typeof(BaseWfmModel), new WfmModelBinder());
            
            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            if (databaseInstalled)
            {
                //remove all view engines
                ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }
            else
                Environment.FailFast("Unable to connect to the database.");

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new WfmMetadataProvider();

            //Registering some regular mvc stuff
            AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters); //todo: enable this one for IP addresses
            RegisterRoutes(RouteTable.Routes);
            
            //fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new WfmValidatorFactory()));

            //start scheduled tasks - moved to Wfm.WindowsService
            //if (databaseInstalled)
            //{
            //    TaskManager.Instance.Initialize();
            //    TaskManager.Instance.Start();
            //}

            //log application start
            if (databaseInstalled)
            {
                try
                {
                    //log
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Information("Application started", null, null);
                }
                catch (Exception)
                {
                    //don't throw new exception if occurs
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            ////keep alive page requested (we ignore it to prevent creating a guest customer records)
            //string keepAliveUrl = string.Format("{0}keepalive/index", webHelper.GetFranchiseUrl());
            //if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
            //    return;

            //ensure database is installed
            //if (!DataSettingsHelper.DatabaseIsInstalled())
            //    return;

            //miniprofiler
            if (EngineContext.Current.Resolve<FranchiseInformationSettings>().DisplayMiniProfilerInPublicWeb)
            {
                MiniProfiler.Start();
                //store a value indicating whether profiler was started
                HttpContext.Current.Items["wfm.MiniProfilerStarted"] = true;
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //miniprofiler
            var miniProfilerStarted = HttpContext.Current.Items.Contains("wfm.MiniProfilerStarted") &&
                 Convert.ToBoolean(HttpContext.Current.Items["wfm.MiniProfilerStarted"]);
            if (miniProfilerStarted)
            {
                MiniProfiler.Stop();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //we don't do it in Application_BeginRequest because a user is not authenticated yet
            SetWorkingCulture();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            //log error
            LogException(exception);

            //process 404 HTTP errors
            var httpException = exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                if (!webHelper.IsStaticResource(this.Request))
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;

                    // Call target Controller and pass the routeData.
                    IController errorController = EngineContext.Current.Resolve<CommonController>();

                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Common");
                    routeData.Values.Add("action", "PageNotFound");

                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }
            }
        }
        
        protected void SetWorkingCulture()
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            ////keep alive page requested (we ignore it to prevent creation of guest customer records)
            //string keepAliveUrl = string.Format("{0}keepalive/index", webHelper.GetFranchiseUrl());
            //if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
            //    return;


            if (webHelper.GetThisPageUrl(false).StartsWith(string.Format("{0}admin", webHelper.GetFranchiseUrl()),
                StringComparison.InvariantCultureIgnoreCase))
            {
                //admin area


                //always set culture to 'en-US'
                //we set culture of admin area to 'en-US' because current implementation of Telerik grid 
                //doesn't work well in other cultures
                //e.g., editing decimal value in russian culture
                var culture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            else
            {
                //public web
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;
            
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore 404 HTTP errors
            var httpException = exc as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404 &&
                !EngineContext.Current.Resolve<CommonSettings>().Log404Errors)
                return;

            try
            {
                //log
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentAccount);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}
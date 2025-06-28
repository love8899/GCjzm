using System;
using Autofac;
using Wfm.Core;
using Wfm.Core.Infrastructure;

//using Autofac.Integration.Wcf;

namespace WcfServices
{
    public class Global : System.Web.HttpApplication
    {
        private static IContainer container { get; set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            //initialize engine context
            //EngineContext.Initialize(false);
            container = AutofacContainerBuilder.BuildContainer();
            //AutofacHostFactory.Container = container;
            //container.Resolve<ICacheManager>();
           // var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            AutoMapperBuilder.CreateMap();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            //public web
            //var workContext = EngineContext.Current.Resolve<IWorkContext>();
            //var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
            //Thread.CurrentThread.CurrentCulture = culture;
            //Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
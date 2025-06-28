using Autofac;
using Autofac.Core;
using Wfm.Core.Caching;
using Wfm.Core.Infrastructure;
using Wfm.Core.Infrastructure.DependencyManagement;
using Wfm.Services.Logging;
using Wfm.Web.Controllers;
using Wfm.Web.Infrastructure.Installation;

namespace Wfm.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();

            //we cache presentation models between requests
            builder.RegisterType<BlogController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));
            builder.RegisterType<CandidateController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));
            builder.RegisterType<JobPostController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));
            builder.RegisterType<CommonController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));
            builder.RegisterType<WidgetController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));


            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}

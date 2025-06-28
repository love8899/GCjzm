using Autofac;
using Autofac.Core;
using Wfm.Core.Caching;
using Wfm.Core.Infrastructure;
using Wfm.Core.Infrastructure.DependencyManagement;
using Wfm.Plugin.Widgets.NivoSlider.Controllers;

namespace Wfm.Plugin.Widgets.NivoSlider.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //we cache presentation models between requests
            builder.RegisterType<WidgetsNivoSliderController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));
        }

        public int Order
        {
            get { return 2; }
        }
    }
}

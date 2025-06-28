using Autofac;
using Wfm.Core.Infrastructure;

namespace WcfServices
{
    public static class AutofacContainerBuilder 
    {
        /// <summary>
        /// Configures and builds Autofac IOC container.
        /// </summary>
        /// <returns></returns>
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            var typeFinder = new AppDomainTypeFinder();

            var _register = new DependencyRegistrar();
            _register.Register(builder, typeFinder);
            
            // build container
            return builder.Build();
        }

       
    }
}
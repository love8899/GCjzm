using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Core.Infrastructure;

namespace Wfm.Web.Framework.Mvc
{
    public class WfmDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            return EngineContext.Current.ContainerManager.ResolveOptional(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
            return (IEnumerable<object>) EngineContext.Current.Resolve(type);
        }
    }
}

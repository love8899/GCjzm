using Autofac;
using Autofac.Core;
using Wfm.Admin.Controllers;
using Wfm.Core.Caching;
using Wfm.Core.Infrastructure;
using Wfm.Core.Infrastructure.DependencyManagement;
using Wfm.Services.Common;
using Wfm.Services.Employees;
using Wfm.Services.Payroll;


namespace Wfm.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //we cache presentation models between requests
            builder.RegisterType<HomeController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"));
            //web servive
            builder.RegisterType<WebService>().As<IWebService>().InstancePerLifetimeScope();
            builder.RegisterType<TaxFormService>().As<ITaxFormService>().InstancePerLifetimeScope();
            builder.RegisterType<PayrollItemService>().As<IPayrollItemService>().InstancePerLifetimeScope();
            builder.RegisterType<TaxFormBoxService>().As<ITaxFormBoxService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateWSIBCommonRateService>().As<ICandidateWSIBCommonRateService>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}

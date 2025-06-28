using Autofac;
using Wfm.Client.Models.Rescheduling;
using Wfm.Core.Infrastructure;
using Wfm.Core.Infrastructure.DependencyManagement;
using Wfm.Services.Scheduling;
using Wfm.Shared.Models.Scheduling;

namespace Wfm.Client.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<EmployeeScheduleDetailModel>().As<IEmployeeScheduleDetailModel>().InstancePerDependency();
            builder.RegisterType<ScheduleDetailErrorModel>().As<IScheduleDetailErrorModel>().InstancePerDependency();
            builder.RegisterType<ShiftViewDayTuple>().As<IShiftViewDay>().InstancePerDependency();
            builder.RegisterType<VacancyViewModel>().As<IVacancyViewModel>().InstancePerDependency();
            builder.RegisterType<RoleScheduleViewModel>().As<IRoleScheduleViewModel>().InstancePerDependency();
            builder.RegisterType<EmployeeScheduleViewModel>().As<IEmployeeScheduleViewModel>().InstancePerDependency();
            builder.RegisterType<EmployeeRescheduling_BL>().InstancePerDependency();
        }

        public int Order
        {
            get { return 3; }
        }
    }
}
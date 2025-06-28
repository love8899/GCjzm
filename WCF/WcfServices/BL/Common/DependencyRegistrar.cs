using System;
using System.Collections.Generic;
using System.Web;
using Autofac;
using Wfm.Services.TimeSheet;
using Wfm.Core.Caching;
using Wfm.Core.Fakes;
using Wfm.Core;
using Wfm.Services.Messages;
using Wfm.Services.Policies;
using Wfm.Services.Payroll;
using Wfm.Services.ClockTime;
using Wfm.Services.Security;
using Wfm.Services.Accounts;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.JobOrders;
using Wfm.Services.Franchises;
using Wfm.Data;
using Wfm.Core.Data;
using Wfm.Services.Configuration;
using Autofac.Core;
using Wfm.Services.Companies;
using Wfm.Services.Candidates;
using Wfm.Core.Infrastructure;
using Wfm.Core.Configuration;
using Wfm.Services.Helpers;
using System.Reflection;
using Autofac.Builder;
using Wfm.Core.Infrastructure.DependencyManagement;
using Wfm.Services.Common;
using Wfm.Services.Tasks;
using Wfm.Web.Framework;
using Wfm.Services.Events;
using Wfm.Services.Authentication;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Media;
using Wfm.Services.Incident;

namespace WcfServices
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            //user agent helper
            //builder.RegisterType<UserAgentHelper>().As<IUserAgentHelper>().InstancePerLifetimeScope();

            //controllers
            //builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();


            builder.Register(x => x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new WfmObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            }
            else
            {
                builder.Register<IDbContext>(c => new WfmObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerLifetimeScope();
            }

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("wfm_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("wfm_cache_per_request").InstancePerLifetimeScope();

            //builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfigurationProvider<>));

            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            //builder.RegisterType<WebFranchiseContext>().As<IFranchiseContext>().InstancePerLifetimeScope();


            //pass MemoryCacheManager to SettingService as cacheManager (cache settings between requests)
            builder.RegisterType<SettingService>().As<ISettingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterSource(new SettingsSource());

            //pass MemoryCacheManager to LocalizationService as cacheManager (cache locales between requests)
            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"))
                .InstancePerLifetimeScope();

            //pass MemoryCacheManager to LocalizedEntityService as cacheManager (cache locales between requests)
            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"))
                .InstancePerLifetimeScope();

            //Candidate Services
            builder.RegisterType<CandidateService>().As<ICandidateService>().InstancePerLifetimeScope();
            //builder.RegisterType<PictureService>().As<IPictureService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateAddressService>().As<ICandidateAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<CanadiateJobOrderService>().As<ICandidateJobOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateJobOrderStatusService>().As<ICandidateJobOrderStatusService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateJobOrderStatusHistoryService>().As<ICandidateJobOrderStatusHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateKeySkillService>().As<ICandidateKeySkillService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateBankAccountService>().As<ICandidateBankAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateWorkHistoryService>().As<ICandidateWorkHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidatePictureService>().As<ICandidatePictureService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateTestResultService>().As<ICandidateTestResultService>().InstancePerLifetimeScope();
            builder.RegisterType<EmployeeTimeChartHistoryService>().As<IEmployeeTimeChartHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<AttendanceListService>().As<IAttendanceListService>().InstancePerLifetimeScope();
            builder.RegisterType<CandidateJobOrderPlacementService>().As<ICandidateJobOrderPlacementService>().InstancePerLifetimeScope();

            builder.RegisterType<CandidateBlacklistService>().As<ICandidateBlacklistService>().InstancePerLifetimeScope();

            builder.RegisterType<PaymentHistoryService>().As<IPaymentHistoryService>().InstancePerLifetimeScope();

            //Schedule Task Service
            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();

            //Configuration Services
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();


            //Common Services 
            //builder.RegisterType<GenderService>().As<IGenderService>().InstancePerLifetimeScope();
            //builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<SalutationService>().As<ISalutationService>().InstancePerLifetimeScope();
            //builder.RegisterType<TransportationService>().As<ITransportationService>().InstancePerLifetimeScope();
            //builder.RegisterType<SourceService>().As<ISourceService>().InstancePerLifetimeScope();
            //builder.RegisterType<AddressTypeService>().As<IAddressTypeService>().InstancePerLifetimeScope();
            //builder.RegisterType<EthnicTypeService>().As<IEthnicTypeService>().InstancePerLifetimeScope();
            //builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerLifetimeScope();
            //builder.RegisterType<VetranTypeService>().As<IVetranTypeService>().InstancePerLifetimeScope();
            //builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();
            //builder.RegisterType<IntersectionService>().As<IIntersectionService>().InstancePerLifetimeScope();
            //builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerLifetimeScope();
            //builder.RegisterType<PdfService>().As<IPdfService>().InstancePerLifetimeScope();
            //builder.RegisterType<SearchTermService>().As<ISearchTermService>().InstancePerLifetimeScope();
            //builder.RegisterType<PositionService>().As<IPositionService>().InstancePerLifetimeScope();
            builder.RegisterType<ShiftService>().As<IShiftService>().InstancePerLifetimeScope();
            //builder.RegisterType<SkillService>().As<ISkillService>().InstancePerLifetimeScope();

            //builder.RegisterType<SecurityQuestionService>().As<ISecurityQuestionService>().InstancePerLifetimeScope();


            ////Directory
            //builder.RegisterType<MapLookupService>().As<IMapLookupService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().InstancePerLifetimeScope();
            builder.RegisterType<CityService>().As<ICityService>().InstancePerLifetimeScope();

            //Companies Sevices
            builder.RegisterType<CompanyDivisionService>().As<ICompanyDivisionService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyService>().As<ICompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyContactService>().As<ICompanyContactService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyCandidateService>().As<ICompanyCandidateService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyBillingService>().As<ICompanyBillingService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyDepartmentService>().As<ICompanyDepartmentService>().InstancePerLifetimeScope();
            builder.RegisterType<RecruiterCompanyService>().As<IRecruiterCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyVendorService>().As<ICompanyVendorService>().InstancePerLifetimeScope();

            //Employee Service
            //builder.RegisterType<EmployeeService>().As<IEmployeeService>().InstancePerLifetimeScope();

            //Franchises Service 
            builder.RegisterType<FranchiseService>().As<IFranchiseService>().InstancePerLifetimeScope();
            //builder.RegisterType<VendorCertificateService>().As<IVendorCertificateService>().InstancePerLifetimeScope();

            //Incident Service
            builder.RegisterType<IncidentService>().As<IIncidentService>().InstancePerLifetimeScope();

            //Timeoff Service
            //builder.RegisterType<TimeoffService>().As<ITimeoffService>().InstancePerLifetimeScope();

            //JobOrder Service
            builder.RegisterType<JobOrderCategoryService>().As<IJobOrderCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<JobOrderService>().As<IJobOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<JobOrderTypeService>().As<IJobOrderTypeService>().InstancePerLifetimeScope();
            builder.RegisterType<JobOrderStatusService>().As<IJobOrderStatusService>().InstancePerLifetimeScope();
            builder.RegisterType<JobOrderTestCategoryService>().As<IJobOrderTestCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<BasicJobOrderInfoService>().As<IBasicJobOrderInfoService>().InstancePerLifetimeScope();
            //builder.RegisterType<OTRulesForJobOrderService>().As<IOTRulesForJobOrderService>().InstancePerLifetimeScope();

            //JobPost Service
            //builder.RegisterType<JobPostService>().As<IJobPostService>().InstancePerLifetimeScope();

            //Language Service
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerLifetimeScope();
            // builder.RegisterType<Loca>().As<ILanguageService>().InstancePerLifetimeScope();

            //Logging Service
            builder.RegisterType<AccessLogService>().As<IAccessLogService>().InstancePerLifetimeScope();
            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<ActivityLogService>().As<IActivityLogService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"))
                .InstancePerLifetimeScope();
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();

            //Media Service
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentTypeService>().As<IAttachmentTypeService>().InstancePerLifetimeScope();
            builder.RegisterType<DocumentTypeService>().As<IDocumentTypeService>().InstancePerLifetimeScope();


            //Security Service
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<AccountPasswordPolicyService>().As<IAccountPasswordPolicyService>().InstancePerLifetimeScope();

            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            builder.RegisterType<PermissionService>().As<IPermissionService>()
                   .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("wfm_cache_static"))
                   .InstancePerLifetimeScope();

            //Authentication Service
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerLifetimeScope();

            //Calendar Service
            builder.RegisterType<PayrollCalendarService>().As<IPayrollCalendarService>().InstancePerLifetimeScope();

            //TimeClocks Service
            builder.RegisterType<ClockDeviceService>().As<IClockDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<SmartCardService>().As<ISmartCardService>().InstancePerLifetimeScope();
            builder.RegisterType<ClockTimeService>().As<IClockTimeService>().InstancePerLifetimeScope();

            //TimeSheet Service
            builder.RegisterType<TimeSheetService>().As<ITimeSheetService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkTimeService>().As<IWorkTimeService>().InstancePerLifetimeScope();

            //Payroll Service
            builder.RegisterType<OvertimeRuleSettingService>().As<IOvertimeRuleSettingService>().InstancePerLifetimeScope();

            //Policy Service
            builder.RegisterType<MealPolicyService>().As<IMealPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<BreakPolicyService>().As<IBreakPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<RoundingPolicyService>().As<IRoundingPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<SchedulePolicyService>().As<ISchedulePolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<PasswordPolicyService>().As<IPasswordPolicyService>().InstancePerLifetimeScope();


            //Scheduling Service
            //builder.RegisterType<SchedulingDemandService>().As<ISchedulingDemandService>().InstancePerLifetimeScope();
            //builder.RegisterType<AutoScheduleService>().As<IAutoScheduleService>().InstancePerLifetimeScope();

            //Helper Service
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<GenericHelper>().As<IGenericHelper>().InstancePerLifetimeScope();

            ////Export Service
            //builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerLifetimeScope();
            ////Import Service
            //builder.RegisterType<ImportManager>().As<IImportManager>().InstancePerLifetimeScope();


            //Message Service
            builder.RegisterType<MessageHistoryService>().As<IMessageHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<CampaignService>().As<ICampaignService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerLifetimeScope();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerLifetimeScope();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();
            builder.RegisterType<MessageCategoryService>().As<IMessageCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<ClientNotificationService>().As<IClientNotificationService>().InstancePerLifetimeScope();

            // Feature
            //builder.RegisterType<FeatureService>().As<IFeatureService>().InstancePerLifetimeScope();
            //builder.RegisterType<UserFeatureService>().As<IUserFeatureService>().InstancePerLifetimeScope();

            //Register event consumers
            //var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            //foreach (var consumer in consumers)
            //{
            //    builder.RegisterType(consumer)
            //        .As(consumer.FindInterfaces((type, criteria) =>
            //        {
            //            var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
            //            return isMatch;
            //        }, typeof(IConsumer<>)))
            //        .InstancePerLifetimeScope();
            //}
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();

        }

        public int Order
        {
            get { return 0; }
        }
        public class SettingsSource : IRegistrationSource
        {
            static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
                "BuildRegistration",
                BindingFlags.Static | BindingFlags.NonPublic);

            public IEnumerable<IComponentRegistration> RegistrationsFor(
                    Service service,
                    Func<Service, IEnumerable<IComponentRegistration>> registrations)
            {
                var ts = service as TypedService;
                if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
                {
                    var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                    yield return (IComponentRegistration)buildMethod.Invoke(null, null);
                }
            }

            static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
            {
                return RegistrationBuilder
                    .ForDelegate((c, p) =>
                    {
                        return c.Resolve<ISettingService>().LoadSetting<TSettings>();
                    })
                    .InstancePerLifetimeScope()
                    .CreateRegistration();
            }

            public bool IsAdapterForIndividualComponents { get { return false; } }
        }
    }
}
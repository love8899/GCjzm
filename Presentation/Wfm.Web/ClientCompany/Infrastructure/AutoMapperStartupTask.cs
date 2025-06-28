using AutoMapper;
using Wfm.Core.Infrastructure;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Configuration;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Localization;
using Wfm.Core.Domain.Logging;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Client.Models.Candidate;
using Wfm.Client.Models.Common;
using Wfm.Client.Models.Companies;
using Wfm.Client.Models.CompanyBilling;
using Wfm.Client.Models.Settings;
using Wfm.Client.Models.Franchises;
using Wfm.Client.Models.JobOrder;
using Wfm.Client.Models.Logging;
using Wfm.Client.Models.Accounts;
using Wfm.Client.Models.ClockTime;
using Wfm.Client.Models.TimeSheet;
using Wfm.Client.Models.CompanyContact;
using Wfm.Client.Models.Incident;
using Wfm.Core.Domain.Incident;
using Wfm.Core.Domain.JobPosting;
using Wfm.Client.Models.Employees;
using Wfm.Core.Domain.Employees;
using Wfm.Shared.Models.JobPosting;
using Wfm.Shared.Mapping;
using Wfm.Shared.Models.Accounts;
using Wfm.Client.Models.Media;
using Wfm.Core.Domain.Media;
using Wfm.Shared.Models.Localization;


namespace Wfm.Client.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            Mapper.CreateMap<AccountDelegate, AccountDelegateModel>()
                .ForMember(m => m.DelegateAccountName, opt => opt.MapFrom(e => e.DelegateAccount.Username));
            Mapper.CreateMap<AccountDelegateModel, AccountDelegate>();
            Mapper.CreateMap<AccountDelegateHistory, AccountDelegateHistoryModel>()
                .ForMember(m => m.DelegateAccountName, opt => opt.MapFrom(e => e.DelegateAccount.Username));

            Mapper.CreateMap<Candidate, CandidateModel>();
            Mapper.CreateMap<CandidateModel, Candidate>();


            Mapper.CreateMap<Candidate, CandidateBasicInfoModel>();
            Mapper.CreateMap<CandidateBasicInfoModel, Candidate>();


            Mapper.CreateMap<JobOrder, JobOrderModel>();
            Mapper.CreateMap<JobOrderModel, JobOrder>();

            Mapper.CreateMap<JobPosting, JobPostingModel>();
            Mapper.CreateMap<JobPostingModel, JobPosting>();

            Mapper.CreateMap<JobPosting, JobPostingEditModel>();
            Mapper.CreateMap<JobPostingEditModel, JobPosting>();

            Mapper.CreateMap<Company, CompanyModel>();
            Mapper.CreateMap<CompanyModel, Company>();

            Mapper.CreateMap<CompanyDepartment, CompanyDepartmentModel>();
            Mapper.CreateMap<CompanyDepartmentModel, CompanyDepartment>();

            Mapper.CreateMap<CompanyBillingRate, CompanyBillingRateModel>();
            Mapper.CreateMap<CompanyBillingRateModel, CompanyBillingRate>();

            Mapper.CreateMap<CompanyVendor, CompanyVendorModel>()
              .ForMember(dest => dest.VendorName, mo => mo.MapFrom(src => src.Vendor.FranchiseName))
              .ForMember(dest => dest.VendorWebsite, mo => mo.MapFrom(src => src.Vendor.WebSite));

            Mapper.CreateMap<VendorCertificate, VendorCertificateModel>();

           
            Mapper.CreateMap<JobOrderCategory, JobOrderCategoryModel>();
            Mapper.CreateMap<JobOrderCategoryModel, JobOrderCategory>();

            Mapper.CreateMap<Shift, ShiftModel>();
            Mapper.CreateMap<ShiftModel, Shift>();

            Mapper.CreateMap<JobOrderType, JobOrderTypeModel>();
            Mapper.CreateMap<JobOrderTypeModel, JobOrderType>();

            Mapper.CreateMap<JobOrderStatus, JobOrderStatusModel>();
            Mapper.CreateMap<JobOrderStatusModel, JobOrderStatus>();

            Mapper.CreateMap<Franchise, FranchiseModel>();
            Mapper.CreateMap<FranchiseModel, Franchise>();

            Mapper.CreateMap<FranchiseAddress, FranchiseAddressModel>();
            Mapper.CreateMap<FranchiseAddressModel, FranchiseAddress>();

            Mapper.CreateMap<CompanyLocation, CompanyLocationModel>();
            Mapper.CreateMap<CompanyLocationModel, CompanyLocation>();

            Mapper.CreateMap<CandidateAttachment, CandidateAttachmentModel>();
            Mapper.CreateMap<CandidateAttachmentModel, CandidateAttachment>();

            Mapper.CreateMap<AttachmentTypeModel, AttachmentType>();
            Mapper.CreateMap<AttachmentType, AttachmentTypeModel>();
            /****** CLONED *****/
            Mapper.CreateMap<CandidateJobOrder, CandidateJobOrderModel>();
            Mapper.CreateMap<CandidateJobOrderModel, CandidateJobOrder>();

            // re-scheduling
            Mapper.CreateMap<CandidateJobOrder, CandidatePlacementModel>();
            Mapper.CreateMap<CandidatePlacementModel, CandidateJobOrder>();

            Mapper.CreateMap<CandidateKeySkill, CandidateKeySkillModel>();
            Mapper.CreateMap<CandidateKeySkillModel, CandidateKeySkill>();

            Mapper.CreateMap<CandidateTestResult, CandidateTestResultModel>();

            Mapper.CreateMap<CandidateJobOrderStatus, CandidateJobOrderStatusModel>();
            Mapper.CreateMap<CandidateJobOrderStatusModel, CandidateJobOrderStatus>();

            Mapper.CreateMap<CandidateAddress, CandidateAddressModel>();
            Mapper.CreateMap<CandidateAddressModel, CandidateAddress>();


            Mapper.CreateMap<EthnicType, EthnicTypeModel>();
            Mapper.CreateMap<EthnicTypeModel, EthnicType>();

            Mapper.CreateMap<Language, LanguageModel>();
            Mapper.CreateMap<LanguageModel, Language>();

            Mapper.CreateMap<LocaleStringResource, LocaleStringResourceModel>();
            Mapper.CreateMap<LocaleStringResourceModel, LocaleStringResource>();

             /**** CLONED ***/

            Mapper.CreateMap<AccessLog, AccessLogModel>();
            Mapper.CreateMap<AccessLogModel, AccessLog>();

            Mapper.CreateMap<Account, AccountModel>();
            Mapper.CreateMap<AccountModel, Account>();

            Mapper.CreateMap<Skill, SkillModel>();
            Mapper.CreateMap<SkillModel, Skill>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<StateProvince, StateProvinceModel>();
            Mapper.CreateMap<StateProvinceModel, StateProvince>();

            Mapper.CreateMap<City, CityModel>();
            Mapper.CreateMap<CityModel, City>();


            Mapper.CreateMap<CandidateClockTime, CandidateClockTimeModel>()
                .ForMember(dest => dest.FranchiseId, mo => mo.MapFrom(src => src.Candidate == null ? 0 : src.Candidate.FranchiseId))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate == null ? string.Empty : src.Candidate.EmployeeId));
            Mapper.CreateMap<CandidateClockTimeModel, CandidateClockTime>();

            Mapper.CreateMap<CandidateWorkTime, CandidateWorkTimeModel>()
                //.ForMember(m => m.AllowDailyApproval, opt => opt.MapFrom(e => e.JobOrder.AllowDailyApproval))
                .ForMember(m => m.AllowSuperVisorModifyWorkTime, opt => opt.MapFrom(e => e.JobOrder.AllowSuperVisorModifyWorkTime));
            Mapper.CreateMap<CandidateWorkTimeModel, CandidateWorkTime>();


            Mapper.CreateMap<CompanyClockDevice, CompanyClockDeviceModel>();
            Mapper.CreateMap<CompanyClockDeviceModel, CompanyClockDevice>();

            Mapper.CreateMap<Setting, SettingModel>();
            Mapper.CreateMap<SettingModel, Setting>();

            Mapper.CreateMap<Account, CompanyContactViewModel>();

            Mapper.CreateMap<EmployeeWorkTimeApproval, EmployeeWorkTimeApprovalModel>();
            Mapper.CreateMap<EmployeeWorkTimeApproval, CandidateWorkTimeModel>();
            Mapper.CreateMap<EmployeeWorkTimeApproval, DailyTimeSheetModel>();

            Mapper.CreateMap<IncidentReportTemplateModel, IncidentReportTemplate>();
            Mapper.CreateMap<IncidentReportTemplate, IncidentReportTemplateModel>()
                .ForMember(m => m.IncidentCategoryCode, opt => opt.MapFrom(e => e.IncidentCategory.IncidentCategoryCode))
                .ForMember(m => m.Vendor, opt => opt.MapFrom(e => e.Franchise != null ? e.Franchise.FranchiseName : "<All Vendors>"));

            Mapper.CreateMap<EmployeeAvailability, EmployeeAvailabilityModel>()
                .ForMember(m => m.EmployeeIntId, opt => opt.MapFrom(e => e.EmployeeId))
                .ForMember(m => m.IsAllDay, opt => opt.MapFrom(e => e.WholeDay));
            Mapper.CreateMap<EmployeeAvailabilityModel, EmployeeAvailability>()
                .ForMember(e => e.EmployeeId, opt => opt.MapFrom(m => m.EmployeeIntId))
                .ForMember(e => e.WholeDay, opt => opt.MapFrom(m => m.IsAllDay));
            //

            Mapper.CreateMap<DailyAttendanceList, DailyAttendanceListModel>();

            DefaultMappingRule.Create();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
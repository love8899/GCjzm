using AutoMapper;
using System.Linq;
using System.Web;
using Wfm.Core.Domain.Forums;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Configuration;
using Wfm.Core.Domain.Features;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Localization;
using Wfm.Core.Domain.Logging;
using Wfm.Core.Domain.Messages;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Domain.Policies;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Payroll;
using Wfm.Core.Domain.Tasks;
using Wfm.Core.Infrastructure;
using Wfm.Admin.Models.Blogs;
using Wfm.Admin.Models.Forums;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.Common;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Models.CompanyBilling;
using Wfm.Admin.Models.Directory;
using Wfm.Admin.Models.Employee;
using Wfm.Admin.Models.Settings;
using Wfm.Admin.Models.Features;
using Wfm.Admin.Models.Franchises;
using Wfm.Admin.Models.JobOrder;
using Wfm.Admin.Models.Logging;
using Wfm.Admin.Models.Messages;
using Wfm.Admin.Models.Accounts;
using Wfm.Admin.Models.Test;
using Wfm.Admin.Models.ClockTime;
using Wfm.Admin.Models.TimeSheet;
using Wfm.Admin.Models.Policies;
using Wfm.Admin.Models.Payroll;
using Wfm.Admin.Models.ScheduleTask;
using Wfm.Admin.Models.CompanyContact;
using Wfm.Core.Domain.Incident;
using Wfm.Admin.Models.Incident;
using Wfm.Core.Domain.JobPosting;
using Wfm.Shared.Models.JobPosting;
using Wfm.Shared.Mapping;
using Wfm.Core.Domain.Media;
using Wfm.Admin.Models.Media;
using Wfm.Shared.Models.Accounts;
using Wfm.Shared.Models.Localization;
using Wfm.Shared.Models.Policies;
using Wfm.Admin.Models.Timeoff;
using Wfm.Core.Domain.Employees;
using System;
using Wfm.Core.Domain.Announcements;
using Wfm.Admin.Models.Announcements;
using Wfm.Core.Domain.WSIBS;
using Wfm.Admin.Models.WSIBs;
using Wfm.Core.Domain.WSIB;

namespace Wfm.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            Mapper.CreateMap<Candidate, CandidateModel>();
            Mapper.CreateMap<CandidateModel, Candidate>();
            Mapper.CreateMap<Candidate, CandidatePoolModel>()
               .ForMember(m => m.MajorIntersection, option => option.MapFrom(e =>
                   (!string.IsNullOrEmpty(e.MajorIntersection1) && !string.IsNullOrEmpty(e.MajorIntersection2) ? e.MajorIntersection1 + "/" + e.MajorIntersection2 : null)));

            Mapper.CreateMap<Candidate, EmployeeModel>();
            Mapper.CreateMap<EmployeeModel, Candidate>();

            Mapper.CreateMap<Candidate, EmployeeListModel>();

            Mapper.CreateMap<Candidate, ContactInfoModel>();
            Mapper.CreateMap<ContactInfoModel, Candidate>();

            Mapper.CreateMap<Candidate, CandidateOnboardingModel>();
            Mapper.CreateMap<CandidateOnboardingModel, Candidate>();

            Mapper.CreateMap<Candidate, CandidateBlacklist>();
            Mapper.CreateMap<CandidateBlacklist, CandidateBlacklistModel>()
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate.CandidateGuid))
                .ForMember(dest => dest.FranchiseId, mo => mo.MapFrom(src => src.Candidate.FranchiseId))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate.EmployeeId))
                .ForMember(dest => dest.GenderId, mo => mo.MapFrom(src => src.Candidate.GenderId))
                .ForMember(dest => dest.FirstName, mo => mo.MapFrom(src => src.Candidate.FirstName))
                .ForMember(dest => dest.LastName, mo => mo.MapFrom(src => src.Candidate.LastName))
                .ForMember(dest => dest.BirthDate, mo => mo.MapFrom(src => src.Candidate.BirthDate))
                .ForMember(dest => dest.SocialInsuranceNumber, mo => mo.MapFrom(src => src.Candidate.SocialInsuranceNumber));

            Mapper.CreateMap<Candidate, SimpleCandidateModel>();

            Mapper.CreateMap<JobOrder, JobOrderModel>()
                .ForMember(dest => dest.CompanyName, mo => mo.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.ShiftName, mo => mo.MapFrom(src => src.Shift.ShiftName));
            Mapper.CreateMap<JobOrderModel, JobOrder>();

            Mapper.CreateMap<JobOrder, DirectHireJobOrderModel>()
             .ForMember(dest => dest.CompanyName, mo => mo.MapFrom(src => src.Company.CompanyName))        
             .ForMember(dest => dest.CompanyGuid, mo => mo.MapFrom(src => src.Company.CompanyGuid))        
             .ForMember(dest => dest.Status, mo => mo.MapFrom(src => src.JobOrderStatus.JobOrderStatusName))        
             .ForMember(dest => dest.FeeTypeName, mo => mo.MapFrom(src => src.FeeType.FeeTypeName));
            Mapper.CreateMap<DirectHireJobOrderModel, JobOrder>();
            Mapper.CreateMap<DirectHireJobOrderList, DirectHireJobOrderListModel>();
            Mapper.CreateMap<DirectHireCandidatePoolList, DirectHireCandidatePoolListModel>();

            Mapper.CreateMap<JobOrderOpening, OpeningModel>()
                .ForMember(d => d.OpeningId, mo => mo.MapFrom(s => s.Id))
                .ForMember(d => d.Start, mo => mo.MapFrom(s => s.StartDate))
                .ForMember(d => d.End, mo => mo.MapFrom(s => s.EndDate ?? DateTime.MaxValue))
                .ForMember(d => d.Title, mo => mo.MapFrom(s => s.OpeningNumber.ToString()))
                .ForMember(d => d.Description, mo => mo.MapFrom(s => string.Empty))
                .ForMember(d => d.IsAllDay, mo => mo.MapFrom(s => true));

            Mapper.CreateMap<Company, CompanyModel>();
            Mapper.CreateMap<CompanyModel, Company>();

            Mapper.CreateMap<CandidateWithAddress, SearchCompanyCandidateModel>();

            Mapper.CreateMap<CompanyDepartment, CompanyDepartmentModel>();
            Mapper.CreateMap<CompanyDepartmentModel, CompanyDepartment>();

            Mapper.CreateMap<CompanyBillingRate, CompanyBillingRateModel>();
            Mapper.CreateMap<CompanyBillingRateModel, CompanyBillingRate>()
                .ForMember(dest => dest.Quotations, mo => mo.Ignore());

            Mapper.CreateMap<Quotation, QuotationModel>();

            Mapper.CreateMap<PasswordPolicy, PasswordPolicyModel>();
            Mapper.CreateMap<PasswordPolicyModel, PasswordPolicy>();

            Mapper.CreateMap<RoundingPolicy, RoundingPolicyModel>();
            Mapper.CreateMap<RoundingPolicyModel, RoundingPolicy>();

            Mapper.CreateMap<MealPolicy, MealPolicyModel>();
            Mapper.CreateMap<MealPolicyModel, MealPolicy>();

            Mapper.CreateMap<BreakPolicy, BreakPolicyModel>();
            Mapper.CreateMap<BreakPolicyModel, BreakPolicy>();

            Mapper.CreateMap<SchedulePolicy, SchedulePolicyModel>();
            Mapper.CreateMap<SchedulePolicyModel, SchedulePolicy>();

            Mapper.CreateMap<JobOrderCategory, JobOrderCategoryModel>();
            Mapper.CreateMap<JobOrderCategoryModel, JobOrderCategory>();

            Mapper.CreateMap<Shift, ShiftModel>();
            Mapper.CreateMap<ShiftModel, Shift>();

            Mapper.CreateMap<JobPosting, JobPostingModel>();
            Mapper.CreateMap<JobPostingModel, JobPosting>();

            Mapper.CreateMap<JobPosting, JobPostingEditModel>();
            Mapper.CreateMap<JobPostingEditModel, JobPosting>();

            Mapper.CreateMap<JobPosting, JobOrder>();

            Mapper.CreateMap<JobOrderType, JobOrderTypeModel>();
            Mapper.CreateMap<JobOrderTypeModel, JobOrderType>();

            Mapper.CreateMap<JobOrderStatus, JobOrderStatusModel>();
            Mapper.CreateMap<JobOrderStatusModel, JobOrderStatus>();

            Mapper.CreateMap<Franchise, FranchiseModel>();
            Mapper.CreateMap<FranchiseModel, Franchise>()
               .ForMember(dest => dest.Note, option => option.MapFrom(src => HttpUtility.HtmlDecode(src.Note)));

            Mapper.CreateMap<VendorCertificate, VendorCertificateModel>();
            Mapper.CreateMap<VendorCertificateModel, VendorCertificate>();

            Mapper.CreateMap<FranchiseAddress, FranchiseAddressModel>();
            Mapper.CreateMap<FranchiseAddressModel, FranchiseAddress>();

            Mapper.CreateMap<FranchiseSetting, FranchiseSettingModel>();
            Mapper.CreateMap<FranchiseSettingModel, FranchiseSetting>();

            Mapper.CreateMap<CompanySetting, CompanySettingModel>();
            Mapper.CreateMap<CompanySettingModel, CompanySetting>();

            Mapper.CreateMap<CompanyLocation, CompanyLocationModel>();
            Mapper.CreateMap<CompanyLocationModel, CompanyLocation>();

            Mapper.CreateMap<CompanyLocation, CompanyLocationListModel>();
            Mapper.CreateMap<CompanyLocationListModel, CompanyLocation>();

            Mapper.CreateMap<CompanyCandidate, CompanyCandidateModel>();
            Mapper.CreateMap<CompanyCandidateModel, CompanyCandidate>();

            Mapper.CreateMap<CompanyCandidate, CompanyCandidateViewModel>()
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate.CandidateGuid))
                .ForMember(dest => dest.CandidateId, mo => mo.MapFrom(src => src.Candidate.Id))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate.EmployeeId))
                .ForMember(dest => dest.Email, mo => mo.MapFrom(src => src.Candidate.Email))
                .ForMember(dest => dest.FirstName, mo => mo.MapFrom(src => src.Candidate.FirstName))
                .ForMember(dest => dest.LastName, mo => mo.MapFrom(src => src.Candidate.LastName))
                .ForMember(dest => dest.GenderName, mo => mo.MapFrom(src => src.Candidate.Gender.GenderName))
                .ForMember(dest => dest.HomePhone, mo => mo.MapFrom(src => src.Candidate.HomePhone))
                .ForMember(dest => dest.MobilePhone, mo => mo.MapFrom(src => src.Candidate.MobilePhone))
                .ForMember(dest => dest.PreferredWorkLocation, mo => mo.MapFrom(src => src.Candidate.PreferredWorkLocation))
                .ForMember(dest => dest.Intersection, mo => mo.MapFrom(src => src.Candidate.MajorIntersection1 + "/" + src.Candidate.MajorIntersection2))
                .ForMember(dest => dest.TransportationName, mo => mo.MapFrom(src => src.Candidate.Transportation != null ? src.Candidate.Transportation.TransportationName : null))
                .ForMember(dest => dest.ShiftName, mo => mo.MapFrom(src => src.Candidate.Shift != null ? src.Candidate.Shift.ShiftName : null))
                .ForMember(dest => dest.Status, mo => mo.MapFrom(src => src.EndDate == null || src.EndDate >= DateTime.Today ? "Active" : "Inactive"));

            Mapper.CreateMap<CompanyCandidatePoolVM, CandidatePoolModel>()
                .ForMember(dest => dest.Id, mo => mo.MapFrom(src => src.CompanyCandidateId));

            Mapper.CreateMap<OvertimeRuleSetting, OvertimeRuleSettingModel>();
            Mapper.CreateMap<OvertimeRuleSettingModel, OvertimeRuleSetting>();

            Mapper.CreateMap<CompanyOvertimeRule, CompanyOvertimeRuleModel>();
            Mapper.CreateMap<CompanyOvertimeRuleModel, CompanyOvertimeRule>();

            Mapper.CreateMap<JobOrderOvertimeRule, JobOrderOvertimeRuleModel>();
            Mapper.CreateMap<JobOrderOvertimeRuleModel, JobOrderOvertimeRule>();

            Mapper.CreateMap<CandidateAttachment, CandidateAttachmentModel>();
            Mapper.CreateMap<CandidateAttachmentModel, CandidateAttachment>();

            Mapper.CreateMap<CandidatePicture, CandidatePictureModel>();
            Mapper.CreateMap<CandidatePictureModel, CandidatePicture>();

            Mapper.CreateMap<CandidateTestResult, CandidateTestResultModel>();
            Mapper.CreateMap<CandidateTestResultModel, CandidateTestResult>();

            Mapper.CreateMap<TestCategory, TestCategoryModel>();
            Mapper.CreateMap<TestCategoryModel, TestCategory>();

            Mapper.CreateMap<TestQuestion, TestQuestionModel>();
            Mapper.CreateMap<TestQuestionModel, TestQuestion>();

            Mapper.CreateMap<TestChoice, TestChoiceModel>();
            Mapper.CreateMap<TestChoiceModel, TestChoice>();

            Mapper.CreateMap<TestMaterial, TestMaterialModel>();
            Mapper.CreateMap<TestMaterialModel, TestMaterial>();

            /**** CLONED ****/
            Mapper.CreateMap<CandidateJobOrder, CandidateJobOrderModel>();
            Mapper.CreateMap<CandidateJobOrderModel, CandidateJobOrder>();

            Mapper.CreateMap<CandidateKeySkill, CandidateKeySkillModel>();
            Mapper.CreateMap<CandidateKeySkillModel, CandidateKeySkill>();

            Mapper.CreateMap<CandidateWorkHistory, CandidateWorkHistoryModel>();
            Mapper.CreateMap<CandidateWorkHistoryModel, CandidateWorkHistory>();

            Mapper.CreateMap<CandidateJobOrderStatus, CandidateJobOrderStatusModel>();
            Mapper.CreateMap<CandidateJobOrderStatusModel, CandidateJobOrderStatus>();

            Mapper.CreateMap<CandidateAddress, CandidateAddressModel>();
            Mapper.CreateMap<CandidateAddressModel, CandidateAddress>();

            Mapper.CreateMap<CandidateJobOrderStatusHistory, CandidateJobOrderStatusHistoryModel>()
                .ForMember(dest => dest.JobOrderGuid, mo => mo.MapFrom(src => src.JobOrder.JobOrderGuid))
                .ForMember(dest => dest.JobTitle, mo => mo.MapFrom(src => src.JobOrder.JobTitle))
                .ForMember(dest => dest.CompanyName, mo => mo.MapFrom(src => src.JobOrder.Company.CompanyName))
                .ForMember(dest => dest.EnteredName, mo => mo.MapFrom(src => src.Account.FirstName + " " + src.Account.LastName));
            Mapper.CreateMap<CandidateJobOrderStatusHistoryModel, CandidateJobOrderStatusHistory>();

            Mapper.CreateMap<CandidateDirectHireStatusHistory, CandidateDirectHireStatusHistoryModel>();
            Mapper.CreateMap<CandidateDirectHireStatusHistoryModel, CandidateDirectHireStatusHistory>();

            Mapper.CreateMap<EthnicType, EthnicTypeModel>();
            Mapper.CreateMap<EthnicTypeModel, EthnicType>();

            Mapper.CreateMap<Language, LanguageModel>();
            Mapper.CreateMap<LanguageModel, Language>();

            Mapper.CreateMap<LocaleStringResource, LocaleStringResourceModel>();
            Mapper.CreateMap<LocaleStringResourceModel, LocaleStringResource>();

            /**** CLONED ****/

            Mapper.CreateMap<ActivityLogType, ActivityLogTypeModel>();
            Mapper.CreateMap<ActivityLogTypeModel, ActivityLogType>();

            Mapper.CreateMap<AccessLog, AccessLogModel>();
            Mapper.CreateMap<AccessLogModel, AccessLog>();

            Mapper.CreateMap<ActivityLog, ActivityLogModel>();
            Mapper.CreateMap<ActivityLogModel, ActivityLog>();

            Mapper.CreateMap<CandidateActivityLog, CandidateActivityLogModel>();
            Mapper.CreateMap<CandidateActivityLogModel, CandidateActivityLog>();


            Mapper.CreateMap<Intersection, IntersectionModel>();
            Mapper.CreateMap<IntersectionModel, Intersection>();

            Mapper.CreateMap<Account, AccountFullModel>();
            Mapper.CreateMap<AccountFullModel, Account>();

            Mapper.CreateMap<Account, AccountModel>()
                .ForMember(dest => dest.AccountRoleSystemName,
                           mo => mo.MapFrom(src => src.AccountRoles.FirstOrDefault() != null ? src.AccountRoles.FirstOrDefault().SystemName : string.Empty));
            Mapper.CreateMap<AccountModel, Account>();

            Mapper.CreateMap<Skill, SkillModel>();
            Mapper.CreateMap<SkillModel, Skill>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<StateProvince, StateProvinceModel>();
            Mapper.CreateMap<StateProvinceModel, StateProvince>();

            Mapper.CreateMap<City, CityModel>();
            Mapper.CreateMap<CityModel, City>();


            Mapper.CreateMap<CandidateSmartCard, CandidateSmartCardModel>()
                .ForMember(dest=>dest.FranchiseId,mo=>mo.MapFrom(src=>src.Candidate.FranchiseId))
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate.CandidateGuid))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate.EmployeeId))
                ;
            Mapper.CreateMap<CandidateSmartCardModel, CandidateSmartCard>();

            Mapper.CreateMap<CandidateSmartCard, CandidateSmartCardMatchModel>()
                .ForMember(dest => dest.FranchiseId, mo => mo.MapFrom(src => src.Candidate.FranchiseId));

            Mapper.CreateMap<HandTemplate, HandTemplateModel>()
                .ForMember(dest => dest.FranchiseId, mo => mo.MapFrom(src => src.Candidate.FranchiseId))
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate.CandidateGuid))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate.EmployeeId))
                ;
            Mapper.CreateMap<HandTemplateModel, HandTemplate>();

            Mapper.CreateMap<CandidateClockTime, CandidateClockTimeModel>()
                .ForMember(dest=>dest.FranchiseId,mo=>mo.MapFrom(src=>src.Candidate==null?0:src.Candidate.FranchiseId))
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate == null ? Guid.Empty : src.Candidate.CandidateGuid))
                .ForMember(dest=>dest.EmployeeId,mo=>mo.MapFrom(src=>src.Candidate==null?string.Empty:src.Candidate.EmployeeId));

            Mapper.CreateMap<CandidateClockTimeModel, CandidateClockTime>();

            Mapper.CreateMap<CompanyClockDevice, CompanyClockDeviceModel>();
            Mapper.CreateMap<CompanyClockDeviceModel, CompanyClockDevice>();


            Mapper.CreateMap<MessageTemplate, MessageTemplateModel>();
            Mapper.CreateMap<MessageTemplateModel, MessageTemplate>();

            Mapper.CreateMap<MessageHistory, MessageHistoryModel>()
                .ForMember(dest => dest.MessageCategory, mo => mo.MapFrom(src => src.MessageCategory.CategoryName));
            Mapper.CreateMap<MessageHistoryModel, MessageHistory>();

            Mapper.CreateMap<Message, MessageModel>()
                .ForMember(dest => dest.MailFrom, mo => mo.MapFrom(src => src.MessageHistory.MailFrom))
                .ForMember(dest => dest.FromName, mo => mo.MapFrom(src => src.MessageHistory.FromName))
                .ForMember(dest => dest.MailTo, mo => mo.MapFrom(src => src.MessageHistory.MailTo))
                .ForMember(dest => dest.ToName, mo => mo.MapFrom(src => (src.MessageHistory.ToName==null ||src.MessageHistory.ToName.Trim() == string.Empty)? src.MessageHistory.MailTo : src.MessageHistory.ToName))
                .ForMember(dest => dest.CC, mo => mo.MapFrom(src => src.MessageHistory.CC))
                .ForMember(dest => dest.Bcc, mo => mo.MapFrom(src => src.MessageHistory.Bcc))
                .ForMember(dest => dest.Subject, mo => mo.MapFrom(src => src.MessageHistory.Subject))
                .ForMember(dest => dest.Body, mo => mo.MapFrom(src => src.MessageHistory.Body))
                .ForMember(dest => dest.AttachmentFileName, mo => mo.MapFrom(src => src.MessageHistory.AttachmentFileName))
                .ForMember(dest => dest.Note, mo => mo.MapFrom(src => src.MessageHistory.Note))
                .ForMember(dest => dest.MessageCategory, mo => mo.MapFrom(src => src.MessageHistory.MessageCategory.CategoryName))
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.MapFrom(src => src.MessageHistory.CreatedOnUtc))
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());

            Mapper.CreateMap<Resume, ResumeModel>()
                .ForMember(dest => dest.Body, mo => mo.Ignore())
                .ForMember(dest => dest.HtmlBody, mo => mo.Ignore())
                .ForMember(dest => dest.Date, mo => mo.Ignore());

            Mapper.CreateMap<ResumeHistory, ResumeHistoryModel>()
                .ForMember(dest => dest.Who, mo => mo.MapFrom(src => src.Account.FullName));

            Mapper.CreateMap<RecruiterCompany, RecruiterCompanyModel>();
            Mapper.CreateMap<RecruiterCompanyModel, RecruiterCompany>();

            Mapper.CreateMap<CompanyVendor, CompanyVendorModel>()
                .ForMember(dest => dest.VendorName, mo => mo.MapFrom(src => src.Vendor.FranchiseName));
            Mapper.CreateMap<CompanyVendorModel, CompanyVendor>();

            //queued email
            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName, mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                .ForMember(dest => dest.AttachmentFileName, mo => mo.Ignore());


            //forums
            Mapper.CreateMap<ForumGroup, ForumGroupModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ForumGroupModel, ForumGroup>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Forums, mo => mo.Ignore());
            Mapper.CreateMap<Forum, ForumModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.ForumGroups, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ForumModel, Forum>()
                .ForMember(dest => dest.NumTopics, mo => mo.Ignore())
                .ForMember(dest => dest.NumPosts, mo => mo.Ignore())
                .ForMember(dest => dest.LastTopicId, mo => mo.Ignore())
                .ForMember(dest => dest.LastPostId, mo => mo.Ignore())
                .ForMember(dest => dest.LastPostAccountId, mo => mo.Ignore())
                .ForMember(dest => dest.LastPostTime, mo => mo.Ignore())
                .ForMember(dest => dest.ForumGroup, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());

            //blog
            Mapper.CreateMap<BlogPost, BlogPostModel>();
            Mapper.CreateMap<BlogPostModel, BlogPost>();

            Mapper.CreateMap<BlogComment, BlogCommentModel>();
            Mapper.CreateMap<BlogCommentModel, BlogComment>();

            Mapper.CreateMap<JobOrderTestCategory, JobOrderTestCategoryModel>();
            Mapper.CreateMap<JobOrderTestCategoryModel, JobOrderTestCategory>();

            Mapper.CreateMap<Shift, ShiftModel>();
            Mapper.CreateMap<ShiftModel, Shift>()
               .ForMember(dest => dest.MinStartTime, option => option.MapFrom(src => src.MinStartTime.HasValue ? src.MinStartTime.Value.TimeOfDay : (TimeSpan?)null))
               .ForMember(dest => dest.MaxEndTime, option => option.MapFrom(src => src.MaxEndTime.HasValue ? src.MaxEndTime.Value.TimeOfDay : (TimeSpan?)null));

            Mapper.CreateMap<CandidateWorkTime, CandidateWorkTimeModel>()
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate.CandidateGuid))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate.EmployeeId))
                .ForMember(dest => dest.EmployeeFirstName, mo => mo.MapFrom(src => src.Candidate.FirstName))
                .ForMember(dest => dest.EmployeeLastName, mo => mo.MapFrom(src => src.Candidate.LastName))
                .ForMember(dest => dest.JobOrderGuid, mo => mo.MapFrom(src => src.JobOrder.JobOrderGuid))
                .ForMember(dest => dest.JobTitle, mo => mo.MapFrom(src => src.JobOrder.JobTitle))
                .ForMember(dest => dest.CompanyName, mo => mo.MapFrom(src => src.JobOrder.Company.CompanyName))
                .ForMember(dest => dest.LocationName, mo => mo.MapFrom(src => src.JobOrder.Company.CompanyLocations.Where(x => x.Id == src.CompanyLocationId).FirstOrDefault().LocationName))
                .ForMember(dest => dest.DepartmentName, mo => mo.MapFrom(src => src.CompanyDepartmentId == 0 ? string.Empty : src.JobOrder.Company.CompanyDepartments.Where(x => x.Id == src.CompanyDepartmentId).FirstOrDefault().DepartmentName))
                .ForMember(dest => dest.ContactName, mo => mo.MapFrom(src => src.JobOrder.CompanyContact == null ? string.Empty : src.JobOrder.CompanyContact.FirstName + " " + src.JobOrder.CompanyContact.LastName));
                //.ForMember(dest => dest.ContactName, mo => mo.MapFrom(src => src.JobOrder.CompanyContact == null ? string.Empty : src.JobOrder.CompanyContact.FullName));

            Mapper.CreateMap<CandidateWorkTimeModel, CandidateWorkTime>();

            Mapper.CreateMap<CandidateWorkTime, CandidateWorkTimeBaseModel>();
            Mapper.CreateMap<CandidateWorkTimeLog, CandidateWorkTimeLogModel>();


            Mapper.CreateMap<CandidateMissingHour, CandidateMissingHourModel>()
                .ForMember(dest => dest.CandidateGuid, mo => mo.MapFrom(src => src.Candidate.CandidateGuid))
                .ForMember(dest => dest.FranchiseId, mo => mo.MapFrom(src => src.Candidate.FranchiseId))
                .ForMember(dest => dest.EmployeeId, mo => mo.MapFrom(src => src.Candidate.EmployeeId))
                .ForMember(dest => dest.EmployeeFirstName, mo => mo.MapFrom(src => src.Candidate.FirstName))
                .ForMember(dest => dest.EmployeeLastName, mo => mo.MapFrom(src => src.Candidate.LastName))
                .ForMember(dest => dest.JobOrderGuid, mo => mo.MapFrom(src => src.JobOrder.JobOrderGuid))
                .ForMember(dest => dest.JobTitle, mo => mo.MapFrom(src => src.JobOrder.JobTitle))
                .ForMember(dest => dest.CompanyName, mo => mo.MapFrom(src => src.JobOrder.Company.CompanyName))
                .ForMember(dest => dest.LocationName, mo => mo.MapFrom(src => src.JobOrder.Company.CompanyLocations.Where(x => x.Id == src.JobOrder.CompanyLocationId).FirstOrDefault().LocationName))
                .ForMember(dest => dest.DepartmentName, mo => mo.MapFrom(src => src.JobOrder.CompanyDepartmentId == 0 ? string.Empty : src.JobOrder.Company.CompanyDepartments.Where(x => x.Id == src.JobOrder.CompanyDepartmentId).FirstOrDefault().DepartmentName))
                .ForMember(dest => dest.ContactName, mo => mo.MapFrom(src => src.JobOrder.CompanyContact == null ? string.Empty : src.JobOrder.CompanyContact.FirstName + " " + src.JobOrder.CompanyContact.LastName));

            Mapper.CreateMap<CandidateMissingHourModel, CandidateMissingHour>();

            Mapper.CreateMap<MissingHourDocument, MissingHourDocumentModel>();
            Mapper.CreateMap<MissingHourDocumentModel, MissingHourDocument>();


            Mapper.CreateMap<Setting, SettingModel>();
            Mapper.CreateMap<SettingModel, Setting>();

            Mapper.CreateMap<ScheduleTask, ScheduleTaskModel>();
            Mapper.CreateMap<ScheduleTaskModel, ScheduleTask>();

            Mapper.CreateMap<ClientTimeSheetDocument, ClientTimeSheetDocumentModel>();
            Mapper.CreateMap<ClientTimeSheetDocumentModel, ClientTimeSheetDocument>();

            Mapper.CreateMap<EmployeeTimeoffTypeModel, EmployeeTimeoffType>();
            Mapper.CreateMap<EmployeeTimeoffType, EmployeeTimeoffTypeModel>();

            Mapper.CreateMap<CandidateWithAddress, CandidateWithAddressModel>();
            Mapper.CreateMap<CandidateWithAddressModel, CandidateWithAddress>();

            Mapper.CreateMap<Account, CompanyContactViewModel>();
            Mapper.CreateMap<Account, AccountsViewModel>()
                .ForMember(dest => dest.AccountRoleSystemName,
                           mo => mo.MapFrom(src => src.AccountRoles.FirstOrDefault() != null ? src.AccountRoles.FirstOrDefault().SystemName : string.Empty));

            Mapper.CreateMap<AccountPasswordPolicy, AccountPasswordPolicyModel>();
            Mapper.CreateMap<AccountPasswordPolicyModel, AccountPasswordPolicy>();

            Mapper.CreateMap<CandidateAddressWithName, CandidateAddressModel>();

            Mapper.CreateMap<CandidateJobHistory, CandidateJobHistoryModel>();

            Mapper.CreateMap<JobOrderWithCompanyAddress, JobOrderWithCompanyAddressModel>();
            Mapper.CreateMap<IncidentCategory, IncidentCategoryModel>()
                .ForMember(m => m.Vendor, opt => opt.MapFrom(e => e.Franchise != null ? e.Franchise.FranchiseName : "<All Vendors>"));
            Mapper.CreateMap<IncidentCategoryModel, IncidentCategory>();
            Mapper.CreateMap<IncidentReportTemplateModel, IncidentReportTemplate>();
            Mapper.CreateMap<IncidentReportTemplate, IncidentReportTemplateModel>();

            Mapper.CreateMap<CompanyContact, CompanyContactViewModel>();

            //
            Mapper.CreateMap<StatutoryHolidayModel, StatutoryHoliday>();
            Mapper.CreateMap<StatutoryHoliday, StatutoryHolidayModel>();
            //


            Mapper.CreateMap<DocumentType, DocumentTypeModel>();
            Mapper.CreateMap<DocumentTypeModel, DocumentType>();


            Mapper.CreateMap<MessageCategory, MessageCategoryModel>();
            Mapper.CreateMap<MessageCategoryModel, MessageCategory>();


            Mapper.CreateMap<Feature, FeatureModel>();
            Mapper.CreateMap<FeatureModel, Feature>();

            Mapper.CreateMap<UserFeature, UserFeatureModel>()
                .ForMember(d => d.FeatureCode, opt => opt.MapFrom(s => s.Feature.Code))
                .ForMember(d => d.FeatureName, opt => opt.MapFrom(s => s.Feature.Name))
                .ForMember(d => d.FeatureDescription, opt => opt.MapFrom(s => s.Feature.Description));

            Mapper.CreateMap<CompanyActivity, CompanyActivityModel>();
            Mapper.CreateMap<CompanyActivityModel, CompanyActivity>();

            Mapper.CreateMap<Announcement, AnnouncementModel>();
            Mapper.CreateMap<AnnouncementModel, Announcement>();

            Mapper.CreateMap<Company, CompanyBasicInformation>();
            Mapper.CreateMap<CompanyBasicInformation, Company>();

            Mapper.CreateMap<RecruiterCompany, RecruiterCompanySimpleModel>();
            Mapper.CreateMap<RecruiterCompanySimpleModel, RecruiterCompany>();

            Mapper.CreateMap<Position, PositionModel>();
            Mapper.CreateMap<PositionModel, Position>();

            Mapper.CreateMap<Account, CompanyContactModel>();
            Mapper.CreateMap<CompanyContactModel, Account>();

            Mapper.CreateMap<WSIB, WSIBModel>();
            Mapper.CreateMap<WSIBModel, WSIB>();

            Mapper.CreateMap<CompanyEmailTemplate, CompanyEmailTemplateModel>();
            Mapper.CreateMap<CompanyEmailTemplateModel, CompanyEmailTemplate>();

            Mapper.CreateMap<CompanyAttachment, CompanyAttachmentModel>();
            Mapper.CreateMap<CompanyAttachmentModel, CompanyAttachment>();

            Mapper.CreateMap<DNRReason, DNRReasonModel>();
            Mapper.CreateMap<DNRReasonModel, DNRReason>();
            
            #region Payroll
            Mapper.CreateMap<PayGroup, PayGroupModel>()
                .ForMember(d => d.VendorName, opt => opt.MapFrom(s => s.Vendor.FranchiseName));

            Mapper.CreateMap<PayGroupModel, PayGroup>()
                .ForSourceMember(x => x.VendorName, y => y.Ignore());
            
            Mapper.CreateMap<PayFrequencyType, PayFrequencyTypeModel>();

            Mapper.CreateMap<Payroll_Calendar, PayrollCalendarModel>();
            Mapper.CreateMap<PayrollCalendarModel, Payroll_Calendar>();

            Mapper.CreateMap<EmployeePayrollSetting, EmployeePayrollSettingModel>()
                .ForMember(d => d.EmployeeTypeId, opt => opt.MapFrom(s => s.Employee.EmployeeTypeId));
            Mapper.CreateMap<EmployeePayrollSettingModel, EmployeePayrollSetting>();

            Mapper.CreateMap<EmployeeTD1, EmployeeTD1OverviewModel>();
            Mapper.CreateMap<EmployeeTD1, EmployeeTD1Model>()
                .ForMember(d => d.EmployeeTD1_Id, opt => opt.MapFrom(s => s.Id));
            Mapper.CreateMap<EmployeeTD1Model, EmployeeTD1>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.EmployeeTD1_Id));
            
            Mapper.CreateMap<CandidateBankAccount, CandidateBankAccountModel>();
            Mapper.CreateMap<CandidateBankAccountModel, CandidateBankAccount>();
            
            Mapper.CreateMap<Candidate_Payment_History_Detail, PaymentDetailModel>()
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Payroll_Item.Code))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Payroll_Item.Description))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Payroll_Item.TypeID + " - " + s.Payroll_Item.Payroll_Item_Type.Name))
                .ForMember(d => d.SubTypeCode, opt => opt.MapFrom(s => s.Payroll_Item.Payroll_Item_SubType.Name));
            
            Mapper.CreateMap<EmailSetting, PayrollEmailSettingModel>();
            Mapper.CreateMap<PayrollEmailSettingModel, EmailSetting>();

            Mapper.CreateMap<EmailSetting, PayrollEmailSettingDetailModel>();
            Mapper.CreateMap<PayrollEmailSettingDetailModel, EmailSetting>();

            Mapper.CreateMap<Payroll_Item, PayrollItemModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Payroll_Item_Type.Name))
                .ForMember(d => d.VendorName, opt => opt.MapFrom(s => s.Vendor.FranchiseName));
            Mapper.CreateMap<Payroll_Item, PayrollItemDetailModel>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Payroll_Item_Type.Code))
                .ForMember(d => d.SubType, opt => opt.MapFrom(s => s.Payroll_Item_SubType.Code));
            Mapper.CreateMap<PayrollItemDetailModel, Payroll_Item>();
            #endregion

            Mapper.CreateMap<JobBoard, JobBoardModel>();
            Mapper.CreateMap<JobBoardModel, JobBoard>();

            Mapper.CreateMap<TaxFormBox, TaxFormBoxModel>();
            Mapper.CreateMap<TaxFormBoxModel, TaxFormBox>();

            Mapper.CreateMap<CandidateWSIBCommonRate, CandidateWSIBCommonRateModel>();
            Mapper.CreateMap<CandidateWSIBCommonRateModel, CandidateWSIBCommonRate>();
            
            DefaultMappingRule.Create();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
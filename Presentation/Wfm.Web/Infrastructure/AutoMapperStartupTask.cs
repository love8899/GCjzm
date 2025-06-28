using AutoMapper;
using System;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Infrastructure;
using Wfm.Web.Models.Blogs;
using Wfm.Web.Models.Candidate;
using Wfm.Web.Models.Common;
using Wfm.Web.Models.Companies;
using Wfm.Web.Models.Franchises;
using Wfm.Web.Models.JobOrder;
using Wfm.Web.Models.Test;
using Wfm.Web.Models.TimeSheet;
using Wfm.Core.Domain.Accounts;
using Wfm.Web.Models.Accounts;
using Wfm.Shared.Models.Common;
using Wfm.Web.Models.Home;


namespace Wfm.Web.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            Mapper.CreateMap<AccountDelegate, AccountDelegateModel>()
                .ForMember(m => m.AccountName, opt => opt.MapFrom(e => e.Account.Username));

            Mapper.CreateMap<Candidate, CandidateModel>();
            Mapper.CreateMap<CandidateModel, Candidate>();

            Mapper.CreateMap<JobOrder, JobOrderModel>();
            Mapper.CreateMap<JobOrderModel, JobOrder>();

            Mapper.CreateMap<Company, CompanyModel>();
            Mapper.CreateMap<CompanyModel, Company>();

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


            Mapper.CreateMap<CompanyLocation, CompanyLocationModel>();
            Mapper.CreateMap<CompanyLocationModel, CompanyLocation>();



            Mapper.CreateMap<CandidateAttachment, CandidateAttachmentModel>();
            Mapper.CreateMap<CandidateAttachmentModel, CandidateAttachment>();

            Mapper.CreateMap<TestCategory, TestCategoryModel>();
            Mapper.CreateMap<TestCategoryModel, TestCategory>();

            Mapper.CreateMap<TestQuestion, TestQuestionModel>();
            Mapper.CreateMap<TestQuestionModel, TestQuestion>();

            Mapper.CreateMap<TestChoice, TestChoiceModel>();
            Mapper.CreateMap<TestChoiceModel, TestChoice>();

            Mapper.CreateMap<CandidateTestResult, CandidateTestResultModel>();
            Mapper.CreateMap<CandidateTestResultModel, CandidateTestResult>();

            Mapper.CreateMap<TestMaterial, TestMaterialModel>();


            Mapper.CreateMap<CandidateAddress, CandidateAddressModel>();
            Mapper.CreateMap<CandidateAddressModel, CandidateAddress>();

            Mapper.CreateMap<CandidateJobOrder, CandidateJobOrderModel>();
            Mapper.CreateMap<CandidateJobOrderModel, CandidateJobOrder>();

            Mapper.CreateMap<CandidateKeySkill, CandidateKeySkillModel>();
            Mapper.CreateMap<CandidateKeySkillModel, CandidateKeySkill>();

            Mapper.CreateMap<CandidateWorkHistory, CandidateWorkHistoryModel>();
            Mapper.CreateMap<CandidateWorkHistoryModel, CandidateWorkHistory>();

            Mapper.CreateMap<CandidateJobOrderStatus, CandidateJobOrderStatusModel>();
            Mapper.CreateMap<CandidateJobOrderStatusModel, CandidateJobOrderStatus>();


            Mapper.CreateMap<Salutation, SalutationModel>();
            Mapper.CreateMap<SalutationModel, Salutation>();

            Mapper.CreateMap<Gender, GenderModel>();
            Mapper.CreateMap<GenderModel, Gender>();

            Mapper.CreateMap<Transportation, TransportationModel>();
            Mapper.CreateMap<TransportationModel, Transportation>();

            Mapper.CreateMap<Skill, SkillModel>();
            Mapper.CreateMap<SkillModel, Skill>();

            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<StateProvince, StateProvinceModel>();
            Mapper.CreateMap<StateProvinceModel, StateProvince>();

            Mapper.CreateMap<City, CityModel>();
            Mapper.CreateMap<CityModel, City>();

            Mapper.CreateMap<CandidateAttachment, CandidateAttachmentModel>();
            Mapper.CreateMap<CandidateAttachmentModel, CandidateAttachment>();

            Mapper.CreateMap<JobOrderTestCategory, JobOrderTestCategoryModel>();
            Mapper.CreateMap<JobOrderTestCategoryModel, JobOrderTestCategory>();

            Mapper.CreateMap<BlogPost, BlogPostModel>();
            Mapper.CreateMap<BlogPostModel, BlogPost>();

            Mapper.CreateMap<BlogComment, BlogCommentModel>();
            Mapper.CreateMap<BlogCommentModel, BlogComment>();

            Mapper.CreateMap<Shift, ShiftModel>();
            Mapper.CreateMap<ShiftModel, Shift>();

            Mapper.CreateMap<Shift, ShiftModel>();
            Mapper.CreateMap<ShiftModel, Shift>();

            Mapper.CreateMap<CandidateAvailability, CandidateAvailabilityModel>()
                .ForMember(dest => dest.Title, mo => mo.MapFrom(src => src.Shift.ShiftName))
                .ForMember(dest => dest.Start, mo => mo.MapFrom(src => src.Date))
                .ForMember(dest => dest.End, mo => mo.MapFrom(src => src.Date))
                .ForMember(dest => dest.IsAllDay, mo => mo.MapFrom(src => true))
                .ForMember(dest => dest.ReadOnly, mo => mo.MapFrom(src => src.Date <= DateTime.Today));
            Mapper.CreateMap<CandidateAvailabilityModel, CandidateAvailability>()
                .ForMember(dest => dest.Date, mo => mo.MapFrom(src => src.Start));

            Mapper.CreateMap<CandidateWorkTime, CandidateWorkTimeModel>();
            Mapper.CreateMap<CandidateWorkTimeModel, CandidateWorkTime>();

            Mapper.CreateMap<SecurityQuestion, SecurityQuestionModel>();
            Mapper.CreateMap<SecurityQuestionModel, SecurityQuestion>();


            Mapper.CreateMap<Candidate, CandidateUpdateProfileModel>();
            Mapper.CreateMap<CandidateUpdateProfileModel, Candidate>();
            Mapper.CreateMap<FranchiseAddress, FranchiseAddressModel>();

            Mapper.CreateMap<PaymentHistoryWithPayStub, PaymentHistoryWithPayStubModel>();
            Mapper.CreateMap<PaymentHistoryWithPayStubModel, PaymentHistoryWithPayStub>();

            Mapper.CreateMap<CandidateAppliedJobs, CandidateAppliedJobModel>();
            Mapper.CreateMap<CandidateAppliedJobModel, CandidateAppliedJobs>();


        }

        public int Order
        {
            get { return 0; }
        }
    }
}
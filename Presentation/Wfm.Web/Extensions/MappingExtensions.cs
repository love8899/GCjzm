using AutoMapper;
using System.Collections.Generic;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Web.Models.Blogs;
using Wfm.Web.Models.Candidate;
using Wfm.Web.Models.Common;
using Wfm.Web.Models.Companies;
using Wfm.Web.Models.Franchises;
using Wfm.Web.Models.JobOrder;
using Wfm.Web.Models.Test;
using Wfm.Web.Models.TimeSheet;
using Wfm.Web.Models.Accounts;
using Wfm.Core.Domain.Accounts;
using Wfm.Shared.Models.Common;
using Wfm.Web.Models.Home;

namespace Wfm.Web.Extensions
{
    public static class MappingExtensions
    {

        #region Franchise

        public static FranchiseModel ToModel(this Franchise entity)
        {
            return Mapper.Map<Franchise, FranchiseModel>(entity);
        }

        public static Franchise ToEntity(this FranchiseModel model)
        {
            return Mapper.Map<FranchiseModel, Franchise>(model);
        }

        public static Franchise ToEntity(this FranchiseModel model, Franchise destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 


        #region JobOrder

        public static JobOrderModel ToModel(this JobOrder entity)
        {
            return Mapper.Map<JobOrder, JobOrderModel>(entity);
        }

        public static JobOrder ToEntity(this JobOrderModel model)
        {
            return Mapper.Map<JobOrderModel, JobOrder>(model);
        }

        public static JobOrder ToEntity(this JobOrderModel model, JobOrder destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region JobOrderCategory

        public static JobOrderCategoryModel ToModel(this JobOrderCategory entity)
        {
            return Mapper.Map<JobOrderCategory, JobOrderCategoryModel>(entity);
        }

        public static JobOrderCategory ToEntity(this JobOrderCategoryModel model)
        {
            return Mapper.Map<JobOrderCategoryModel, JobOrderCategory>(model);
        }

        public static JobOrderCategory ToEntity(this JobOrderCategoryModel model, JobOrderCategory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region JobOrderType 

        public static JobOrderTypeModel ToModel(this JobOrderType entity)
        {
            return Mapper.Map<JobOrderType, JobOrderTypeModel>(entity);
        }

        public static JobOrderType ToEntity(this JobOrderTypeModel model)
        {
            return Mapper.Map<JobOrderTypeModel, JobOrderType>(model);
        }

        public static JobOrderType ToEntity(this JobOrderTypeModel model, JobOrderType destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region JobOrderStatus

        public static JobOrderStatusModel ToModel(this JobOrderStatus entity)
        {
            return Mapper.Map<JobOrderStatus, JobOrderStatusModel>(entity);
        }

        public static JobOrderStatus ToEntity(this JobOrderStatusModel model)
        {
            return Mapper.Map<JobOrderStatusModel, JobOrderStatus>(model);
        }

        public static JobOrderStatus ToEntity(this JobOrderStatusModel model, JobOrderStatus destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region JobOrderTestCategory

        public static JobOrderTestCategoryModel ToModel(this JobOrderTestCategory entity)
        {
            return Mapper.Map<JobOrderTestCategory, JobOrderTestCategoryModel>(entity);
        }

        public static JobOrderTestCategory ToEntity(this JobOrderTestCategoryModel model)
        {
            return Mapper.Map<JobOrderTestCategoryModel, JobOrderTestCategory>(model);
        }

        public static JobOrderTestCategory ToEntity(this JobOrderTestCategoryModel model, JobOrderTestCategory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 


        #region Company

        public static CompanyModel ToModel(this Company entity)
        {
            return Mapper.Map<Company, CompanyModel>(entity);
        }

        public static Company ToEntity(this CompanyModel model)
        {
            return Mapper.Map<CompanyModel, Company>(model);
        }

        public static Company ToEntity(this CompanyModel model, Company destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CompanyLocation

        public static CompanyLocationModel ToModel(this CompanyLocation entity)
        {
            return Mapper.Map<CompanyLocation, CompanyLocationModel>(entity);
        }

        public static CompanyLocation ToEntity(this CompanyLocationModel model)
        {
            return Mapper.Map<CompanyLocationModel, CompanyLocation>(model);
        }

        public static CompanyLocation ToEntity(this CompanyLocationModel model, CompanyLocation destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 


        #region Candidate

        public static CandidateModel ToModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, CandidateModel>(entity);
        }

        public static Candidate ToEntity(this CandidateModel model)
        {
            return Mapper.Map<CandidateModel, Candidate>(model);
        }

        public static Candidate ToEntity(this CandidateModel model, Candidate destination)
        {
            return Mapper.Map(model, destination);
        }

        public static Candidate ToCandidateUpdateProfileEntity(this CandidateUpdateProfileModel model, Candidate destination)
        {
            return Mapper.Map(model, destination);
        }
        public static CandidateUpdateProfileModel ToCandidateUpdateProfileModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, CandidateUpdateProfileModel>(entity); 
        }

        #endregion

        #region CandidateAddress

        public static CandidateAddressModel ToModel(this CandidateAddress entity)
        {
            return Mapper.Map<CandidateAddress, CandidateAddressModel>(entity);
        }

        public static CandidateAddress ToEntity(this CandidateAddressModel model)
        {
            return Mapper.Map<CandidateAddressModel, CandidateAddress>(model);
        }

        public static CandidateAddress ToEntity(this CandidateAddressModel model, CandidateAddress destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region CandidateAttachment

        public static CandidateAttachmentModel ToModel(this CandidateAttachment entity)
        {
            return Mapper.Map<CandidateAttachment, CandidateAttachmentModel>(entity);
        }

        public static CandidateAttachment ToEntity(this CandidateAttachmentModel model)
        {
            return Mapper.Map<CandidateAttachmentModel, CandidateAttachment>(model);
        }

        public static CandidateAttachment ToEntity(this CandidateAttachmentModel model, CandidateAttachment destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Candidate Availability

        public static CandidateAvailabilityModel ToModel(this CandidateAvailability entity)
        {
            return Mapper.Map<CandidateAvailability, CandidateAvailabilityModel>(entity);
        }


        public static CandidateAvailability ToEntity(this CandidateAvailabilityModel model)
        {
            return Mapper.Map<CandidateAvailabilityModel, CandidateAvailability>(model);
        }

        #endregion 


        #region CandidateJobOrder

        public static CandidateJobOrderModel ToModel(this CandidateJobOrder entity)
        {
            return Mapper.Map<CandidateJobOrder, CandidateJobOrderModel>(entity);
        }

        public static CandidateJobOrder ToEntity(this CandidateJobOrderModel model)
        {
            return Mapper.Map<CandidateJobOrderModel, CandidateJobOrder>(model);
        }

        public static CandidateJobOrder ToEntity(this CandidateJobOrderModel model, CandidateJobOrder destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 


        #region CandidateJobOrderStatus

        public static CandidateJobOrderStatusModel ToModel(this CandidateJobOrderStatus entity)
        {
            return Mapper.Map<CandidateJobOrderStatus, CandidateJobOrderStatusModel>(entity);
        }

        public static CandidateJobOrderStatus ToEntity(this CandidateJobOrderStatusModel model)
        {
            return Mapper.Map<CandidateJobOrderStatusModel, CandidateJobOrderStatus>(model);
        }

        public static CandidateJobOrderStatus ToEntity(this CandidateJobOrderStatusModel model, CandidateJobOrderStatus destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region CandidateKeySkill

        public static CandidateKeySkillModel ToModel(this CandidateKeySkill entity)
        {
            return Mapper.Map<CandidateKeySkill, CandidateKeySkillModel>(entity);
        }

        public static CandidateKeySkill ToEntity(this CandidateKeySkillModel model)
        {
            return Mapper.Map<CandidateKeySkillModel, CandidateKeySkill>(model);
        }

        public static CandidateKeySkill ToEntity(this CandidateKeySkillModel model, CandidateKeySkill destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region CandidateWorkHistory

        public static CandidateWorkHistoryModel ToModel(this CandidateWorkHistory entity)
        {
            return Mapper.Map<CandidateWorkHistory, CandidateWorkHistoryModel>(entity);
        }

        public static CandidateWorkHistory ToEntity(this CandidateWorkHistoryModel model)
        {
            return Mapper.Map<CandidateWorkHistoryModel, CandidateWorkHistory>(model);
        }

        public static CandidateWorkHistory ToEntity(this CandidateWorkHistoryModel model, CandidateWorkHistory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Skill

        public static SkillModel ToModel(this Skill entity)
        {
            return Mapper.Map<Skill, SkillModel>(entity);
        }

        public static Skill ToEntity(this SkillModel model)
        {
            return Mapper.Map<SkillModel, Skill>(model);
        }

        public static Skill ToEntity(this SkillModel model, Skill destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Salutation

        public static SalutationModel ToModel(this Salutation entity)
        {
            return Mapper.Map<Salutation, SalutationModel>(entity);
        }

        public static Salutation ToEntity(this SalutationModel model)
        {
            return Mapper.Map<SalutationModel, Salutation>(model);
        }

        public static Salutation ToEntity(this SalutationModel model, Salutation destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Gender

        public static GenderModel ToModel(this Gender entity)
        {
            return Mapper.Map<Gender, GenderModel>(entity);
        }

        public static Gender ToEntity(this GenderModel model)
        {
            return Mapper.Map<GenderModel, Gender>(model);
        }

        public static Gender ToEntity(this GenderModel model, Gender destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region VetranType

        public static VetranTypeModel ToModel(this VetranType entity)
        {
            return Mapper.Map<VetranType, VetranTypeModel>(entity);
        }

        public static VetranType ToEntity(this VetranTypeModel model)
        {
            return Mapper.Map<VetranTypeModel, VetranType>(model);
        }

        public static VetranType ToEntity(this VetranTypeModel model, VetranType destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Source

        public static SourceModel ToModel(this Source entity)
        {
            return Mapper.Map<Source, SourceModel>(entity);
        }

        public static Source ToEntity(this SourceModel model)
        {
            return Mapper.Map<SourceModel, Source>(model);
        }

        public static Source ToEntity(this SourceModel model, Source destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Transportation

        public static TransportationModel ToModel(this Transportation entity)
        {
            return Mapper.Map<Transportation, TransportationModel>(entity);
        }

        public static Transportation ToEntity(this TransportationModel model)
        {
            return Mapper.Map<TransportationModel, Transportation>(model);
        }

        public static Transportation ToEntity(this TransportationModel model, Transportation destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Country

        public static CountryModel ToModel(this Country entity)
        {
            return Mapper.Map<Country, CountryModel>(entity);
        }

        public static Country ToEntity(this CountryModel model)
        {
            return Mapper.Map<CountryModel, Country>(model);
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region StateProvince

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return Mapper.Map<StateProvince, StateProvinceModel>(entity);
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return Mapper.Map<StateProvinceModel, StateProvince>(model);
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region City

        public static CityModel ToModel(this City entity)
        {
            return Mapper.Map<City, CityModel>(entity);
        }

        public static City ToEntity(this CityModel model)
        {
            return Mapper.Map<CityModel, City>(model);
        }

        public static City ToEntity(this CityModel model, City destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region TestCategory

        public static TestCategoryModel ToModel(this TestCategory entity)
        {
            return Mapper.Map<TestCategory, TestCategoryModel>(entity);
        }

        public static TestCategory ToEntity(this TestCategoryModel model)
        {
            return Mapper.Map<TestCategoryModel, TestCategory>(model);
        }

        public static TestCategory ToEntity(this TestCategoryModel model, TestCategory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region TestQuestion

        public static TestQuestionModel ToModel(this TestQuestion entity)
        {
            return Mapper.Map<TestQuestion, TestQuestionModel>(entity);
        }

        public static TestQuestion ToEntity(this TestQuestionModel model)
        {
            return Mapper.Map<TestQuestionModel, TestQuestion>(model);
        }

        public static TestQuestion ToEntity(this TestQuestionModel model, TestQuestion destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region TestChoice

        public static TestChoiceModel ToModel(this TestChoice entity)
        {
            return Mapper.Map<TestChoice, TestChoiceModel>(entity);
        }

        public static TestChoice ToEntity(this TestChoiceModel model)
        {
            return Mapper.Map<TestChoiceModel, TestChoice>(model);
        }

        public static TestChoice ToEntity(this TestChoiceModel model, TestChoice destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 

        #region CandidateTestResult

        public static CandidateTestResultModel ToModel(this CandidateTestResult entity)
        {
            var model = Mapper.Map<CandidateTestResult, CandidateTestResultModel>(entity);
            model.TestCategoryModel = entity.TestCategory.ToModel();

            return model;
        }

        public static CandidateTestResult ToEntity(this CandidateTestResultModel model)
        {
            return Mapper.Map<CandidateTestResultModel, CandidateTestResult>(model);
        }

        public static CandidateTestResult ToEntity(this CandidateTestResultModel model, CandidateTestResult destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Test Material

        public static TestMaterialModel ToModel(this TestMaterial entity)
        {
            return Mapper.Map<TestMaterial, TestMaterialModel>(entity);
        }

        #endregion

        #region BlogPost

        public static BlogPostModel ToModel(this BlogPost entity)
        {
            return Mapper.Map<BlogPost, BlogPostModel>(entity);
        }

        public static BlogPost ToEntity(this BlogPostModel model)
        {
            return Mapper.Map<BlogPostModel, BlogPost>(model);
        }

        public static BlogPost ToEntity(this BlogPostModel model, BlogPost destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region BlogPostCommnent

        public static BlogCommentModel ToModel(this BlogComment entity)
        {
            return Mapper.Map<BlogComment, BlogCommentModel>(entity);
        }

        public static BlogComment ToEntity(this BlogCommentModel model)
        {
            return Mapper.Map<BlogCommentModel, BlogComment>(model);
        }

        public static BlogComment ToEntity(this BlogCommentModel model, BlogComment destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Shift 

        public static ShiftModel ToModel(this Shift entity)
        {
            return Mapper.Map<Shift, ShiftModel>(entity);
        }

        public static Shift ToEntity(this ShiftModel model)
        {
            return Mapper.Map<ShiftModel, Shift>(model);
        }

        public static Shift ToEntity(this ShiftModel model, Shift destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion 


        #region CandidateWorkTime

        public static CandidateWorkTimeModel ToModel(this CandidateWorkTime entity)
        {
            var model = Mapper.Map<CandidateWorkTime, CandidateWorkTimeModel>(entity);
            model.ReadOnly = model.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved ||
                             model.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Rejected ||
                             !entity.JobOrder.AllowTimeEntry;

            return model;
        }

        public static CandidateWorkTime ToEntity(this CandidateWorkTimeModel model)
        {
            return Mapper.Map<CandidateWorkTimeModel, CandidateWorkTime>(model);
        }

        public static CandidateWorkTime ToEntity(this CandidateWorkTimeModel model, CandidateWorkTime destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region AccountDelegate
        public static AccountDelegateModel ToModel(this AccountDelegate entity)
        {
            return Mapper.Map<AccountDelegate, AccountDelegateModel>(entity);
        }
        #endregion

        #region FranchiseAddress

        public static FranchiseAddressModel ToModel(this FranchiseAddress entity)
        {
            var model = Mapper.Map<FranchiseAddress, FranchiseAddressModel>(entity);
            if (entity.City != null)
                model.City = entity.City.CityName;
            if (entity.Country != null)
                model.Country= entity.Country.CountryName;
            if (entity.StateProvince != null)
                model.StateProvince = entity.StateProvince.Abbreviation;
            return model;
        }

        #endregion

        #region PaymentHistoryWithPayStub
        public static PaymentHistoryWithPayStubModel ToModel(this PaymentHistoryWithPayStub entity)
        {
            return Mapper.Map<PaymentHistoryWithPayStub, PaymentHistoryWithPayStubModel>(entity);
        }

        public static PaymentHistoryWithPayStub ToEntity(this PaymentHistoryWithPayStubModel model)
        {
            return Mapper.Map<PaymentHistoryWithPayStubModel, PaymentHistoryWithPayStub>(model);
        }

        public static PaymentHistoryWithPayStub ToEntity(this PaymentHistoryWithPayStubModel model, PaymentHistoryWithPayStub destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion
    }
}
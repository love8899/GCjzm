using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Forums;
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
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Policies;
using Wfm.Core.Domain.Payroll;
using Wfm.Core.Domain.Tasks;
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
using Wfm.Web.Framework;
using Wfm.Core.Domain.Media;
using Wfm.Admin.Models.Media;
using Wfm.Shared.Models.Accounts;
using Wfm.Shared.Models.Localization;
using Wfm.Admin.Models.Timeoff;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Announcements;
using Wfm.Admin.Models.Announcements;
using Wfm.Shared.Models.Common;
using Wfm.Admin.Models.WSIBs;
using Wfm.Core.Domain.WSIBS;
using System.IO;
using System;
using Wfm.Core.Domain.WSIB;


namespace Wfm.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }


        #region Candidate

        public static CandidateModel ToModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, CandidateModel>(entity);
        }
        
        public static CandidatePoolModel ToPoolModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, CandidatePoolModel>(entity);
        }
        
        public static Candidate ToEntity(this CandidateModel model)
        {
            return Mapper.Map<CandidateModel, Candidate>(model);
        }

        public static Candidate ToEntity(this CandidateModel model, Candidate destination)
        {
            return Mapper.Map(model, destination);
        }

        public static SimpleCandidateModel ToSimpleModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, SimpleCandidateModel>(entity);
        }

        public static EmployeeListModel ToEmployeeListModel(this Candidate entity)
        {
            var model = Mapper.Map<Candidate, EmployeeListModel>(entity);
            var bankaccount = entity.CandidateBankAccounts.Where(x => x.IsActive && !x.IsDeleted).FirstOrDefault();
            if (bankaccount != null)
                model.BankAccount = bankaccount.AccountNumber;

            return model;
        }

        public static EmployeeModel ToEmployeeModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, EmployeeModel>(entity);
        }

        public static Candidate ToCandidateEntity(this EmployeeModel model)
        {
            return Mapper.Map<EmployeeModel, Candidate>(model);
        }

        public static Candidate ToCandidateEntity(this EmployeeModel model, Candidate destination)
        {
            return Mapper.Map(model, destination);
        }

        public static ContactInfoModel ToContactInfoModel(this Candidate entity)
        {
            return Mapper.Map<Candidate, ContactInfoModel>(entity);
        }

        #endregion


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
            var entity = Mapper.Map(model, destination);
            if(String.IsNullOrEmpty(model.FranchiseLogoFileName))
            {
                entity.FranchiseLogo = null;
            }
            return entity;
        }

        #endregion

        #region VendorCertificate
        public static VendorCertificateModel ToModel(this VendorCertificate entity)
        {
            return Mapper.Map<VendorCertificate, VendorCertificateModel>(entity);
        }

        public static VendorCertificate ToEntity(this VendorCertificateModel model)
        {
            return Mapper.Map<VendorCertificateModel, VendorCertificate>(model);
        }

        public static VendorCertificate ToEntity(this VendorCertificateModel model, VendorCertificate destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region FranchiseAddress

        public static FranchiseAddressModel ToModel(this FranchiseAddress entity)
        {
            var model =  Mapper.Map<FranchiseAddress, FranchiseAddressModel>(entity);
            if (entity.City != null)
                 model.CityName = entity.City.CityName;
            if (entity.Country != null)
                 model.CountryName = entity.Country.CountryName;
            if (entity.StateProvince != null)
                 model.StateProvinceName = entity.StateProvince.StateProvinceName;
            return model;
        }

        public static FranchiseAddress ToEntity(this FranchiseAddressModel model)
        {
            return Mapper.Map<FranchiseAddressModel, FranchiseAddress>(model);
        }

        public static FranchiseAddress ToEntity(this FranchiseAddressModel model, FranchiseAddress destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region FranchiseSetting

        public static FranchiseSettingModel ToModel(this FranchiseSetting entity)
        {
            return Mapper.Map<FranchiseSetting, FranchiseSettingModel>(entity);
        }

        public static FranchiseSetting ToEntity(this FranchiseSettingModel model)
        {
            return Mapper.Map<FranchiseSettingModel, FranchiseSetting>(model);
        }

        public static FranchiseSetting ToEntity(this FranchiseSettingModel model, FranchiseSetting destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region JobOrder

        public static JobOrderModel ToModel(this JobOrder entity)
        {
            var model = Mapper.Map<JobOrder, JobOrderModel>(entity);
            model.CompanyGuid = entity.Company.CompanyGuid;
            
            return model;
        }

        public static JobOrder ToEntity(this JobOrderModel model)
        {
            return Mapper.Map<JobOrderModel, JobOrder>(model);
        }

        public static JobOrder ToEntity(this JobOrderModel model, JobOrder destination)
        {
            return Mapper.Map(model, destination);
        }

        public static DirectHireJobOrderModel ToDirectHireJobOrderModel(this JobOrder entity)
        {
            var model = Mapper.Map<JobOrder, DirectHireJobOrderModel>(entity);
            model.CompanyGuid = entity.Company.CompanyGuid;
            return model;
        }
        public static DirectHireJobOrderListModel ToModel(this DirectHireJobOrderList entity)
        {
            return Mapper.Map<DirectHireJobOrderList, DirectHireJobOrderListModel>(entity);
        }
        public static JobOrder ToEntity(this DirectHireJobOrderModel model)
        {
            return Mapper.Map<DirectHireJobOrderModel, JobOrder>(model); 
        }
        public static JobOrder ToEntity(this DirectHireJobOrderModel model, JobOrder destination)
        {
            return Mapper.Map(model, destination);
        }

        public static DirectHireCandidatePoolListModel ToModel(this DirectHireCandidatePoolList entity)
        {
            return Mapper.Map<DirectHireCandidatePoolList, DirectHireCandidatePoolListModel>(entity);
        }

        #endregion


        #region Opening

        public static OpeningModel ToModel(this JobOrderOpening entity, DateTime startDate, DateTime endDate, 
            IEnumerable<StatutoryHoliday> holidays, string noWorkMark, bool zeroIfNoWork = false, bool readOnly = false)
        {
            var model = Mapper.Map<JobOrderOpening, OpeningModel>(entity);
            model.ReadOnly = readOnly;

            var noWorkNote = string.Empty;
            model.NoWork = _IsNoWork(entity.JobOrder, model.Start, holidays, noWorkMark, out noWorkNote);
            if (zeroIfNoWork && model.NoWork)
            {
                model.OpeningNumber = 0;
                model.Title = model.OpeningNumber.ToString();
            }
            model.NoWorkNote = noWorkNote;

            return model;
        }


        private static bool _IsNoWork(JobOrder jobOrder, DateTime refDate, IEnumerable<StatutoryHoliday> holidays, string noWorkMark, out string noWorkNote)
        {
            var holiday = holidays.FirstOrDefault(x => x.HolidayDate == refDate);
            var isHoliday = holiday != null;
            var holidayName = isHoliday ? holiday.StatutoryHolidayName : string.Empty;

            noWorkNote = string.Empty;
            var noWork = !jobOrder.DailySwitches[(int)refDate.DayOfWeek] || (!jobOrder.IncludeHolidays && isHoliday);
            if (noWork)
                noWorkNote = !jobOrder.DailySwitches[(int)refDate.DayOfWeek] ? String.Format(noWorkMark, refDate.DayOfWeek.ToString()) : holidayName;

            return noWork;
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

        //public static CompanyEditModel ToEditModel(this Company entity)
        //{
        //    return Mapper.Map<Company, CompanyEditModel>(entity);
        //}

        //public static Company ToEntity(this CompanyEditModel model)
        //{
        //    return Mapper.Map<CompanyEditModel, Company>(model);
        //}

        //public static Company ToEntity(this CompanyEditModel model, Company destination)
        //{
        //    return Mapper.Map(model, destination);
        //}

        public static SearchCompanyCandidateModel ToSearchCompanyCandidateModel(this CandidateWithAddress entity)
        {
            return Mapper.Map<CandidateWithAddress, SearchCompanyCandidateModel>(entity);
        }
        #endregion 

        #region CompanySetting

        public static CompanySettingModel ToModel(this CompanySetting entity)
        {
            return Mapper.Map<CompanySetting, CompanySettingModel>(entity);
        }

        public static CompanySetting ToEntity(this CompanySettingModel model)
        {
            return Mapper.Map<CompanySettingModel, CompanySetting>(model);
        }

        public static CompanySetting ToEntity(this CompanySettingModel model, CompanySetting destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CompanyDepartment

        public static CompanyDepartmentModel ToModel(this CompanyDepartment entity)
        {
            CompanyDepartmentModel model = Mapper.Map<CompanyDepartment, CompanyDepartmentModel>(entity);
            //model.CompanyGuid = entity.Company.CompanyGuid;
            return model;
        }

        public static CompanyDepartment ToEntity(this CompanyDepartmentModel model)
        {
            return Mapper.Map<CompanyDepartmentModel, CompanyDepartment>(model);
        }

        public static CompanyDepartment ToEntity(this CompanyDepartmentModel model, CompanyDepartment destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region CompanyBilling

        public static CompanyBillingRateModel ToModel(this CompanyBillingRate entity)
        {
            return Mapper.Map<CompanyBillingRate, CompanyBillingRateModel>(entity);
        }

        public static CompanyBillingRate ToEntity(this CompanyBillingRateModel model)
        {
            return Mapper.Map<CompanyBillingRateModel, CompanyBillingRate>(model);
        }

        public static CompanyBillingRate ToEntity(this CompanyBillingRateModel model, CompanyBillingRate destination)
        {
            return Mapper.Map(model, destination);
        }


        public static QuotationModel ToModel(this Quotation entity)
        {
            return Mapper.Map<Quotation, QuotationModel>(entity);
        }

        #endregion


        #region CompanyOvertimeRule

        public static CompanyOvertimeRuleModel ToModel(this CompanyOvertimeRule entity)
        {
            CompanyOvertimeRuleModel model = Mapper.Map<CompanyOvertimeRule, CompanyOvertimeRuleModel>(entity);
            //model.CompanyGuid = entity.Company.CompanyGuid;
            model.Code = entity.OvertimeRuleSetting.Code;
            model.Description = entity.OvertimeRuleSetting.Description;
            model.TypeName = entity.OvertimeRuleSetting.OvertimeType.Name;
            model.ApplyAfter = entity.OvertimeRuleSetting.ApplyAfter;
            model.Rate = entity.OvertimeRuleSetting.Rate;
            return model;
        }

        public static CompanyOvertimeRule ToEntity(this CompanyOvertimeRuleModel model)
        {
            return Mapper.Map<CompanyOvertimeRuleModel, CompanyOvertimeRule>(model);
        }

        public static CompanyOvertimeRule ToEntity(this CompanyOvertimeRuleModel model, CompanyOvertimeRule destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region JobOrderOvertimeRule

        public static JobOrderOvertimeRuleModel ToModel(this JobOrderOvertimeRule entity)
        {
            return Mapper.Map<JobOrderOvertimeRule, JobOrderOvertimeRuleModel>(entity);
        }

        public static JobOrderOvertimeRule ToEntity(this JobOrderOvertimeRuleModel model)
        {
            return Mapper.Map<JobOrderOvertimeRuleModel, JobOrderOvertimeRule>(model);
        }

        public static JobOrderOvertimeRule ToEntity(this JobOrderOvertimeRuleModel model, JobOrderOvertimeRule destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region RoundingPolicy

        public static RoundingPolicyModel ToModel(this RoundingPolicy entity)
        {
            return Mapper.Map<RoundingPolicy, RoundingPolicyModel>(entity);
        }

        public static RoundingPolicy ToEntity(this RoundingPolicyModel model)
        {
            return Mapper.Map<RoundingPolicyModel, RoundingPolicy>(model);
        }

        public static RoundingPolicy ToEntity(this RoundingPolicyModel model, RoundingPolicy destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region MealPolicy

        public static MealPolicyModel ToModel(this MealPolicy entity)
        {
            return Mapper.Map<MealPolicy, MealPolicyModel>(entity);
        }

        public static MealPolicy ToEntity(this MealPolicyModel model)
        {
            return Mapper.Map<MealPolicyModel, MealPolicy>(model);
        }

        public static MealPolicy ToEntity(this MealPolicyModel model, MealPolicy destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region BreakPolicy

        public static BreakPolicyModel ToModel(this BreakPolicy entity)
        {
            return Mapper.Map<BreakPolicy, BreakPolicyModel>(entity);
        }

        public static BreakPolicy ToEntity(this BreakPolicyModel model)
        {
            return Mapper.Map<BreakPolicyModel, BreakPolicy>(model);
        }

        public static BreakPolicy ToEntity(this BreakPolicyModel model, BreakPolicy destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        //#region Password Policy
        //public static PasswordPolicyModel ToModel(this PasswordPolicy entity)
        //{
        //    return Mapper.Map<PasswordPolicy, PasswordPolicyModel>(entity);
        //}

        //public static PasswordPolicy ToEntity(this PasswordPolicyModel model)
        //{
        //    return Mapper.Map<PasswordPolicyModel, PasswordPolicy>(model);
        //}

        //public static PasswordPolicy ToEntity(this PasswordPolicyModel model, PasswordPolicy destination)
        //{
        //    return Mapper.Map(model, destination);
        //}
        //#endregion

        #region SchedulePolicy

        public static SchedulePolicyModel ToModel(this SchedulePolicy entity)
        {
            return Mapper.Map<SchedulePolicy, SchedulePolicyModel>(entity);
        }

        public static SchedulePolicy ToEntity(this SchedulePolicyModel model)
        {
            return Mapper.Map<SchedulePolicyModel, SchedulePolicy>(model);
        }

        public static SchedulePolicy ToEntity(this SchedulePolicyModel model, SchedulePolicy destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CompanyLocation

        public static CompanyLocationModel ToModel(this CompanyLocation entity)
        {
            CompanyLocationModel model = Mapper.Map<CompanyLocation, CompanyLocationModel>(entity);
            model.CompanyGuid = entity.Company.CompanyGuid;
            return model;
        }

        public static CompanyLocation ToEntity(this CompanyLocationModel model)
        {
            return Mapper.Map<CompanyLocationModel, CompanyLocation>(model);
        }

        public static CompanyLocation ToEntity(this CompanyLocationModel model, CompanyLocation destination)
        {
            return Mapper.Map(model, destination);
        }

        public static CompanyLocationListModel ToListModel(this CompanyLocation entity)
        {
            return Mapper.Map<CompanyLocation, CompanyLocationListModel>(entity);
        }

        public static CompanyLocation ToEntity(this CompanyLocationListModel model)
        {
            return Mapper.Map<CompanyLocationListModel, CompanyLocation>(model);
        }

        public static CompanyLocation ToEntity(this CompanyLocationListModel model, CompanyLocation destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region CompanyCandidate

        public static CompanyCandidateModel ToModel(this CompanyCandidate entity)
        {
            return Mapper.Map<CompanyCandidate, CompanyCandidateModel>(entity);
        }

        public static CompanyCandidate ToEntity(this CompanyCandidateModel model)
        {
            return Mapper.Map<CompanyCandidateModel, CompanyCandidate>(model);
        }

        #endregion


        #region CompanyVendor

        public static CompanyVendorModel ToModel(this CompanyVendor entity)
        {
            var model = Mapper.Map<CompanyVendor, CompanyVendorModel>(entity);
            model.VendorName = entity.Vendor.FranchiseName;
            return model;
        }

        public static CompanyVendor ToEntity(this CompanyVendorModel model)
        {
            return Mapper.Map<CompanyVendorModel, CompanyVendor>(model);
        }

        public static CompanyVendor ToEntity(this CompanyVendorModel model, CompanyVendor destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region CandidateAttachment

        public static CandidateAttachmentModel ToModel(this CandidateAttachment entity)
        {
            CandidateAttachmentModel model = Mapper.Map<CandidateAttachment, CandidateAttachmentModel>(entity);
            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.FirstName = entity.Candidate.FirstName;
            model.LastName = entity.Candidate.LastName;
            model.UseForDirectPlacement = entity.Candidate.UseForDirectPlacement;
            model.Note = entity.Candidate.Note;
            model.EmployeeId = entity.Candidate.EmployeeId;
            model.CompanyName = entity.Company == null ? String.Empty : entity.Company.CompanyName;            
            if (!String.IsNullOrEmpty(entity.ContentText)&&entity.ContentText.Length > 536870902)
                model.ContentText = entity.ContentText.Substring(0, 536870901);

            return model;
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

        #region CandidatePicture

        public static CandidatePictureModel ToModel(this CandidatePicture entity)
        {
            return Mapper.Map<CandidatePicture, CandidatePictureModel>(entity);
        }

        public static CandidatePicture ToEntity(this CandidatePictureModel model)
        {
            return Mapper.Map<CandidatePictureModel, CandidatePicture>(model);
        }

        public static CandidatePicture ToEntity(this CandidatePictureModel model, CandidatePicture destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region TestResult

        public static CandidateTestResultModel ToModel(this CandidateTestResult entity)
        {
            return Mapper.Map<CandidateTestResult, CandidateTestResultModel>(entity);
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

        #region CandidateJobOrder

        public static CandidateJobOrderModel ToModel(this CandidateJobOrder entity)
        {
            var modle = Mapper.Map<CandidateJobOrder, CandidateJobOrderModel>(entity);
            modle.JobTitle = entity.JobOrder.JobTitle;
            modle.JobOrderGuid = entity.JobOrder.JobOrderGuid;
            return modle;
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

        #region CandidateAddress

        public static CandidateAddressModel ToModel(this CandidateAddress entity)
        {
            var model = Mapper.Map<CandidateAddress, CandidateAddressModel>(entity);
            model.CandidateGuid = entity.Candidate.CandidateGuid;
            return model;
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

        #region CandidateJobOrderStatusHistory

        public static CandidateJobOrderStatusHistoryModel ToModel(this CandidateJobOrderStatusHistory entity)
        {
            var model = Mapper.Map<CandidateJobOrderStatusHistory, CandidateJobOrderStatusHistoryModel>(entity);
            model.JobOrderGuid = entity.JobOrder.JobOrderGuid;
            model.JobTitle = entity.JobOrder.JobTitle;
            model.CompanyName = entity.JobOrder.Company.CompanyName;
            if(entity.Account!=null)
                model.EnteredName = string.Concat(entity.Account.FullName);
            if (entity.CandidateJobOrderStatus != null)
                model.StatusName = entity.CandidateJobOrderStatus.StatusName;

            return model;
        }

        public static CandidateJobOrderStatusHistory ToEntity(this CandidateJobOrderStatusHistoryModel model)
        {
            return Mapper.Map<CandidateJobOrderStatusHistoryModel, CandidateJobOrderStatusHistory>(model);
        }

        public static CandidateJobOrderStatusHistory ToEntity(this CandidateJobOrderStatusHistoryModel model, CandidateJobOrderStatusHistory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CandidateDirectHireStatusHistory

        public static CandidateDirectHireStatusHistoryModel ToModel(this CandidateDirectHireStatusHistory entity)
        {
            return Mapper.Map<CandidateDirectHireStatusHistory, CandidateDirectHireStatusHistoryModel>(entity);
        }

        public static CandidateDirectHireStatusHistory ToEntity(this CandidateDirectHireStatusHistoryModel model)
        {
            return Mapper.Map<CandidateDirectHireStatusHistoryModel, CandidateDirectHireStatusHistory>(model);
        }

        public static CandidateDirectHireStatusHistory ToEntity(this CandidateDirectHireStatusHistoryModel model, CandidateDirectHireStatusHistory destination)
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
            CandidateKeySkillModel model = Mapper.Map<CandidateKeySkill, CandidateKeySkillModel>(entity);
            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeId = entity.Candidate.EmployeeId;
            return model;
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

        #region CandidateBankAccount

        public static CandidateBankAccountModel ToModel(this CandidateBankAccount entity)
        {
            return Mapper.Map<CandidateBankAccount, CandidateBankAccountModel>(entity);
        }

        public static CandidateBankAccount ToEntity(this CandidateBankAccountModel model)
        {
            return Mapper.Map<CandidateBankAccountModel, CandidateBankAccount>(model);
        }

        public static CandidateBankAccount ToEntity(this CandidateBankAccountModel model, CandidateBankAccount destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion




        #region ActivityLogType

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return Mapper.Map<ActivityLogType, ActivityLogTypeModel>(entity);
        }

        public static ActivityLogType ToEntity(this ActivityLogTypeModel model)
        {
            return Mapper.Map<ActivityLogTypeModel, ActivityLogType>(model);
        }

        public static ActivityLogType ToEntity(this ActivityLogTypeModel model, ActivityLogType destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region AccessLog

        public static AccessLogModel ToModel(this AccessLog entity)
        {
            return Mapper.Map<AccessLog, AccessLogModel>(entity);
        }

        public static AccessLog ToEntity(this AccessLogModel model)
        {
            return Mapper.Map<AccessLogModel, AccessLog>(model);
        }

        public static AccessLog ToEntity(this AccessLogModel model, AccessLog destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region ActivityLog

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return Mapper.Map<ActivityLog, ActivityLogModel>(entity);
        }

        public static ActivityLog ToEntity(this ActivityLogModel model)
        {
            return Mapper.Map<ActivityLogModel, ActivityLog>(model);
        }

        public static ActivityLog ToEntity(this ActivityLogModel model, ActivityLog destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CandidateActivityLog

        public static CandidateActivityLogModel ToModel(this CandidateActivityLog entity)
        {
            return Mapper.Map<CandidateActivityLog, CandidateActivityLogModel>(entity);
        }

        public static CandidateActivityLog ToEntity(this CandidateActivityLogModel model)
        {
            return Mapper.Map<CandidateActivityLogModel, CandidateActivityLog>(model);
        }

        public static CandidateActivityLog ToEntity(this CandidateActivityLogModel model, CandidateActivityLog destination)
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

        #region Intersection

        public static IntersectionModel ToModel(this Intersection entity)
        {
            return Mapper.Map<Intersection, IntersectionModel>(entity);
        }

        public static Intersection ToEntity(this IntersectionModel model)
        {
            return Mapper.Map<IntersectionModel, Intersection>(model);
        }

        public static Intersection ToEntity(this IntersectionModel model, Intersection destination)
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

        #region Country

        public static Wfm.Admin.Models.Directory.CountryModel ToModel(this Country entity)
        {
            return Mapper.Map<Country, Wfm.Admin.Models.Directory.CountryModel>(entity);
        }

        public static Country ToEntity(this Wfm.Admin.Models.Directory.CountryModel model)
        {
            return Mapper.Map<Wfm.Admin.Models.Directory.CountryModel, Country>(model);
        }

        public static Country ToEntity(this Wfm.Admin.Models.Directory.CountryModel model, Country destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region StateProvince

        public static Wfm.Admin.Models.Directory.StateProvinceModel ToModel(this StateProvince entity)
        {
            return Mapper.Map<StateProvince, Wfm.Admin.Models.Directory.StateProvinceModel>(entity);
        }

        public static StateProvince ToEntity(this Wfm.Admin.Models.Directory.StateProvinceModel model)
        {
            return Mapper.Map<Wfm.Admin.Models.Directory.StateProvinceModel, StateProvince>(model);
        }

        public static StateProvince ToEntity(this Wfm.Admin.Models.Directory.StateProvinceModel model, StateProvince destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region City

        public static Wfm.Admin.Models.Directory.CityModel ToModel(this City entity)
        {
            return Mapper.Map<City, Wfm.Admin.Models.Directory.CityModel>(entity);
        }

        public static City ToEntity(this Wfm.Admin.Models.Directory.CityModel model)
        {
            return Mapper.Map<Wfm.Admin.Models.Directory.CityModel, City>(model);
        }

        public static City ToEntity(this Wfm.Admin.Models.Directory.CityModel model, City destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Language

        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region LocaleStringResource

        public static LocaleStringResourceModel ToModel(this LocaleStringResource entity)
        {
            return Mapper.Map<LocaleStringResource, LocaleStringResourceModel>(entity);
        }

        public static LocaleStringResource ToEntity(this LocaleStringResourceModel model)
        {
            return Mapper.Map<LocaleStringResourceModel, LocaleStringResource>(model);
        }

        public static LocaleStringResource ToEntity(this LocaleStringResourceModel model, LocaleStringResource destination)
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


        #region SmartCard

        public static CandidateSmartCardModel ToModel(this CandidateSmartCard entity)
        {
            var model = Mapper.Map<CandidateSmartCard, CandidateSmartCardModel>(entity);
            
            if (entity.Candidate != null)
            {
                model.CandidateFirstName = entity.Candidate.FirstName;
                model.CandidateLastName = entity.Candidate.LastName;
                model.CandidateGuid = entity.Candidate.CandidateGuid;
                model.EmployeeId = entity.Candidate.EmployeeId;
                model.FranchiseId = entity.Candidate.FranchiseId;
            }
            return model;
        }

        public static CandidateSmartCard ToEntity(this CandidateSmartCardModel model)
        {
            return Mapper.Map<CandidateSmartCardModel, CandidateSmartCard>(model);
        }

        public static CandidateSmartCard ToEntity(this CandidateSmartCardModel model, CandidateSmartCard destination)
        {
            return Mapper.Map(model, destination);
        }

        public static CandidateSmartCardMatchModel ToMatchModel(this CandidateSmartCard entity, string smartCardUid)
        {
            var model = Mapper.Map<CandidateSmartCard, CandidateSmartCardMatchModel>(entity);
            if (entity.Candidate != null)
            {
                model.CandidateFirstName = entity.Candidate.FirstName;
                model.CandidateLastName = entity.Candidate.LastName;
                model.EmployeeId = entity.Candidate.EmployeeId;
                model.FranchiseId = entity.Candidate.FranchiseId;

                if (model.SmartCardUid == smartCardUid)
                    model.CandidateSmartCardMatchStatusId = model.IsActive ?
                                                            (int)CandidateSmartCardMatchStatus.Active :
                                                            (int)CandidateSmartCardMatchStatus.Inactive;
                else if (model.SmartCardUid.Contains(smartCardUid))
                    model.CandidateSmartCardMatchStatusId = (int)CandidateSmartCardMatchStatus.Partial;
                else
                    model.CandidateSmartCardMatchStatusId = (int)CandidateSmartCardMatchStatus.Unknown;
            }

            return model;
        }

        #endregion

        #region ClockTime


        public static CandidateClockTimeModel ToModel(this CandidateClockTime entity)
        {
            var model = Mapper.Map<CandidateClockTime, CandidateClockTimeModel>(entity);
            if (entity.Candidate != null)
            {
                model.CandidateGuid = entity.Candidate.CandidateGuid;
                model.FranchiseId = entity.Candidate.FranchiseId;
                model.EmployeeId = entity.Candidate.EmployeeId;
            }
            return model;
        }

        public static CandidateClockTime ToEntity(this CandidateClockTimeModel model)
        {
            return Mapper.Map<CandidateClockTimeModel, CandidateClockTime>(model);
        }

        public static CandidateClockTime ToEntity(this CandidateClockTimeModel model, CandidateClockTime destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region ClockDevice

        public static CompanyClockDeviceModel ToModel(this CompanyClockDevice entity)
        {
            var model = Mapper.Map<CompanyClockDevice, CompanyClockDeviceModel>(entity);
            model.CompanyLocationModel = entity.CompanyLocation.ToModel();
            model.CompanyLocationModel.CompanyModel = entity.CompanyLocation.Company.ToModel();

            return model;
        }

        public static CompanyClockDevice ToEntity(this CompanyClockDeviceModel model)
        {
            return Mapper.Map<CompanyClockDeviceModel, CompanyClockDevice>(model);
        }

        public static CompanyClockDevice ToEntity(this CompanyClockDeviceModel model, CompanyClockDevice destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Setting

        public static SettingModel ToModel(this Setting entity)
        {
            return Mapper.Map<Setting, SettingModel>(entity);
        }

        public static Setting ToEntity(this SettingModel model)
        {
            return Mapper.Map<SettingModel, Setting>(model);
        }

        public static Setting ToEntity(this SettingModel model, Setting destination)
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

        #region Gender

        public static EthnicTypeModel ToModel(this EthnicType entity)
        {
            return Mapper.Map<EthnicType, EthnicTypeModel>(entity);
        }

        public static EthnicType ToEntity(this EthnicTypeModel model)
        {
            return Mapper.Map<EthnicTypeModel, EthnicType>(model);
        }

        public static EthnicType ToEntity(this EthnicTypeModel model, EthnicType destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region MessageTemplate

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            var model = Mapper.Map<MessageTemplate, MessageTemplateModel>(entity);
            model.AccountRolesIds = entity.AccountRoles.Select(x => x.AccountRoleId).ToList();
            return model;
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return Mapper.Map<MessageTemplateModel, MessageTemplate>(model);
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return Mapper.Map(model, destination);
        }


        #endregion

        #region EmialHistory

        public static MessageHistoryModel ToModel(this MessageHistory entity)
        {
            var model = Mapper.Map<MessageHistory, MessageHistoryModel>(entity);
            model.Body = HttpUtility.HtmlDecode(entity.Body);

            return model;
        }

        public static MessageHistory ToEntity(this MessageHistoryModel model)
        {
            return Mapper.Map<MessageHistoryModel, MessageHistory>(model);
        }

        public static MessageHistory ToEntity(this MessageHistoryModel model, MessageHistory destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return Mapper.Map<QueuedEmail, QueuedEmailModel>(entity);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            var eneity = Mapper.Map<QueuedEmailModel, QueuedEmail>(model);
            eneity.AttachmentFileName = model.AttachmentFileName;
            eneity.AttachmentFilePath = model.AttachmentFilePath;
            eneity.EmailAccountId = model.EmailAccountId;
            eneity.UpdatedOnUtc = model.UpdatedOnUtc;
            eneity.CreatedOnUtc = model.CreatedOnUtc;
            return eneity;
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
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


        #region Forums

        //forum groups
        public static ForumGroupModel ToModel(this ForumGroup entity)
        {
            return Mapper.Map<ForumGroup, ForumGroupModel>(entity);
        }

        public static ForumGroup ToEntity(this ForumGroupModel model)
        {
            return Mapper.Map<ForumGroupModel, ForumGroup>(model);
        }

        public static ForumGroup ToEntity(this ForumGroupModel model, ForumGroup destination)
        {
            return Mapper.Map(model, destination);
        }
        //forums
        public static ForumModel ToModel(this Forum entity)
        {
            return Mapper.Map<Forum, ForumModel>(entity);
        }

        public static Forum ToEntity(this ForumModel model)
        {
            return Mapper.Map<ForumModel, Forum>(model);
        }

        public static Forum ToEntity(this ForumModel model, Forum destination)
        {
            return Mapper.Map(model, destination);
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

        #region Position

        public static PositionModel ToModel(this Position entity)
        {
            return Mapper.Map<Position, PositionModel>(entity);
        }

        public static Position ToEntity(this PositionModel model)
        {
            return Mapper.Map<PositionModel, Position>(model);
        }

        public static Position ToEntity(this PositionModel model, Position destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region CandidateWorkTime

        public static CandidateWorkTimeModel ToModel(this CandidateWorkTime entity)
        {
            var model = Mapper.Map<CandidateWorkTime, CandidateWorkTimeModel>(entity);
            
            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeId = entity.Candidate.EmployeeId;
            model.EmployeeFirstName = entity.Candidate.FirstName;
            model.EmployeeLastName = entity.Candidate.LastName;
            model.FranchiseId = entity.Candidate.FranchiseId;
            model.JobOrderGuid = entity.JobOrder.JobOrderGuid;
            model.JobTitle = entity.JobOrder.JobTitle;
            model.CompanyName = entity.JobOrder.Company.CompanyName;
            model.LocationName = entity.JobOrder.Company.CompanyLocations.Where(x => x.Id == entity.CompanyLocationId).FirstOrDefault().LocationName;
            model.DepartmentName = entity.CompanyDepartmentId == 0 ? string.Empty : entity.JobOrder.Company.CompanyDepartments.Where(x => x.Id == entity.CompanyDepartmentId).FirstOrDefault().DepartmentName;
            model.ContactName = entity.JobOrder.CompanyContact == null ? string.Empty : entity.JobOrder.CompanyContact.FullName;

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

        public static CandidateWorkTimeBaseModel ToBaseModel(this CandidateWorkTime entity)
        {
            var model = Mapper.Map<CandidateWorkTime, CandidateWorkTimeBaseModel>(entity);

            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeId = entity.Candidate.EmployeeId;
            model.EmployeeFirstName = entity.Candidate.FirstName;
            model.EmployeeLastName = entity.Candidate.LastName;
            model.FranchiseId = entity.FranchiseId;
            model.JobOrderGuid = entity.JobOrder.JobOrderGuid;
            model.JobTitle = entity.JobOrder.JobTitle;
            model.CompanyName = entity.JobOrder.Company.CompanyName;
            model.LocationName = entity.JobOrder.Company.CompanyLocations.Where(x => x.Id == entity.CompanyLocationId).FirstOrDefault().LocationName;
            model.DepartmentName = entity.CompanyDepartmentId == 0 ? string.Empty : entity.JobOrder.Company.CompanyDepartments.Where(x => x.Id == entity.CompanyDepartmentId).FirstOrDefault().DepartmentName;
            model.ContactName = entity.JobOrder.CompanyContact == null ? string.Empty : entity.JobOrder.CompanyContact.FullName;

            return model;
        }

        public static CandidateWorkTimeLogModel ToModel(this CandidateWorkTimeLog entity)
        {
            return Mapper.Map<CandidateWorkTimeLog, CandidateWorkTimeLogModel>(entity);
        }

        #endregion


        #region CandidateMissingHour

        public static CandidateMissingHourModel ToModel(this CandidateMissingHour entity)
        {
            var model = Mapper.Map<CandidateMissingHour, CandidateMissingHourModel>(entity);

            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeId = entity.Candidate.EmployeeId;
            model.EmployeeFirstName = entity.Candidate.FirstName;
            model.EmployeeLastName = entity.Candidate.LastName;
            model.FranchiseId = entity.Candidate.FranchiseId;
            model.JobOrderGuid = entity.JobOrder.JobOrderGuid;
            model.JobTitle = entity.JobOrder.JobTitle;
            model.CompanyName = entity.JobOrder.Company.CompanyName;
            model.LocationName = entity.JobOrder.Company.CompanyLocations.Where(x => x.Id == entity.JobOrder.CompanyLocationId).FirstOrDefault().LocationName;
            model.DepartmentName = entity.JobOrder.CompanyDepartmentId == 0 ? string.Empty : entity.JobOrder.Company.CompanyDepartments.Where(x => x.Id == entity.JobOrder.CompanyDepartmentId).FirstOrDefault().DepartmentName;
            model.ContactName = entity.JobOrder.CompanyContact == null ? string.Empty : entity.JobOrder.CompanyContact.FullName;

            return model;
        }

        public static CandidateMissingHour ToEntity(this CandidateMissingHourModel model)
        {
            return Mapper.Map<CandidateMissingHourModel, CandidateMissingHour>(model);
        }

        public static CandidateMissingHour ToEntity(this CandidateMissingHourModel model, CandidateMissingHour destination)
        {
            return Mapper.Map(model, destination);
        }

        public static MissingHourDocumentModel ToModel(this MissingHourDocument entity)
        {
            return Mapper.Map<MissingHourDocument, MissingHourDocumentModel>(entity);
        }

        public static MissingHourDocument ToEntity(this MissingHourDocumentModel model)
        {
            return Mapper.Map<MissingHourDocumentModel, MissingHourDocument>(model);
        }

        public static MissingHourDocument ToEntity(this MissingHourDocumentModel model, MissingHourDocument destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region ClientTimeSheetDocument

        public static ClientTimeSheetDocumentModel ToModel(this ClientTimeSheetDocument entity)
        {
            return Mapper.Map<ClientTimeSheetDocument, ClientTimeSheetDocumentModel>(entity);
        }

        public static ClientTimeSheetDocument ToEntity(this ClientTimeSheetDocumentModel model)
        {
            return Mapper.Map<ClientTimeSheetDocumentModel, ClientTimeSheetDocument>(model);
        }

        public static ClientTimeSheetDocument ToEntity(this ClientTimeSheetDocumentModel model, ClientTimeSheetDocument destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Account

        public static AccountFullModel ToFullModel(this Account entity)
        {
            return Mapper.Map<Account, AccountFullModel>(entity);
        }

        public static Account ToEntity(this AccountFullModel model)
        {
            return Mapper.Map<AccountFullModel, Account>(model);
        }

        public static Account ToEntity(this AccountFullModel model, Account destination)
        {
            return Mapper.Map(model, destination);
        }

        public static AccountModel ToModel(this Account entity)
        {
            return Mapper.Map<Account, AccountModel>(entity);
        }

        public static Account ToEntity(this AccountModel model)
        {
            return Mapper.Map<AccountModel, Account>(model);
        }

        public static Account ToEntity(this AccountModel model, Account destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion



        #region OvertimeRuleSetting

        public static OvertimeRuleSettingModel ToModel(this OvertimeRuleSetting entity)
        {
            return Mapper.Map<OvertimeRuleSetting, OvertimeRuleSettingModel>(entity);
        }

        public static OvertimeRuleSetting ToEntity(this OvertimeRuleSettingModel model)
        {
            return Mapper.Map<OvertimeRuleSettingModel, OvertimeRuleSetting>(model);
        }

        public static OvertimeRuleSetting ToEntity(this OvertimeRuleSettingModel model, OvertimeRuleSetting destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region ScheduleTask

        public static ScheduleTaskModel ToModel(this ScheduleTask entity)
        {
            return Mapper.Map<ScheduleTask, ScheduleTaskModel>(entity);
        }

        public static ScheduleTask ToEntity(this ScheduleTaskModel model)
        {
            return Mapper.Map<ScheduleTaskModel, ScheduleTask>(model);
        }

        public static ScheduleTask ToEntity(this ScheduleTaskModel model, ScheduleTask destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Bank

        public static BankModel ToModel(this Bank entity)
        {
            return Mapper.Map<Bank, BankModel>(entity);
        }

        public static Bank ToEntity(this BankModel model)
        {
            return Mapper.Map<BankModel, Bank>(model);
        }

        public static Bank ToEntity(this BankModel model, Bank destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CandidateWithAddress

        public static CandidateWithAddressModel ToModel(this CandidateWithAddress entity)
        {
            CandidateWithAddressModel item = Mapper.Map<CandidateWithAddress, CandidateWithAddressModel>(entity);
            if (entity.BirthDate != null)
            {
                item.Age = entity.BirthDate.Value.CalculateAge().ToString();
            }
            return item;
        }

        public static CandidateWithAddress ToEntity(this CandidateWithAddressModel model)
        {
            return Mapper.Map<CandidateWithAddressModel, CandidateWithAddress>(model);
        }

        public static CandidateWithAddress ToEntity(this CandidateWithAddressModel model, CandidateWithAddress destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CompanyContact


        public static CompanyContactViewModel ToCompanyContactViewModel(this CompanyContact entity)
        {
            return Mapper.Map<CompanyContact, CompanyContactViewModel>(entity);
        }

        #endregion

        public static AccountsViewModel ToAccountsViewModel(this Account entity)
        {
            return Mapper.Map<Account, AccountsViewModel>(entity);
        }

        public static CandidateAddressModel ToModel(this CandidateAddressWithName entity)
        {
            return Mapper.Map<CandidateAddressWithName, CandidateAddressModel>(entity);
        }

        public static CandidateJobHistoryModel ToModel(this CandidateJobHistory entity)
        {
            return Mapper.Map<CandidateJobHistory, CandidateJobHistoryModel>(entity);
        }

        public static JobOrderWithCompanyAddressModel ToModel(this JobOrderWithCompanyAddress entity)
        {
            return Mapper.Map<JobOrderWithCompanyAddress, JobOrderWithCompanyAddressModel>(entity);
        }

        #region Incident
        public static IncidentCategoryModel ToModel(this IncidentCategory entity)
        {
            return Mapper.Map<IncidentCategory, IncidentCategoryModel>(entity);
        }
        public static IncidentCategory ToEntity(this IncidentCategoryModel model)
        {
            return Mapper.Map<IncidentCategoryModel, IncidentCategory>(model);
        }
        public static IncidentReportTemplateModel ToModel(this IncidentReportTemplate entity)
        {
            return Mapper.Map<IncidentReportTemplate, IncidentReportTemplateModel>(entity);
        }
        public static IncidentReportTemplate ToEntity(this IncidentReportTemplateModel model)
        {
            return Mapper.Map<IncidentReportTemplateModel, IncidentReportTemplate>(model);
        }
        #endregion

        #region RecruiterCompany
        public static RecruiterCompanyModel ToModel(this RecruiterCompany entity)
        {
            RecruiterCompanyModel model = Mapper.Map<RecruiterCompany, RecruiterCompanyModel>(entity);
            model.Email = entity.Account.Email;
            model.FirstName = entity.Account.FirstName;
            model.LastName = entity.Account.LastName;
            //model.MiddleName = entity.Account.MiddleName;
            model.HomePhone = entity.Account.HomePhone;
            model.MobilePhone = entity.Account.MobilePhone;
            model.WorkPhone = entity.Account.WorkPhone;
            model.FranchiseId = entity.Account.FranchiseId;
            model.FranchiseName = entity.Account.Franchise.FranchiseName;
            model.IsActive = entity.Account.IsActive;
            //model.CompanyGuid = entity.Company.CompanyGuid;
            return model;
        }

        public static RecruiterCompany ToEntity(this RecruiterCompanyModel model)
        {
            return Mapper.Map<RecruiterCompanyModel, RecruiterCompany>(model);
        }
        #endregion

        #region Timeoff
        public static EmployeeTimeoffTypeModel ToModel(this EmployeeTimeoffType entity)
        {
            return Mapper.Map<EmployeeTimeoffType, EmployeeTimeoffTypeModel>(entity);
        }
        public static EmployeeTimeoffType ToEntity(this EmployeeTimeoffTypeModel model)
        {
            return Mapper.Map<EmployeeTimeoffTypeModel, EmployeeTimeoffType>(model);
        }
        public static StatutoryHolidayModel ToModel(this StatutoryHoliday entity)
        {
            return Mapper.Map<StatutoryHoliday, StatutoryHolidayModel>(entity);
        }
        public static StatutoryHoliday ToEntity(this StatutoryHolidayModel model)
        {
            return Mapper.Map<StatutoryHolidayModel, StatutoryHoliday>(model);
        }
        public static StatutoryHoliday ToEntity(this StatutoryHolidayModel model, StatutoryHoliday destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region DocumentType

        public static DocumentTypeModel ToModel(this DocumentType entity)
        {
            return Mapper.Map<DocumentType, DocumentTypeModel>(entity);
        }

        public static DocumentType ToEntity(this DocumentTypeModel model)
        {
            return Mapper.Map<DocumentTypeModel, DocumentType>(model);
        }
        public static DocumentType ToEntity(this DocumentTypeModel model, DocumentType destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region MessageCategory

        public static MessageCategoryModel ToModel(this MessageCategory entity)
        {
            return Mapper.Map<MessageCategory, MessageCategoryModel>(entity);
        }

        public static MessageCategory ToEntity(this MessageCategoryModel model)
        {
            return Mapper.Map<MessageCategoryModel, MessageCategory>(model);
        }
        public static MessageCategory ToEntity(this MessageCategoryModel model, MessageCategory destination)
        {
            return Mapper.Map(model, destination);
        }
       

        #endregion


        #region Message

        public static MessageModel ToModel(this Message entity)
        {
            var model = Mapper.Map<Message, MessageModel>(entity);
            
            model.MailFrom = entity.MessageHistory.MailFrom;
            model.FromName = entity.MessageHistory.FromName;
            model.MailTo = entity.MessageHistory.MailTo;
            model.ToName = entity.MessageHistory.ToName ?? entity.MessageHistory.MailTo;
            model.CC = entity.MessageHistory.CC;
            model.Bcc = entity.MessageHistory.Bcc;
            model.Subject = entity.MessageHistory.Subject;
            model.Body = HttpUtility.HtmlDecode(entity.MessageHistory.Body);
            model.AttachmentFileName = entity.MessageHistory.AttachmentFileName;
            model.Note = entity.MessageHistory.Note;
            if(entity.MessageHistory.MessageCategory!=null)
                model.MessageCategory = entity.MessageHistory.MessageCategory.CategoryName;
            model.CreatedOnUtc = entity.MessageHistory.CreatedOnUtc;

            return model;
        }


        public static ResumeModel ToModel(this Resume entity)
        {
            var model = Mapper.Map<Resume, ResumeModel>(entity);

            model.Date = entity.Date.ToLocalTime();

            if (!String.IsNullOrWhiteSpace(entity.Body))
                model.Body = entity.Body.Replace(Environment.NewLine, "<br />");

            if (!String.IsNullOrWhiteSpace(entity.HtmlBody))
                model.HtmlBody = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(entity.HtmlBody));

            //if (entity.ResumeHistories.Any())
            //    model.ResumeHistoryModels = entity.ResumeHistories.Select(x => x.ToModel()).ToList();

            return model;
        }


        public static ResumeHistoryModel ToModel(this ResumeHistory entity, bool ReadOnly = false)
        {
            var model = Mapper.Map<ResumeHistory, ResumeHistoryModel>(entity);
            model.ContactedOn = model.ContactedOnUtc.ToLocalTime();

            return model;
        }

        #endregion


        #region Account Password Policy
        public static AccountPasswordPolicyModel ToModel(this AccountPasswordPolicy entity)
        {
            return Mapper.Map<AccountPasswordPolicy, AccountPasswordPolicyModel>(entity);
        }

        public static AccountPasswordPolicy ToEntity(this AccountPasswordPolicyModel model)
        {
            return Mapper.Map<AccountPasswordPolicyModel, AccountPasswordPolicy>(model);
        }
        public static AccountPasswordPolicy ToEntity(this AccountPasswordPolicyModel model, AccountPasswordPolicy destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion


        #region Feature

        public static Feature ToEntity(this FeatureModel model)
        {
            return Mapper.Map<FeatureModel, Feature>(model);
        }
        public static Feature ToEntity(this FeatureModel model, Feature destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CompanyActivity
        public static CompanyActivityModel ToModel(this CompanyActivity entity)
        {
            return Mapper.Map<CompanyActivity, CompanyActivityModel>(entity);
        }
        public static CompanyActivity ToEntity(this CompanyActivityModel model)
        {
            return Mapper.Map<CompanyActivityModel, CompanyActivity>(model);
        }
        public static CompanyActivity ToEntity(this CompanyActivityModel model, CompanyActivity destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Announcement

        public static AnnouncementModel ToModel(this Announcement entity)
        {
            return Mapper.Map<Announcement, AnnouncementModel>(entity);
        }
        public static Announcement ToEntity(this AnnouncementModel model)
        {
            return Mapper.Map<AnnouncementModel, Announcement>(model);
        }
        public static Announcement ToEntity(this AnnouncementModel model, Announcement destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region CompanyBasicInformation
        public static CompanyBasicInformation ToBasicInformationModel(this Company entity)
        {
            return Mapper.Map<Company, CompanyBasicInformation>(entity);
        }
        public static Company ToEntity(this CompanyBasicInformation model)
        {
            return Mapper.Map<CompanyBasicInformation, Company>(model);
        }
        public static Company ToEntity(this CompanyBasicInformation model, Company destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Company Recruiter Simple Model
        public static RecruiterCompanySimpleModel ToSimpleModel(this RecruiterCompany entity)
        {
            var model = Mapper.Map<RecruiterCompany, RecruiterCompanySimpleModel>(entity);
            model.FranchiseId = entity.Account.FranchiseId;
            model.CompanyGuid = entity.Company.CompanyGuid;
            model.FranchiseGuid = entity.Account.Franchise.FranchiseGuid;
            return model;
        }
        public static RecruiterCompany ToEntity(this RecruiterCompanySimpleModel model)
        {
            return Mapper.Map<RecruiterCompanySimpleModel, RecruiterCompany>(model);
        }
        public static RecruiterCompany ToEntity(this RecruiterCompanySimpleModel model, RecruiterCompany destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Company Contact Model
        public static CompanyContactModel ToCompanyContactModel(this Account entity)
        {
            var model = Mapper.Map<Account, CompanyContactModel>(entity);
            model.AccountRoleSystemName = entity.AccountRoles.FirstOrDefault().SystemName;
            return model;
        }
        public static Account ToEntity(this CompanyContactModel model)
        {
            return Mapper.Map<CompanyContactModel, Account>(model);
        }
        public static Account ToEntity(this CompanyContactModel model, Account destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region WSIB
        public static WSIBModel ToModel(this WSIB entity)
        {
            var model = Mapper.Map<WSIB, WSIBModel>(entity);
            model.CountryId = entity.Province.CountryId;
            model.CountryName = entity.Province.Country.CountryName;
            model.ProvinceName = entity.Province.StateProvinceName;
            return model;
        }
        public static WSIB ToEntity(this WSIBModel model)
        {
            return Mapper.Map<WSIBModel, WSIB>(model);
        }
        public static WSIB ToEntity(this WSIBModel model, WSIB destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Company Email Template
        public static CompanyEmailTemplateModel ToModel(this CompanyEmailTemplate entity)
        {
            var model = Mapper.Map<CompanyEmailTemplate, CompanyEmailTemplateModel>(entity);
            model.CompanyGuid = entity.Company.CompanyGuid;
            model.KeepFile1 = model.KeepFile2 = model.KeepFile3 = true;
            return model;
        }
        public static CompanyEmailTemplate ToEntity(this CompanyEmailTemplateModel model)
        {
            var entity = Mapper.Map<CompanyEmailTemplateModel, CompanyEmailTemplate>(model);

            if (model.CompanyDepartmentId == 0)
                entity.CompanyDepartmentId = null;
            if (model.CompanyLocationId == 0)
                entity.CompanyLocationId = null;
            if (!string.IsNullOrEmpty(model.AttachmentFileName))
                entity.AttachmentFileName = Path.GetFileName(entity.AttachmentFileName);
            if (!string.IsNullOrEmpty(model.AttachmentFileName2))
                entity.AttachmentFileName2 = Path.GetFileName(entity.AttachmentFileName2);
            if (!string.IsNullOrEmpty(model.AttachmentFileName3))
                entity.AttachmentFileName3 = Path.GetFileName(entity.AttachmentFileName3);
            return entity;
        }
        public static CompanyEmailTemplate ToEntity(this CompanyEmailTemplateModel model, CompanyEmailTemplate destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Compny Attachment

        public static CompanyAttachmentModel ToModel(this CompanyAttachment entity, Account account)
        {
            var model = Mapper.Map<CompanyAttachment, CompanyAttachmentModel>(entity);
            model.Readable = account.IsAdministrator() || account.IsPayrollAdministrator() || !entity.IsRestricted || (entity.EnteredBy.HasValue && entity.EnteredBy == account.Id);
            model.Writeable = model.Deletable = account.IsAdministrator() || account.IsPayrollAdministrator() || (entity.EnteredBy.HasValue && entity.EnteredBy == account.Id);

            return model;
        }
        
        public static CompanyAttachment ToEntity(this CompanyAttachmentModel model)
        {
            return Mapper.Map<CompanyAttachmentModel, CompanyAttachment>(model);
        }
        public static CompanyAttachment ToEntity(this CompanyAttachmentModel model, CompanyAttachment destination)
        {
            return Mapper.Map(model, destination);
        }
        
        #endregion

        #region DNR reasons
        public static DNRReasonModel ToModel(this DNRReason entity)
        {
            var model = Mapper.Map<DNRReason, DNRReasonModel>(entity);
            return model;
        }
        public static DNRReason ToEntity(this DNRReasonModel model)
        {
            return Mapper.Map<DNRReasonModel, DNRReason>(model);
        }
        public static DNRReason ToEntity(this DNRReasonModel model, DNRReason destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Employee Payroll Setting

        public static EmployeePayrollSettingModel ToModel(this EmployeePayrollSetting entity, int employeeTypeId)
        {
            var model = Mapper.Map<EmployeePayrollSetting, EmployeePayrollSettingModel>(entity);
            model.EmployeeTypeId = employeeTypeId;

            return model;
        }


        public static EmployeePayrollSetting ToEntity(this EmployeePayrollSettingModel model)
        {
            return Mapper.Map<EmployeePayrollSettingModel, EmployeePayrollSetting>(model);
        }


        public static EmployeePayrollSetting ToEntity(this EmployeePayrollSettingModel model, EmployeePayrollSetting destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion


        #region Employee TD1

        public static EmployeeTD1Model ToModel(this EmployeeTD1 entity)
        {
            var model = Mapper.Map<EmployeeTD1, EmployeeTD1Model>(entity);
            return model;
        }


        public static EmployeeTD1 ToEntity(this EmployeeTD1Model model)
        {
            return Mapper.Map<EmployeeTD1Model, EmployeeTD1>(model);
        }


        public static EmployeeTD1 ToEntity(this EmployeeTD1Model model, EmployeeTD1 destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Payroll Email Setting
        public static PayrollEmailSettingModel ToModel(this EmailSetting entity)
        {
            var model = Mapper.Map<EmailSetting, PayrollEmailSettingModel>(entity);
            model.Simple = true;
            return model;
        }


        public static EmailSetting ToEntity(this PayrollEmailSettingModel model)
        {
            return Mapper.Map<PayrollEmailSettingModel, EmailSetting>(model);
        }


        public static EmailSetting ToEntity(this PayrollEmailSettingModel model, EmailSetting destination)
        {
            return Mapper.Map(model, destination);
        }

        public static PayrollEmailSettingDetailModel ToDetailedModel(this EmailSetting entity)
        {
            var model = Mapper.Map<EmailSetting, PayrollEmailSettingDetailModel>(entity);
            model.Simple = false;
            return model;
        }


        public static EmailSetting ToEntity(this PayrollEmailSettingDetailModel model)
        {
            return Mapper.Map<PayrollEmailSettingDetailModel, EmailSetting>(model);
        }


        public static EmailSetting ToEntity(this PayrollEmailSettingDetailModel model, EmailSetting destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Test Material

        public static TestMaterialModel ToModel(this TestMaterial entity)
        {
            return Mapper.Map<TestMaterial, TestMaterialModel>(entity);
        }

        public static TestMaterial ToEntity(this TestMaterialModel model)
        {
            return Mapper.Map<TestMaterialModel, TestMaterial>(model);
        }
        public static TestMaterial ToEntity(this TestMaterialModel model, TestMaterial destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Job Board
        public static JobBoardModel ToModel(this JobBoard entity)
        {
            return Mapper.Map<JobBoard, JobBoardModel>(entity);
        }

        public static JobBoard ToEntity(this JobBoardModel model)
        {
            return Mapper.Map<JobBoardModel, JobBoard>(model);
        }
        public static JobBoard ToEntity(this JobBoardModel model, JobBoard destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Payroll Item
        public static PayrollItemModel ToViewModel(this Payroll_Item entity)
        {
            return Mapper.Map<Payroll_Item, PayrollItemModel>(entity);
        }

        public static PayrollItemDetailModel ToModel(this Payroll_Item entity)
        {
            return Mapper.Map<Payroll_Item, PayrollItemDetailModel>(entity);
        }

        public static Payroll_Item ToEntity(this PayrollItemDetailModel model)
        {
            return Mapper.Map<PayrollItemDetailModel, Payroll_Item>(model);
        }
        public static Payroll_Item ToEntity(this PayrollItemDetailModel model, Payroll_Item destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion


        #region Tax Form Box
        public static TaxFormBoxModel ToModel(this TaxFormBox entity)
        {
            var model = Mapper.Map<TaxFormBox, TaxFormBoxModel>(entity);
            return model;
        }
        public static TaxFormBoxModel ToSelectedModel(this TaxFormBox entity,int payrollItemId)
        {
            var model = entity.ToModel();
            if (entity.PayrollItems.Count > 0 && entity.PayrollItems.Where(x => x.ID == payrollItemId).Count() > 0)
                model.IsSelected = true;
            else
                model.IsSelected = false;
            return model;
        }

        public static TaxFormBox ToEntity(this TaxFormBoxModel model)
        {
            return Mapper.Map<TaxFormBoxModel, TaxFormBox>(model);
        }
        public static TaxFormBox ToEntity(this TaxFormBoxModel model, TaxFormBox destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region Candidate WSIB Common Rate
        public static CandidateWSIBCommonRateModel ToModel(this CandidateWSIBCommonRate entity)
        {
            var model = Mapper.Map<CandidateWSIBCommonRate, CandidateWSIBCommonRateModel>(entity);
            return model;
        }

        public static CandidateWSIBCommonRate ToEntity(this CandidateWSIBCommonRateModel model)
        {
            return Mapper.Map<CandidateWSIBCommonRateModel, CandidateWSIBCommonRate>(model);
        }
        public static CandidateWSIBCommonRate ToEntity(this CandidateWSIBCommonRateModel model, CandidateWSIBCommonRate destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion
    }
}

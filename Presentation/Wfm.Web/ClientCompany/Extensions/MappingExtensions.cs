using AutoMapper;
using System;
using System.Linq;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Client.Models.Candidate;
using Wfm.Client.Models.Companies;
using Wfm.Client.Models.CompanyBilling;
using Wfm.Client.Models.JobOrder;
using Wfm.Client.Models.Accounts;
using Wfm.Client.Models.ClockTime;
using Wfm.Client.Models.TimeSheet;
using Wfm.Client.Models.CompanyContact;
using Wfm.Client.Models.Incident;
using Wfm.Core.Domain.Incident;
using Wfm.Core.Domain.Employees;
using Wfm.Client.Models.Employees;
using Wfm.Shared.Models.Accounts;
using Wfm.Client.Models.Media;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Franchises;
using Wfm.Client.Models.Franchises;


namespace Wfm.Client.Extensions
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

        #region Account

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

        #endregion

        #region CandidateBasicInfo
        public static CandidateBasicInfoModel ToBasicModel(this Candidate entity)
        {
            var model = Mapper.Map<Candidate, CandidateBasicInfoModel>(entity);
            model.Gender = entity.Gender.GenderName;
            return model;
        }
        #endregion

        #region CandidatekeySkills
        public static CandidateKeySkillModel ToModel(this CandidateKeySkill entity)
        {
            return Mapper.Map<CandidateKeySkill, CandidateKeySkillModel>(entity);
        }
        #endregion


        #region CandidateTestResults

        public static CandidateTestResultModel ToModel(this CandidateTestResult entity)
        {
            return Mapper.Map<CandidateTestResult, CandidateTestResultModel>(entity);
        }
        
        #endregion


        #region CandidateAttachments
        public static CandidateAttachmentModel ToModel(this CandidateAttachment entity)
        {
            var model = Mapper.Map<CandidateAttachment, CandidateAttachmentModel>(entity);
            model.DocumentTypeName = entity.DocumentType.TypeName;
            model.AttachmentTypeModel = entity.AttachmentType.ToModel();
            return model;
        }
        #endregion

        #region AttachmentType
        public static AttachmentTypeModel ToModel(this AttachmentType entity)
        {
            return Mapper.Map<AttachmentType, AttachmentTypeModel>(entity);
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


        #region CandidatePlacment (re-scheduling)

        public static CandidatePlacementModel ToPlacementModel(this CandidateJobOrder entity, DateTime refDate)
        {
            var model = Mapper.Map<CandidateJobOrder, CandidatePlacementModel>(entity);
            var overNight = entity.JobOrder.StartTime.TimeOfDay > entity.JobOrder.EndTime.TimeOfDay;

            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeId = entity.Candidate.EmployeeId;
            model.EmployeeFirstName = entity.Candidate.FirstName;
            model.EmployeeLastName = entity.Candidate.LastName;

            model.JobTitle = entity.JobOrder.JobTitle;
            model.ShiftId = entity.JobOrder.ShiftId;
            model.ShiftName = entity.JobOrder.Shift.ShiftName;
            model.CompanyId = entity.JobOrder.CompanyId;
            model.CompanyName = entity.JobOrder.Company.CompanyName;
            model.CompanyLocationId = entity.JobOrder.CompanyLocationId;
            model.CompanyDepartmentId = entity.JobOrder.CompanyDepartmentId;
            model.CompanyContactId = entity.JobOrder.CompanyContactId;
            model.StartTime = refDate.Date + entity.JobOrder.StartTime.TimeOfDay;
            model.EndTime = refDate.Date.AddDays(overNight ? 1 : 0) + entity.JobOrder.EndTime.TimeOfDay;

            return model;
        }

        public static CandidateJobOrder ToEntity(this CandidatePlacementModel model)
        {
            return Mapper.Map<CandidatePlacementModel, CandidateJobOrder>(model);
        }

        public static CandidateJobOrder ToEntity(this CandidatePlacementModel model, CandidateJobOrder destination)
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

        #region CompanyDepartment

        public static CompanyDepartmentModel ToModel(this CompanyDepartment entity)
        {
            var model =  Mapper.Map<CompanyDepartment, CompanyDepartmentModel>(entity);
            model.CompanyLocationName = entity.CompanyLocation.LocationName;
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

        #region Companyvendors

        public static CompanyVendorModel ToModel(this CompanyVendor entity)
        {
            var model = Mapper.Map<CompanyVendor, CompanyVendorModel>(entity);
            model.VendorName = entity.Vendor.FranchiseName;
            model.Email = entity.Vendor.Email;
            model.FranchiseGuid = entity.Vendor.FranchiseGuid;
            model.VendorWebsite = entity.Vendor.WebSite;
            return model;
        }
        #endregion

        #region VendorCertificate
        public static VendorCertificateModel ToModel(this VendorCertificate entity)
        {
            return Mapper.Map<VendorCertificate, VendorCertificateModel>(entity);
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

        #region CompanyBilling

        public static CompanyBillingRateModel ToModel(this CompanyBillingRate entity)
        {
            var model = Mapper.Map<CompanyBillingRate, CompanyBillingRateModel>(entity);
            model.FranchiseName = entity.Franchise.FranchiseName;
            model.PositionCode = entity.Position.Name;
            return model;
        }

        public static CompanyBillingRate ToEntity(this CompanyBillingRateModel model)
        {
            return Mapper.Map<CompanyBillingRateModel, CompanyBillingRate>(model);
        }

        public static CompanyBillingRate ToEntity(this CompanyBillingRateModel model, CompanyBillingRate destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region ClockTime


        public static CandidateClockTimeModel ToModel(this CandidateClockTime entity)
        {
            return Mapper.Map<CandidateClockTime, CandidateClockTimeModel>(entity);
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

        #region CandidateWorkTime

        public static CandidateWorkTimeModel ToModel(this CandidateWorkTime entity)
        {
            var model = Mapper.Map<CandidateWorkTime, CandidateWorkTimeModel>(entity);
            model.EmployeeId = entity.Candidate.EmployeeId;
            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeFirstName = entity.Candidate.FirstName;
            model.EmployeeLastName = entity.Candidate.LastName;
            model.FranchiseId = entity.Candidate.FranchiseId;
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

        public static EmployeeWorkTimeApprovalModel ToModel(this EmployeeWorkTimeApproval entity)
        {
            var model = Mapper.Map<EmployeeWorkTimeApproval, EmployeeWorkTimeApprovalModel>(entity);
            return model;
        }
     
        public static DailyTimeSheetModel ToDailyTimeSheetModel(this EmployeeWorkTimeApproval entity)
        {
            var model = Mapper.Map<EmployeeWorkTimeApproval, DailyTimeSheetModel>(entity);
            return model;
        }

        #endregion


        #region ClockDevice

        public static CompanyClockDeviceModel ToModel(this CompanyClockDevice entity)
        {
            return Mapper.Map<CompanyClockDevice, CompanyClockDeviceModel>(entity);
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


        #region AccountDelegate
        public static AccountDelegateModel ToModel(this AccountDelegate entity)
        {
            return Mapper.Map<AccountDelegate, AccountDelegateModel>(entity);
        }

        public static AccountDelegate ToEntity(this AccountDelegateModel model)
        {
            return Mapper.Map<AccountDelegateModel, AccountDelegate>(model);
        }
        public static AccountDelegateHistoryModel ToModel(this AccountDelegateHistory entity)
        {
            return Mapper.Map<AccountDelegateHistory, AccountDelegateHistoryModel>(entity);
        }
        #endregion

        #region CompanyContact


        public static CompanyContactViewModel ToCompanyContactViewModel(this Account entity)
        {
            return Mapper.Map<Account, CompanyContactViewModel>(entity);
        }

        #endregion

        #region Incident related
        public static IncidentReportTemplateModel ToModel(this IncidentReportTemplate entity)
        {
            return Mapper.Map<IncidentReportTemplate, IncidentReportTemplateModel>(entity);
        }
        #endregion

        #region Employee Scheduling
        public static EmployeeAvailabilityModel ToModel(this EmployeeAvailability entity)
        {
            return Mapper.Map<EmployeeAvailability, EmployeeAvailabilityModel>(entity);
        }
        public static EmployeeAvailability ToEntity(this EmployeeAvailabilityModel model)
        {
            return Mapper.Map<EmployeeAvailabilityModel, EmployeeAvailability>(model);
        }
        #endregion

        public static DailyAttendanceListModel ToDailyTimeSheetModel(this DailyAttendanceList entity)
        {
            var model = Mapper.Map<DailyAttendanceList, DailyAttendanceListModel>(entity);
            return model;
        }
    }
}
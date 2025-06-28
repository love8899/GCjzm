using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.Common;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Models.CompanyBilling;
using Wfm.Admin.Validators.JobOrder;
using Wfm.Shared.Models.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.JobOrder
{
 
    [Validator(typeof(JobOrderValidator))]
    public partial class JobOrderWithCompanyAddressModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public int? CompanyDepartmentId { get; set; }

        [WfmResourceDisplayName("Common.Contact")]
        public int? CompanyContactId { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.CompanyJobNumber")]
        public string CompanyJobNumber { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [WfmResourceDisplayName("Common.StartTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? StartTime { get; set; }

        [WfmResourceDisplayName("Common.EndTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.HiringDurationExpiredDate")]
        public DateTime? HiringDurationExpiredDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.EstimatedFinishingDate")]
        public DateTime? EstimatedFinishingDate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.EstimatedMargin")]
        public string EstimatedMargin { get; set; }

        [WfmResourceDisplayName("Common.SchedulePolicy")]
        public int? SchedulePolicyId { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobOrderTypeId")]
        public int? JobOrderTypeId { get; set; }


        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.Salary")]
        public string Salary { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.OpeningNumber")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int? OpeningNumber { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.OpeningAvailable")]
        public int? OpeningAvailable { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.ShiftSchedule")]
        public string ShiftSchedule { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.Supervisor")]
        public string Supervisor { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.HoursPerWeek")]
        public decimal? HoursPerWeek { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafeEquipment")]
        public bool RequireSafeEquipment { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafetyShoe")]
        public bool RequireSafetyShoe { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int JobOrderStatusId { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobOrderCategoryId")]
        public int JobOrderCategoryId { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.BillingRateCode")]
        public string BillingRateCode { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobDescription")]
        [AllowHtml]
        public string JobDescription { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.AllowSuperVisorModifyWorkTime")]
        public bool AllowSuperVisorModifyWorkTime { get; set; }

        [WfmResourceDisplayName("Common.IsPublished")]
        public bool IsPublished { get; set; }

        [WfmResourceDisplayName("Common.InternalPosting")]
        public bool IsInternalPosting { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Recruiter")]
        public int RecruiterId { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int OwnerId { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }


        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.MaxRate")]
        public decimal? MaxRate { get; set; }

        [WfmResourceDisplayName("Common.BillingRate")]
        public Decimal? BillingRate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.PayRate")]
        public decimal? PayRate { get; set; }

        [WfmResourceDisplayName("Common.OT.BillingRate")]
        public Decimal? OvertimeBillingRate { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.OvertimePayRate")]
        public decimal? OvertimePayRate { get; set; }



        [WfmResourceDisplayName("Common.Recruiter")]
        public string RecruiterName { get; set; }
        [WfmResourceDisplayName("Common.Owner")]
        public string OwnerName { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string CompanyLocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string CompanyDepartmentName { get; set; }
        [WfmResourceDisplayName("Common.FranchiseName")]
        public string FranchiseName { get; set; }



        public string ShiftName { get; set; }
        public string SeName { get; set; }


        [WfmResourceDisplayName("Common.SchedulePolicy")]
        public string SchedulePolicyName { get; set; }

        [WfmResourceDisplayName("Common.AddressLine1")]
        public string AddressLine1 { get; set; }

        public string CityName { get; set; } 


        public virtual IList<CandidateJobOrderModel> CandidateJobOrderModels { get; set; }

        public virtual JobOrderCategoryModel JobOrderCategoryModel { get; set; }
        public virtual ShiftModel ShiftModel { get; set; }

        public virtual JobOrderTypeModel JobOrderTypeModel { get; set; }
        public virtual JobOrderStatusModel JobOrderStatusModel { get; set; }

        public virtual CompanyModel CompanyModel { get; set; }
     
        public virtual AccountModel CompanyContactModel { get; set; }
        public virtual CompanyBillingRateModel CompanyBillingRateModel { get; set; }
        public DateTime? ReferenceDate { get; set; }

    }
}
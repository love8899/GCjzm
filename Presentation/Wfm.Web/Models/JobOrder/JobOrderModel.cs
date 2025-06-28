using System;
using Wfm.Web.Models.Candidate;
using Wfm.Web.Models.Companies;
using Wfm.Web.Models.Franchises;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.JobOrder
{
    public partial class JobOrderModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public int? CompanyDepartmentId { get; set; }

        [WfmResourceDisplayName("Common.Contact")]
        public int? CompanyContactId { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.CompanyJobNumber")]
        public string CompanyJobNumber { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime? StartDate { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? EndDate { get; set; }

        [WfmResourceDisplayName("Common.StartTime")]
        public DateTime? StartTime { get; set; }

        [WfmResourceDisplayName("Common.EndTime")]
        public DateTime? EndTime { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.HiringDurationExpiredDate")]
        public DateTime? HiringDurationExpiredDate { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.EstimatedFinishingDate")]
        public DateTime? EstimatedFinishingDate { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.EstimatedMargin")]
        public string EstimatedMargin { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.SchedulePolicyId")]
        public int? SchedulePolicyId { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.JobOrderTypeId")]
        public int? JobOrderTypeId { get; set; }


        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Salary")]
        public string Salary { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.OpeningNumber")]
        public int? OpeningNumber { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.OpeningAvailable")]
        public int? OpeningAvailable { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.ShiftSchedule")]
        public string ShiftSchedule { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Supervisor")]
        public string Supervisor { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.HoursPerWeek")]
        public decimal? HoursPerWeek { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafeEquipment")]
        public bool RequireSafeEquipment { get; set; }

        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafetyShoe")]
        public bool RequireSafetyShoe { get; set; }

        [WfmResourceDisplayName("Common.JobOrderStatus")]
        public int JobOrderStatusId { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.JobOrderCategoryId")]
        public int JobOrderCategoryId { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.BillingRateCode")]
        public string BillingRateCode { get; set; }

        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.JobDescription")]
        public string JobDescription { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }


        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }


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


        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }


        public string SeName { get; set; }


        public virtual JobOrderCategoryModel JobOrderCategoryModel { get; set; }
        public virtual ShiftModel ShiftModel { get; set; }
        public virtual JobOrderTypeModel JobOrderTypeModel { get; set; }
        public virtual JobOrderStatusModel JobOrderStatusModel { get; set; }
        public virtual CompanyModel CompanyModel { get; set; }
        public virtual CompanyLocationModel CompanyLocationModel { get; set; }
        public virtual FranchiseModel FranchiseModel { get; set; }
    }
}
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    [Validator(typeof(ScheduleJobOrderValidator))]
    public class ScheduleJobOrderModel : BaseWfmEntityModel
    {
        public int JobOrderId { get; set; }
        [WfmResourceDisplayName("Web.JobOrder.JobOrder.Fields.Supervisor")]
        public int? SupervisorId { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

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

        //[WfmResourceDisplayName("Common.SchedulePolicy")]
        //public int? SchedulePolicyId { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobOrderTypeId")]
        //public int? JobOrderTypeId { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.OpeningNumber")]
        //[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        //public int? OpeningNumber { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.OpeningAvailable")]
        //public int? OpeningAvailable { get; set; }

        //[WfmResourceDisplayName("Common.Shift")]
        //public int? ShiftId { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.ShiftSchedule")]
        //public string ShiftSchedule { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.Supervisor")]
        //public string Supervisor { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.HoursPerWeek")]
        //public decimal? HoursPerWeek { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafeEquipment")]
        //public bool RequireSafeEquipment { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafetyShoe")]
        //public bool RequireSafetyShoe { get; set; }

        //[WfmResourceDisplayName("Common.Status")]
        //public int JobOrderStatusId { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobOrderCategoryId")]
        //public int JobOrderCategoryId { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.BillingRateCode")]
        //public string BillingRateCode { get; set; }

        //public int? JobPostingId { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobDescription")]
        //[AllowHtml]
        //public string JobDescription { get; set; }

        //[WfmResourceDisplayName("Common.Note")]
        //public string Note { get; set; }

        //[WfmResourceDisplayName("Common.IsHot")]
        //public bool IsHot { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.AllowSuperVisorModifyWorkTime")]
        //public bool AllowSuperVisorModifyWorkTime { get; set; }
        ////[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.AllowDailyApproval")]
        ////public bool AllowDailyApproval { get; set; }

        //[WfmResourceDisplayName("Common.IsPublished")]
        //public bool IsPublished { get; set; }

        //[WfmResourceDisplayName("Common.InternalPosting")]
        //public bool IsInternalPosting { get; set; }

        //[WfmResourceDisplayName("Common.EnteredBy")]
        //public int EnteredBy { get; set; }

        //[WfmResourceDisplayName("Common.Recruiter")]
        //public int RecruiterId { get; set; }

        //[WfmResourceDisplayName("Common.Owner")]
        //public int OwnerId { get; set; }

        //[WfmResourceDisplayName("Common.FranchiseId")]
        //public int FranchiseId { get; set; }

        //[WfmResourceDisplayName("Common.IsDeleted")]
        //public bool IsDeleted { get; set; }





        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.MaxRate")]
        //public decimal? MaxRate { get; set; }

        //[WfmResourceDisplayName("Common.BillingRate")]
        //public Decimal? BillingRate { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.PayRate")]
        //public decimal? PayRate { get; set; }

        //[WfmResourceDisplayName("Common.OT.BillingRate")]
        //public Decimal? OvertimeBillingRate { get; set; }

        //[WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.OvertimePayRate")]
        //public decimal? OvertimePayRate { get; set; }



        //[WfmResourceDisplayName("Common.Recruiter")]
        //public string RecruiterName { get; set; }
        //[WfmResourceDisplayName("Common.Owner")]
        //public string OwnerName { get; set; }
        //[WfmResourceDisplayName("Common.CompanyName")]
        //public string CompanyName { get; set; }
        //[WfmResourceDisplayName("Common.Location")]
        //public string CompanyLocationName { get; set; }
        //[WfmResourceDisplayName("Common.Department")]
        //public string CompanyDepartmentName { get; set; }
        //[WfmResourceDisplayName("Common.FranchiseName")]
        //public string FranchiseName { get; set; }



        //public string ShiftName { get; set; }
        //public string SeName { get; set; }


        //[WfmResourceDisplayName("Common.SchedulePolicy")]
        //public string SchedulePolicyName { get; set; }

    }
}

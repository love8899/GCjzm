using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Shared.Validators.JobPost;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.JobPosting
{
    [Validator(typeof(JobPostingValidator))]
    public class JobPostingEditModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.Contact")]
        public int CompanyContactId { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.CompanyJobNumber")]
        public string CompanyJobNumber { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.JobDescription")]
        [AllowHtml]
        public string JobDescription { get; set; }
        [WfmResourceDisplayName("Common.Openings")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int NumberOfOpenings { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.HiringDurationExpiredDate")]
        public DateTime? HiringDurationExpiredDate { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.EstimatedFinishingDate")]
        public DateTime? EstimatedFinishingDate { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.EstimatedMargin")]
        public string EstimatedMargin { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [WfmResourceDisplayName("Common.StartTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [WfmResourceDisplayName("Common.EndTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        [WfmResourceDisplayName("Client.JobPosting.JobPostTypeId")]
        public int JobTypeId { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.Salary")]
        public string Salary { get; set; }
        [WfmResourceDisplayName("Client.JobPosting.JobPostStatusId")]
        public int JobPostingStatusId { get; set; }
        [WfmResourceDisplayName("Client.JobPosting.JobPostCategoryId")]
        public int JobCategoryId { get; set; }
        [WfmResourceDisplayName("Common.Position")]
        public int PositionId { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public int ShiftId { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.ShiftSchedule")]
        public string ShiftSchedule { get; set; }
        [WfmResourceDisplayName("Common.SchedulePolicy")]
        public int SchedulePolicyId { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.HoursPerWeek")]
        public decimal? HoursPerWeek { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafeEquipment")]
        public bool RequireSafeEquipment { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.RequireSafetyShoe")]
        public bool RequireSafetyShoe { get; set; }
        [WfmResourceDisplayName("Common.IsPublished")]
        public bool IsPublished { get; set; }
        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }
        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }
        [WfmResourceDisplayName("Common.PublishedOn")]
        public DateTime? PublishedOnUtc { get; set; }
        [WfmResourceDisplayName("Common.IsSubmitted")]
        public bool IsSubmitted { get; set; }
        [WfmResourceDisplayName("Common.SubmittedOn")]
        public DateTime? SubmittedOnUtc { get; set; }

        [WfmResourceDisplayName("Common.SubmittedBy")]
        public int? SubmittedBy { get; set; }
        public string LabourType { get; set; }

        [WfmResourceDisplayName("Common.Monday")]
        public bool MondaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Tuesday")]
        public bool TuesdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Wednesday")]
        public bool WednesdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Thursday")]
        public bool ThursdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Friday")]
        public bool FridaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Saturday")]
        public bool SaturdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Sunday")]
        public bool SundaySwitch { get; set; }
        public bool IncludeHolidays { get; set; }

    }
}

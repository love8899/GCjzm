using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Core.Domain.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Shared.Models.JobPosting
{
    public class JobPostingModel : BaseWfmEntityModel
    {
        public Guid JobPostingGuid { get; set; }
        public Guid CompanyGuid { get; set; }
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int? CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public string DepartmentName { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
        [WfmResourceDisplayName("Common.Type")]
        public int JobTypeId { get; set; }
        [WfmResourceDisplayName("Admin.JobOrder.JobOrder.Fields.CompanyJobNumber")]
        public string CompanyJobNumber { get; set; }
        [WfmResourceDisplayName("Common.Position")]
        public int PositionId { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public int ShiftId { get; set; }
        [WfmResourceDisplayName("Common.SchedulePolicy")]
        public int SchedulePolicyId { get; set; }

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

        [WfmResourceDisplayName("Common.Status")]
        public int JobPostingStatusId { get; set; }
        [WfmResourceDisplayName("Common.Openings")]
        public int NumberOfOpenings { get; set; }

        [WfmResourceDisplayName("Common.IsSubmitted")]
        public bool IsSubmitted { get; set; }
        [WfmResourceDisplayName("Common.SubmittedOn")]
        public DateTime? SubmittedOnUtc { get; set; }

        [WfmResourceDisplayName("Common.IsPublished")]
        public bool IsPublished { get; set; }
        [WfmResourceDisplayName("Common.PublishedOn")]
        public DateTime? PublishedOnUtc { get; set; }

        [WfmResourceDisplayName("Common.SubmittedBy")]
        public int? SubmittedBy { get; set; }

        [WfmResourceDisplayName("Common.SubmittedBy")]
        public string SubmittedByName { get; set; }

        public bool CancelRequestSent { get; set; }

        public virtual DateTime? SubmittedOn
        {
            get { return SubmittedOnUtc.HasValue ? SubmittedOnUtc.Value.ToLocalTime() : (DateTime?)null; }

            set { }
        }

        public virtual DateTime? PublishedOn
        {
            get { return PublishedOnUtc.HasValue ? PublishedOnUtc.Value.ToLocalTime() : (DateTime?)null; }

            set { }
        }
        public string LabourType { get; set; }

        public string LabourTypeName { get { if (LabourType == "DL")return "Direct Labour"; else return "Indirect Labour"; } }

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

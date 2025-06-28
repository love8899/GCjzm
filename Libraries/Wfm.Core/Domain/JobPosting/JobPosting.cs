using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.JobPosting
{
    public class JobPosting : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobPostingGuid { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int CompanyLocationId { get; set; }
        public virtual CompanyLocation CompanyLocation { get; set; }
        public int? CompanyDepartmentId { get; set; }
        public virtual CompanyDepartment CompanyDepartment { get; set; }
        public int CompanyContactId { get; set; }
        //public virtual Account CompanyContact { get; set; }
        public string CompanyJobNumber { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int NumberOfOpenings { get; set; }
        public DateTime? HiringDurationExpiredDate { get; set; }
        public DateTime? EstimatedFinishingDate { get; set; }
        public string EstimatedMargin { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int JobTypeId { get; set; }
        public virtual JobOrderType JobType { get; set; }
        public string Salary { get; set; }
        public int JobPostingStatusId { get; set; }
        public virtual JobOrderStatus JobPostingStatus { get; set; }
        public int JobCategoryId { get; set; }
        public virtual JobOrderCategory JobCategory { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }
        public string ShiftSchedule { get; set; }
        public int SchedulePolicyId { get; set; }
        public virtual Shift SchedulePolicy { get; set; }
        public string BillingRateCode { get { return Position.Name + " / " + Shift.ShiftName; } }
        public decimal? HoursPerWeek { get; set; }
        public string Note { get; set; }
        public bool RequireSafeEquipment { get; set; }
        public bool RequireSafetyShoe { get; set; }
        public bool IsPublished { get; set; }
        public int EnteredBy { get; set; }
        public int FranchiseId { get; set; }
        public virtual Franchise Franchise { get; set; }
        public bool IsDeleted { get; set; }
        public int? CloseReason { get; set; }
        public DateTime? PublishedOnUtc { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? SubmittedOnUtc { get; set; }

        public int? SubmittedBy { get; set; }
        public bool CancelRequestSent { get; set; }
        public virtual Account Account { get; set; }
        public string LabourType { get; set; }

        public bool MondaySwitch { get; set; }
        public bool TuesdaySwitch { get; set; }
        public bool WednesdaySwitch { get; set; }
        public bool ThursdaySwitch { get; set; }
        public bool FridaySwitch { get; set; }
        public bool SaturdaySwitch { get; set; }
        public bool SundaySwitch { get; set; }
        public bool IncludeHolidays { get; set; }

    }
}

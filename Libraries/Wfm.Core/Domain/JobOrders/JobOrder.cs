using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Seo;
using Wfm.Core.Domain.TimeSheet;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Core.Domain.JobOrders
{
    /// <summary>
    /// JobOrder
    /// </summary>
    public class JobOrder : BaseEntity, ISlugSupported
    {
        private ICollection<CandidateJobOrder> _candidateJobOrders;
        private ICollection<JobOrderOpening> _jobOrderOpenings;

        public JobOrder()
        {
           // this.JobOrderGuid = Guid.NewGuid();
            ClientTimeSheetDocuments = new List<ClientTimeSheetDocument>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobOrderGuid { get; set; }
        public int CompanyId { get; set; }
        public int CompanyLocationId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public int CompanyContactId { get; set; }
        public string CompanyJobNumber { get; set; }
        public int? JobPostingId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public DateTime? HiringDurationExpiredDate { get; set;}
        public DateTime? EstimatedFinishingDate { get; set; }
        public string EstimatedMargin { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SchedulePolicyId { get; set; }
        public int JobOrderTypeId { get; set; }
        public string Salary { get; set; }
        public int JobOrderStatusId { get; set; }
        public int JobOrderCategoryId { get; set; }

        public string BillingRateCode { get; set; }
        
        public int ShiftId { get; set; }
        public string ShiftSchedule { get; set; }
        //public string Supervisor { get; set; }
        public decimal? HoursPerWeek { get; set; }
        public string Note { get; set; }
        public bool RequireSafeEquipment { get; set; }
        public bool RequireSafetyShoe { get; set; }
        public bool IsInternalPosting { get; set; }
        public bool IsPublished { get; set; }
        public bool IsHot { get; set; }
        public bool AllowSuperVisorModifyWorkTime { get; set; }
        public bool AllowTimeEntry { get; set; }

        public int RecruiterId { get; set; }
        public int OwnerId { get; set; }
        public int EnteredBy { get; set; }
        public int FranchiseId { get; set; }
        public JobOrderCloseReasonCode? JobOrderCloseReason { get; set; }

        public bool IsDeleted { get; set; }

        public string LabourType { get; set; }
        public int? PositionId { get; set; }

        [NotMapped]
        public bool[] DailySwitches { get { return new bool[] { SundaySwitch, MondaySwitch, TuesdaySwitch, WednesdaySwitch, ThursdaySwitch, FridaySwitch, SaturdaySwitch }; } }

        public bool MondaySwitch { get; set; }
        public bool TuesdaySwitch { get; set; }
        public bool WednesdaySwitch { get; set; }
        public bool ThursdaySwitch { get; set; }
        public bool FridaySwitch { get; set; }
        public bool SaturdaySwitch { get; set; }
        public bool SundaySwitch { get; set; }
        public bool IncludeHolidays { get; set; }

        public decimal? SalaryMax { get; set; }
        public decimal? SalaryMin { get; set; }     
        public decimal? FeeMin { get; set; }
        public decimal? FeeMax { get; set; }
        public decimal? FeePercent { get; set; }


        public decimal? FeeAmount { get; set; }
        public int? FeeTypeId { get; set; }

        public string MonsterPostingId { get; set; }


        public virtual Position Position { get; set; }
        public virtual JobOrderCategory JobOrderCategory { get; set; }
        public virtual JobOrderType JobOrderType { get; set; }
        public virtual JobOrderStatus JobOrderStatus { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Company Company { get; set; }
        public virtual Account CompanyContact { get; set; }
        public virtual FeeType FeeType { get; set; } 

        public virtual List<JobOrderOvertimeRule> JobOrderOvertimeRules { get; set; }

        public virtual ICollection<ClientTimeSheetDocument> ClientTimeSheetDocuments { get; set; }

        public virtual ICollection<CandidateJobOrder> CandidateJobOrders
        {
            get { return _candidateJobOrders ?? (_candidateJobOrders = new List<CandidateJobOrder>()); }
            protected set { _candidateJobOrders = value; }
        }

        public virtual ICollection<JobOrderOpening> JobOrderOpenings
        {
            get { return _jobOrderOpenings ?? (_jobOrderOpenings = new List<JobOrderOpening>()); }
            protected set { _jobOrderOpenings = value; }
        }

        public virtual int OpeningNumber
        {
            get
            {
                var setting = JobOrderOpenings.Where(x => x.StartDate <= DateTime.Today && (!x.EndDate.HasValue || x.EndDate.Value > DateTime.Today)).FirstOrDefault();
                return setting != null ? setting.OpeningNumber : 0;
            }
        }
        public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; }
        public virtual ICollection<CandidateAppliedJobs> CandidateApplied { get; set; }
    }

    public enum JobOrderCloseReasonCode
    {
        Unknown = 0, 
        ClientCancel = 1,
        Cancel = 2,
        ClientFinished = 3,
    }
}
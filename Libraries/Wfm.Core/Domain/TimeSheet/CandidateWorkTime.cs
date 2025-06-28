using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Core.Domain.TimeSheet
{
    /// <summary>
    /// Record One Candidate, Daily, each JobOrder, 
    ///     1. Punch In/Out Time,
    ///     2. Candidate input workhours, 
    ///     3. Employer adjustment for candidate input workhours
    ///     4. The punch time, input workhours is pending for employer confirming. or confirmed by employer.
    ///     5. The punch in/out time loading from working time table. 
    ///     6. After employer confirmed, this record can not be edit.
    ///     7. Pending status, employee can edit his input workhours.
    ///     8. If employer denied this record, employee can edit his workhours, then the status change to pending for confirming.
    /// </summary>
    public class CandidateWorkTime : BaseEntity
    {
        private ICollection<CandidateWorkTimeLog> _workTimeChanges;
        
        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public int CompanyId { get; set; }
        public int CandidateWorkTimeStatusId { get; set; }
        public int ShiftId { get; set; }
        public int CompanyLocationId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public int CompanyContactId { get; set; }
        public DateTime JobStartDateTime { get; set; }
        public DateTime JobEndDateTime { get; set; }

        public int Year { get; set; }
        public int WeekOfYear { get; set; }

        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }

        public string ClockDeviceUid { get; set; }
        public string SmartCardUid { get; set; }
        public string Source { get; set; }

        public decimal LateInTimeInMinutes { get; set; }
        public decimal EarlyOutTimeInMinutes { get; set; }
        public decimal GracePeriodInMinutes { get; set; }

        public bool IsStrictSchedule { get; set; }
        public decimal OvertimeGracePeriodInMinutes { get; set; }

        [NotMapped]
        public decimal MinWorkHoursForMealBreak { get; set; }
        public decimal MealTimeInMinutes { get; set; }
        public decimal BreakTimeInMinutes { get; set; }
        public decimal RoundingIntervalInMinutes { get; set; }
        public decimal TotalAbsenceTimeInMinutes { get; set; }

        public decimal JobOrderDurationInMinutes { get; set; }
        public decimal JobOrderDurationInHours { get; set; }

        public decimal ClockTimeInMinutes { get; set; }
        public decimal ClockTimeInHours { get; set; }

        public decimal LateOutTimeInMinutes { get; set; }
        public decimal LateOutTimeInHours { get; set; }

        public decimal GrossWorkTimeInMinutes { get; set; }
        public decimal GrossWorkTimeInHours { get; set; }
        public decimal NetWorkTimeInMinutes { get; set; }
        public decimal NetWorkTimeInHours { get; set; }

        public decimal AdjustmentInMinutes { get; set; }

        public string Note { get; set; }

        public int EnteredBy { get; set; }
        public int UpdatedBy { get; set; }

        public int ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedOnUtc { get; set; }

        public int FranchiseId { get; set; }

        public int? Payroll_BatchId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the Work Time Status
        /// </summary>
        public CandidateWorkTimeStatus CandidateWorkTimeStatus
        {
            get
            {
                return (CandidateWorkTimeStatus)this.CandidateWorkTimeStatusId;
            }
            set
            {
                this.CandidateWorkTimeStatusId = (int)value;
            }
        }

        public virtual Candidate Candidate { get; set; }
        public virtual JobOrder JobOrder { get; set; }

        public virtual ICollection<CandidateWorkTimeLog> CandidateWorkTimeChanges
        {
            get { return _workTimeChanges ?? (_workTimeChanges = new List<CandidateWorkTimeLog>()); }
            protected set { _workTimeChanges = value; }
        }

        public bool IsLocked { get; set; }

        public int SignatureBy { get; set; }
        public string SignatureByName { get; set; }
        public DateTime? SignatureOnUtc { get; set; }
    }
}

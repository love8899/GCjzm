using System;
using System.Collections.Generic;


namespace Wfm.Core.Domain.TimeSheet 
{
 
    public class EmployeeWorkTimeApproval
    {
        private ICollection<CandidateWorkTimeLog> _workTimeChanges;

        public int Id { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }

        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }
        public int CompanyId { get; set; }

        public int CandidateWorkTimeStatusId { get; set; }

        public Guid CandidateGuid { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

        public string JobTitle { get; set; }
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

        //public int TimeApprovedBy { get; set; }
        //public string TimeApprovedByName { get; set; }
        //public DateTime? TimeApprovedOnUtc { get; set; }

        public int ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedOnUtc { get; set; }

        public int FranchiseId { get; set; }

        public int? Payroll_BatchId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public bool AllowSuperVisorModifyWorkTime { get; set; }
        //public bool AllowDailyApproval { get; set; }

        public int SignatureBy { get; set; }
        public string SignatureByName { get; set; }
        public DateTime? SignatureOnUtc { get; set; }

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

        public virtual ICollection<CandidateWorkTimeLog> CandidateWorkTimeChanges
        {
            get { return _workTimeChanges ?? (_workTimeChanges = new List<CandidateWorkTimeLog>()); }
            protected set { _workTimeChanges = value; }
        }
    }
}

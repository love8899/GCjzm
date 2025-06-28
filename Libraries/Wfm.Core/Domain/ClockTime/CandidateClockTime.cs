using System;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Core.Domain.ClockTime
{
    /// <summary>
    /// Record daily Candidate punch in and out time.
    /// </summary>
    public class CandidateClockTime : BaseEntity
    {
        public string ClockDeviceUid { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyLocationId { get; set; }
        public int RecordNumber { get; set; }
        public string SmartCardUid { get; set; }
        public int? CandidateId { get; set; }
        public string CandidateLastName { get; set; }
        public string CandidateFirstName { get; set; }
        public DateTime ClockInOut { get; set; }
        public string Source { get; set; }
        public string PunchClockFileName { get; set; }
        public int CandidateClockTimeStatusId { get; set; }
        public string Note { get; set; }
        public int EnteredBy { get; set; }
        public int UpdatedBy { get; set; }

        public bool IsRescheduleClockTime { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the Clock Time Status
        /// </summary>
        public CandidateClockTimeStatus CandidateClockTimeStatus
        {
            get
            {
                return (CandidateClockTimeStatus)this.CandidateClockTimeStatusId;
            }
            set
            {
                this.CandidateClockTimeStatusId = (int)value;
            }
        }

        public virtual Candidate Candidate { get; set; }

    }
}
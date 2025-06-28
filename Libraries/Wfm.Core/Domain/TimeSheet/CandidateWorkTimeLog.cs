using System;

namespace Wfm.Core.Domain.TimeSheet
{
    public class CandidateWorkTimeLog
    {
        public int Id { get; set; }
        
        public int CandidateWorkTimeId { get; set; }

        public decimal OriginalHours { get; set; }

        public decimal NewHours { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public virtual CandidateWorkTime CandidateWorkTime { get; set; }
    }
}

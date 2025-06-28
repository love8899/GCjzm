using System;
using Wfm.Core.Domain.Common;


namespace Wfm.Core.Domain.Candidates
{
    public class CandidateAvailability : BaseEntity
    {
        public int CandidateId { get; set; }
        public DateTime Date { get; set; }
        public int ShiftId { get; set; }
        public int TypeId { get; set; }
        public string Note { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual Shift Shift { get; set; }
    }
}

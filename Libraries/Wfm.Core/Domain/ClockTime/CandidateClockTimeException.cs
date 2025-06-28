using System;


namespace Wfm.Core.Domain.ClockTime
{
    public class CandidateClockTimeException
    {
        public int Id { get; set; }
        public int CandidateClockTimeId { get; set; }
        public bool UnknownCandidate { get; set; }
        public bool NotOnboarded { get; set; }
        public bool NotPlaced { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public virtual CandidateClockTime CandidateClockTime { get; set; }
    }
}

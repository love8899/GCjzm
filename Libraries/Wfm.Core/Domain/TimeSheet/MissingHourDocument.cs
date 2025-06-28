namespace Wfm.Core.Domain.TimeSheet
{
    public class MissingHourDocument : BaseEntity
    {
        public int CandidateMissingHourId { get; set; }
        public string FileName { get; set; }
        public byte[] Stream { get; set; }

        public virtual CandidateMissingHour CandidateMissingHour { get; set; }
    }
}

namespace Wfm.Core.Domain.TimeSheet
{
    /// <summary>
    /// Represents a Work Time Status
    /// </summary>
    public enum CandidateWorkTimeStatus
    {
        Matched = 10,
        PendingApproval = 11,
        Voided = 13,
        Processed = 15,
        //TimeApproved = 16,
        Approved = 17,
        Rejected = 19,
    }
    public class CandidateWorkTimeStatusClass
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}

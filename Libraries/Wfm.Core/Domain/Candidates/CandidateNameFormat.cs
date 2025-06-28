namespace Wfm.Core.Domain.Candidates
{
    /// <summary>
    /// Represents the customer name fortatting enumeration
    /// </summary>
    public enum CandidateNameFormat : int
    {
        /// <summary>
        /// Show emails
        /// </summary>
        ShowEmails = 1,
        /// <summary>
        /// Show usernames
        /// </summary>
        ShowUsernames = 2,
        /// <summary>
        /// Show full names
        /// </summary>
        ShowFullNames = 3,
        /// <summary>
        /// Show first name
        /// </summary>
        ShowFirstName = 10
    }
}

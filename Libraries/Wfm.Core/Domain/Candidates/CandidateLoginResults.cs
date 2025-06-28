namespace Wfm.Core.Domain.Candidates
{
    public enum CandidateLoginResults : int
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,
        /// <summary>
        /// Candidate not exist (email or username)
        /// </summary>
        CandidateNotExist = 2,
        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword = 3,
        /// <summary>
        /// Account have not been activated
        /// </summary>
        NotActive = 4,
        /// <summary>
        /// Candidate has been deleted 
        /// </summary>
        Deleted = 5,
        /// <summary>
        /// Candidate not registered 
        /// </summary>
        NotRegistered = 6
    }
}

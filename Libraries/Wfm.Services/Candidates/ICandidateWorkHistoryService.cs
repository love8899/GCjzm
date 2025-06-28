using Wfm.Core;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateWorkHistoryService
    {

        /// <summary>
        /// Gets the candidate workhistory by id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        CandidateWorkHistory GetCandidateWorkHistoryById(int Id);

        /// <summary>
        /// Inserts the candidate work history.
        /// </summary>
        /// <param name="candidateWorkHistory">The candidate work history.</param>
        void InsertCandidateWorkHistory(CandidateWorkHistory candidateWorkHistory);

        /// <summary>
        /// Deletes the candidate work history.
        /// </summary>
        /// <param name="candidateWorkHistory">The candidate work history.</param>
        void DeleteCandidateWorkHistory(CandidateWorkHistory candidateWorkHistory);

        /// <summary>
        /// Updates the candidate work history.
        /// </summary>
        /// <param name="candidateWorkHistory">The candidate work history.</param>
        void UpdateCandidateWorkHistory(CandidateWorkHistory candidateWorkHistory);

        /// <summary>
        /// Gets the candidate work history by candidate id.
        /// </summary>
        /// <param name="CandidateId">The candidate id.</param>
        /// <returns></returns>
        IPagedList<CandidateWorkHistory> GetCandidateWorkHistoryByCandidateId(int CandidateId, int pageIndex = 0, int PageSize = int.MaxValue);

    }
}

using System;
using System.Linq;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Data;
using Wfm.Core;

namespace Wfm.Services.Candidates
{
    public partial class CandidateWorkHistoryService :ICandidateWorkHistoryService
    {

        #region Fileds

        private readonly IRepository<CandidateWorkHistory> _candidateWorkHistoryRepository;

        #endregion

        #region Cotr

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateWorkHistoryService"/> class.
        /// </summary>
        /// <param name="candidateWorkHistoryRepository">The candidate work history repository.</param>
        public CandidateWorkHistoryService(IRepository<CandidateWorkHistory> candidateWorkHistoryRepository)
        {
            _candidateWorkHistoryRepository = candidateWorkHistoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts the candidate work history.
        /// </summary>
        /// <param name="candidateWorkHistory">The candidate work history.</param>
        /// <exception cref="System.ArgumentNullException">candidate</exception>
        public void InsertCandidateWorkHistory(CandidateWorkHistory candidateWorkHistory)
        {
            if (candidateWorkHistory == null)
                throw new ArgumentNullException("candidate");

            //insert
            _candidateWorkHistoryRepository.Insert(candidateWorkHistory);
        }

        /// <summary>
        /// Deletes the candidate work history.
        /// </summary>
        /// <param name="candidateWorkHistory">The candidate work history.</param>
        public void DeleteCandidateWorkHistory(CandidateWorkHistory candidateWorkHistory)
        {
            _candidateWorkHistoryRepository.Delete(candidateWorkHistory);
        }

        /// <summary>
        /// Updates the candidate work history.
        /// </summary>
        /// <param name="candidateWorkHistory">The candidate work history.</param>
        public void UpdateCandidateWorkHistory(CandidateWorkHistory candidateWorkHistory)
        {
            if (candidateWorkHistory != null)
            _candidateWorkHistoryRepository.Update(candidateWorkHistory);
        }

        /// <summary>
        /// Gets the CandidateWorkHistory by id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns>CandidateWorkHistory </returns>
        public CandidateWorkHistory GetCandidateWorkHistoryById(int Id)
        {
            return _candidateWorkHistoryRepository.GetById(Id);
        }

        /// <summary>
        /// Gets the candidate workhistory by candidateId.
        /// </summary>
        /// <param name="CandidateId">The candidate id.</param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public IPagedList<CandidateWorkHistory> GetCandidateWorkHistoryByCandidateId(int CandidateId, int pageIndex = 0, int PageSize = int.MaxValue)
        {
            IPagedList<CandidateWorkHistory> _candidateWorkHistory = null;

            var query = _candidateWorkHistoryRepository.Table.Where(b => b.CandidateId == CandidateId);
            var total = _candidateWorkHistoryRepository.Table.Where(b => b.CandidateId == CandidateId).Count();
            query = query.OrderByDescending(b => b.StartDate);

            _candidateWorkHistory = new PagedList<CandidateWorkHistory>(query, pageIndex, PageSize, total);

            return _candidateWorkHistory;
        }

        #endregion
    }
}

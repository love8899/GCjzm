using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Caching;

namespace Wfm.Services.Candidates
{
    public partial class CandidateJobOrderStatusService : ICandidateJobOrderStatusService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : ID
        /// </remarks>
        private const string CANDIDATEJOBORDERSTATUS_BY_ID_KEY = "Wfm.candidatejoborderstatus.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string CANDIDATEJOBORDERSTATUS_ALL_KEY = "Wfm.candidatejoborderstatus.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CANDIDATEJOBORDERSTATUS_PATTERN_KEY = "Wfm.candidatejoborderstatus.";

        #endregion

        #region Fields

        private readonly IRepository<CandidateJobOrderStatus> _candidateJobOrderStatusRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateJobOrderStatusService"/> class.
        /// </summary>
        /// <param name="candidateJobOrderStatusRepository">The candidate job order status repository.</param>
        public CandidateJobOrderStatusService(
            IRepository<CandidateJobOrderStatus> candidateJobOrderStatusRepository,
            ICacheManager cacheManager
            )
        {
            _candidateJobOrderStatusRepository = candidateJobOrderStatusRepository;
            _cacheManager = cacheManager;
        }

        #endregion


        #region CRUD

        /// <summary>
        /// Inserts the candidate jobOrder status.
        /// </summary>
        /// <param name="candidateJobOrderStatus">The candidate job order status.</param>
        /// <exception cref="System.ArgumentNullException">candidateJobOrderStatus</exception>
        public void InsertCandidateJobOrderStatus(CandidateJobOrderStatus candidateJobOrderStatus)
        {
            if (candidateJobOrderStatus == null)
                throw new ArgumentNullException("candidateJobOrderStatus");

            _candidateJobOrderStatusRepository.Insert(candidateJobOrderStatus);

            //cache
            _cacheManager.RemoveByPattern(CANDIDATEJOBORDERSTATUS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the candidate jobOrder status.
        /// </summary>
        /// <param name="candidateJobOrderStatus">The candidate job order status.</param>
        /// <exception cref="System.ArgumentNullException">candidateJobOrderStatus</exception>
        public void UpdateCandidateJobOrderStatus(CandidateJobOrderStatus candidateJobOrderStatus)
        {
            if (candidateJobOrderStatus == null)
                throw new ArgumentNullException("candidateJobOrderStatus");

            _candidateJobOrderStatusRepository.Update(candidateJobOrderStatus);

            //cache
            _cacheManager.RemoveByPattern(CANDIDATEJOBORDERSTATUS_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes the candidate jobOrder status.
        /// </summary>
        /// <param name="candidateJobOrderStatus">The candidate job order status.</param>
        public void DeleteCandidateJobOrderStatus(CandidateJobOrderStatus candidateJobOrderStatus)
        {
            if (candidateJobOrderStatus == null)
                throw new ArgumentNullException("candidateJobOrderStatus");

            candidateJobOrderStatus.IsDeleted = true;
            UpdateCandidateJobOrderStatus(candidateJobOrderStatus);

            //cache
            _cacheManager.RemoveByPattern(CANDIDATEJOBORDERSTATUS_PATTERN_KEY);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the candidate jobOrderStatus by Id.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>CandidateJobOrderStatus</returns>
        public CandidateJobOrderStatus GetCandidateJobOrderStatusById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _candidateJobOrderStatusRepository.GetById(id);

            // Using caching
            string key = string.Format(CANDIDATEJOBORDERSTATUS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _candidateJobOrderStatusRepository.GetById(id));
        }

        /// <summary>
        /// Gets all candidate jobOrder status.
        /// </summary>
        /// <returns>IList CandidateJobOrderStatus</returns>
        public IList<CandidateJobOrderStatus> GetAllCandidateJobOrderStatus(bool showInactive = false)
        {
            //no caching
            //-----------------------------
            //var query = _candidateJobOrderStatusRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            ////if (!showHidden)
            ////    query = query.Where(c => c.IsDeleted == false);

            //query = from b in query
            //        orderby b.DisplayOrder, b.StatusName
            //        select b;

            //return query.ToList();



            //using caching
            //-----------------------------
            string key = string.Format(CANDIDATEJOBORDERSTATUS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _candidateJobOrderStatusRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                //if (!showHidden)
                //    query = query.Where(c => c.IsDeleted == false);

                query = from b in query
                        orderby b.DisplayOrder, b.StatusName
                        select b;

                return query.ToList();
            });
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Data;
using Wfm.Core.Caching;

namespace Wfm.Services.JobOrders
{
    public partial class JobOrderStatusService : IJobOrderStatusService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : joborderstatus ID
        /// </remarks>
        private const string JOBORDERSTATUS_BY_ID_KEY = "Wfm.joborderstatus.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string JOBORDERSTATUS_ALL_KEY = "Wfm.joborderstatus.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string JOBORDERSTATUS_PATTERN_KEY = "Wfm.joborderstatus.";

        #endregion

        #region Fields

        private readonly IRepository<JobOrderStatus> _jobOrderStatusRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public JobOrderStatusService(
            IRepository<JobOrderStatus> jobOrderStatusRepository,
            ICacheManager cacheManager
            )
        {
            _jobOrderStatusRepository = jobOrderStatusRepository;
            _cacheManager = cacheManager;
        }

        #endregion


        #region CRUD

        public void InsertJobOrderStatus(JobOrderStatus jobOrderStatus)
        {
            if (jobOrderStatus == null)
                throw new ArgumentNullException("jobOrderStatus");

            //insert
            _jobOrderStatusRepository.Insert(jobOrderStatus);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERSTATUS_PATTERN_KEY);
        }

        public void UpdateJobOrderStatus(JobOrderStatus jobOrderStatus)
        {
            if (jobOrderStatus == null)
                throw new ArgumentNullException("jobOrderStatus");

            _jobOrderStatusRepository.Update(jobOrderStatus);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERSTATUS_PATTERN_KEY);
        }

        public void DeleteJobOrderStatus(JobOrderStatus jobOrderStatus)
        {
            if (jobOrderStatus == null)
                throw new ArgumentNullException("jobOrderStatus");

            _jobOrderStatusRepository.Delete(jobOrderStatus);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERSTATUS_PATTERN_KEY);
        }

        #endregion

        #region JobOrderStatus

        public JobOrderStatus GetJobOrderStatusById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _jobOrderStatusRepository.GetById(id);

            // Using caching
            string key = string.Format(JOBORDERSTATUS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _jobOrderStatusRepository.GetById(id));
        }

        public int? GetJobOrderStatusIdByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            int result = _jobOrderStatusRepository.Table.Where(x => x.JobOrderStatusName == name && x.IsActive && !x.IsDeleted).Select(x=>x.Id).FirstOrDefault();
            return result;
        }
        #endregion

        #region LIST

        public IList<JobOrderStatus> GetAllJobOrderStatus(bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //var query = _jobOrderStatusRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = from b in query
            //        orderby b.DisplayOrder, b.JobOrderStatusName
            //        select b;

            //return query.ToList();


            //using cache
            //-----------------------------
            string key = string.Format(JOBORDERSTATUS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _jobOrderStatusRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = from b in query
                        orderby b.DisplayOrder, b.JobOrderStatusName
                        select b;

                return query.ToList();
            });
        }

        #endregion
    }
}

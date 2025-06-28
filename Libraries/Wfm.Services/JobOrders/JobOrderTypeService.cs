using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Data;
using Wfm.Core.Caching;

namespace Wfm.Services.JobOrders
{
    public partial class JobOrderTypeService : IJobOrderTypeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : jobordertype ID
        /// </remarks>
        private const string JOBORDERTYPES_BY_ID_KEY = "Wfm.jobordertype.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string JOBORDERTYPES_ALL_KEY = "Wfm.jobordertype.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string JOBORDERTYPES_PATTERN_KEY = "Wfm.jobordertype.";

        #endregion

        #region Fields

        private readonly IRepository<JobOrderType> _jobOrderTypeRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public JobOrderTypeService(
            IRepository<JobOrderType> jobOrderTypeRepository,
            ICacheManager cacheManager
            )
        {
            _jobOrderTypeRepository = jobOrderTypeRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region CRUD

        public void InsertJobOrderType(JobOrderType jobOrderType)
        {
            if (jobOrderType == null)
                throw new ArgumentNullException("jobOrderType");

            //insert
            _jobOrderTypeRepository.Insert(jobOrderType);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERTYPES_PATTERN_KEY);
        }

        public void UpdateJobOrderType(JobOrderType jobOrderType)
        {
            if (jobOrderType == null)
                throw new ArgumentNullException("jobOrderType");

            _jobOrderTypeRepository.Update(jobOrderType);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERTYPES_PATTERN_KEY);
        }

        public void DeleteJobOrderType(JobOrderType jobOrderType)
        {
            if (jobOrderType == null)
                throw new ArgumentNullException("jobOrderType");

            _jobOrderTypeRepository.Delete(jobOrderType);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERTYPES_PATTERN_KEY);
        }

        #endregion

        #region Method

        public JobOrderType GetJobOrderTypeById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _jobOrderTypeRepository.GetById(id);

            // Using caching
            string key = string.Format(JOBORDERTYPES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _jobOrderTypeRepository.GetById(id));
        }

        public JobOrderType GetJobOrderTypeByTypeName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;
            var result = _jobOrderTypeRepository.TableNoTracking.Where(x => x.JobOrderTypeName == name).FirstOrDefault();
            return result;
        }
        #endregion

        #region LIST

        public virtual IList<JobOrderType> GetAllJobOrderTypes(bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //var query = _jobOrderTypeRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = from b in query
            //        orderby b.DisplayOrder, b.JobOrderTypeName
            //        select b;

            //return query.ToList();


            //using cache
            //-----------------------------
            string key = string.Format(JOBORDERTYPES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _jobOrderTypeRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = from b in query
                        orderby b.DisplayOrder, b.JobOrderTypeName
                        select b;

                return query.ToList();
            });
        }

        #endregion
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Data;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.JobOrders
{
    public partial class JobOrderCategoryService : IJobOrderCategoryService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : jobordercategory ID
        /// </remarks>
        private const string JOBORDERCATEGORIES_BY_ID_KEY = "Wfm.jobordercategory.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string JOBORDERCATEGORIES_ALL_KEY = "Wfm.jobordercategory.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string JOBORDERCATEGORIES_PATTERN_KEY = "Wfm.jobordercategory.";

        #endregion

        #region Fields

        private readonly IRepository<JobOrderCategory> _jobOrderCategoryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public JobOrderCategoryService(
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<JobOrderCategory> jobOrderCategoryRepository
        )
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _jobOrderCategoryRepository = jobOrderCategoryRepository;
        }

        #endregion


        #region CRUD

        public void InsertJobOrderCategory(JobOrderCategory jobOrderCategory)
        {
            if (jobOrderCategory == null)
                throw new ArgumentNullException("jobOrderCategory");

            _jobOrderCategoryRepository.Insert(jobOrderCategory);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERCATEGORIES_PATTERN_KEY);
        }

        public void UpdateJobOrderCategory(JobOrderCategory jobOrderCategory)
        {
            if (jobOrderCategory == null)
                throw new ArgumentNullException("jobOrderCategory");

            _jobOrderCategoryRepository.Update(jobOrderCategory);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERCATEGORIES_PATTERN_KEY);
        }

        public void DeleteJobOrderCategory(JobOrderCategory jobOrderCategory)
        {
            if (jobOrderCategory == null)
                throw new ArgumentNullException("jobOrderCategory");

            _jobOrderCategoryRepository.Delete(jobOrderCategory);

            //cache
            _cacheManager.RemoveByPattern(JOBORDERCATEGORIES_PATTERN_KEY);
        }

        #endregion

        #region JobOrderCategory

        public JobOrderCategory GetJobOrderCategoryById(int id)
        {
            if (id == 0)
                return null;

            return _jobOrderCategoryRepository.GetById(id);
        }
        
        #endregion

        #region LIST

        public IList<JobOrderCategory> GetAllJobOrderCategories(bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //var query = _jobOrderCategoryRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(jo => jo.IsDeleted == false);


            //query = from j in query
            //        orderby j.DisplayOrder, j.CategoryName
            //        select j;

            //return query.ToList();


            //using cache
            //-----------------------------
            string key = string.Format(JOBORDERCATEGORIES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _jobOrderCategoryRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(jo => jo.IsDeleted == false);


                query = from j in query
                        orderby j.DisplayOrder, j.CategoryName
                        select j;

                return query.ToList();
            });
        }

        public IQueryable<JobOrderCategory> GetAllJobOrderCategoriesAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            var query = _jobOrderCategoryRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(jo => jo.IsDeleted == false);

            query = from j in query
                    orderby j.DisplayOrder, j.CategoryName
                    select j;

            return query.AsQueryable();
        }

        #endregion

    }
}

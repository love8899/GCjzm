using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.Common
{
    public partial class IntersectionService : IIntersectionService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string INTERSECTIONS_ALL_KEY = "Wfm.intersection.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string INTERSECTIONS_PATTERN_KEY = "Wfm.intersection.";

        #endregion

        #region Fields

        private readonly IRepository<Intersection> _intersectionRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public IntersectionService(ICacheManager cacheManager,
            IRepository<Intersection> intersectionRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _intersectionRepository = intersectionRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion
        

        #region CRUD

        /// <summary>
        /// Inserts the intersection.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        /// <exception cref="System.ArgumentNullException">intersection is null</exception>
        public void InsertIntersection(Intersection intersection)
        {
            if (intersection == null)
                throw new ArgumentNullException("intersection");

            _intersectionRepository.Insert(intersection);

            //cache
            _cacheManager.RemoveByPattern(INTERSECTIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the intersection.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        /// <exception cref="System.ArgumentNullException">intersection</exception>
        public void UpdateIntersection(Intersection intersection)
        {
            if (intersection == null)
                throw new ArgumentNullException("intersection");

            _intersectionRepository.Update(intersection);

            //cache
            _cacheManager.RemoveByPattern(INTERSECTIONS_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the intersection.
        /// </summary>
        /// <param name="intersection">The intersection.</param>
        /// <exception cref="System.ArgumentNullException">intersection</exception>
        public void DeleteIntersection(Intersection intersection)
        {
            if (intersection == null)
                throw new ArgumentNullException("intersection");

            intersection.IsActive = false;
            _intersectionRepository.Update(intersection);

            //cache
            _cacheManager.RemoveByPattern(INTERSECTIONS_PATTERN_KEY);
        }

        #endregion

        #region Intersection

        /// <summary>
        /// Gets the intersection by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Intersection GetIntersectionById(int id)
        {
            if (id == 0)
                return null;

            return _intersectionRepository.GetById(id);
        }


        public int GetIntersectionIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var xsection = _intersectionRepository.Table.Where(x => x.IntersectionName.ToLower().Contains(name.ToLower())).FirstOrDefault();

            return xsection != null ? xsection.Id : 0;
        }


        #endregion

        #region LIST

        /// <summary>
        /// Gets all intersections. Use to save into DropDownList
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public List<Intersection> GetAllIntersections(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(INTERSECTIONS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _intersectionRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.IntersectionName
                        select s;

                return query.ToList();
            });
        }

        #endregion

    }
}

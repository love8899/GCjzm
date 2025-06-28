using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public partial class TransportationService : ITransportationService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string TRANSPORTATIONS_ALL_KEY = "Wfm.transportation.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TRANSPORTATIONS_PATTERN_KEY = "Wfm.transportation.";

        #endregion

        #region Fields

        private readonly IRepository<Transportation> _transportationRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public TransportationService(ICacheManager cacheManager,
            IRepository<Transportation> transportationRepository)
        {
            _cacheManager = cacheManager;
            _transportationRepository = transportationRepository;
        }

        #endregion


        #region CRUD

        /// <summary>
        /// Inserts the transportation.
        /// </summary>
        /// <param name="transportation">The transportation.</param>
        /// <exception cref="System.ArgumentNullException">transportation is null</exception>
        public void InsertTransportation(Transportation transportation)
        {
            if (transportation == null)
                throw new ArgumentNullException("transportation");

            _transportationRepository.Insert(transportation);

            //cache
            _cacheManager.RemoveByPattern(TRANSPORTATIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the transportation.
        /// </summary>
        /// <param name="transportation">The transportation.</param>
        /// <exception cref="System.ArgumentNullException">transportation</exception>
        public void UpdateTransportation(Transportation transportation)
        {
            if (transportation == null)
                throw new ArgumentNullException("transportation");

            _transportationRepository.Update(transportation);

            //cache
            _cacheManager.RemoveByPattern(TRANSPORTATIONS_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the transportation.
        /// </summary>
        /// <param name="transportation">The transportation.</param>
        /// <exception cref="System.ArgumentNullException">transportation</exception>
        public void DeleteTransportation(Transportation transportation)
        {
            if (transportation == null)
                throw new ArgumentNullException("transportation");

            transportation.IsActive = false;
            _transportationRepository.Update(transportation);

            //cache
            _cacheManager.RemoveByPattern(TRANSPORTATIONS_PATTERN_KEY);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the transportation by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Transportation GetTransportationById(int id)
        {
            if (id == 0)
                return null;

            return _transportationRepository.GetById(id);
        }

        public int GetTransportationIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var xportation = _transportationRepository.Table.Where(x => x.TransportationName.ToLower() == name.ToLower()).FirstOrDefault();

            return xportation != null ? xportation.Id : 0;
        }

        #endregion

        #region LIST

        /// <summary>
        /// Gets all transportations. Use to save into DropDownList
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public List<Transportation> GetAllTransportations(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(TRANSPORTATIONS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _transportationRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.TransportationName
                        select s;

                return query.ToList();
            });
        }

        #endregion

    }
}

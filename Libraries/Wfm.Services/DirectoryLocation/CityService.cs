using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.DirectoryLocation
{
    public partial class CityService : ICityService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : city ID
        /// </remarks>
        private const string CITIES_BY_ID_KEY = "Wfm.city.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string CITIES_ALL_KEY = "Wfm.city.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CITIES_PATTERN_KEY = "Wfm.city.";

        #endregion

        #region Fields

        private readonly IRepository<City> _cityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public CityService(ICacheManager cacheManager,
            IRepository<City> cityRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._cityRepository = cityRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region CRUD

        public void InsertCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            //insert
            _cityRepository.Insert(city);

            //cache
            _cacheManager.RemoveByPattern(CITIES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(city);
        }

        public void UpdateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Update(city);

            //cache
            _cacheManager.RemoveByPattern(CITIES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(city);
        }

        public virtual void DeleteCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _cityRepository.Delete(city);


            //cache
            _cacheManager.RemoveByPattern(CITIES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(city);
        }


        #endregion

        #region City

        public City GetCityById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _cityRepository.GetById(id);

            // Using caching
            string key = string.Format(CITIES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _cityRepository.GetById(id));
        }

        public int GetCityIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var city = _cityRepository.Table.Where(x => x.CityName.ToLower() == name.ToLower()).FirstOrDefault();
            
            return city != null ? city.Id : 0;
        } 

        #endregion

        #region LIST

        public IList<City> GetAllCities(bool showInactive = false, bool showHidden = false)
        {
            var query = _cityRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = from c in query
                    orderby c.DisplayOrder, c.CityName
                    select c;

            return query.ToList();
        }

        public IList<City> GetAllCitiesByStateProvinceId(int stateProvinceId, bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //if (stateProvinceId == 0)
            //    return null;

            //var query = _cityRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = query.Where(c => c.StateProvinceId == stateProvinceId);

            //query = from c in query
            //        orderby c.DisplayOrder, c.CityName, c.CreatedOnUtc descending
            //        select c;

            //return query.ToList();



            //using cache
            //-----------------------------
            string key = string.Format(CITIES_ALL_KEY, stateProvinceId);
            return _cacheManager.Get(key, () =>
            {
                var query = _cityRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                // province city
                query = query.Where(c => c.StateProvinceId == stateProvinceId);

                query = from s in query
                        orderby s.DisplayOrder, s.CityName
                        select s;

                var cities = query.ToList();
                return cities;
            });
        }


        /// <summary>
        /// Gets all cities asynchronous queryable.
        /// </summary>
        /// <returns></returns>
        public IQueryable<City> GetAllCitiesAsQueryable(bool showHidden =  false)
        {
            var query = _cityRepository.Table;

            if (!showHidden)
                query = query.Where(c => !c.IsDeleted);

            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.CityName);

            query = from c in query
                        select c;

            return query.AsQueryable();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Services.Events;

namespace Wfm.Services.DirectoryLocation
{
    public partial class CountryService : ICountryService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// </remarks>
        private const string COUNTRIES_BY_ID_KEY = "Wfm.country.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string COUNTRIES_ALL_KEY = "Wfm.country.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string COUNTRIES_PATTERN_KEY = "Wfm.country.";

        #endregion
        
        #region Fields

        private readonly IRepository<Country> _countryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public CountryService(ICacheManager cacheManager,
            IRepository<Country> countryRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._countryRepository = countryRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region CRUD

        public void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            //insert
            _countryRepository.Insert(country);

            //cache
            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(country);
        }

        public void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Update(country);

            //cache
            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(country);

        }

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void DeleteCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Delete(country);

            //cache
            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(country);
        }

        #endregion

        #region Country

        public Country GetCountryById(int countryId)
        {
            if (countryId == 0)
                return null;

            // No caching
            //return _countryRepository.GetById(countryId);

            // Using caching
            string key = string.Format(COUNTRIES_BY_ID_KEY, countryId);
            return _cacheManager.Get(key, () => _countryRepository.GetById(countryId));
        }

        public int GetCountryIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var country = _countryRepository.Table.Where(x => x.CountryName.ToLower() == name.ToLower()).FirstOrDefault();
            
            return country != null ? country.Id : 0;
        }

        #endregion


        #region LIST

        public IList<Country> GetAllCountries(bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------------
            //var query = _countryRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = from c in query
            //        orderby c.DisplayOrder, c.CountryName, c.CreatedOnUtc descending
            //        select c;

            //return query.ToList();



            //using cache
            //-----------------------------
            string key = string.Format(COUNTRIES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _countryRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.CountryName
                        select s;

                var countries = query.ToList();
                return countries;
            });
        }


        public virtual IList<Country> GetCountriesByIds(int[] countryIds)
        {
            if (countryIds == null || countryIds.Length == 0)
                return new List<Country>();

            var query = from c in _countryRepository.Table
                        where countryIds.Contains(c.Id)
                        select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<Country>();
            foreach (int id in countryIds)
            {
                var country = countries.Find(x => x.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }


        public IQueryable<Country> GetAllCountriesAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            var query = _countryRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => !c.IsDeleted);


            query = from c in query
                    orderby c.DisplayOrder, c.CountryName, c.CreatedOnUtc descending
                    select c;

            return query.AsQueryable();
        }

   
        public  IList<Country> GetCountriesByTwoLetterCode(string[] countryCodes)
        {
            if (countryCodes == null || countryCodes.Length == 0)
                return new List<Country>();

            var query = from c in _countryRepository.Table
                        where countryCodes.Contains(c.TwoLetterIsoCode)
                        select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<Country>();
            foreach (string code in countryCodes)
            {
                var country = countries.Find(x => x.TwoLetterIsoCode == code);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }

        #endregion
    }
}

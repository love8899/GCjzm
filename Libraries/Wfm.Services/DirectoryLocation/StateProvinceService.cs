using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Services.Events;
using System.Data.Entity;
using Wfm.Core;

namespace Wfm.Services.DirectoryLocation
{
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : city ID
        /// </remarks>
        private const string STATEPROVINCES_BY_ID_KEY = "Wfm.stateprovince.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {1} : country ID
        /// </remarks>
        private const string STATEPROVINCES_ALL_KEY = "Wfm.stateprovince.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STATEPROVINCES_PATTERN_KEY = "Wfm.stateprovince.";

        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<StatutoryHoliday> _statutoryHolidayRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public StateProvinceService(
            IWorkContext workContext,
            ICacheManager cacheManager,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<StatutoryHoliday> statutoryHolidayRepository,
            IEventPublisher eventPublisher)
        {
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._stateProvinceRepository = stateProvinceRepository;
            this._statutoryHolidayRepository = statutoryHolidayRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region CRUD

        public void InsertStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            //insert
            _stateProvinceRepository.Insert(stateProvince);

            //cache
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(stateProvince);
        }

        public void UpdateStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Update(stateProvince);

            //cache
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(stateProvince);
        }

        /// <summary>
        /// Deletes a stateProvince
        /// </summary>
        /// <param name="country">StateProvince</param>
        public virtual void DeleteStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Delete(stateProvince);

            //cache
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(stateProvince);
        }

        #endregion

        #region StateProvince

        public StateProvince GetStateProvinceById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _stateProvinceRepository.GetById(id);

            string key = string.Format(STATEPROVINCES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _stateProvinceRepository
                .Table.Where(x => x.Id == id)
                .Include(x => x.StatutoryHolidays).FirstOrDefault());
        }

        public int GetStateProvinceIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var province = _stateProvinceRepository.Table.Where(x => x.StateProvinceName.ToLower() == name.ToLower()).Include(x => x.StatutoryHolidays).FirstOrDefault();
            
            return province != null ? province.Id : 0;
        }

        public StateProvince GetStateProvinceByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return _stateProvinceRepository.Table.Where(x => x.StateProvinceName.ToLower() == name.ToLower()).Include(x => x.StatutoryHolidays).FirstOrDefault();
        }

        #endregion

        #region LIST

        public IList<StateProvince> GetAllStateProvinces(bool showInactive = false, bool showHidden = false)
        {
            var query = _stateProvinceRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = query.Include(x => x.StatutoryHolidays)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.StateProvinceName).ThenByDescending(c => c.CreatedOnUtc);

            return query.ToList();
        }

        public IList<StateProvince> GetAllStateProvincesByCountryId(int countryId, bool showInactive = false, bool showHidden = false)
        {
            //no cache
            //-----------------------
            //var query = _stateProvinceRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = query.Where(c => c.CountryId == countryId);

            //query = from c in query
            //        orderby c.DisplayOrder, c.StateProvinceName, c.CreatedOnUtc descending
            //        select c;

            //return query.ToList();


            //using cache
            //-----------------------
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            return _cacheManager.Get(key, () =>
            {
                var query = _stateProvinceRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = query.Where(x => x.CountryId == countryId).Include(x => x.StatutoryHolidays)
                    .OrderBy(c => c.DisplayOrder).ThenBy(c => c.StateProvinceName).ThenByDescending(c => c.CreatedOnUtc);
                var stateProvinces = query.ToList();

                return stateProvinces;
            });
        }


        /// <summary>
        /// Gets all state provinces asynchronous queryable.
        /// </summary>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IQueryable<StateProvince> GetAllStateProvincesAsQueryable(bool showHidden = false)
        {
            var query = _stateProvinceRepository.Table;

            if (!showHidden)
                query = query.Where(p => !p.IsDeleted);

            query = query.Include(x => x.StatutoryHolidays)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.StateProvinceName).ThenByDescending(c => c.CreatedOnUtc);

            return query.AsQueryable();
        }


        public IQueryable<StatutoryHoliday> GetAllStatutoryHolidays(DateTime? startDate = null, DateTime? endDate = null)
        {
            return _statutoryHolidayRepository.Table
                .Where(x => !startDate.HasValue || x.HolidayDate >= startDate)
                .Where(x => !endDate.HasValue || x.HolidayDate <= endDate);
        }


        public IQueryable<StatutoryHoliday> GetAllStatutoryHolidaysOfStateProvince(int statepProvinceId)
        {
            return _statutoryHolidayRepository.Table.Where(x => x.StateProvinceId == statepProvinceId);
        }

        public StatutoryHoliday GetStatutoryHolidayById(int statutoryHolidayId)
        {
            return _statutoryHolidayRepository.GetById(statutoryHolidayId);
        }

        public void UpdateStatutoryHoliday(StatutoryHoliday entity)
        {
            entity.EnteredBy = this._workContext.CurrentAccount.Id;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _statutoryHolidayRepository.Update(entity);
        }

        public void AddNewStatutoryHoliday(StatutoryHoliday entity)
        {
            entity.EnteredBy = this._workContext.CurrentAccount.Id;
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _statutoryHolidayRepository.Insert(entity);
        }

        public void DeleteStatutoryHoliday(int statutoryHolidayId)
        {
            _statutoryHolidayRepository.Delete(_statutoryHolidayRepository.GetById(statutoryHolidayId));
        }
        #endregion

    }
}

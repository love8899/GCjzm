using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;
using System.Web.Mvc;

namespace Wfm.Services.Common
{
    public class ShiftService : IShiftService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string SHIFTS_ALL_KEY = "Wfm.shift.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SHIFTS_PATTERN_KEY = "Wfm.shift.";

        #endregion

        #region Fields

        private readonly IRepository<Shift> _shiftRepository;
        private readonly ICacheManager _cacheManager;
        #endregion 

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftService"/> class.
        /// </summary>
        /// <param name="shiftRepository">The shift repository.</param>
        public ShiftService(ICacheManager cacheManager,
            IRepository<Shift> shiftRepository)
        {
            
            _cacheManager = cacheManager;
            _shiftRepository = shiftRepository;
        }

        #endregion 

        #region CRUD

        /// <summary>
        /// Inserts the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <exception cref="System.ArgumentNullException">shift is null</exception>
        public void InsertShift(Shift shift)
        {
            if (shift == null)
                throw new ArgumentNullException("shift");

            _shiftRepository.Insert(shift);

            //cache
            _cacheManager.RemoveByPattern(SHIFTS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <exception cref="System.ArgumentNullException">shift</exception>
        public void UpdateShift(Shift shift)
        {
            if (shift == null)
                throw new ArgumentNullException("shift");

            _shiftRepository.Update(shift);

            //cache
            _cacheManager.RemoveByPattern(SHIFTS_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <exception cref="System.ArgumentNullException">shift</exception>
        public void DeleteShift(Shift shift)
        {
            if (shift == null)
                throw new ArgumentNullException("shift");

            shift.IsActive = false;
            shift.IsDeleted = true;
            _shiftRepository.Update(shift);

            //cache
            _cacheManager.RemoveByPattern(SHIFTS_PATTERN_KEY);
        }

        #endregion

        #region Shift

        /// <summary>
        /// Gets the shift by shiftid.
        /// </summary>
        /// <param name="id">The shift id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Shift is null</exception>
        public Shift GetShiftById(int id)
        {
            if (id == 0)
                return null;

            return _shiftRepository.GetById(id);
        }

        public int GetShiftIdByName(string name, int? companyId = 0, bool includeDeleted = true)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var shift = _shiftRepository.Table.Where(x => !companyId.HasValue || x.CompanyId == companyId)
                        .Where(x => includeDeleted || !x.IsDeleted)
                        .Where(x => x.ShiftName.ToLower() == name.ToLower()).FirstOrDefault();

            return shift != null ? shift.Id : 0;
        }

        public bool AnyDuplicate(int id, int companyId, string name, bool includeDeleted = false)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            var query = _shiftRepository.Table.Where(x => x.Id != id)
                        .Where(x => x.CompanyId == companyId)
                        .Where(x => x.ShiftName.ToLower() == name.ToLower())
                        .Where(x => includeDeleted || !x.IsDeleted);

            return query.Any();
        }

        #endregion

        #region LIST

        public IQueryable<Shift> GetAllShiftsAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            var query = _shiftRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(s => s.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(s => s.IsDeleted == false);

            query = from s in query
                    orderby s.DisplayOrder, s.ShiftName
                    select s;

            return query;
        }

        /// <summary>
        /// Gets all shifts.
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<Shift> GetAllShifts(bool showInactive = false, bool showHidden = false, int? companyId = 0)
        {
            //using cache
            //-----------------------------
            string key = string.Format(SHIFTS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = this.GetAllShiftsAsQueryable(showInactive, showHidden).Where(x => !companyId.HasValue || x.CompanyId == companyId);

                return query.ToList();
            });
        }

        public IList<SelectListItem> GetAllShiftsForDropDownList(bool showInactive = false, bool showHidden = false, int? companyId = 0, bool idVal = true)
        {
            var query = this.GetAllShiftsAsQueryable(showInactive, showHidden)
                .Where(x => !companyId.HasValue || x.CompanyId == companyId)
                .Select(x => new SelectListItem()
                {
                    Text = x.ShiftName,
                    Value = idVal ? x.Id.ToString() : x.ShiftName
                });

            return query.ToList();
        }


        public IQueryable<Shift> GetAllShiftsEnableInSchedule()
        {
            return GetAllShiftsAsQueryable().Where(x => x.EnableInSchedule);
        }

        #endregion

    }
}

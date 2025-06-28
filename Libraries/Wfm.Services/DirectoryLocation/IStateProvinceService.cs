using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.DirectoryLocation
{
    public partial interface IStateProvinceService
    {
        #region CRUD

        void InsertStateProvince(StateProvince stateProvince);

        void UpdateStateProvince(StateProvince stateProvince);

        void DeleteStateProvince(StateProvince stateProvince);

        #endregion


        StateProvince GetStateProvinceById(int id);

        int GetStateProvinceIdByName(string name);

        IList<StateProvince> GetAllStateProvinces(bool showInactive = false, bool showHidden = false);

        IList<StateProvince> GetAllStateProvincesByCountryId(int countryId, bool showInactive = false, bool showHidden = false);

        /// <summary>
        /// Gets all state provinces asynchronous queryable.
        /// </summary>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        IQueryable<StateProvince> GetAllStateProvincesAsQueryable(bool showHidden = false);

        IQueryable<StatutoryHoliday> GetAllStatutoryHolidays(DateTime? startDate = null, DateTime? endDate = null);
        IQueryable<StatutoryHoliday> GetAllStatutoryHolidaysOfStateProvince(int statepProvinceId);

        StatutoryHoliday GetStatutoryHolidayById(int statutoryHolidayId);
        void UpdateStatutoryHoliday(StatutoryHoliday entity);
        void AddNewStatutoryHoliday(StatutoryHoliday entity);
        void DeleteStatutoryHoliday(int statutoryHolidayId);
    }
}

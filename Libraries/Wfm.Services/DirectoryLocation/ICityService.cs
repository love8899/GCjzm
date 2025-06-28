using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.DirectoryLocation
{
    public partial interface ICityService
    {

        #region CRUD

        void UpdateCity(City city);

        void InsertCity(City city);

        void DeleteCity(City city);

        #endregion


        City GetCityById(int id);

        int GetCityIdByName(string name);


        IList<City> GetAllCities(bool showInactive = false, bool showHidden = false);

        IList<City> GetAllCitiesByStateProvinceId(int stateProvinceId, bool showInactive = false, bool showHidden = false);

        /// <summary>
        /// Gets all cities asynchronous queryable.
        /// </summary>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        IQueryable<City> GetAllCitiesAsQueryable(bool showHidden = false);

    }
}

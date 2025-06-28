using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.DirectoryLocation
{
    public partial interface ICountryService
    {
        #region CRUD

        void InsertCountry(Country country);

        void UpdateCountry(Country country);

        void DeleteCountry(Country country);

        #endregion


        Country GetCountryById(int countryId);

        int GetCountryIdByName(string name);
        
        IList<Country> GetAllCountries(bool showInactive = false, bool showHidden = false);

        IList<Country> GetCountriesByIds(int[] countryIds);

        IQueryable<Country> GetAllCountriesAsQueryable(bool showInactive = false, bool showHidden = false);

        IList<Country> GetCountriesByTwoLetterCode(string[] countryCodes);
    }
}

using System;
using Wfm.Core;
using Wfm.Core.Domain.Common;
using Wfm.Services.Logging;

namespace Wfm.Services.DirectoryLocation
{
    /// <summary>
    /// GEO lookup service
    /// </summary>
    public partial class MapLookupService : IMapLookupService
    {
        #region Const

        private const string GoogleMapLink = @"http://maps.google.com/maps?q=";
        private const string BingMapLink = @"http://www.bing.com/maps/?v=2&where1=";

        #endregion

        #region Fields

        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;

        #endregion

        #region Ctor

        public MapLookupService(ILogger logger,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            IWebHelper webHelper)
        {
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _logger = logger;
            _webHelper = webHelper;
        }

        #endregion


        #region Helper

        private string GetAddressStr(string addressLine1, string addressLine2, string addressLine3, string city, string stateProvince, string country, string postalCode)
        {
            string whereStr = (String.IsNullOrWhiteSpace(addressLine1) ? "" : addressLine1) +
                (String.IsNullOrWhiteSpace(addressLine2) ? "" : "," + addressLine2) +
                (String.IsNullOrWhiteSpace(addressLine3) ? "" : "," + addressLine3) +
                (String.IsNullOrWhiteSpace(city) ? "" : "," + city) +
                (String.IsNullOrWhiteSpace(stateProvince) ? "" : "," + stateProvince) +
                (String.IsNullOrWhiteSpace(country) ? "" : "," + country) +
                (String.IsNullOrWhiteSpace(postalCode) ? "" : "," + postalCode);

            return whereStr;
        }

        #endregion

        #region Methods

        public string LookupMap(string addressLine1, int cityId, int stateProvinceId, int countryId)
        {
            City city = _cityService.GetCityById(cityId);
            StateProvince stateProvince = _stateProvinceService.GetStateProvinceById(stateProvinceId);
            Country country = _countryService.GetCountryById(countryId);

            //return LookupBingMap(addressLine1, "", "", city == null ? "" : city.CityName, stateProvince == null ? "" : stateProvince.StateProvinceName, country == null ? "" : country.CountryName, "");
            return LookupGoogleMap(addressLine1, "", "", city == null ? "" : city.CityName, stateProvince == null ? "" : stateProvince.StateProvinceName, country == null ? "" : country.CountryName, "");
        }
        public string LookupMap(string addressLine1, string cityName, string stateProvinceName, string countryName)
        {
            //return LookupBingMap(addressLine1, "", "", "", "", cityName, stateProvinceName, countryName, "");
            return LookupGoogleMap(addressLine1, "", "", cityName, stateProvinceName, countryName, "");
        }



        public string LookupGoogleMap(string addressLine1, string addressLine2, string addressLine3, string cityName, string stateProvinceName, string countryName, string postalCode)
        {
            return GoogleMapLink + GetAddressStr(addressLine1, addressLine2, addressLine3, cityName, stateProvinceName, countryName, postalCode);
        }
        public string LookupBingMap(string addressLine1, string addressLine2, string addressLine3, string cityName, string stateProvinceName, string countryName, string postalCode)
        {
            return BingMapLink + GetAddressStr(addressLine1, addressLine2, addressLine3, cityName, stateProvinceName, countryName, postalCode);
        }

        #endregion
    }
}
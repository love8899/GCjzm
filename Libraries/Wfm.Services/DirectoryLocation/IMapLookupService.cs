namespace Wfm.Services.DirectoryLocation
{
    /// <summary>
    /// Map lookup service
    /// </summary>
    public partial interface IMapLookupService
    {
        string LookupMap(string addressLine1, int cityId, int stateProvinceId, int countryId);
        string LookupMap(string addressLine1, string cityName, string stateProvinceName, string countryName);


        string LookupGoogleMap(string addressLine1, string addressLine2, string addressLine3, string cityName, string stateProvinceName, string countryName, string postalCode);
        string LookupBingMap(string addressLine1, string addressLine2, string addressLine3, string cityName, string stateProvinceName, string countryName, string postalCode);
    }
}
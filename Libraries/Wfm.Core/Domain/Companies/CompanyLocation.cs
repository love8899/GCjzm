using System.Collections.Generic;
using Wfm.Core.Domain.Common;


namespace Wfm.Core.Domain.Companies
{
    /// <summary>
    /// Company Location
    /// </summary>
    public class CompanyLocation : BaseEntity
    {
        public int CompanyId { get; set; }
        public string LocationName { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string FaxNumber { get; set; }
        public string UnitNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
        public System.DateTime? LastPunchClockFileUploadDateTimeUtc { get; set; }
        public System.DateTime? LastWorkTimeCalculationDateTimeUtc { get; set; }

        public object Clone()
        {
            var addr = new CompanyLocation()
            {
                CompanyId = this.CompanyId,
                LocationName = this.LocationName,
                PrimaryPhone = this.PrimaryPhone,
                SecondaryPhone = this.SecondaryPhone,
                FaxNumber = this.FaxNumber,
                UnitNumber = this.UnitNumber,
                AddressLine1 = this.AddressLine1,
                AddressLine2 = this.AddressLine2,
                AddressLine3 = this.AddressLine3,
                CityId = this.CityId,
                StateProvinceId = this.StateProvinceId,
                CountryId = this.CountryId,
                PostalCode = this.PostalCode,
                IsActive = this.IsActive,
            };
            return addr;
        }

        public string LocationFullAddress 
        {
            get {
                string cityName = this.City == null ? "" : this.City.CityName;
                string stateProvinceName = this.StateProvince == null ? "" : this.StateProvince.Abbreviation;
                string countryName = this.Country == null ? "" : this.Country.TwoLetterIsoCode;
                return string.Format("{0}, {1}, {2}, {3}, {4}", this.AddressLine1, cityName,stateProvinceName,countryName,PostalCode);
            } 
        }
        public virtual Company Company { get; set; }

        public virtual List<CompanyLocationOvertimeRule> CompanyLocationOvertimeRules { get; set; }

        public virtual City City { get; set; }
        public virtual StateProvince StateProvince { get; set; }
        public virtual Country Country { get; set; }
    }
}

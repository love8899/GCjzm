using Wfm.Core.Domain.Common;
namespace Wfm.Core.Domain.Franchises
{
    public partial class FranchiseAddress : BaseEntity
    {
        public int FranchiseId { get; set; }
        public string LocationName { get; set; }
        public string PrimaryPhone { get; set; }
        public string PrimaryPhoneExtension { get; set; }
        public string SecondaryPhone { get; set; }
        public string SecondaryPhoneExtension { get; set; }
        public string FaxNumber { get; set; }
        public string UnitNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        //public string City { get; set; }
        //public string StateProvince { get; set; }
        //public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHeadOffice { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual City City { get; set; }
        public virtual StateProvince StateProvince { get; set; }
        public virtual Country Country { get; set; }
        public object Clone()
        {
            var addr = new FranchiseAddress()
            {
                FranchiseId = this.FranchiseId,
                LocationName = this.LocationName,
                PrimaryPhone = this.PrimaryPhone,
                PrimaryPhoneExtension = this.PrimaryPhoneExtension,
                SecondaryPhone = this.SecondaryPhone,
                SecondaryPhoneExtension = this.SecondaryPhoneExtension,
                FaxNumber = this.FaxNumber,
                UnitNumber = this.UnitNumber,
                AddressLine1 = this.AddressLine1,
                AddressLine2 = this.AddressLine2,
                AddressLine3 = this.AddressLine3,
                City = this.City,
                StateProvince = this.StateProvince,
                Country = this.Country,
                PostalCode = this.PostalCode,
                Note = this.Note,
                IsActive = this.IsActive,
            };
            return addr;
        }
    }
}

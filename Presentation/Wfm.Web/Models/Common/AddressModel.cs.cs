using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Common
{
    public class AddressModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Common.UnitNumber")]
        public string UnitNumber { get; set; }

        [WfmResourceDisplayName("Common.AddressLine1")]
        public string AddressLine1 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine2")]
        public string AddressLine2 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine3")]
        public string AddressLine3 { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public string City { get; set; }

        [WfmResourceDisplayName("Common.StateProvince")]
        public string StateProvince { get; set; }

        [WfmResourceDisplayName("Common.Country")]
        public string Country { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        public string PostalCode { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public int CityId { get; set; }

        [WfmResourceDisplayName("Common.StateProvince")]
        public int StateProvinceId { get; set; }

        [WfmResourceDisplayName("Common.Country")]
        public int CountryId { get; set; }
    }
}
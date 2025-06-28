using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Common
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
        public int CityId { get; set; }
        [WfmResourceDisplayName("Common.StateProvince")]
        public int StateProvinceId { get; set; }
        [WfmResourceDisplayName("Common.Country")]
        public int CountryId { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        public string PostalCode { get; set; }

    }
}
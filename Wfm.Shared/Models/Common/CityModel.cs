using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Common
{
    public partial class CityModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.StateProvince")]
        public int StateProvinceId { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public string CityName { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        // For UI
        [WfmResourceDisplayName("Common.StateProvince")]
        public virtual string StateProvinceName { get; set; }

        [WfmResourceDisplayName("Common.Country")]
        public virtual int CountryId { get; set; }
        [WfmResourceDisplayName("Common.Country")]
        public virtual string CountryName { get; set; }


        public virtual StateProvinceModel StateProvinceModel { get; set; }
    }
}
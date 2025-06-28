using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Common
{
    public partial class StateProvinceModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Country")]
        public int CountryId { get; set; }

        [WfmResourceDisplayName("Common.StateProvince")]
        public string StateProvinceName { get; set; }

        [WfmResourceDisplayName("Common.Abbreviation")] 
        public string Abbreviation { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        // For UI
        public string CountryName { get; set; }

        public virtual CountryModel CountryModel { get; set; }
    }
}
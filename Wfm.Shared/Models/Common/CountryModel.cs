using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Common
{
    public partial class CountryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Web.Directory.Country.Fields.CountryName")]
        public string CountryName { get; set; }

        [WfmResourceDisplayName("Web.Directory.Country.Fields.TwoLetterIsoCode")]
        public string TwoLetterIsoCode { get; set; }

        [WfmResourceDisplayName("Web.Directory.Country.Fields.ThreeLetterIsoCode")]
        public string ThreeLetterIsoCode { get; set; }

        [WfmResourceDisplayName("Web.Directory.Country.Fields.NumericIsoCode")]
        public string NumericIsoCode { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
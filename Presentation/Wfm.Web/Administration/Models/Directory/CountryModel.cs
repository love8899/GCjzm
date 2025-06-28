using FluentValidation.Attributes;
using Wfm.Admin.Validators.Directory;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Directory
{
    [Validator(typeof(CountryValidator))]
    public partial class CountryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Country.Fields.CountryName")]
        public string CountryName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Country.Fields.TwoLetterIsoCode")]
        public string TwoLetterIsoCode { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Country.Fields.ThreeLetterIsoCode")]
        public string ThreeLetterIsoCode { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Country.Fields.NumericIsoCode")]
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
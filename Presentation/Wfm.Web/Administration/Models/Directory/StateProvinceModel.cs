using FluentValidation.Attributes;
using Wfm.Admin.Validators.Directory;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Directory
{
    [Validator(typeof(StateProvinceValidator))]
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
        [WfmResourceDisplayName("Common.Country")]
        public string CountryName { get; set; }

        public virtual CountryModel CountryModel { get; set; }
        //public List<StatutoryHolidayModel> StatutoryHolidays { get; set; }
    }
}
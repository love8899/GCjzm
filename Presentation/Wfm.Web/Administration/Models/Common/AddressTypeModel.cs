using FluentValidation.Attributes;
using Wfm.Admin.Validators.Common;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    [Validator(typeof(AddressTypeValidator))]
    public class AddressTypeModel :BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TypeName")]
        public string AddressTypeName { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

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
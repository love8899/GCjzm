using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Common
{
    public class TransportationModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Transportation.Fields.TransportationName")]
        public string TransportationName { get; set; }
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
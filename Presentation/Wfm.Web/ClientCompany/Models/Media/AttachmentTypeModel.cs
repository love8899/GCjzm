using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Client.Validators.Media;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Media
{
    [Validator(typeof(AttachmentTypeValidator))]
    public class AttachmentTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TypeName")]
        [AllowHtml]
        public string TypeName { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        [AllowHtml]
        public string Description { get; set; }

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
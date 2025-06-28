using FluentValidation.Attributes;
using Wfm.Admin.Validators.Media;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Media
{
    [Validator(typeof(AttachmentTypeValidator))]
    public class AttachmentTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TypeName")]
        public string TypeName { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.AttachmentType.Fields.FileExtensions")]
        public string FileExtensions { get; set; }

        [WfmResourceDisplayName("Common.Description")]
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
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Media;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Media
{
    [Validator(typeof(DocumentTypeValidator))]
    public class DocumentTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TypeName")]
        public string TypeName { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Code")]
        public string InternalCode { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.TimeClockFile.Fields.FileName")]
        public string FileName { get; set; }

        public bool IsPublic { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
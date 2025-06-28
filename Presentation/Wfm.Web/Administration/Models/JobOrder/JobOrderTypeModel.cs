using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.JobOrder;

namespace Wfm.Admin.Models.JobOrder
{
    [Validator(typeof(JobOrderTypeValidator))]
    public partial class JobOrderTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Type")]
        public string JobOrderTypeName { get; set; }

        //[WfmResourceDisplayName("Common.Note")]
        //public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        public bool IsDirectHire { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
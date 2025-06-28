using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.JobOrder;
using Wfm.Admin.Models.Test;

namespace Wfm.Admin.Models.JobOrder
{
    [Validator(typeof(JobOrderTestCategoryValidator))]
    public partial class JobOrderTestCategoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }

        [WfmResourceDisplayName("Common.TestCategory")]
        public int TestCategoryId { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }


        public virtual TestCategoryModel TestCategoryModel { get; set; }
        public virtual JobOrderModel JobOrderModel { get; set; }

    }
}
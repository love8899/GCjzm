using FluentValidation.Attributes;
using Wfm.Admin.Validators.JobOrder;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.JobOrder
{
    [Validator(typeof(JobOrderCategoryValidator))]
    public partial class JobOrderCategoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.JobOrderCategory.Fields.CategoryName")]
        public string CategoryName { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
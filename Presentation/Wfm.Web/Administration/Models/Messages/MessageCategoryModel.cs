using FluentValidation.Attributes;
using Wfm.Admin.Validators.Messages;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Messages 
{
    [Validator(typeof(MessageCategoryValidator))]
    public class MessageCategoryModel : BaseWfmEntityModel
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
    }
}
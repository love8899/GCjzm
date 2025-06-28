using System.Collections.Generic;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Forums;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Forums
{
    [Validator(typeof(ForumValidator))]
    public partial class ForumModel : BaseWfmEntityModel
    {
        public ForumModel()
        {
            ForumGroups = new List<ForumGroupModel>();
        }

        [WfmResourceDisplayName("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId")]
        public int ForumGroupId { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }



        public List<ForumGroupModel> ForumGroups { get; set; }
    }
}
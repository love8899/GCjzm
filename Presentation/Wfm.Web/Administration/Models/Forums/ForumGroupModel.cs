using System.Collections.Generic;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Forums;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Forums
{
    [Validator(typeof(ForumGroupValidator))]
    public partial class ForumGroupModel : BaseWfmEntityModel
    {
        public ForumGroupModel()
        {
            ForumModels = new List<ForumModel>();
        }

        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }



        //use ForumModel
        public IList<ForumModel> ForumModels { get; set; }
    }
}
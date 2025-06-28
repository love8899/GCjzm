using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Blogs
{
    public partial class AddBlogCommentModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Web.Blog.Comments.CommentText")]
        [AllowHtml]
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
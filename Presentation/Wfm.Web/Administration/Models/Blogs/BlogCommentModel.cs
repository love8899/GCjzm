using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Blogs
{
    public partial class BlogCommentModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.BlogPostId")]
        public int BlogPostId { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.BlogPostTitle")]
        public string BlogPostTitle { get; set; }

        [WfmResourceDisplayName("Common.Account")]
        public int AccountId { get; set; }

        [WfmResourceDisplayName("Common.AccountDetails")]
        public string AccountInfo { get; set; }

        [WfmResourceDisplayName("Common.Comment")]
        public string Comment { get; set; }
    }
}
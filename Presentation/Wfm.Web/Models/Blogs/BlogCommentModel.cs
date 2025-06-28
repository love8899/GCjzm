using System.Web.Mvc;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Blogs
{
    public partial class BlogCommentModel : BaseWfmEntityModel
    {
        //[NopResourceDisplayName("Web.ContentManagement.Blog.Comments.Fields.BlogPostId")]
        public int BlogPostId { get; set; }

        //[NopResourceDisplayName("Web.ContentManagement.Blog.Comments.Fields.BlogPostTitle")]
        [AllowHtml]
        public string BlogPostTitle { get; set; }

        //[NopResourceDisplayName("Web.ContentManagement.Blog.Comments.Fields.AccountId")]
        public int AccountId { get; set; }

        //[NopResourceDisplayName("Web.ContentManagement.Blog.Comments.Fields.AccountInfo")]
        public string AccountInfo { get; set; }

        //[NopResourceDisplayName("Web.ContentManagement.Blog.Comments.Fields.Comment")]
        [AllowHtml]
        public string Comment { get; set; }

    }
}
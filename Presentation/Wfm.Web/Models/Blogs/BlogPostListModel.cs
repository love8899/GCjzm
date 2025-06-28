using System.Collections.Generic;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Blogs
{
    public partial class BlogPostListModel :BaseWfmModel
    {
        public BlogPostListModel()
        {
            PagingFilteringContext = new BlogPagingFilteringModel();
            BlogPosts = new List<BlogPostModel>();
        }

        public int WorkingLanguageId { get; set; }
        public BlogPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<BlogPostModel> BlogPosts { get; set; }
    }
}
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Blogs
{
    public class BlogPostTagModel : BaseWfmModel
    {
        public string Name { get; set; }

        public int BlogPostCount { get; set; }
    }
}
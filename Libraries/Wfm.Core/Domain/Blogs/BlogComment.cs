using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Blogs
{
    public class BlogComment : BaseEntity
    {

        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the blog post identifier
        /// </summary>
        public int BlogPostId { get; set; }


        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// Gets or sets the blog post
        /// </summary>
        public virtual BlogPost BlogPost { get; set; }
    }
}

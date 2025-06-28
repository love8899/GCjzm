using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Localization;
using Wfm.Core.Domain.Seo;

namespace Wfm.Core.Domain.Blogs
{
    public class BlogPost : BaseEntity ,ISlugSupported
    {
        private ICollection<BlogComment> _blogComments;

        public int LanguageId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public bool AllowComments { get; set; }

        public int CommentCount { get; set; }

        public string Tags { get; set; }

        public DateTime? StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }


        public virtual ICollection<BlogComment> BlogComments
        {
            get { return _blogComments ?? (_blogComments = new List<BlogComment>()); }
            protected set { _blogComments = value; }
        }

        public virtual Language Language { get; set; }

        public int EnteredBy { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Blogs;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Blogs
{
    [Validator(typeof(BlogPostValidator))]
    public partial class BlogPostModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.LanguageId")]
        public int LanguageId { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.LanguageName")]
        public string LanguageName { get; set; }

        [WfmResourceDisplayName("Common.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [WfmResourceDisplayName("Common.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.AllowComments")]
        public bool AllowComments { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags")]
        [AllowHtml]
        public string Tags { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Comments")]
        public int Comments { get; set; }

        [UIHint("DateTimeNullable")]
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime? StartDateUtc { get; set; }

        [UIHint("DateTimeNullable")]
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? EndDateUtc { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [WfmResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}
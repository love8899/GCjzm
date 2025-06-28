using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Domain.Blogs;
using Wfm.Services.Blogs;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Wfm.Web.Extensions;
using Wfm.Services.Seo;
using Wfm.Web.Framework.Seo;
using Wfm.Web.Models.Blogs;


namespace Wfm.Web.Controllers
{

    public partial class BlogController : BaseWfmController
    {
        #region Fields

        private readonly IBlogService _blogService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly BlogSettings _blogSettings;


        #endregion

        #region Constructors

        public BlogController(IBlogService blogService,
                              IWorkContext workContext,
                              ILocalizationService localizationService,
                              ICacheManager cacheManager,
                              IDateTimeHelper dateTimeHelper,
                              BlogSettings blogSettings
            )
        {
            this._blogService = blogService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._blogSettings = blogSettings;
        }

        #endregion

        [NonAction]
        protected BlogPostModel PrepareBlogPostModel(BlogPost blogPost, bool prepareComments)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            var model = blogPost.ToModel();

            model.SeName = blogPost.GetSeName(blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            model.Tags = blogPost.ParseTags().ToList();
            model.NumberOfComments = blogPost.CommentCount;

            return model;
        }


        [NonAction]
        protected BlogPostListModel PrepareBlogPostListModel(BlogPagingFilteringModel command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var model = new BlogPostListModel();
            model.PagingFilteringContext.Tag = command.Tag;
            model.PagingFilteringContext.Month = command.Month;
            //model.WorkingLanguageId = _workContext.WorkingLanguage.Id;
            model.WorkingLanguageId = 1;

            if (command.PageSize <= 0)
                command.PageSize = _blogSettings.PostsPageSize > 0 ? _blogSettings.PostsPageSize : 3;
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            //DateTime? dateFrom = command.GetFromMonth();
            //DateTime? dateTo = command.GetToMonth();
            DateTime? dateFrom = DateTime.MinValue;
            DateTime? dateTo = DateTime.MaxValue;

            Core.IPagedList<BlogPost> blogPosts = null; 
            if (String.IsNullOrEmpty(command.Tag))
                blogPosts = _blogService.GetAllBlogPosts(0, 1, dateFrom, dateTo, command.PageNumber - 1, command.PageSize, true);
            else
                blogPosts = _blogService.GetAllBlogPostsByTag(0, 1, command.Tag, command.PageNumber - 1, command.PageSize, true);

            model.PagingFilteringContext.LoadPagedList(blogPosts);
            model.BlogPosts = blogPosts.Select(x => PrepareBlogPostModel(x, false)).ToList();

            return model;
        }


        public ActionResult List(BlogPagingFilteringModel command, int? page = 1)
        {
            return View("List", PrepareBlogPostListModel(command));
        }


        public ActionResult BlogPostSeo(int blogPostId)
        {
            var blogPost = _blogService.GetBlogPostById(blogPostId);

            var model = PrepareBlogPostModel(blogPost, true);

            return RedirectToActionPermanent("BlogPostDetails", new { blogPostId = blogPostId, seoName = SeoHelper.ToSeoUrl(blogPost.Title) });
        }


        public ActionResult BlogPostDetails(int blogPostId, string seoName)
        {

            var blogPost = _blogService.GetBlogPostById(blogPostId);
            var model = new BlogPostModel();
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            if (seoName != SeoHelper.ToSeoUrl(blogPost.Title))
                return RedirectToActionPermanent("BlogPostDetails", new { blogPostId = blogPostId, seoName = SeoHelper.ToSeoUrl(blogPost.Title) });

            //PrepareBlogPostModel(model, blogPost, true);

            model = blogPost.ToModel();
            return View(model);
        }

        //
        // GET: /Blog/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Blog/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Blog/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Blog/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Blog/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Blog/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Blog/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult _LatestBlogs(int pageSize = 3)
        {
            var model = _blogService.GetAllBlogPosts(0, 1, System.DateTime.MinValue, System.DateTime.MaxValue, 0, pageSize, true)
                .Select(x => x.ToModel());

            return PartialView(model);
        }

    }
}

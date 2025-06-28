using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.Blogs;
using Wfm.Core.Domain.Blogs;
using Wfm.Services.Logging;
using Wfm.Services.Blogs;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Controllers
{
    public class BlogController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IBlogService _blogService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public BlogController(IActivityLogService activityLogService,
            IBlogService blogService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _activityLogService = activityLogService;
            _blogService = blogService;
            _languageService = languageService;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion 

        #region GET :/Blog/Index

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #endregion

        #region GET :/Blog/List

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Blog/List

        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPosts = _blogService.GetAllBlogPosts(0, 0, null, null, request.Page - 1, request.PageSize, true);

            var result = new DataSourceResult
            {
                Data = blogPosts.Select(x =>
                {
                    var model = x.ToModel();
                    model.LanguageName = x.Language.Name;
                    model.Comments = x.CommentCount;
                    return model;
                }),
                Total = blogPosts.TotalCount
            };

            return Json(result);
        }

        #endregion 

        #region GET :/Blog/Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var model = new BlogPostModel();
            model.StartDateUtc = System.DateTime.Now;
            model.EndDateUtc = System.DateTime.Now;

            ViewBag.AllLanguages = _languageService.GetAllLanguages();
            model.AllowComments = true;

            return View(model);
        }

        #endregion

        #region POST:/Blog/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(BlogPostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity();

                blogPost.CreatedOnUtc = DateTime.UtcNow;
                blogPost.UpdatedOnUtc = DateTime.UtcNow;

                _blogService.InsertBlogPost(blogPost);


                //activity log
                _activityLogService.InsertActivityLog("AddNewBlogPost",
                    _localizationService.GetResource("ActivityLog.AddNewBlogPost"), model.Title);


                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Added"));
                return continueEditing ? RedirectToAction("Edit", new {id = blogPost.Id}) : RedirectToAction("List");
            }

            return View(model);
        }

        #endregion

        #region GET :/Blog/Edit

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPost = _blogService.GetBlogPostById(id);

            if (blogPost == null) return RedirectToAction("List");

            ViewBag.AllLanguages = _languageService.GetAllLanguages();
            var model = blogPost.ToModel();
            model.StartDateUtc = blogPost.StartDateUtc;
            model.EndDateUtc = blogPost.EndDateUtc;

            return View(model);            
        }

        #endregion

        #region POST:/Blog/Edit

        [HttpPost,ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(BlogPostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPost = _blogService.GetBlogPostById(model.Id);
            if (blogPost == null) return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                blogPost = model.ToEntity(blogPost);
                blogPost.StartDateUtc = model.StartDateUtc;
                blogPost.EndDateUtc = model.EndDateUtc;
                _blogService.UpdateBlogPost(blogPost);
                ViewBag.AllLanguages = _languageService.GetAllLanguages();

                //activity log
                _activityLogService.InsertActivityLog("UpdateBlogPost", _localizationService.GetResource("ActivityLog.UpdateBlogPost"), model.Title);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = blogPost.Id }) : RedirectToAction("List");
            }

            return View(model);
        }

        #endregion

        #region POST:/Blog/Delete

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPost = _blogService.GetBlogPostById(Id);
            if (blogPost == null) return RedirectToAction("List");

            if (blogPost == null)
                //No blog post found with the specified id
                return RedirectToAction("List");

            _blogService.DeleteBlogPost(blogPost);

            //activity log
            _activityLogService.InsertActivityLog("DeleteBlogPost", _localizationService.GetResource("ActivityLog.DeleteBlogPost"), blogPost.Title);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Comments

        public ActionResult Comments(int? filterByBlogPostId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            ViewBag.FilterByBlogPostId = filterByBlogPostId;
            return View();
        }

        [HttpPost]
        public ActionResult Comments(int? filterByBlogPostId, DataSourceRequest command)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            IList<BlogComment> comments;
            if (filterByBlogPostId.HasValue)
            {
                //filter comments by blog
                var blogPost = _blogService.GetBlogPostById(filterByBlogPostId.Value);
                comments = blogPost.BlogComments.OrderBy(bc => bc.CreatedOnUtc).ToList();
            }
            else
            {
                //load all blog comments
                comments = _blogService.GetAllComments(0);
            }

            var result = new DataSourceResult()
            {
                Data = comments.Select(blogComment =>
                {
                    var commentModel = new BlogCommentModel();
                    commentModel.Id = blogComment.Id;
                    commentModel.BlogPostId = blogComment.BlogPostId;
                    commentModel.BlogPostTitle = blogComment.BlogPost.Title;
                    commentModel.AccountId = blogComment.AccountId;
                    var account = blogComment.Account;
                    commentModel.AccountInfo = account == null ? account.Email : _localizationService.GetResource("Common.Account");

                    commentModel.Comment = Core.Html.HtmlHelper.FormatText(blogComment.CommentText, false, true, false, false, false, false);

                    return commentModel;
                }),

                Total = comments.Count,
            };

            return Json(result);
        }


        [HttpPost]
        public ActionResult CommentDelete(int? filterByBlogPostId, int id, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var comment = _blogService.GetBlogCommentById(id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var blogPost = comment.BlogPost;
            _blogService.DeleteBlogComment(comment);
            //update totals
            blogPost.CommentCount = blogPost.BlogComments.Count;
            _blogService.UpdateBlogPost(blogPost);

            //activity log
            _activityLogService.InsertActivityLog("DeleteBlogComment", _localizationService.GetResource("ActivityLog.DeleteBlogComment"), blogPost.Title);

            //return Comments(filterByBlogPostId, command);
            return new NullJsonResult();
        }

        #endregion

    }
}

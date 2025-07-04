﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Forums;
using Wfm.Services.Forums;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.UI;

namespace Wfm.Admin.Controllers
{
    public partial class ForumController : BaseAdminController
    {
        #region Fields

        private readonly IForumService _forumService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ForumController(IForumService forumService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._forumService = forumService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion


        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroups = _forumService.GetAllForumGroups();

            var modelList = new List<ForumGroupModel>();
            foreach (var item in forumGroups)
            {
                ForumGroupModel model = item.ToModel();
                var froums = _forumService.GetAllForumsByGroupId(item.Id);
                model.ForumModels = froums.Select(x => x.ToModel()).ToList();
                modelList.Add(model);
            }

            return View(modelList);
        }

        [HttpPost, ActionName("List")]
        public ActionResult ForumGroupList([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroups = _forumService.GetAllForumGroups();
            var gridModel = new DataSourceResult
            {
                Data = forumGroups.Select(fg =>
                {
                    var model = fg.ToModel();
                    return model;
                }),
                Total = forumGroups.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ForumList(int forumGroupId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(forumGroupId);
            if (forumGroup == null)
                throw new Exception("Forum group cannot be loaded");

            var forums = forumGroup.Forums;
            var gridModel = new DataSourceResult
            {
                Data = forums.Select(f =>
                {
                    var forumModel = f.ToModel();
                    //forumModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(f.CreatedOnUtc, DateTimeKind.Utc);
                    return forumModel;
                }),
                Total = forums.Count
            };

            return Json(gridModel);
        }

        #endregion

        #region Create

        public ActionResult CreateForumGroup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            return View(new ForumGroupModel { DisplayOrder = 1 });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult CreateForumGroup(ForumGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var forumGroup = model.ToEntity();
                forumGroup.CreatedOnUtc = DateTime.UtcNow;
                forumGroup.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.InsertForumGroup(forumGroup);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Added"));
                return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult CreateForum()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var model = new ForumModel();
            foreach (var forumGroup in _forumService.GetAllForumGroups())
            {
                var forumGroupModel = forumGroup.ToModel();
                model.ForumGroups.Add(forumGroupModel);
            }
            model.DisplayOrder = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult CreateForum(ForumModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var forum = model.ToEntity();
                forum.CreatedOnUtc = DateTime.UtcNow;
                forum.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.InsertForum(forum);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Added"));
                return continueEditing ? RedirectToAction("EditForum", new { forum.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            foreach (var forumGroup in _forumService.GetAllForumGroups())
            {
                var forumGroupModel = forumGroup.ToModel();
                model.ForumGroups.Add(forumGroupModel);
            }
            return View(model);
        }

        #endregion

        #region Edit

        public ActionResult EditForumGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null)
                //No forum group found with the specified id
                return RedirectToAction("List");

            var model = forumGroup.ToModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult EditForumGroup(ForumGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(model.Id);
            if (forumGroup == null)
                //No forum group found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                forumGroup = model.ToEntity(forumGroup);
                forumGroup.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.UpdateForumGroup(forumGroup);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Updated"));
                return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult EditForum(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forum = _forumService.GetForumById(id);
            if (forum == null)
                //No forum found with the specified id
                return RedirectToAction("List");

            var model = forum.ToModel();
            foreach (var forumGroup in _forumService.GetAllForumGroups())
            {
                var forumGroupModel = forumGroup.ToModel();
                model.ForumGroups.Add(forumGroupModel);
            }
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult EditForum(ForumModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forum = _forumService.GetForumById(model.Id);
            if (forum == null)
                //No forum found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                forum = model.ToEntity(forum);
                forum.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.UpdateForum(forum);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Updated"));
                return continueEditing ? RedirectToAction("EditForum", forum.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            foreach (var forumGroup in _forumService.GetAllForumGroups())
            {
                var forumGroupModel = forumGroup.ToModel();
                model.ForumGroups.Add(forumGroupModel);
            }
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public ActionResult DeleteForumGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null)
                //No forum group found with the specified id
                return RedirectToAction("List");

            _forumService.DeleteForumGroup(forumGroup);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteForum(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forum = _forumService.GetForumById(id);
            if (forum == null)
                //No forum found with the specified id
                return RedirectToAction("List");

            _forumService.DeleteForum(forum);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}

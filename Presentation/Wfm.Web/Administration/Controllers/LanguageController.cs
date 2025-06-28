using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Core.Domain.Localization;
using Wfm.Core;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.UI;
using Wfm.Shared.Models.Localization;

namespace Wfm.Admin.Controllers
{
    public class LanguageController : BaseAdminController
    {

        #region Fields/Localization

        private readonly IActivityLogService _activityLogService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public LanguageController(IActivityLogService activityLogService,
            ILanguageService languageService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _languageService = languageService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _workContext = workContext;
        }
        #endregion

        #region Get:/Language/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            IList<Language> languages = _languageService.GetAllLanguages();

            List<LanguageModel> languageModelList = new List<LanguageModel>();
            foreach (var item in languages)
            {
                LanguageModel l = item.ToModel();

                languageModelList.Add(l);
            }

            return View(languageModelList);

            //var gridModel = new GridModel<LanguageModel>
            //{
            //    Data = tempModel,
            //    Total = temp.TotalCount
            //};

            //return View(gridModel);
        }

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var languages = _languageService.GetAllLanguages(); //it's a IPageList

            List<LanguageModel> languageModelList = new List<LanguageModel>();

            foreach (var item in languages)
            {
                LanguageModel i = MappingExtensions.ToModel(item);

                languageModelList.Add(i);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = languageModelList, // Process data (paging and sorting applied)
                Total = languages.Count // Total number of records
            };

            return Json(result);
        }

        #endregion

        #region Get:/Language/Create
        [HttpGet]
        public ActionResult Create()
        {
            LanguageModel languageModel = new LanguageModel();

            languageModel.IsActive = true;
            languageModel.UpdatedOnUtc = System.DateTime.UtcNow;
            languageModel.CreatedOnUtc = System.DateTime.UtcNow;
            languageModel.EnteredBy = _workContext.CurrentAccount.Id;
            languageModel.DisplayOrder = 0;
            return View(languageModel);
        }
        #endregion

        #region POST:/Language/Create
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                Language language = model.ToEntity();
                _languageService.Insert(language);

                //activity log
                _activityLogService.InsertActivityLog("AddNewLanguage", _localizationService.GetResource("ActivityLog.AddNewLanguage"), language.Name);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region Get:/Language/Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            Language language = _languageService.GetLanguageById(id);

            LanguageModel languageModel = language.ToModel();
            languageModel.UpdatedOnUtc = System.DateTime.UtcNow;

            return View(languageModel);
        }
        #endregion

        #region POST:/Language/Edit
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(LanguageModel languageModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            languageModel.UpdatedOnUtc = System.DateTime.UtcNow;

            Language language = _languageService.GetLanguageById(languageModel.Id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                // the fellowing step is mapping languageModel contents to a entity name as language
                // and assignment to Language
                language = languageModel.ToEntity(language);
                _languageService.Update(language);

                //activity log
                _activityLogService.InsertActivityLog("UpdateLanguage", _localizationService.GetResource("ActivityLog.UpdateLanguage"), language.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(languageModel);
        }
        #endregion

        #region POST:/Language/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("Index");

            try
            {
                if (_localizationService.GetAllResources(language.Id).Count > 0)
                    throw new WfmException("The language can't be deleted. It has associated resources");

                _languageService.Delete(language);

                //activity log
                _activityLogService.InsertActivityLog("DeleteLanguage", _localizationService.GetResource("ActivityLog.DeleteLanguage"), language.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Deleted"));
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }
        }
        #endregion
    }
}

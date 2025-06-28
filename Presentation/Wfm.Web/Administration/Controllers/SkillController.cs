using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Common;
using Wfm.Core.Domain.Common;
using Wfm.Services.Logging;
using Wfm.Services.Common;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;

namespace Wfm.Admin.Controllers
{
    //[AdminAuthorize(Roles = "Admin, System")]
    public class SkillController : BaseAdminController
    {
        // private WorkforceDBEntities db = new WorkforceDBEntities();
        #region Fields
        
        private readonly IActivityLogService _activityLogService;
        private readonly ISkillService _skillService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public SkillController(IActivityLogService activityLogService,
            ISkillService iSkillService, 
            IPermissionService permissionService,
            ILocalizationService localizationService)
        {
            _activityLogService = activityLogService;
            _skillService = iSkillService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }
        #endregion

        #region GET :/Skill/Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();

            var skills = _skillService.GetAllSkills(true);
 
            List<SkillModel> skillModelList = new List<SkillModel>();

            foreach (var item in skills)
            {
                SkillModel model = MappingExtensions.ToModel(item);
                skillModelList.Add(model);
            }

            return View(skillModelList);
        }

        #endregion     

        #region POST:/Skill/Index
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();

            var skills = _skillService.GetAllSkills(true);
 
            List<SkillModel> skillModelList = new List<SkillModel>();

            foreach (var item in skills)
            {
                SkillModel model = MappingExtensions.ToModel(item);
                skillModelList.Add(model);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = skillModelList, // Process data (paging and sorting applied)
                Total = skills.Count // Total number of records
            };

            return Json(result);
        }

        #endregion

        #region  GET:/Skill/Create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();

            SkillModel skillModel = new SkillModel();

            skillModel.IsActive = true;
            skillModel.UpdatedOnUtc = System.DateTime.UtcNow;
            skillModel.CreatedOnUtc = System.DateTime.UtcNow;

            return View(skillModel);
        }
        #endregion

        #region POST:/Skill/Create
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(SkillModel skillModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();

            skillModel.UpdatedOnUtc = System.DateTime.UtcNow;
            skillModel.CreatedOnUtc = System.DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                Skill skill = skillModel.ToEntity();
                _skillService.InsertSkill(skill);

                //activity log
                _activityLogService.InsertActivityLog("AddNewCandidateKeySkill", _localizationService.GetResource("ActivityLog.AddNewCandidateKeySkill"), skill.SkillName);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Skill.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = skill.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(skillModel);
        }
        #endregion

        #region GET :/Skill/Details

        public ActionResult Details(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();

            Skill skill = _skillService.GetSkillById(id);

            SkillModel skillModel = MappingExtensions.ToModel(skill);

            ViewBag.EditMode = 0;

            return View(skillModel);
        }

        #endregion

        #region GET :/Skill/Edit
        public ActionResult Edit(int id)
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                    return AccessDeniedView();

                Skill skill = _skillService.GetSkillById(id);

                SkillModel skillModel = skill.ToModel();
                skillModel.UpdatedOnUtc = System.DateTime.UtcNow;

                return View(skillModel);
            }
        #endregion

        #region POST:/Skill/Edit/
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(SkillModel skillModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();


            skillModel.UpdatedOnUtc = System.DateTime.UtcNow;

            //skillModel.EnteredBy = skillModel.EnteredBy;

            Skill skill = _skillService.GetSkillById(skillModel.Id);

            if (skill == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                // the fellowing step is mapping countryModel contents to a entity name as country
                // and assignment to Country
                skill = skillModel.ToEntity(skill);
                _skillService.UpdateSkill(skill);

                //activity log
                _activityLogService.InsertActivityLog("UpdateCandidateKeySkill", _localizationService.GetResource("ActivityLog.UpdateCandidateKeySkill"), skill.SkillName);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Skill.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = skill.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(skillModel);
        }

        #endregion

        #region POST:/Skill/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSkills))
                return AccessDeniedView();

            var skill = _skillService.GetSkillById(id);
            if (skill == null)
                //No country found with the specified id
                return RedirectToAction("Index");

            try
            {
                _skillService.DeleteSkill(skill);

                //activity log
                _activityLogService.InsertActivityLog("DeleteCandidateKeySkill", _localizationService.GetResource("ActivityLog.DeleteCandidateKeySkill"), skill.SkillName);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Skill.Deleted"));
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = skill.Id });
            }
        }
        #endregion
          
    } //end controller
} //end namespace

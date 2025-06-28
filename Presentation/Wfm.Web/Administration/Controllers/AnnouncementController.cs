using Kendo.Mvc.UI;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Services.Accounts;
using Wfm.Services.Announcements;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Core.Domain.Announcements;
using System;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Admin.Models.Announcements;

namespace Wfm.Admin.Controllers  
{
    public class AnnouncementController : BaseAdminController 
    {
      
         #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IAnnouncementService _announcementService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IAccountService _accountService;
        private readonly ICompanyService _companyService;
        private readonly IFranchiseService _franchiseService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        public AnnouncementController(
            IActivityLogService activityLogService,
            IAnnouncementService announcementService, 
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IAccountService accountService,
            IWorkContext workContext,
            ICompanyService companyService,
            IFranchiseService franchiseService,
            ILogger logger
            ) 
        {
            _activityLogService = activityLogService;
            _announcementService = announcementService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _accountService = accountService;
            _workContext = workContext;
            _companyService = companyService;
            _franchiseService = franchiseService; 
            _logger = logger; 
        }

        #endregion

          // GET: Announcement
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();

            var announcement_BL = new Announcement_BL(_activityLogService, _announcementService, _localizationService, _accountService, _workContext, _companyService
                                                     , _franchiseService);

           var result =  announcement_BL.GetAllAnnouncementList(request);        
            return Json(result);
        }

        #region Get:Announcement/Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();

            var announcement_BL = new Announcement_BL(_activityLogService, _announcementService, _localizationService, _accountService, _workContext, _companyService
                                                    , _franchiseService);
            AnnouncementModel announcementModel = announcement_BL.GetCreateAnnouncement();            
            return View(announcementModel);
        }

        #endregion

        #region POST:/Announcement/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(AnnouncementModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();        
            if (ModelState.IsValid)
            {
                  var announcement_BL = new Announcement_BL(_activityLogService, _announcementService, _localizationService, _accountService, _workContext, _companyService
                                                    , _franchiseService);
                  var announcementGuid = announcement_BL.CreateAnnouncement(model);             
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Announcement.Added"));
                return continueEditing ? RedirectToAction("Edit", new { guid = announcementGuid }) : RedirectToAction("Index");

            }
            else {
                model.SelectedCompanyList = new SelectListItem[] { };
                model.SelectedFranchiseList = new SelectListItem[] { };
            }
            return View(model);
        } //end create post

        #endregion

        #region GET :/Announcement/Edit/

        public ActionResult Edit(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();

            var announcement_BL = new Announcement_BL(_activityLogService, _announcementService, _localizationService, _accountService, _workContext, _companyService
                                                    , _franchiseService);
            var announcementModel = announcement_BL.GetEditAnnouncement(guid);
            if (announcementModel == null)
                return RedirectToAction("Index");      

            return View(announcementModel);
        }
        #endregion

        #region POST:/Announcement/Edit/

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(AnnouncementModel announcementModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();

            Announcement announcement = _announcementService.GetAnnouncementById(announcementModel.Id);
            if (announcement == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                var announcement_BL = new Announcement_BL(_activityLogService, _announcementService, _localizationService, _accountService, _workContext, _companyService
                                                   , _franchiseService);
                var AnnouncementGuid = announcement_BL.UpdateAnnouncement(announcementModel, announcement);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Announcement.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { guid = announcement.AnnouncementGuid }) : RedirectToAction("Index");
            }
            return View(announcementModel);
        }
        #endregion

        #region POST:/Announcement/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAnnouncements))
                return AccessDeniedView();

            var announcement = _announcementService.GetAnnouncementById(id);
            if (announcement == null)                
                return RedirectToAction("Index");
            try
            {
                _announcementService.MarkAnnouncementAsDeleted(announcement);
                _activityLogService.InsertActivityLog("DeleteAnnouncement", _localizationService.GetResource("ActivityLog.DeleteAnnouncement"), announcement.Id);
               SuccessNotification(_localizationService.GetResource("Admin.Configuration.Announcement.Deleted"));
                return RedirectToAction("Index", "Announcement");
            }
            catch (Exception exc)
            {
                _logger.Error("Announcement/Delete:" + exc, userAgent: Request.UserAgent);
                ErrorNotification(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                return RedirectToAction("Edit", "Announcement", new { guid = announcement.AnnouncementGuid });
            }
        }
        #endregion
    }
}
using System;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Services.Announcements;
using Wfm.Services.Security;
using Wfm.Web.Framework.Feature;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Dashboard")]
    public class HomeController : BaseClientController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IAnnouncementService _announcementService;
        private readonly IWorkContext _workContext;

        #endregion


        #region Ctor

        public HomeController(
            IPermissionService permissionService,
            IAnnouncementService announcementService,
            IWorkContext workContext
            )
        {
            _permissionService = permissionService;
            _announcementService = announcementService;
            _workContext = workContext;
        }

        #endregion


        public ActionResult News()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessClientPanel))
                return AccessDeniedView();

            return View();
        }


        public ActionResult Index()
        {
            // return View();
            return RedirectToAction("SignIn", "Account");
        }       


        public ActionResult _TabAnnouncements()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessClientPanel))
                return AccessDeniedView();

            return PartialView();
        }


        public ActionResult GetAnnouncementView()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessClientPanel))
                return AccessDeniedView();

            return PartialView("_Announcements", _announcementService.GetAnnouncementsForClient(_workContext.CurrentAccount));
        }


        public ActionResult MarkAnnouncementAsRead(Guid announcementGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessClientPanel))
                return AccessDeniedJson();

            var result = _announcementService.MarkAnnouncementAsRead(_workContext.CurrentAccount, announcementGuid);

            return Json(new { Succeed = result, Error = "Failed to update!" });
        }
    }
}

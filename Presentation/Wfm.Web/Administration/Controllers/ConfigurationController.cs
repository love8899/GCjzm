using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Services.Security;

namespace Wfm.Admin.Controllers
{

   // [AdminAuthorize(Roles = "Admin, System")]
    public class ConfigurationController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;

        #endregion

        #region Cotr

        public ConfigurationController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #endregion

        #region Configuration/Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageConfiguration))
                return AccessDeniedView();

            return View();
        }

        #endregion

    }
}

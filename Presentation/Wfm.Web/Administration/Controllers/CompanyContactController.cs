using Kendo.Mvc.UI;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Services.Companies;
using Wfm.Services.Security;
using Wfm.Admin.Models.CompanyContact;

namespace Wfm.Admin.Controllers
{
    public class CompanyContactController : BaseAdminController
    {
        #region Fields

        private readonly ICompanyContactService _companyContactService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IRecruiterCompanyService _recruiterCompanyService;

        #endregion

        #region Ctor

        public CompanyContactController(
            ICompanyContactService companyContactService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IRecruiterCompanyService recruiterCompanyService
            )
        {
            _companyContactService = companyContactService;
            _permissionService = permissionService;
            _workContext = workContext;
            _recruiterCompanyService = recruiterCompanyService;
        }

        #endregion

        #region GET :/CompanyContact/Index

        [HttpGet]
        public ActionResult Index()
         {
             if (!_permissionService.Authorize(StandardPermissionProvider.ViewContacts))
                 return AccessDeniedView();

             return View();
         }

        #endregion

        #region POST:/CompanyContact/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewContacts))
                return AccessDeniedView();
            CompanyContactViewModel_BL model_BL = new CompanyContactViewModel_BL();


            var result = model_BL.GetCompanyContactViewModel(_companyContactService, _workContext, _recruiterCompanyService, request);

            return Json(result);
        }

        #endregion

    }
}

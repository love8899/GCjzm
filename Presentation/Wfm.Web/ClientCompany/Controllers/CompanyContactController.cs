using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Client.Extensions;
using Wfm.Core;
using Wfm.Services.Companies;
using Wfm.Services.Security;
using Wfm.Web.Framework;
using Wfm.Client.Models.CompanyContact;
using Wfm.Web.Framework.Feature;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Contact")]
    public class CompanyContactController : BaseClientController
    {
        #region Fields

        private readonly ICompanyContactService _companyContactService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CompanyContactController(
            ICompanyContactService companyContactService,
            IPermissionService permissionService,
            IWorkContext workContext
            )
        {
            _companyContactService = companyContactService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region GET :/CompanyContact/Index

        [HttpGet]
        public ActionResult Index()
         {
             if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyContacts))
                 return AccessDeniedView();

             ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;

             return View();
         }

        #endregion

        #region POST:/CompanyContact/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyContacts))
                return AccessDeniedView();

            var accounts = _companyContactService.GetAllCompanyContactsByAccountAsQueryable(_workContext.CurrentAccount, false).PagedForCommand(request);

            List<CompanyContactViewModel> accountModelList = new List<CompanyContactViewModel>(); 
            foreach (var item in accounts)
            {
                CompanyContactViewModel accountModel = item.ToCompanyContactViewModel();
                accountModel.AccountRoleSystemName = item.AccountRoles.FirstOrDefault().AccountRoleName;
                accountModelList.Add(accountModel);
            }

            var result = new DataSourceResult()
            {
                Data = accountModelList,
                Total = accounts.TotalCount
            };

            return Json(result);
        }

        #endregion


        #region //JsonResult : GetCascadeContacts

        public JsonResult GetCascadeContacts( int locationId, int departmentId)
        {
            var contacts = _companyContactService.GetCompanyContactsByCompanyIdAndLocationIdAndDepartmentId(_workContext.CurrentAccount.CompanyId, locationId, departmentId)
                .Select(x => new SelectListItem()
                {
                    Text = x.FullName,
                    Value = x.Id.ToString()
                });

            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}

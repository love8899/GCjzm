using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Accounts;
using Wfm.Services.Accounts;
using Wfm.Services.Policies;
using Wfm.Shared.Extensions;
using Wfm.Shared.Models.Policies;

namespace Wfm.Admin.Controllers
{
    public class PasswordPolicyController : Controller
    {
        #region Fields
        private readonly IPasswordPolicyService _passwordPolicyService;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        #endregion

        #region Ctor
        public PasswordPolicyController(IPasswordPolicyService passwordPolicyService,
                                        IAccountPasswordPolicyService accountPasswordPolicyService)
        {
            _passwordPolicyService = passwordPolicyService;
            _accountPasswordPolicyService = accountPasswordPolicyService;
        }
        #endregion

        #region Password Policy Method
        

        // GET: PasswordPolicy
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(DataSourceRequest request)
        {
            var result = _passwordPolicyService.GetAllPasswordPolicies();
            return Json(result.ToDataSourceResult(request,x=>x.ToModel()));
        }

        [HttpPost]
        public ActionResult _CreateNewPasswordPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PasswordPolicyModel> models)
        {
            var results = new List<PasswordPolicyModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _passwordPolicyService.Insert(model.ToEntity());
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditPasswordPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PasswordPolicyModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _passwordPolicyService.Update(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult _DeletePasswordPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PasswordPolicyModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _passwordPolicyService.Delete(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request,ModelState));
        }
        #endregion

        #region Account Password Policy Method
        public ActionResult AccountPasswordPolicy()
        {
            ViewBag.PasswordPolicies = _passwordPolicyService.GetAllPasswordPoliciesAsSelectItem();
            return View();
        }

        [HttpPost]
        public ActionResult GetAllAccountPasswordPolicy(DataSourceRequest request)
        {
            var models =  _accountPasswordPolicyService.GetAllAccountPasswordPolicy().ToList();
            return Json(models.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _CreateNewAccountPasswordPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AccountPasswordPolicyModel> models)
        {
            var results = new List<AccountPasswordPolicyModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    _accountPasswordPolicyService.Create(model.ToEntity());
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditAccountPasswordPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AccountPasswordPolicyModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    _accountPasswordPolicyService.Update(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult _DeleteAccountPasswordPolicy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AccountPasswordPolicyModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _accountPasswordPolicyService.Delete(model.ToEntity());
                }
            }
            

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}
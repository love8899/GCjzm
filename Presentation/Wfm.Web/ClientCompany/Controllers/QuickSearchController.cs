using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Wfm.Client.Extensions;
using System;
using Wfm.Client.Models.Companies;
using Wfm.Client.Models.JobOrder;
using Wfm.Client.Models.QuickSearch;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Security;

namespace Wfm.Client.Controllers
{
    public class QuickSearchController : BaseClientController
    {
        #region Fields
        private readonly IJobOrderService _jobOrderService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        #endregion

        #region Ctor

        public QuickSearchController(
            IJobOrderService jobOrderService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IPermissionService permissionService
            )
        {
            _jobOrderService = jobOrderService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _workContext = workContext;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities
        private string GetCandidateKeySkillSet(List<CandidateKeySkill> s)
        {
            IList<String> ltStr = new List<String>();
            foreach (var item in s)
            {
                ltStr.Add(item.KeySkill);
            }

            return string.Join(" / ", ltStr);
        }

        #endregion

        #region GET :/QuickSearch/Index
        [ChildActionOnly]
        public ActionResult Index(string SearchKey, string SearchArea)
        {
            QuickSearchModel model = new QuickSearchModel() { KeyWord = SearchKey, Domain = SearchArea };
            return PartialView(model);
        }
        #endregion

        #region POST:/QuickSearch/QuickSearchResult
        [HttpPost]
        public ActionResult QuickSearchResult(QuickSearchModel model)
        {
            if (model.KeyWord == null)
                return RedirectToAction("News", "Home");

            // Quick cleanup for search key
            model.KeyWord = Regex.Replace(model.KeyWord.Trim(), @"\s+", " ");

            // JobOrder search
            //if (model.Domain == "JobOrder")

            // Only allow job order search for client
            return RedirectToAction("JobOrdersResult", new { searchKey = model.KeyWord });

            //return View();
        }
        #endregion


        #region JobOrdersResult
        public ActionResult JobOrdersResult(string searchKey)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyJobOrders))
                return AccessDeniedView();

            IList<JobOrder> jobOrders = _jobOrderService.SearchCompanyJobOrders(_workContext.CurrentAccount, searchKey, 100);

            if (jobOrders.Count == 0)
                InformationNotification(_localizationService.GetResource("Admin.QuickSearch.NoRecordFound"));
            if (jobOrders.Count >= 100)
                WarningNotification(_localizationService.GetResource("Admin.QuickSearch.TooManyRecords"));

            List<JobOrderModel> jobOrderModelList = new List<JobOrderModel>();
            foreach (var item in jobOrders)
            {
                JobOrderModel model = item.ToModel();

                model.CompanyModel = item.Company.ToModel();

                //model.JobOrderTypeModel = item.JobOrderType.ToModel();
                //model.JobOrderStatusModel = item.JobOrderStatus.ToModel();
                //model.JobOrderCategoryModel = item.JobOrderCategory.ToModel();
                //model.JobOrderStatusModel = item.JobOrderStatus.ToModel();

                CompanyLocation companyLocation = _companyDivisionService.GetCompanyLocationById(item.CompanyLocationId);
                if (companyLocation != null)
                {
                    model.CompanyLocationModel = companyLocation.ToModel();
                }
                else
                {
                    model.CompanyLocationModel = new CompanyLocationModel();
                }

                var department = _companyDepartmentService.GetCompanyDepartmentById(item.CompanyDepartmentId);
                model.CompanyDepartmentName = department != null ? department.DepartmentName : "";

                jobOrderModelList.Add(model);
            }

            ViewBag.SearchKey = searchKey;
            ViewBag.SearchArea = "JobOrder";

            return View(jobOrderModelList);
        }
        #endregion


    } // end of class
}

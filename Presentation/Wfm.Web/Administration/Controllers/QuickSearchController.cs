using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using System;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Models.JobOrder;
using Wfm.Admin.Models.QuickSearch;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Security;


namespace Wfm.Admin.Controllers
{
    public class QuickSearchController : BaseAdminController
    {
        #region Const
        private const int maxReturnRec = 200;
        #endregion

        #region Fields
        private readonly ICandidateService _candidatesService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        #endregion

        #region Ctor

        public QuickSearchController(ICandidateService candidatesService,
            IJobOrderService jobOrderService,
            ICompanyService companyService,
            ICompanyDivisionService companyDivisionService,
            ILocalizationService localizationService,
            IPermissionService permissionService
            )
        {
            _candidatesService = candidatesService;
            _jobOrderService = jobOrderService;
            _companyService = companyService;
            _companyDivisionService = companyDivisionService;
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

        #region GET :/QuickSearch/IndexAjax
        [ChildActionOnly]
        public ActionResult IndexAjax(string SearchKey, string SearchArea)
        {
            QuickSearchModel model = new QuickSearchModel() { KeyWord = SearchKey, Domain = SearchArea };
            return PartialView(model);
        }
        #endregion

        #region POST:/QuickSearch/QuickSearchResult
        [HttpPost]
        public ActionResult QuickSearchResult(QuickSearchModel model)
        {
            // Validate search key
            if (String.IsNullOrWhiteSpace(model.KeyWord))
            {
                ErrorNotification(_localizationService.GetResource("Admin.QuickSearch.InvalidSearchKey"));
                return View(model);
            }

            // Quick cleanup for search key
            model.KeyWord = Regex.Replace(model.KeyWord.Trim(), @"\s+", " ");

            switch (model.Domain)
            {
                case "JobOrder":
                    return RedirectToAction("JobOrdersResult", new { SearchKey = model.KeyWord });
                    //break;
                case "Company":
                    return RedirectToAction("CompaniesResult", new { SearchKey = model.KeyWord });
                    //break;
                case "Employee":
                    return RedirectToAction("EmployeesResult", new { SearchKey = model.KeyWord });
                //break;
                default:
                    //Default - Candidate
                    return RedirectToAction("CandidatesResult", new { SearchKey = model.KeyWord });
                    //break;
            }
        }
        #endregion


        #region CandidatesResult
        public ActionResult CandidatesResult(string SearchKey)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidates))
                return AccessDeniedView();

            IList<Candidate> candidates = _candidatesService.SearchCandidates(SearchKey, maxReturnRec, true, true);

            if (candidates.Count == 0)
                InformationNotification(_localizationService.GetResource("Admin.QuickSearch.NoRecordFound"));
            if (candidates.Count >= maxReturnRec)
                WarningNotification(_localizationService.GetResource("Admin.QuickSearch.TooManyRecords"));

            List<CandidateModel> candidateModelList = new List<CandidateModel>();
            foreach (var c in candidates)
            {
                var candidateModel = c.ToModel();
                candidateModelList.Add(candidateModel);
            }

            ViewBag.SearchKey = SearchKey;
            ViewBag.SearchArea = "Candidate";

            return View(candidateModelList);
        }
        #endregion


        #region Employees Result

        public ActionResult EmployeesResult([DataSourceRequest] DataSourceRequest request, string SearchKey)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmployees))
                return AccessDeniedView();

            IList<Candidate> candidates = _candidatesService.SearchCandidates(SearchKey, maxReturnRec, true, true, employeeOnly: true);

            if (candidates.Count == 0)
                InformationNotification(_localizationService.GetResource("Admin.QuickSearch.NoRecordFound"));
            if (candidates.Count >= maxReturnRec)
                WarningNotification(_localizationService.GetResource("Admin.QuickSearch.TooManyRecords"));

            ViewBag.SearchKey = SearchKey;
            ViewBag.SearchArea = "Employee";

            return View(candidates.Select(x => x.ToEmployeeModel()));
        }

        #endregion


        #region JobOrdersResult
        public ActionResult JobOrdersResult(string SearchKey)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobOrders))
                return AccessDeniedView();

            IList<JobOrder> jobOrders = _jobOrderService.SearchJobOrders(SearchKey, maxReturnRec, true);

            if (jobOrders.Count == 0)
                InformationNotification(_localizationService.GetResource("Admin.QuickSearch.NoRecordFound"));
            if (jobOrders.Count >= maxReturnRec)
                WarningNotification(_localizationService.GetResource("Admin.QuickSearch.TooManyRecords"));

            List<JobOrderModel> jobOrderModelList = new List<JobOrderModel>();
            foreach (var item in jobOrders)
            {
                JobOrderModel model = item.ToModel();


                model.CompanyModel = item.Company.ToModel();
                CompanyLocation companyLocation = _companyDivisionService.GetCompanyLocationById(item.CompanyLocationId);
                if (companyLocation != null)
                    model.CompanyLocationModel = companyLocation.ToModel();
                else
                    model.CompanyLocationModel = new CompanyLocationModel();

                //model.JobOrderTypeModel = item.JobOrderType.ToModel();
                //model.JobOrderStatusModel = item.JobOrderStatus.ToModel();
                //model.JobOrderCategoryModel = item.JobOrderCategory.ToModel();
                //model.JobOrderStatusModel = item.JobOrderStatus.ToModel();
                //model.ShiftModel = item.Shift.ToModel();
                //model.CompanyName = model.CompanyModel.CompanyName;
                //model.ShiftName = model.ShiftModel.ShiftName;

                jobOrderModelList.Add(model);
            }

            ViewBag.SearchKey = SearchKey;
            ViewBag.SearchArea = "JobOrder";

            return View(jobOrderModelList);
        }
        #endregion

        #region CompaniesResult
        public ActionResult CompaniesResult(string SearchKey)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCompanies))
                return AccessDeniedView();

            IList<Company> companies = _companyService.SearchCompanies(SearchKey, maxReturnRec, true);

            if (companies.Count == 0)
                InformationNotification(_localizationService.GetResource("Admin.QuickSearch.NoRecordFound"));
            if (companies.Count >= maxReturnRec)
                WarningNotification(_localizationService.GetResource("Admin.QuickSearch.TooManyRecords"));

            List<CompanyModel> companyModelList = new List<CompanyModel>();
            foreach (var c in companies)
            {
                CompanyModel companyModel = c.ToModel();
                companyModelList.Add(companyModel);
            }

            ViewBag.SearchKey = SearchKey;
            ViewBag.SearchArea = "Company";

            return View(companyModelList);
        }
        #endregion

    } // end of class
}

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Companies;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Web.Extensions;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Filters;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Framework.Security;
using Wfm.Web.Framework.Seo;
using Wfm.Web.Models.JobOrder;


namespace Wfm.Web.Controllers
{
    public partial class JobPostController : BaseWfmController
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IJobOrderTypeService _jobOrderTypeService;
        private readonly IJobOrderCategoryService _jobOrderCategoryService;
        private readonly IJobOrderStatusService _jobOrderStatusService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public JobPostController(
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            IJobOrderService jobOrderService,
            IJobOrderTypeService jobOrderTypeService,
            IJobOrderCategoryService jobOrderCateoryService,
            IJobOrderStatusService jobOrderStatusService,
            IWorkflowMessageService workflowMessageService,
            ICompanyDivisionService companyDivisionService,
            ILocalizationService localizationService
            )
        {
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _jobOrderService = jobOrderService;
            _jobOrderTypeService = jobOrderTypeService;
            _jobOrderCategoryService = jobOrderCateoryService;
            _jobOrderStatusService = jobOrderStatusService;
            _workflowMessageService = workflowMessageService;
            _companyDivisionService = companyDivisionService;
            _localizationService = localizationService;
        }

        #endregion


        #region GET :/JobPost/Index

        [ValidateInput(false)]
        public ActionResult Index(string searchString, int? page = 1, int jobOrderCategoryId = 0, string seoName = null, int pagenumber = 0)
        {
            pagenumber = pagenumber - 1;
            if (pagenumber == -1)
                pagenumber = 0;
            
            // clean up
            searchString = searchString.ExtractAlphanumericText();

            var jobOrders = _jobOrderService.GetAllPublishedJobOrders(searchString, jobOrderCategoryId, pagenumber, 20);

            IList<JobOrderModel> jobOrderModels = new List<JobOrderModel>();

            foreach (var item in jobOrders)
            {
                JobOrderModel model = item.ToModel();

                JobOrderType jobOrderType = _jobOrderTypeService.GetJobOrderTypeById(item.JobOrderTypeId);
                if (jobOrderType != null)
                    model.JobOrderTypeModel = jobOrderType.ToModel();

                CompanyLocation companyLocation = _companyDivisionService.GetCompanyLocationById(item.CompanyLocationId);
                if (companyLocation != null)
                {
                    model.CompanyLocationModel = companyLocation.ToModel();

                    model.CompanyLocationModel.CountryModel =
                        _countryService.GetCountryById(companyLocation.CountryId).ToModel();
                    model.CompanyLocationModel.StateProvinceModel =
                        _stateProvinceService.GetStateProvinceById(companyLocation.StateProvinceId).ToModel();
                    model.CompanyLocationModel.CityModel =
                        _cityService.GetCityById(companyLocation.CityId).ToModel();
                }

                jobOrderModels.Add(model);
            }

            //search key word
            ViewBag.SearchString = searchString;
            //job post paging
            ViewBag.JobOrderTypeList = new SelectList(_jobOrderTypeService.GetAllJobOrderTypes(), "Id", "JobOrderTypeName");
            ViewBag.JobOrderCategoryList = new SelectList(_jobOrderCategoryService.GetAllJobOrderCategories(), "Id", "CategoryName");
            ViewBag.JobOrderStatusList = new SelectList(_jobOrderStatusService.GetAllJobOrderStatus(), "Id", "JobOrderStatusName");
            //job category paging
            ViewBag.JobOrderCategoryId = jobOrderCategoryId;

            var jobOrderListModel = new JobOrderListModel();
            jobOrderListModel.PagingFilterContext.LoadPagedList(jobOrders);
            jobOrderListModel.JobOrders = jobOrderModels;
            
            return View(jobOrderListModel);
        }

        #endregion

        #region GET :/JobPost/JobDetailsSeo

        public ActionResult JobDetailsSeo(int jobOrderId = 0)
        {
            if (jobOrderId == 0)
                return RedirectToAction("Index");

            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            if (jobOrder == null)
                return RedirectToAction("Index"); //return HttpNotFound();

            return RedirectToActionPermanent("JobDetails", new { jobOrderId = jobOrderId, seoName = SeoHelper.ToSeoUrl(jobOrder.JobTitle) });
        }

        private void GetJobOrderLocation(JobOrderModel jobOrder)
        {
            CompanyLocation companyLocation = _companyDivisionService.GetCompanyLocationById(jobOrder.CompanyLocationId);
            if (companyLocation != null)
            {
                jobOrder.CompanyLocationModel = companyLocation.ToModel();

                jobOrder.CompanyLocationModel.CountryModel =
                    _countryService.GetCountryById(companyLocation.CountryId).ToModel();
                jobOrder.CompanyLocationModel.StateProvinceModel =
                    _stateProvinceService.GetStateProvinceById(companyLocation.StateProvinceId).ToModel();
                jobOrder.CompanyLocationModel.CityModel =
                    _cityService.GetCityById(companyLocation.CityId).ToModel();
            }
        }

        //[ValidateInput(false)]
        public ActionResult JobDetails(int jobOrderId = 0, string seoName = "")
        {
            if (jobOrderId == 0)
                return RedirectToAction("Index");

            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            if (jobOrder == null)
                return RedirectToAction("Index"); //return HttpNotFound();


            JobOrderModel model = jobOrder.ToModel();

            JobOrderType jobOrderType = _jobOrderTypeService.GetJobOrderTypeById(jobOrder.JobOrderTypeId);
            if (jobOrderType != null)
                model.JobOrderTypeModel = jobOrderType.ToModel();

            this.GetJobOrderLocation(model);

            model.JobDescription = HttpUtility.HtmlDecode(model.JobDescription);

            // Redirect to proper name
            if (seoName != SeoHelper.ToSeoUrl(model.JobTitle))
                return RedirectToActionPermanent("JobDetails", new { jobOrderId = jobOrderId, seoName = SeoHelper.ToSeoUrl(model.JobTitle) });

            return View(model);
        }

        #endregion

        #region GET :/JobPost/JobCategoriesSeo

        public ActionResult JobCategoriesSeo(string searchString, int jobOrderCategoryId)
        {
            var categorynameSEO = "";

            if (jobOrderCategoryId != 0)
            {
                var jobordercategory = _jobOrderCategoryService.GetJobOrderCategoryById(jobOrderCategoryId);
                categorynameSEO = SeoHelper.ToSeoUrl(jobordercategory.CategoryName);
            }

            return RedirectToActionPermanent("JobCategories", new { searchString, jobOrderCategoryId, seoName = categorynameSEO });
        }


        [ValidateInput(false)]
        public ActionResult JobCategories(string searchString, int jobOrderCategoryId, string seoName, int pagenumber = 0)
        {
            pagenumber = pagenumber - 1;
            if (pagenumber == -1)
            {
                pagenumber = 0;
            }

            // clean up
            searchString = searchString.ExtractAlphanumericText();

            var jobordercategory = _jobOrderCategoryService.GetJobOrderCategoryById(jobOrderCategoryId);

            var joborders = _jobOrderService.GetAllPublishedJobOrders(searchString, jobOrderCategoryId, pagenumber, 20);

            IList<JobOrderModel> joborderModels = new List<JobOrderModel>();
            foreach (var item in joborders)
            {
                JobOrderModel model = item.ToModel();

                JobOrderType jobOrderType = _jobOrderTypeService.GetJobOrderTypeById(item.JobOrderTypeId);
                if (jobOrderType != null)
                    model.JobOrderTypeModel = jobOrderType.ToModel();

                this.GetJobOrderLocation(model);

                joborderModels.Add(model);
            }

            //search key word
            ViewBag.SearchString = searchString;
            //job post paging
            ViewBag.JobOrderTypeId = new SelectList(_jobOrderTypeService.GetAllJobOrderTypes(), "Id", "JobOrderTypeName");
            ViewBag.JobOrderCategoryId = new SelectList(_jobOrderCategoryService.GetAllJobOrderCategories(), "Id", "CategoryName");
            ViewBag.JobOrderStatusId = new SelectList(_jobOrderStatusService.GetAllJobOrderStatus(), "Id", "JobOrderStatusName");
            //job category paging
            ViewBag.Id = jobOrderCategoryId;

            JobOrderListModel joborderlistmodel = new JobOrderListModel();
            joborderlistmodel.PagingFilterContext.LoadPagedList(joborders);
            joborderlistmodel.JobOrders = joborderModels;

            if (seoName != SeoHelper.ToSeoUrl(jobordercategory.CategoryName))
                return RedirectToActionPermanent("JobCategories", new { searchString, jobOrderCategoryId, seoName = SeoHelper.ToSeoUrl(jobordercategory.CategoryName) });

            return View(joborderlistmodel);
        }

        #endregion

        #region JobPost/EmailJobOrderAFriend

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult EmailJobOrderAFriend(int jobOrderId)
        {
            var model = new JobOrderEmailAFriendModel()
            {
                JobOrderId = jobOrderId
            };

            return PartialView("_EmailJobOrderToFriend",model);
        }


        [HttpPost, ActionName("EmailJobOrderAFriend")]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult EmailJobOrderAFriendSend(JobOrderEmailAFriendModel model)
        {
            Candidate candidate = null;
            //Candidate candidate = _candidateService.GetCandidateByEmail(User.Identity.Name);
            //if (candidate == null || !candidate.IsActive || candidate.IsBanned || candidate.IsDeleted)
            //    return RedirectToRoute("CandidateSignIn");

            var jobOrder = _jobOrderService.GetJobOrderById(model.JobOrderId);
            if (jobOrder == null || jobOrder.IsDeleted || !jobOrder.IsPublished)
                return RedirectToRoute("HomePage");

            string response = this.Request["g-recaptcha-response"];
            if (String.IsNullOrWhiteSpace(response) || !reCaptchaValidate.Validate(response))
                ModelState.AddModelError("reCaptcha", _localizationService.GetResource("Common.WrongCaptcha"));
            else
                ModelState.Remove("reCaptcha");

            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendJobOrderEmailAFriendMessage(candidate,
                        1, jobOrder,
                        model.YourEmailAddress, model.FriendEmail,
                        Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.JobOrderId = jobOrder.Id;

                model.SuccessfullySent = true;

                //SuccessNotification(_localizationService.GetResource("Web.JobOrder.EmailAFriend.EmailSent"));
                //return RedirectToAction("JobDetails", new { jobOrderId = model.JobOrderId });

                return Json(new { success = true, msg = _localizationService.GetResource("Web.JobOrder.EmailAFriend.EmailSent") }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_EmailJobOrderToFriend", model);
        }

        #endregion


        // Home page job search

        #region POST:/JobPost/GetAllJobCategories

        [HttpPost]
        public JsonResult GetAllJobCategories()
        {
            var categories = _jobOrderCategoryService.GetAllJobOrderCategories();

            var availableJobCategories = new List<SelectListItem>();
            availableJobCategories.Add(new SelectListItem { Text = "All Categories", Value = "0" });
            foreach (var item in categories)
            {
                availableJobCategories.Add(new SelectListItem { Text = item.CategoryName, Value = item.Id.ToString() });
            }

            return Json(availableJobCategories);
        }

        #endregion

        #region POST:/JobPost/GetLatestJobs

        [HttpPost]
        public JsonResult GetLatestJobs(int? id)
        {
            if (!id.HasValue)
                return new NullJsonResult();

            var jobs = _jobOrderService.GetLastPublishedJobOrders(id.Value, 0, 3); // get last three jobs

            var jobList = new List<SelectListItem>();
            foreach (var item in jobs)
            {
                jobList.Add(new SelectListItem { Text = item.JobTitle, Value = item.Id.ToString() });
            }

            return Json(jobList);
        }

        #endregion


        public ActionResult _LatestJobs(int pageSize = 4)
        {
            var jobOrderModels = new List<JobOrderModel>();

            var jobOrders = _jobOrderService.GetLastPublishedJobOrders(pageSize: pageSize);
            foreach (var item in jobOrders)
            {
                var model = item.ToModel();

                var jobOrderType = _jobOrderTypeService.GetJobOrderTypeById(item.JobOrderTypeId);
                if (jobOrderType != null)
                    model.JobOrderTypeModel = jobOrderType.ToModel();

                var companyLocation = _companyDivisionService.GetCompanyLocationById(item.CompanyLocationId);
                if (companyLocation != null)
                {
                    model.CompanyLocationModel = companyLocation.ToModel();

                    model.CompanyLocationModel.CountryModel =
                        _countryService.GetCountryById(companyLocation.CountryId).ToModel();
                    model.CompanyLocationModel.StateProvinceModel =
                        _stateProvinceService.GetStateProvinceById(companyLocation.StateProvinceId).ToModel();
                    model.CompanyLocationModel.CityModel =
                        _cityService.GetCityById(companyLocation.CityId).ToModel();
                }

                jobOrderModels.Add(model);
            }

            return PartialView(jobOrderModels);
        }

    }
}


using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.JobPosting;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.JobPosting;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Extensions;
using Wfm.Shared.Models.JobPosting;
using Wfm.Shared.Models.Search;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework.Feature;
using System.Linq;
using System.Text;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Job Posting")]
    public class JobPostingController : BaseClientController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly IAccountService _accountService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IWorkContext _workContext;
        private readonly IJobPostService _jobPostService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IShiftService _shiftService;
        private readonly IFranchiseService _franchiseService;
        private readonly IOrgNameService _orgNameService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly JobPosting_BL _jobPosting_BL;
        private readonly ILogger _logger;
        private readonly IJobOrderTypeService _jobOrderTypeService;
        
        #endregion


        #region Ctor

        public JobPostingController(IPermissionService permissionService,
                                    ICandidateService candidateService,
                                    ICandidateBlacklistService candidateBlacklistService,
                                    IAccountService accountService,
                                    IRecruiterCompanyService recruiterCompanyService,
                                    IWorkContext workContext,
                                    IJobPostService jobPostService,
                                    IJobOrderService jobOrderService,
                                    ICompanyDivisionService companyDivisionService,
                                    ICompanyDepartmentService companyDepartmentService,
                                    ICompanyBillingService companyBillingService,
                                    ICompanyVendorService companyVendorService,
                                    ICompanyService companyService,
                                    ICandidateJobOrderService candidateJobOrderService,
                                    IWorkTimeService workTimeService,
                                    IShiftService shiftService,
                                    IFranchiseService franchiseService,
                                    IOrgNameService orgNameService,
                                    ILocalizationService localizationService,
                                    IActivityLogService activityLogService,
                                    ICompanyContactService companyContactService,
                                    IWorkflowMessageService workflowMessageService,
                                    ILogger logger,
                                    IJobOrderTypeService jobOrderTypeService
                                    )
        {
            _permissionService = permissionService;
            _candidateService = candidateService;
            _candidateBlacklistService = candidateBlacklistService;
            _accountService = accountService;
            _recruiterCompanyService = recruiterCompanyService;
            _workContext = workContext;
            _jobPostService = jobPostService;
            _jobOrderService = jobOrderService;
            _companyDepartmentService = companyDepartmentService;
            _companyBillingService = companyBillingService;
            _companyDivisionService = companyDivisionService;
            _companyVendorService = companyVendorService;
            _companyService = companyService;
            _candidateJobOrderService = candidateJobOrderService;
            _companyContactService = companyContactService;
            _workTimeService = workTimeService;
            _shiftService = shiftService;
            _franchiseService = franchiseService;
            _orgNameService = orgNameService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _workflowMessageService = workflowMessageService;
            _logger = logger;
            _jobOrderTypeService = jobOrderTypeService;
            _jobPosting_BL = new JobPosting_BL(_candidateService, _candidateBlacklistService, _accountService, _recruiterCompanyService, _jobPostService, _jobOrderService, 
                                               _companyDivisionService, _companyDepartmentService, _companyBillingService, _companyVendorService, _companyService,
                                               _candidateJobOrderService, _workTimeService, _shiftService, _franchiseService,
                                               _localizationService, _activityLogService, _workflowMessageService, _workContext,_jobOrderTypeService);
        }

        #endregion


        #region Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            var from = DateTime.Today.AddMonths(-1).AddDays(1);
            var to = DateTime.Today.AddMonths(1).AddDays(-1);
            var searchBL = new SearchBusinessLogic(_workContext, _orgNameService);
            var model = searchBL.GetSearchJobPostingModel(from, to);

            return View(model);
        }


        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request, SearchJobPostingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedJson();

            var jobPosts = _jobPostService.GetAllJobPosts(_workContext.CurrentAccount.CompanyId, from: model.sf_Start, to: model.sf_End);

            KendoHelper.GetGeneralFilters(request, model, fromName: "Start", toName: "End");

            return Json(jobPosts.ToDataSourceResult(request, x => x.ToModel()));
        }

        #endregion


        #region Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            var companyId = _workContext.CurrentAccount.CompanyId;
            var model = _jobPosting_BL.CreateNewJobPost(companyId);
            ViewBag.Title = _localizationService.GetResource("Common.JobPosting.AddNewJobPosting");

            return View("CreateOrUpdate", model);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(JobPostingEditModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var guid = _jobPosting_BL.SaveJobPost(model);
                SuccessNotification("The new job posting is saved sucessfully!");
                return continueEditing ? RedirectToAction("Edit", new { guid = guid }) : RedirectToAction("Index");
            }

            ViewBag.Title = _localizationService.GetResource("Common.JobPosting.AddNewJobPosting");
            return View("CreateOrUpdate", model);
        }

        #endregion


        #region Edit

        public ActionResult Edit(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            var jobPost = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPost == null)
            {
                ErrorNotification("The job posting does not exist!");
                return RedirectToAction("Index");
            }
            if (jobPost.IsPublished)
            {
                ErrorNotification("The job posting has been published!");
                return RedirectToAction("Index");
            }

            var model = jobPost.ToEditModel();
            model.CompanyName = jobPost.Company.CompanyName;
            ViewBag.Title = _localizationService.GetResource("Common.JobPosting");

            return View("CreateOrUpdate", model);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(JobPostingEditModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            var jobPost = _jobPostService.RetrieveJobPost(model.Id);
            if (jobPost == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                var jpost = model.ToEntity(jobPost);
                jpost.UpdatedOnUtc = DateTime.UtcNow;

                _jobPostService.UpdateJobPost(jpost);
                if (jpost.IsSubmitted)
                {
                    _workflowMessageService.SendJobPostingCreateEditNotification(jpost, _workContext.WorkingLanguage.Id, false);
                }
                SuccessNotification(String.Format("Job posting {0} is updated!", model.Id));
                return continueEditing ? RedirectToAction("Edit", new { guid = jobPost.JobPostingGuid }) : RedirectToAction("Index");
            }

            ViewBag.Title = _localizationService.GetResource("Common.JobPosting");
            return View("CreateOrUpdate", model);
        }
        #endregion


        #region Details

        public ActionResult Details(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            JobPosting jobPost = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPost == null)
            {
                ErrorNotification("The job posting does not exist!");
                return RedirectToAction("Index");
            }

            JobPostingModel model = jobPost.ToModel();

            ViewBag.JobPostingGuid = guid;
            var today = DateTime.Today;
            ViewBag.StartDate = today > jobPost.StartDate ? (jobPost.EndDate.HasValue && today > jobPost.EndDate ? jobPost.EndDate : today) : jobPost.StartDate;
            ViewBag.Publishable = jobPost.JobPostingStatusId == (int)JobOrderStatusEnum.Active && (!jobPost.EndDate.HasValue || DateTime.Today <= jobPost.EndDate);

            return View(model);
        }


        [HttpPost]
        public JsonResult _GetPublishStatus([DataSourceRequest] DataSourceRequest request, Guid? guid, DateTime refDate)
        {
            var result = _jobPosting_BL.GetPublishStatus(guid, refDate);

            return Json(result.ToDataSourceResult(request));
        }


        public JsonResult GetJobOrderPlacedCandidates([DataSourceRequest] DataSourceRequest request, int jobOrderId, DateTime refDate)
        {
            var summary = _jobPosting_BL.JobOrderPlacedCandidateList(_workContext.CurrentAccount, jobOrderId, refDate);

            return Json(summary.ToDataSourceResult(request));
        }

        #endregion


        #region Cancel

        [HttpGet]
        public ActionResult _Cancel(Guid? guid, string formName)
        {
            ViewBag.JobPostingGuid = guid;
            ViewBag.FormName = formName;

            return PartialView();
        }


        [HttpPost, ActionName("_Cancel")]
        public ActionResult _CancelPost(Guid? guid, string note)
        {
            if (string.IsNullOrEmpty(note))
                return Json(new { Succeed = false, Error = "Please provide the reason!" });

            var post = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (post == null)
                return Json(new { Succeed = false, Error = "The Job Posting does not exist!" });

            var jobOrderList = _jobPostService.GetAllJobOrdersByJobPostingId(post.Id).ToList();
            _jobPostService.CancelJobPosting(post, jobOrderList.Count <= 0);
            // send email to supervisor
            _workflowMessageService.SendCancelJobPostingEmail(post, jobOrderList, _workContext.CurrentAccount,_workContext.WorkingLanguage.Id, note, out StringBuilder error);

            return Json(new { Succeed = error.Length == 0, Error = error.ToString() });               
        }

        #endregion


        #region Delete

        public ActionResult _Delete(Guid? guid, string formName)
        {
            ViewBag.JobPostingGuid = guid;
            ViewBag.FormName = formName;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _Delete(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedJson();

            var error = string.Empty;
            var jp = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jp == null)
                error = "Job posting does not exist!";
            else
            {
                jp.IsDeleted = true;
                _jobPostService.UpdateJobPost(jp);
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Submit

        [HttpGet]
        public ActionResult _Submit(Guid? guid, string formName)
        {
            ViewBag.JobPostingGuid = guid;
            ViewBag.FormName = formName;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _Submit(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedJson();

            var error = string.Empty;
            var jp = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jp == null)
                error = "Job posting does not exist!";
            else
            {
                jp.IsSubmitted = true;
                jp.UpdatedOnUtc = DateTime.Now;
                jp.SubmittedOnUtc = DateTime.UtcNow;
                jp.SubmittedBy = _workContext.CurrentAccount.Id;
                _jobPostService.UpdateJobPost(jp);
                _workflowMessageService.SendJobPostingCreateEditNotification(jp, 1, true);
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Copy

        public ActionResult CopyJobPosting(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            var model = _jobPosting_BL.GetCopyOfJobPosting(guid, out string errors, byClient: true);
            if (errors.Length > 0)
            {
                ErrorNotification(errors);
                return RedirectToAction("Index");
            }

            model.JobTitle = String.Concat("COPY OF ", model.JobTitle);
            ViewBag.Title = _localizationService.GetResource("Admin.JobPosting.CopyJobPosting");
            return View("CreateOrUpdate", model);
        }


        [HttpPost]
        public ActionResult CopyJobPosting(JobPostingEditModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var jpost = model.ToEntity();
                _jobPostService.InsertJobPost(jpost);               
                SuccessNotification(String.Format("Job Posting {0} is created!", jpost.Id));
                return RedirectToAction("Index");
            }

            ViewBag.Title = _localizationService.GetResource("Admin.JobPosting.CopyJobPosting");
            return View("CreateOrUpdate", model);
        }

        #endregion


        #region Reject Placement

        public ActionResult _RejectPlacement(int jobOrderId, int candidateId, DateTime refDate, string formName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            ViewBag.JobOrderId = jobOrderId;
            ViewBag.CandidateId = candidateId;
            ViewBag.RefDate = refDate;
            ViewBag.FormName = formName;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _RejectPlacement(int jobOrderId, int candidateId, DateTime refDate, string reason, string comment)
        {
            var error = String.Empty;

            try
            {
                error = _jobPosting_BL.RemoveCandidateFromPlacement(jobOrderId, candidateId, refDate, reason, comment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error("RemoveCandidateFromPlacement():", ex);
                error = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;
                
            }

            return Json(new { Succeed = String.IsNullOrEmpty(error), Error = error });
        }

        #endregion


        #region Publish
        [HttpPost]
        public ActionResult _PublishJobPosting([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<JobPostingPublishModel> models, DateTime refDate)
        {
            var msg = new List<string>();
            var jobOrderIds = new List<string>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                    try
                    {
                        int jobOrderId;
                        var result = _jobPosting_BL.PublishJobPosting(model, refDate, out jobOrderId);
                        if (!String.IsNullOrEmpty(result)) msg.Add(result);
                        if (jobOrderId > 0)
                            jobOrderIds.Add(Convert.ToString(jobOrderId));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("_PublishJobPosting():", ex);
                        msg.Add(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                    }

                // at least 1 job order generated
                if (msg.Count() < models.Count())
                    _jobPosting_BL.UpdateJobPostingPublishStatus(models.First().JobPostingId);

                if (jobOrderIds.Count > 0)
                    _jobPosting_BL.SendEmailNotificationForPublishJobPosting(jobOrderIds, models.First().JobPostingId, toClient: false);
            }

            if (msg.Count > 0)
                ModelState.AddModelError("JobOrderId", String.Join("\n", msg.Distinct()));

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion

    }
}

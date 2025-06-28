using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Wfm.Core;
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
using Wfm.Services.Accounts;
using Wfm.Web.Framework.Controllers;
using Wfm.Shared.Extensions;
using Wfm.Shared.Models.JobPosting;
using Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Admin.Controllers
{
    public class JobPostingController : BaseAdminController
    {
        #region Field

        private readonly IPermissionService _permissionService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly IAccountService _accountService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IJobPostService _jobPostService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly ICompanyService _companyService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IShiftService _shiftService;
        private readonly IFranchiseService _franchiseService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly ICompanyContactService _companyContactService;
        private readonly JobPosting_BL _jobPosting_BL;
        private readonly IJobOrderTypeService _jobOrderTypeService;
        #endregion

        #region Ctor

        public JobPostingController(IPermissionService permissionService,
                                    ICandidateService candidateService,
                                    ICandidateBlacklistService candidateBlacklistService,
                                    IAccountService accountService,
                                    IRecruiterCompanyService recruiterCompanyService,
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
                                    ILocalizationService localizationService,
                                    IActivityLogService activityLogService,
                                    IWorkflowMessageService workflowMessageService,
                                    IWorkContext workContext,
                                    ICompanyContactService companyContactService,
                                    IJobOrderTypeService jobOrderTypeService
                                    )
        {
            _permissionService = permissionService;
            _candidateService = candidateService;
            _candidateBlacklistService = candidateBlacklistService;
            _accountService = accountService;
            _recruiterCompanyService = recruiterCompanyService;
            _jobPostService = jobPostService;
            _jobOrderService = jobOrderService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyBillingService = companyBillingService;
            _companyVendorService = companyVendorService;
            _companyService = companyService;
            _candidateJobOrderService = candidateJobOrderService;
            _workTimeService = workTimeService;
            _shiftService = shiftService;
            _franchiseService = franchiseService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _companyContactService = companyContactService;
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
            
            return View();
        }

        
        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();
            
            var jobPosts = _jobPostService.GetAllJobPosts();

            return Json(jobPosts.ToDataSourceResult(request, x => x.ToModel()));
        }
        
        #endregion

        #region Create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();
            JobPostingEditModel model = _jobPosting_BL.CreateNewJobPost();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(JobPostingEditModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                model.SubmittedBy = _workContext.CurrentAccount.Id;
                Guid guid = _jobPosting_BL.SaveJobPost(model);
                SuccessNotification("A new job post has been created sucessfully!");
                return continueEditing ? RedirectToAction("Edit", new { guid = guid }) : RedirectToAction("Index");
            }
            return View(model);

        }


        public JsonResult GetCascadeContacts(string companyId, string locationId, string departmentId)
        {
            var cmpId = String.IsNullOrEmpty(companyId) ? 0 : Convert.ToInt32(companyId);
            int location = String.IsNullOrEmpty(locationId) ? 0 : Convert.ToInt32(locationId);
            int department = String.IsNullOrEmpty(departmentId) ? 0 : Convert.ToInt32(departmentId);

            var contactDropDownList = new List<SelectListItem>();
            // Add default zero value
            contactDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            var contacts = _companyContactService.GetCompanyContactsByCompanyIdAndLocationIdAndDepartmentId(cmpId,location,department);

            foreach (var c in contacts)
            {
                var item = new SelectListItem()
                {
                    Text = c.FullName,
                    Value = c.Id.ToString()
                };
                contactDropDownList.Add(item);
            }


            return Json(contactDropDownList, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Edit
        public ActionResult Edit(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            JobPosting jobPost = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPost == null)
            {
                ErrorNotification("The JobPosting does not exist!");
                return RedirectToAction("Index");
            }
            JobPostingEditModel model = jobPost.ToEditModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(JobPostingEditModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();
            JobPosting jobPost = _jobPostService.RetrieveJobPost(model.Id);
            if (jobPost == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                JobPosting jpost = model.ToEntity(jobPost);
                jpost.StartTime = model.StartDate.Date + model.StartTime.TimeOfDay;
                jpost.EndTime = model.StartDate.Date + model.EndTime.TimeOfDay;
                jpost.EndTime = jpost.EndTime.AddDays(model.EndTime.TimeOfDay < model.StartTime.TimeOfDay ? 1 : 0);
                jpost.UpdatedOnUtc = DateTime.UtcNow;

                _jobPostService.UpdateJobPost(jpost);
                SuccessNotification(String.Format("Job Post {0} has been updated!", model.Id));
                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion


        #region Publish

        public ActionResult Publish(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            JobPosting jobPost = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPost == null)
            {
                ErrorNotification("The JobPosting does not exist!");
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
            if (result == null)
                return Json(null);
            return Json(result.ToDataSourceResult(request));
        }


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
                        msg.Add(ex.Message);
                    }

                // at least 1 job order generated
                if (msg.Count() < models.Count())
                    _jobPosting_BL.UpdateJobPostingPublishStatus(models.First().JobPostingId);

                if (jobOrderIds.Count > 0)
                {
                    _jobPosting_BL.SendEmailNotificationForPublishJobPosting(jobOrderIds, models.First().JobPostingId); 
                }
            }

            if (msg.Count > 0)
                ModelState.AddModelError("JobOrderId", String.Join("\n", msg));

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        
        #endregion


        #region Close

        [HttpGet]
        public PartialViewResult _CloseJobPosting(Guid? guid)
        {
            var jobPosting = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPosting != null)
            {
                ViewBag.StartDate = jobPosting.StartDate;
                if (jobPosting.EndDate.HasValue)
                    ViewBag.EndDate = jobPosting.EndDate;
                //if (jobPosting.JobOrderCloseReason.HasValue)
                //    ViewBag.JobOrderCloseReason = ((int)(jobPosting.JobOrderCloseReason)).ToString();
            }

            return PartialView();
        }


        [HttpPost]
        public JsonResult _CloseJobPosting(Guid? guid, DateTime? CloseDate, int? CloseReason)
        {
            string errorMessage = string.Empty;

            if (!CloseDate.HasValue || !CloseReason.HasValue)
                errorMessage += "End Date and Close Reason is required.\r\n";

            if (errorMessage.Length == 0)
            {
                var jobPosting = _jobPostService.RetrieveJobPostingByGuid(guid);
                if (jobPosting == null)
                    errorMessage += "\r\nThe Job Posting does not exist !";
                else
                {
                    var result = _jobPosting_BL.CloseJobOrdersByJobPosting(jobPosting, CloseDate.Value, CloseReason.Value);
                    if (!String.IsNullOrEmpty(result))
                        errorMessage += result + "\r\n";
                }
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion
      

        #region Reject Placement


        public ActionResult _RejectPlacement(string comment)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();

            ViewBag.Comment = comment;

            return PartialView();
        }


        [HttpPost]
        public ActionResult _RejectPlacement(int jobOrderId, int candidateId, DateTime refDate, string reason, string comment)
        {
            string msg = String.Empty;

            try
            {
                msg = _jobPosting_BL.RemoveCandidateFromPlacement(jobOrderId, candidateId, refDate, reason, comment);
            }
            catch (InvalidOperationException ex)
            {
                msg = ex.Message;
            }

            return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
        }


        public JsonResult GetJobOrderPlacedCandidates([DataSourceRequest] DataSourceRequest request, int jobOrderId, DateTime refDate)
        {
            var summary = _jobPosting_BL.JobOrderPlacedCandidateList(_workContext.CurrentAccount, jobOrderId, refDate);

            return Json(summary.ToDataSourceResult(request));
        }

        #endregion


        #region Copy
        public ActionResult CopyJobPosting(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();
            string errors = string.Empty;
            JobPostingEditModel model = _jobPosting_BL.GetCopyOfJobPosting(guid, out errors);
            if (errors.Count() > 0)
            {
                ErrorNotification(errors);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CopyJobPosting(JobPostingEditModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobPosting))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                model.SubmittedBy = _workContext.CurrentAccount.Id;
                JobPosting jpost = model.ToEntity();
                _jobPostService.InsertJobPost(jpost);
                SuccessNotification(String.Format("Job Post {0} has been created!", jpost.Id));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

    }
}

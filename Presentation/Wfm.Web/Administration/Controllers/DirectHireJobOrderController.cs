using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Models.JobOrder;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.JobOrders;
using Wfm.Services.ExportImport;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Wfm.Admin.Extensions;
using Wfm.Services.Accounts;
using Wfm.Services.Seo;
using Wfm.Services.Candidates;
using Wfm.Admin.Models.Candidate;
using Wfm.Services.Franchises;
using Wfm.Services.Companies;


namespace Wfm.Admin.Controllers
{
    public class DirectHireJobOrderController : BaseAdminController
    {

        #region Fields
        private readonly IDirectHireJobOrderService _directHireJobOrderService;
        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly DirectHireJobOrder_BL _directHireJobOrder_BL;
        private readonly IPermissionService _permissionService;
        private readonly IAccountService _accountService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateDirectHireStatusHistoryService _candidateDirectHireStatusHistoryService;
        private readonly IJobOrderStatusService _jobOrderStatusService;
        private readonly ILogger _logger;
        private readonly IExportManager _exportManager;
        private readonly IFranchiseService _franchiseService;
        private readonly ICompanyService _companyService;
        #endregion


        #region Ctr
        public DirectHireJobOrderController(
            IDirectHireJobOrderService directHireJobOrderService,
            IWorkContext workContext,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            IAccountService accountService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateDirectHireStatusHistoryService candidateDirectHireStatusHistoryService,
            ILogger logger,
            IExportManager exportManager,
            IJobOrderStatusService jobOrderStatusService,
            IFranchiseService franchiseService,
            ICompanyService companyService

          )
        {
            _directHireJobOrderService = directHireJobOrderService;
            _workContext = workContext;
            _activityLogService = activityLogService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _urlRecordService = urlRecordService;
            _accountService = accountService;
            _candidateJobOrderService = candidateJobOrderService;
            _candidateDirectHireStatusHistoryService = candidateDirectHireStatusHistoryService;

            _logger = logger;
            _exportManager = exportManager;
            _jobOrderStatusService = jobOrderStatusService;
            _franchiseService = franchiseService;
            _companyService = companyService;
            _directHireJobOrder_BL = new DirectHireJobOrder_BL(_workContext, _directHireJobOrderService, _activityLogService, _localizationService, _urlRecordService, _accountService, _candidateDirectHireStatusHistoryService, _candidateJobOrderService, _jobOrderStatusService, _franchiseService, _companyService);
        }
        #endregion
        // GET: DirectHireJobOrder
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();

            var result = _directHireJobOrder_BL.GetAllDirectHireJobOrderList(request, startDate, endDate);
            return Json(result);
        }

        #region GET :/DirectHireJobOrder/Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();
            DirectHireJobOrderModel model = _directHireJobOrder_BL.CreateNewDirectHireJobOrderModel();
            return View(model);
        }

        #endregion
        #region POST:/DirectHireJobOrder/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(DirectHireJobOrderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();


            if (ModelState.IsValid)
            {
                //add new
                var directHireJobOrder = _directHireJobOrder_BL.CreateDirectHireJobOrder(model);
                SuccessNotification(_localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Added"));
                return continueEditing ? RedirectToAction("Edit", new { guid = directHireJobOrder.JobOrderGuid }) : RedirectToAction("Details", new { guid = directHireJobOrder.JobOrderGuid });
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region GET :/DirectHireJobOrder/Details

        [HttpGet]
        public ActionResult Details(Guid? guid, string tabId)
        {

            var model = _directHireJobOrder_BL.GetDirectHireJobOrderDetail(guid);

            if (model == null)
            {
                ErrorNotification("Direct hire job order not found!");
                return RedirectToAction("Index");
            }
            return View(model);

        }
        #endregion

        #region GET :/DirectHireJobOrder/Edit/

        [HttpGet]
        public ActionResult Edit(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();

            DirectHireJobOrderModel model = _directHireJobOrder_BL.GetEditDirectHireJobOrder(guid);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        #endregion

        #region POST:/JobOrder/Edit/5

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(DirectHireJobOrderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();

            JobOrder directHireJobOrder = _directHireJobOrderService.GetDirectHireJobOrderById(model.Id);
            if (directHireJobOrder == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                var result = _directHireJobOrder_BL.UpdateDirectHireJobOrder(model, directHireJobOrder);
                SuccessNotification(_localizationService.GetResource("Admin.JobOrder.DirectHireJobOrder.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { guid = result.JobOrderGuid }) : RedirectToAction("Index");
            }
            return View(model);
        }

        #endregion

        // Job order pipeline

        #region GET :/JobOrder/_TabJobOrderPipeline
        public ActionResult _TabJobOrderPipeline(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();
            var model = _directHireJobOrder_BL.GetJobOrderCandidateModel(guid);
            if (model == null)
            {
                ErrorNotification("The job order does not exist!");
                return new EmptyResult();
            }
            ViewBag.JobOrderGuid = guid;
            ViewBag.CompanyId = model.CompanyId;
            ViewBag.JobOrderId = model.JobOrderId;
            return PartialView("_TabJobOrderPipeline", model);
        }


        [HttpPost]
        public ActionResult DirectHireJobOrderPlacedCandidates([DataSourceRequest] DataSourceRequest request, Guid? guid)
        {
            var result = _directHireJobOrder_BL.DirectHireJobOrderPlacedCandidates(guid);
            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _AddCandidateIntoPipeline(Guid? guid, int candidateId)
        {
            string msg = String.Empty;
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
            {
                msg = String.Format("Cannot move {0} into/out pipeline since you do not have the permission to change the placement!", candidateId);
                return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
            }
            try
            {
                var jobOrder = _directHireJobOrderService.GetDirectHireJobOrderByGuid(guid);
                if (jobOrder == null)
                {
                    ErrorNotification("The job order does not exist!");
                    return new EmptyResult();
                }
                msg = _directHireJobOrder_BL.AddCandidateIntoPipeline(jobOrder.Id, candidateId);
            }
            catch (Exception ex)
            {
                msg = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;
                _logger.Error("_AddCandidateIntoPipeline()", ex, userAgent: Request.UserAgent);
            }

            return Json(new { Result = String.IsNullOrEmpty(msg), ErrorMessage = msg });
        }


        [HttpPost]
        public ActionResult GetDirectHireCandidatePoolList([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDirectHireJobOrders))
                return AccessDeniedView();

            var result = _directHireJobOrder_BL.GetDirectHireCandidatePoolList(request);
            return Json(result);
        }

        public ActionResult _DirectHireCandidatePool(Guid? joOrderGuid)
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetCandidateDirectHireStatusHistory([DataSourceRequest] DataSourceRequest request, int candidateId, int jobOrderId)
        {
            var result = _directHireJobOrder_BL.GetCandidateDirectHireStatusHistory(request, candidateId, jobOrderId);
            return Json(result);
        }

        [HttpGet]
        public ActionResult _CreateEditCandidateDirectHireStatusHistory(int historyId, int candidateId, int jobOrderId)
        {
            if (historyId > 0)
            {
                var result = _candidateDirectHireStatusHistoryService.GetCandidateDirectHireStatusHistoryById(historyId);
                return PartialView(result.ToModel());
            }
            else
            {
                CandidateDirectHireStatusHistoryModel model = new CandidateDirectHireStatusHistoryModel();
                model.CandidateId = candidateId;
                model.JobOrderId = jobOrderId;
                return PartialView(model);
            }
        }

        [HttpPost]
        public ActionResult _CreateEditCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistoryModel candidateDirectHireStatusHistory)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (candidateDirectHireStatusHistory.Id == 0)
                    {
                        _directHireJobOrder_BL.InsertCandidateDirectHireStatusHistory(candidateDirectHireStatusHistory, candidateDirectHireStatusHistory.CandidateId, candidateDirectHireStatusHistory.JobOrderId);
                    }
                    else
                    {
                        _directHireJobOrder_BL.UpdateCandidateDirectHireStatusHistory(candidateDirectHireStatusHistory, candidateDirectHireStatusHistory.CandidateId, candidateDirectHireStatusHistory.JobOrderId);
                    }
                    return Content("done");
                }
                catch (Exception ex)
                {
                    _logger.Error("_CreateEditCandidateDirectHireStatusHistory()", ex, userAgent: Request.UserAgent);
                    return Content(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            else
            {
                return PartialView("_CreateEditCandidateDirectHireStatusHistory", candidateDirectHireStatusHistory);
            }

        }
        #endregion


        #region Invoice

        [HttpPost]
        public ActionResult DirectPlacementInvoice(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApproveTimeSheet))
                return AccessDeniedView();

            var urlString = Request.UrlReferrer.ToString();

            // Export to Xlsx
            try
            {
                var placements = _directHireJobOrder_BL.GetCandidateDirectHireStatusHistoriesByPipelineIds(selectedIds);
                if (placements.Any())
                {
                    string fileName = "DirectPlacemetnInvoice.xlsx";
                    byte[] bytes = null;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.DirectPlacementInvoiceXlsx(stream, placements);
                        bytes = stream.ToArray();
                    }

                    // set invoice date
                    _candidateDirectHireStatusHistoryService.SetDirectHireInvoiceDate(placements, DateTime.Today);

                    // return File(bytes, "text/xls", fileName);
                    return File(bytes, "text/xls", fileName);
                }
                else
                    ErrorNotification("No valid direct hire records.");
            }
            catch (WfmException exc)
            {
                ErrorNotification(exc.Message);
            }
            catch (Exception exc)
            {
                ErrorNotification(_localizationService.GetResource("Common.UnexpectedError"));
                _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
            }

            return Redirect(urlString);
        }

        #endregion

    }
}
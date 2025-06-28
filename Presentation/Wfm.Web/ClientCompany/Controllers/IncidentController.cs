using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Wfm.Client.Extensions;
using Wfm.Client.Models.Incident;
using Wfm.Core;
using Wfm.Services.Companies;
using Wfm.Services.Incident;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Shared.Mapping;
using Wfm.Shared.Models.Incident;
using Wfm.Shared.Models.Search;
using Wfm.Services.Messages;
using Wfm.Web.Framework.Feature;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Incident")]
    public class IncidentController : BaseClientController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IIncidentService _incidentService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly ICompanyDivisionService _locationService;
        private readonly ILogger _logger;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IOrgNameService _orgNameService;

        #endregion


        #region Ctor

        public IncidentController(
            IWorkContext workContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IIncidentService incidentService,
            ICompanyCandidateService companyCandidateService,
            ICompanyDivisionService locationService,
            IWorkflowMessageService workflowMessageService,
            ILogger logger,
            IOrgNameService orgNameService
        )
        {
            _workContext = workContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _incidentService = incidentService;
            _companyCandidateService = companyCandidateService;
            _locationService = locationService;
            _logger = logger;
            _workflowMessageService = workflowMessageService;

            _orgNameService = orgNameService;
        }

        #endregion


        #region List

        private SearchIncidentModel _GetSearchIncidentModel(DateTime? from = null, DateTime? to = null)
        {
            var model = new SearchIncidentModel(from, to);
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            var searchBL = new SearchBusinessLogic(_workContext, _orgNameService);
            searchBL.PopulateSearchPlacementModel(model);

            model.AvailableIncidentCategories = _incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true)
                .Select(x => new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

            return model;
        }


        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            return View(_GetSearchIncidentModel());
        }


        [HttpPost]
        public ActionResult _CompanyIncidentsList([DataSourceRequest] DataSourceRequest request, SearchIncidentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedJson();

            var fromUtc = model.sf_From;                            //.ToUniversalTime();
            var toUtc = model.sf_To.AddDays(1).AddSeconds(-1);      //.ToUniversalTime();
            var incidents = _incidentService.GetCompanyIncidentReports(_workContext.CurrentAccount.CompanyId)
                .Where(x => x.IncidentDateTimeUtc >= fromUtc && x.IncidentDateTimeUtc <= toUtc);

            var mapping = new Dictionary<string, string>()
            {
                { "EmployeeId", "Candidate.EmployeeId" },
                { "EmployeeFirstName", "Candidate.FirstName" },
                { "EmployeeLastName", "Candidate.LastName" },
                { "CompanyLocationId", "LocationId" }
            };
            KendoHelper.CustomizePlacementBasedFilters(request, model, mapping: mapping);

            return Json(incidents.ToDataSourceResult(request, x => x.ToModel()));
        }

        #endregion


        #region Add / Edit / Remove

        private void _PopulateIncidentReportModel(IncidentReportModel model)
        {
            model.CandidateList = new SelectList(_companyCandidateService.GetCompanyCandidatePool(model.CompanyId)
                .Select(x => new { Id = x.CandidateId, Name = x.Candidate.FirstName + " " + x.Candidate.LastName })
                .OrderBy(x => x.Name).ToArray(), "Id", "Name");
            model.IncidentCategoryList = new SelectList(_incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true).ToArray(), 
                "Id", "Description");
            model.LocationList = new SelectList(_locationService.GetAllCompanyLocationsByCompanyId(model.CompanyId).OrderBy(x => x.LocationName).ToArray(),
                "Id", "LocationName");
        }


        public ActionResult _NewIncidentReport(string viewName, string panelName = "addIncident")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            int companyId = _workContext.CurrentAccount.CompanyId;
            var model = new IncidentReportModel()
            {
                CompanyId = companyId,
                IncidentDateTimeUtc = DateTime.Today
            };
            _PopulateIncidentReportModel(model);

            if (!String.IsNullOrWhiteSpace(panelName))
                ViewBag.PanelName = panelName;

            return PartialView(!String.IsNullOrWhiteSpace(viewName) ? viewName : "_CreateOrUpdateIncident", model);
        }

        public ActionResult _EditIncidentReport(int id, string formName, string panelName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            var model = _incidentService.GetIncidentReport(id).ToModel();
            _PopulateIncidentReportModel(model);

            if (!String.IsNullOrWhiteSpace(formName))
                ViewBag.FormName = formName;
            if (!String.IsNullOrWhiteSpace(panelName))
                ViewBag.PanelName = panelName;

            return PartialView("_CreateOrUpdateIncident", model);
        }


        [HttpPost]
        public ActionResult _EditIncidentReport(IncidentReportModel model, bool returnViewOnErr = false)
        {
            var errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.ReportedByAccountId = _workContext.CurrentAccount.Id;
                try
                {
                    if (model.Id == 0)
                    {
                        _incidentService.InsertIncidentReport(entity);
                        //Send Notification to HR
                        _workflowMessageService.SendNotificationWhenNewIncidentCreatedByClient(entity, _workContext.WorkingLanguage.Id);
                    }
                    else
                        _incidentService.UpdateIncidentReport(entity);
                }
                catch (Exception ex)
                {
                    errorMessage = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;
                    _logger.Error("_EditIncidentReport()", ex, userAgent: Request.UserAgent);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_EditIncidentReport():" + errorMessage, userAgent: Request.UserAgent);
            }

            var succeed = String.IsNullOrWhiteSpace(errorMessage);
            if (succeed || !returnViewOnErr)
                return Json(new { Succeed = succeed, Error = errorMessage }, JsonRequestBehavior.AllowGet);
            else
            {
                ModelState.AddModelError("", errorMessage);
                _PopulateIncidentReportModel(model);
                return PartialView(model.Id == 0 ? "_AddIncident" : "_EditIncidentReport", model);
            }
        }


        [HttpPost]
        public ActionResult _RemoveIncidentReport(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedJson();

            if (incidentReportId > 0)
                _incidentService.DeleteIncidentReport(incidentReportId);

            return Json(new { Succeed = true });
        }

        #endregion


        #region Files

        public ActionResult _ManageIncidentReportFiles(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            ViewBag.IncidentReportId = id;
            var model = _incidentService.GetIncidentReportFiles(id).AsEnumerable().Select(x => x.ToModel());

            return PartialView("_AddRemoveIncidentReportFile", model);
        }


        [HttpPost]
        public ActionResult SaveIncidentReportFiles(IEnumerable<HttpPostedFileBase> files, int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedJson();

            var incident_BL = new Incident_BL(_incidentService, _workContext);
            var result = incident_BL.SaveIncidentReportFiles(files, incidentReportId);

            return Json(new { Succeed = String.IsNullOrWhiteSpace(result), Error = result });
        }


        [HttpPost]
        public ActionResult DeleteIncidentReportFile(int reportFileId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedJson();

            _incidentService.RemoveIncidentReportFile(reportFileId);

            return Json(new { Succeed = true });

        }


        public ActionResult DownloadIncidentReportFile(int reportFileId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            Incident_BL companyBillingRate_BL = new Incident_BL(_incidentService, _workContext);
            companyBillingRate_BL.DownloadIncidentReportFile(reportFileId, Response);
            return new EmptyResult();
        }

        #endregion


        #region Templates

        public ActionResult DownloadTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;
            return View(new IncidentReportTemplateModel[] { });
        }


        [HttpPost]
        public ActionResult _DownloadTemplatesList([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            int companyId = _workContext.CurrentAccount.CompanyId;
            var entities = _incidentService.GetIncidentTemplatesForCompany(companyId, true).ToArray();
            var model = new List<IncidentReportTemplateModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }


        public ActionResult DownloadIncidentReportTemplate(int templateId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            var file = _incidentService.GetIncidentReportTemplate(templateId);
            Response.ClearContent();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
            //  manually added System.Web to this project's References.
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.AddHeader("content-disposition", "attachment; filename=" + file.FileName);
            Response.ContentType = MimeMapping.GetMimeMapping(file.FileName);

            Response.BinaryWrite(file.TemplateStream);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        }

        #endregion
    }
}
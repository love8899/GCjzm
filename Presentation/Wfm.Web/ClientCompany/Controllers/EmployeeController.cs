using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Wfm.Client.Extensions;
using Wfm.Client.Models.Employees;
using Wfm.Core;
using Wfm.Services.Companies;
using Wfm.Services.Employees;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Shared.Mapping;
using Wfm.Shared.Models.Employees;
using Wfm.Web.Framework;
using Wfm.Services.Candidates;
using Wfm.Core.Domain.Media;
using Kendo.Mvc.Extensions;
using Wfm.Shared.Models.Incident;
using System.Web;
using Wfm.Services.Incident;
using Wfm.Services.Logging;
using Wfm.Services.Franchises;
using Wfm.Services.Media;
using Wfm.Client.Models.Incident;
using Wfm.Web.Framework.Feature;
using Wfm.Services.Accounts;
using System.Text;
using Wfm.Services.Scheduling;

namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Employee")]
    public class EmployeeController : BaseClientController
    {
        #region Fields

        private readonly IEmployeeService _employeeService;
        private readonly ICompanyService _companyService;
        private readonly ITimeoffService _timeoffService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly IIncidentService _incidentService;
        private readonly ISchedulingDemandService _schedulingDemandService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly IFranchiseService _franchiseService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;
        private readonly IAttachmentService _attachmentService;
        private readonly IWebHelper _webHelper;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        #endregion

        #region Ctor

        public EmployeeController(IEmployeeService employeeService,
            ICompanyService companyService,
            ITimeoffService timeoffService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            ICandidateService candidateService,
            ICandidatePictureService candidatePictureService,
            IIncidentService incidentService,
            ISchedulingDemandService schedulingDemandService,
            MediaSettings mediaSettings,
            IWorkContext workContext,
            IFranchiseService franchiseService,
            ICandidateKeySkillService candidateKeySkillsService,
            ILogger logger,
            IAttachmentService attachmentService,
            IWebHelper webHelper,
            IAccountPasswordPolicyService accountPasswordPolicyService
        )
        {
            _employeeService = employeeService;
            _companyService = companyService;
            _timeoffService = timeoffService;
            _companyDivisionService = companyDivisionService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _candidateService = candidateService;
            _candidatePictureService = candidatePictureService;
            _incidentService = incidentService;
            _schedulingDemandService = schedulingDemandService;
            _mediaSettings = mediaSettings;
            _workContext = workContext;
            _logger = logger;
            _franchiseService = franchiseService;
            _candidateKeySkillsService = candidateKeySkillsService;
            _attachmentService = attachmentService;
            _webHelper = webHelper;
            _accountPasswordPolicyService = accountPasswordPolicyService;
        }

        #endregion

        #region GET :/Employee/Index
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();

            return View();
        }
        #endregion

        #region POST:/Employee/Index

        [HttpPost]
        public ActionResult ListGlobalEmployees([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();

            var employees = _employeeService.GetAllEmployeesByCompany(_workContext.CurrentAccount.CompanyId, true).PagedForCommand(request);

            var modelList = new List<EmployeeGridModel>();
            foreach (var item in employees)
            {
                modelList.Add(item.ToGridModel());
            }

            var result = new DataSourceResult()
            {
                Data = modelList,
                Total = employees.TotalCount,
            };

            return Json(result);
        }

        #endregion

        #region Details
        public ActionResult Details(Guid guid)
        {
            var model = _employeeService.GetEmployeeByGuid(guid).ToModel();
            // employee picture
            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(model.Id, 1).FirstOrDefault();
            model.PictureThumbnailUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize, true);
            return View(model);
        }
        #endregion



        #region Timeoff
        public ActionResult _TabEmployeeTimeoff(Guid employeeId)
        {
            ViewBag.EmployeeId = employeeId;
            return PartialView();
        }
        [HttpPost]
        public ActionResult _GetEmployeeTimeoffEntitlement([DataSourceRequest] DataSourceRequest request, Guid employeeId, int? year = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var entities = _timeoffService.GetEntitlement(candidateId, year.GetValueOrDefault(DateTime.Today.Year));
            return Json(entities.ToDataSourceResult(request,m=>m.ToModel()));
        }
        public ActionResult _UpdateEmployeeTimeoffEntitlement([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<EmployeeTimeoffBalanceModel> entitlements)
        {
            if (entitlements != null && ModelState.IsValid)
            {
                var entities = entitlements.Select(x => x.ToEntity()).ToList();
                entities = _timeoffService.UpdateEntitlements(entities).ToList();
            }
            return Json(entitlements.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult _GetEmployeeTimeoffBookHistory([DataSourceRequest] DataSourceRequest request, Guid employeeId, int? year = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var entities = _timeoffService.GetBookHistoryByEmployee(candidateId, year.GetValueOrDefault(DateTime.Today.Year));

            return Json(entities.ToDataSourceResult(request,m=>m.ToModel()));
        }

        [HttpPost]
        public ContentResult GetHoursBetweenDates(DateTime start, DateTime end, int employeeId, int thisBookingId, bool startHalf, bool endHalf)
        {
            start = start.Date.AddHours(startHalf ? 12 : 0);
            end = end.Date.AddHours(endHalf ? 12 : 23);
            return Content(_timeoffService.GetHoursBetweenDates(employeeId, start, end, thisBookingId).ToString());
        }

        public ActionResult _BookNewTimeoffPopup(Guid employeeId, int timeoffTypeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            EmployeeTimeOff_BL bl = new EmployeeTimeOff_BL(_candidateService, _timeoffService, _employeeService,_schedulingDemandService);
            var model = bl.BookNewTimeoffPopup(employeeId, timeoffTypeId);
            return PartialView("_BookTimeoffPopup", model);
        }

        [HttpPost]
        public ActionResult _GetEmployeeScheduleForTimeoffBooking(EmployeeTimeoffBookingModel model)
        {
            EmployeeTimeOff_BL bl = new EmployeeTimeOff_BL(_candidateService, _timeoffService, _employeeService,_schedulingDemandService);
            var result = bl.GetEmployeeScheduleForTimeoffBooking(model);
            return Json(new { Schedule = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _SaveTimeoffBooking(EmployeeTimeoffBookingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    if (entity.Id > 0)
                    {
                        _timeoffService.UpdateTimeoffBooking(entity);
                    }
                    else
                    {
                        _timeoffService.BookNewtTimeoff(entity);
                    }
                    return Content("done");
                }
                catch (Exception ex)
                {
                    _logger.Error("_SaveTimeoffBooking()", ex, userAgent: Request.UserAgent);
                    return Content(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            else
            {
                model.TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == model.EmployeeTimeoffTypeId }).ToArray();
                return PartialView("_BookTimeoffPopup", model);
            }
        }

        public ActionResult _EditTimeoffPopup(int timeoffBookingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var model = _timeoffService.GetTimeoffBookingById(timeoffBookingId).ToEditModel();
            model.TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == model.EmployeeTimeoffTypeId }).ToArray();
            return PartialView("_BookTimeoffPopup", model);
        }
        #endregion

        #region Employee CRUD
        public ActionResult _BasicInfoEdit(Guid guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var model = _employeeService.GetEmployeeByGuid(guid).ToModel();
            model.Password = model.RePassword = "******";
            //model.PasswordPolicyModel = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel();
            // employee picture
            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(model.Id, 1).FirstOrDefault();
            model.PictureThumbnailUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize, true);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult UpdateEmployee(EmployeeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _candidateService.GetCandidateByIdForClient(model.Id);
                _employeeService.UpdateEmployee(model.ToEntity(entity), model.HireDate, model.TerminationDate, model.CompanyLocationId, model.PrimaryJobRoleId);
                return Content("done");
            }
            else
            {
                string errors = String.Join(" | ", ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage)));
                _logger.Error(String.Concat("UpdateEmployee():",errors ));
                ModelState.AddModelError("", errors);
                return PartialView("_BasicInfoEdit", model);
            }
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var model = new EmployeeModel();
            //model.PasswordPolicyModel = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel();
            // employee picture
            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(0, 1).FirstOrDefault();
            model.PictureThumbnailUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult InsertEmployee(EmployeeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            StringBuilder passwordErrors = new StringBuilder();
            
            var passwordPolicy = _accountPasswordPolicyService.ApplyPasswordPolicy(0, "Candidate", model.Password, String.Empty, Core.Domain.Accounts.PasswordFormat.Clear, String.Empty, out passwordErrors);
            if (passwordErrors.Length > 0)
                ModelState.AddModelError("", passwordErrors.ToString());

            if (ModelState.IsValid)
            {
                model.EnteredBy = _workContext.CurrentAccount.Id;
                model.EnteredByUserName = _workContext.CurrentAccount.FullName;
                var entity = model.ToEntity(null);
                entity.OwnerId = _workContext.CurrentAccount.Id;
                _employeeService.AddNewEmployee(entity, _workContext.CurrentAccount.CompanyId, model.HireDate, model.CompanyLocationId, model.PrimaryJobRoleId);
                return Content("done");
            }
            else
            {
                string errors = String.Join(" | ", ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage)));
                _logger.Error(String.Concat("InsertEmployee():", errors));
                //delete all password
                model.Password = model.RePassword = null;
                return PartialView("_BasicInfoEdit", model);
            }
        }
        #endregion

        #region GET:/Employee/_TabIncidents
        public ActionResult _TabEmployeeIncidents(Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            ViewBag.CandidateGuid = employeeId;
            return PartialView(new IncidentReportModel[] { });
        }
        [HttpPost]
        public ActionResult _TabEmployeeIncidentsList([DataSourceRequest] DataSourceRequest request, Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuidForClient(candidateGuid).Id;
            var entities = _incidentService.GetCandidateIncidentReports(candidateId);
            var model = new List<IncidentReportModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }
        public ActionResult _NewIncidentReport(Guid candidateGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            int companyId = _workContext.CurrentAccount.CompanyId;
            int candidateId = _candidateService.GetCandidateByGuidForClient(candidateGuid).Id;
            var model = new IncidentReportModel()
            {
                CompanyId = companyId,
                CandidateId = candidateId,
                IncidentCategoryList = new SelectList(_incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true).ToArray(),
                    "Id", "Description"),
                LocationList = new SelectList(_companyDivisionService.GetAllCompanyLocationsByCompanyId(companyId).OrderBy(x => x.LocationName).ToArray(),
                    "Id", "LocationName"),
                IncidentDateTimeUtc = DateTime.Today
            };
            return PartialView("_CreateOrUpdateIncident", model);
        }
        public ActionResult _EditIncidentReport(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            var model = _incidentService.GetIncidentReport(incidentReportId).ToModel();
            model.IncidentCategoryList = new SelectList(_incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true).ToArray(),
                    "Id", "Description");
            model.LocationList = new SelectList(_companyDivisionService.GetAllCompanyLocationsByCompanyId(_workContext.CurrentAccount.CompanyId)
                .OrderBy(x => x.LocationName).ToArray(), "Id", "LocationName");
            return PartialView("_CreateOrUpdateIncident", model);
        }
        [HttpPost]
        public ActionResult _EditIncidentReport(IncidentReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            var entity = model.ToEntity();
            entity.ReportedByAccountId = _workContext.CurrentAccount.Id;
            if (model.Id == 0)
            {
                _incidentService.InsertIncidentReport(entity);
            }
            else
            {
                _incidentService.UpdateIncidentReport(entity);
            }
            return Content("done");
        }
        [HttpPost]
        public ActionResult _RemoveIncidentReport(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            if (incidentReportId > 0)
            {
                _incidentService.DeleteIncidentReport(incidentReportId);
            }
            return Content("done");
        }

        public ActionResult _ManageIncidentReportFiles(int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();
            ViewBag.IncidentReportId = incidentReportId;
            var model = _incidentService.GetIncidentReportFiles(incidentReportId).ToArray().Select(x => x.ToModel());
            return PartialView("_AddRemoveIncidentReportFile", model);
        }
        public ActionResult SaveIncidentReportFiles(IEnumerable<HttpPostedFileBase> files, int incidentReportId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            Incident_BL companyBillingRate_BL = new Incident_BL(_incidentService, _workContext);
            string result = companyBillingRate_BL.SaveIncidentReportFiles(files, incidentReportId);
            return Content(result);
        }

        public ActionResult DeleteIncidentReportFile(int reportFileId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyIncidents))
                return AccessDeniedView();

            _incidentService.RemoveIncidentReportFile(reportFileId);

            // Return an empty string to signify success
            return Content("");
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

        #region _TabWorktimePreference
        public ActionResult _TabWorktimePreference(Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var model = _employeeService.GetWorktimePreference(candidateId).ToArray().Select(x => x.ToModel());
            ViewBag.EmployeeId = employeeId;
            ViewBag.CandidateId = candidateId;
            return PartialView(model);
        }
        [HttpPost]
        public virtual ActionResult ReadWorktimePreference([DataSourceRequest] DataSourceRequest request, Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var model = _employeeService.GetWorktimePreference(candidateId).ToArray().Select(x => x.ToModel());

            var result = new DataSourceResult()
            {
                Data = model,
                Total = model.Count(),
            };
            return Json(result);
        }       
        #endregion

        #region Job role
        public ActionResult _TabEmployeeJobRole(Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            //var model = _employeeService.GetWorktimePreference(employeeId).ToArray().Select(x => x.ToModel());
            ViewBag.EmployeeId = employeeId;
            return PartialView(null);
        }
        [HttpPost]
        public virtual ActionResult _TabEmployeeJobRole([DataSourceRequest] DataSourceRequest request, Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();

            int candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var model = _employeeService.GetEmployeJobRoles(candidateId).ToArray().Select(x => x.ToModel());

            var result = new DataSourceResult()
            {
                Data = model,
                Total = model.Count(),
            };
            return Json(result);
        }
        [HttpGet]
        public ActionResult _NewJobRole(Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            int candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var model = new EmployeeJobRoleModel()
            {
                EmployeeIntId = candidateId,
            };
            return PartialView("_CreateEditJobRole", model);
        }
        [HttpGet]
        public ActionResult _EditJobRole(int jobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var entity = _employeeService.GetJobRoleById(jobRoleId);
            var model = entity.ToModel();
            return PartialView("_CreateEditJobRole", model);
        }
        [HttpPost]
        public ActionResult _EditJobRole(EmployeeJobRoleModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    if (model.Id == 0)
                    {
                        _employeeService.AddEmployeeJobRole(model.EmployeeIntId, entity);
                    }
                    else
                    {
                        _employeeService.UpdateEmployeeJobRole(entity);
                    }
                    return Content("done");
                }
                catch (Exception ex)
                {
                    _logger.Error("_EditJobRole()", ex, userAgent: Request.UserAgent);                  
                    return Content(_localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_EditJobRole():" + errorMessage, userAgent: Request.UserAgent);
                return PartialView("_CreateEditJobRole", model);
            }
        }
        [HttpPost]
        public ActionResult _RemoveJobRole(int jobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            if (jobRoleId > 0)
            {
                _employeeService.DeleteEmployeeJobRole(jobRoleId);
            }
            return Content("done");
        }
        #endregion

        #region Schedule 
        public ActionResult _TabEmployeeSchedule(Guid employeeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientScheduling) &&
                !_permissionService.Authorize(StandardPermissionProvider.ManageSchedulingPlacement))
                return AccessDeniedView();
            ViewBag.EmployeeId = employeeId;
            ViewBag.CandidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            return PartialView(null);
        }
        #endregion

        #region Batch edit for Schedule Preference
        [HttpPost]
        public ActionResult CreateWorktimePreference([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<EmployeeAvailabilityModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            var results = new List<EmployeeAvailabilityModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    if (model.IsAllDay)
                    {
                        model.StartTimeOfDay = DateTime.Today.Add(TimeSpan.FromHours(0));
                        model.EndTimeOfDay = DateTime.Today.Add(TimeSpan.FromMinutes(23 * 60 + 59));
                    }
                    var entity = model.ToEntity();

                    _employeeService.AddWorktimePreference(entity);
                    model.Id = entity.Id;
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult UpdateWorktimePreference([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<EmployeeAvailabilityModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    if (model.IsAllDay)
                    {
                        model.StartTimeOfDay = DateTime.Today.Add(TimeSpan.FromHours(0));
                        model.EndTimeOfDay = DateTime.Today.Add(TimeSpan.FromMinutes(23 * 60 + 59));
                    }

                    var entity = model.ToEntity();

                    _employeeService.UpdateWorktimePreference(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult DeleteWorktimePreference([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<EmployeeAvailabilityModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientEmployee))
                return AccessDeniedView();
            if (models.Any())
            {
                foreach (var model in models)
                {
                    //var entity = _employeeService.GetWorktimePreferenceById(model.Id);
                    _employeeService.DeleteWorktimePreference(model.Id);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}
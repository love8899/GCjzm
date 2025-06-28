using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Client.Extensions;
using Wfm.Client.Models.Companies;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;
using Wfm.Shared.Mapping;
using Wfm.Shared.Models.Companies;
using Wfm.Services.Common;
using Wfm.Services.Scheduling;
using Wfm.Web.Framework.Feature;
using Wfm.Core.Infrastructure;
using Wfm.Client.Models.Franchises;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Company")]
    public class CompanyController : BaseClientController
    {
        #region Fields

        private readonly ICompanyService _companyService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ISkillService _skillService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly ILogger _logger;
        private readonly IFranchiseService _franchiseService;
        private readonly ISchedulingDemandService _schedulingDemandService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly IVendorCertificateService _vendorCertificateService;
        private readonly string[] ResourceColors = new string[] {
                    "#E1F5FE", "#E0F7FA", "#E3F2FD", "#E0F2F1",
                    "#B3E5FC", "#B2EBF2", "#BBDEFB", "#B2DFDB",
                    "#81D4FA", "#80DEEA", "#90CAF9", "#80CBC4",
                    "#4FC3F7", "#4DD0E1", "#64B5F6", "#4DB6AC",
                    "#29B6F6", "#26C6DA", "#42A5F5", "#26A69A"
                };
        #endregion

        #region Ctor

        public CompanyController(ICompanyService companyService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            ICompanyContactService companyContactService,
            ISkillService skillService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICompanyBillingService companyBillingService,
            ILogger logger,
            IFranchiseService franchiseService,
            ISchedulingDemandService schedulingDemandService,
            ICompanyVendorService companyVendorService,
             IVendorCertificateService vendorCertificateService
            )
        {
            _companyService = companyService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyContactService = companyContactService;
            _skillService = skillService;
            _workContext = workContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _companyBillingService = companyBillingService;
            _logger = logger;
            _franchiseService = franchiseService;
            _schedulingDemandService = schedulingDemandService;
            _companyVendorService = companyVendorService;
            _vendorCertificateService = vendorCertificateService;

        }

        #endregion

        #region GET :/Company/Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanies))
                return AccessDeniedView();

            if (_workContext.CurrentAccount.CompanyId == 0)
                return AccessDeniedView();

            return RedirectToAction("Details");
        }

        #endregion

        #region GET :/Company/Detials
        public ActionResult Details()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanies))
                return AccessDeniedView();

            int Id = _workContext.CurrentAccount.CompanyId;
            Company company = _companyService.GetCompanyById(Id);

            if (company == null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Companies.Company.Error"));

                CompanyModel model = new CompanyModel();
                return View(model);
            }

            CompanyModel companyModel = company.ToModel();

            return View(companyModel);
        }
        #endregion

        #region GET :/CompanyDetails/_CompanyLocationList
        [HttpGet]
        public ActionResult _CompanyLocationList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanies))
                return AccessDeniedView();

            return View();
        }
        #endregion

        #region POST:/CompanyDetails/_CompanyLocationList
        [HttpPost]
        public ActionResult _CompanyLocationList([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanies))
                return AccessDeniedView();

            var companyLocations = _companyDivisionService.GetAllCompanyLocationsByCompanyId(_workContext.CurrentAccount.CompanyId);
            return Json(companyLocations.ToDataSourceResult(request,x=>x.ToModel()));
        }


        #endregion

        #region GET :/CompanyDetails/_CompanyDepartmentList

        [HttpGet]
        public ActionResult _CompanyDepartmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanies))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/CompanyDetails/_CompanyDepartmentList

        [HttpPost]
        public ActionResult _CompanyDepartmentList([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanies))
                return AccessDeniedView();

            var companyDepartments = _companyDepartmentService.GetAllCompanyDepartmentsByCompanyId(_workContext.CurrentAccount.CompanyId);
            return Json(companyDepartments.ToDataSourceResult(request,x=>x.ToModel()));
        }

        #endregion

        #region JSON: _DepartmentsByLocation

        [HttpPost]
        public ActionResult _DepartmentsByLocation(int locationId)
        {
            var result = _companyDepartmentService.GetAllCompanyDepartmentByLocationId(locationId)
                         .Select(x => new { Id = x.Id, DepartmentName = x.DepartmentName });

            return Json(result);
        }

        #endregion


        #region /Company/Details/_TabCompanyBillingRates
        [HttpGet]
        public ActionResult _TabCompanyBillingRates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyBillingRates))
                return AccessDeniedView();
  
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabCompanyBillingRates([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyBillingRates))
                return AccessDeniedView();
            CompanyBillingRate_BL companyBillingRate_BL = new CompanyBillingRate_BL(_companyBillingService, _companyDivisionService);

            DataSourceResult result = companyBillingRate_BL.GetAllCompanyBillingRates(request, _workContext.CurrentAccount.CompanyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region /Company/Details/_TabCompanyVendors

        [HttpGet]
        public ActionResult _TabCompanyVendors()
        { 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientVendors))
                return AccessDeniedView();

            return PartialView();
        }

        [HttpPost]
        public ActionResult _CompanyVendorList([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientVendors))
                return AccessDeniedView();

            var vendors = _companyVendorService.GetAllCompanyVendorsByCompanyId(_workContext.CurrentAccount.CompanyId);
            return Json(vendors.ToDataSourceResult(request, x => x.ToModel()));
        }
       
        #region /Vendor/_CertificateList
        [HttpPost]
        public ActionResult _CertificateList(DataSourceRequest request, Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientVendors))
                return AccessDeniedView();
            if (guid == null)
            {
                ErrorNotification("The vendor does not exist!");
                return RedirectToAction("Index");
            }
            var vendor = _franchiseService.GetFranchiseByGuid(guid);
            if (vendor == null)
                return Json(null);
            var entities = _vendorCertificateService.GetAllCertificatesByVendorId(vendor.Id).ToList();
            var models = AutoMapper.Mapper.Map<List<VendorCertificate>, List<VendorCertificateModel>>(entities).OrderByDescending(x => x.GeneralLiabilityCertificateExpiryDate);
            return Json(models.ToDataSourceResult(request));
        }
        #endregion

        #region Download Certificate
        public ActionResult _DownloadCertificate(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientVendors))
                return AccessDeniedView();
            if (guid == null)
            {
                ErrorNotification("The certificate does not exist!");
                return new EmptyResult();
            }
            var certificate = _vendorCertificateService.Retrive(guid);
            if (certificate == null)
            {
                ErrorNotification("The certificate does not exist!");
                return new EmptyResult();
            }
            if (certificate.HasCertificate)
            {
                return File(certificate.CertificateFile, certificate.ContentType, certificate.CertificateFileName);
            }
            else
            {
                ErrorNotification("The certificate file does not exist!");
                return new EmptyResult();
            }
        }
        #endregion

        #endregion

        #region GetAllVendors
        public ActionResult GetAllVendors()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientCompanyBillingRates))
                return AccessDeniedView();

            var franchises = _franchiseService.GetAllFranchisesAsSelectList(_workContext.CurrentAccount, true);

            return Json(franchises, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region GetCascadeLocations

        public JsonResult GetCascadeDepartments(int locationId, bool addNone = true)
        {
            var departments = Enumerable.Empty<SelectListItem>().ToList();
            if (locationId > 0)
                departments = _companyDepartmentService.GetAllCompanyDepartmentByLocationId(locationId).OrderBy(x => x.DepartmentName)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.DepartmentName,
                        Value = x.Id.ToString()
                    }).ToList();

            if (addNone)
                departments.Insert(0, new SelectListItem() { Text = "None", Value = "0" });

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeDeptsByLoc(string location, bool idVal = true)
        {
            var departments = Enumerable.Empty<SelectListItem>().ToList();
            if (!String.IsNullOrWhiteSpace(location))
                departments = _companyDepartmentService.GetAllCompanyDepartmentByLocationName(location).OrderBy(x => x.DepartmentName)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.DepartmentName,
                        Value = idVal ? x.Id.ToString() : x.DepartmentName
                    }).ToList();

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region _TabCompanyJobRoles
        [HttpGet]
        public ActionResult _TabCompanyJobRoles()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobRole))
                return AccessDeniedView();

            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabCompanyJobRoles([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobRole))
                return AccessDeniedView();
            var entities = _companyService.GetAllJobRolesByAccount(_workContext.CurrentAccount);
            var model = new List<CompanyJobRoleModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }

        [HttpGet]
        public ActionResult _NewJobRole()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobRole))
                return AccessDeniedView();
            int companyId = _workContext.CurrentAccount.CompanyId;

            var model = new CompanyJobRoleModel()
            {
                CompanyId = companyId,
                LocationList = new SelectList(_companyDivisionService.GetAllCompanyLocationsByAccount(_workContext.CurrentAccount).OrderBy(x => x.LocationName).ToArray(),
                    "Id", "LocationName"),
                SkillList = new SelectList(_skillService.GetAllSkills().OrderBy(x => x.SkillName).ToArray(),
                    "Id", "SkillName"),
                SelectedSkillList = new SelectListItem[] { },
            };
            return PartialView("_CreateEditJobRole", model);
        }

        [HttpGet]
        public ActionResult _EditJobRole(int jobRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobRole))
                return AccessDeniedView();
            var entity = _companyService.GetJobRoleById(jobRoleId);
            var model = entity.ToModel();
            model.LocationList = new SelectList(_companyDivisionService.GetAllCompanyLocationsByAccount(_workContext.CurrentAccount).OrderBy(x => x.LocationName).ToArray(),
                    "Id", "LocationName");
            model.SkillList = new SelectList(_skillService.GetAllSkills().OrderBy(x => x.SkillName).ToArray(),
                    "Id", "SkillName");
            model.SelectedSkillList = new SelectList(entity.RequiredSkills, "Skill.Id", "Skill.SkillName");
            return PartialView("_CreateEditJobRole", model);
        }

        [HttpPost]
        public ActionResult _EditJobRole(CompanyJobRoleModel model)
        {
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                var requiredSkillIds = model.RequiredSkillIds != null ? model.RequiredSkillIds.Select(x => int.Parse(x)).ToArray() : new int[] { };
                try
                {
                    if (model.Id == 0)
                    {
                        _companyService.InsertCompanyJobRole(entity, requiredSkillIds);
                    }
                    else
                    {
                        _companyService.UpdateCompanyJobRole(entity, requiredSkillIds);
                    }
                    return Content("done");
                }
                catch (Exception ex)
                {                  
                    errorMessage = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;
                    _logger.Error("_EditJobRole()", ex, userAgent: Request.UserAgent);
                    return Content(errorMessage);
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
        public ActionResult _RemoveJobRole(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobRole))
                return AccessDeniedView();
            if (Id > 0)
            {
                _companyService.DeleteCompanyJobRole(Id);
            }
            return new EmptyResult();
        }
        #endregion

        #region _TabCompanyJobShifts
        [HttpGet]
        public ActionResult _TabCompanyJobShifts()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();
         
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabCompanyJobShifts([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();

            var entities = _companyService.GetAllShiftsByAccount(_workContext.CurrentAccount);
            var model = new List<CompanyShiftModel>();
            foreach (var e in entities)
            {
                model.Add(e.ToModel());
            }
            return Json(model.ToDataSourceResult(request));
        }

        public ActionResult GetAllCompanyShifts(int schedulePeriodId)
        {
            var schedulePeriod = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);
            var entities = _companyService.GetAllShifts(_workContext.CurrentAccount.CompanyId); //.Where(x=>x.CompanyLocationId==schedulePeriod.CompanyDepartment.CompanyLocation.Id);
            var result = entities.Select(x => new SelectListItem() { Text = x.Shift.ShiftName, Value = x.Id.ToString() }).ToArray();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllCompanyShiftsWithColor(int schedulePeriodId)
        {
            var range = this.ResourceColors.Length;
            var schedulePeriod = _schedulingDemandService.GetSchedulePeriodById(schedulePeriodId);
            var entities = _companyService.GetAllShifts(_workContext.CurrentAccount.CompanyId).ToArray();
              //  .Where(x => schedulePeriod.CompanyDepartment == null || x.CompanyLocationId == schedulePeriod.CompanyDepartment.CompanyLocation.Id).ToArray();
            var results = new List<CompanyShiftWithColor>();
            for (int i = 0; i < entities.Count(); i++)
            {
                CompanyShiftWithColor result = new CompanyShiftWithColor();
                result.Text = entities[i].Shift.ShiftName;
                result.Value = entities[i].Id;
                result.Color = ResourceColors[i % range];
                results.Add(result);
            }
            //var result = entities.Select(x => new { Text = x.Shift.ShiftName, Value = x.Id.ToString(), Color = colorName }).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult _NewJobShift()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();
            var model = new CompanyShiftModel()
            {
                CompanyId = _workContext.CurrentAccount.CompanyId,
            };
            return PartialView("_CreateEditJobShift", model);
        }
        [HttpGet]
        public ActionResult _EditJobShift(int jobShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();
            var entity = _companyService.GetCompanyShiftById(jobShiftId);
            var model = entity.ToModel();
            return PartialView("_CreateEditJobShift", model);
        }
        [HttpPost]
        public ActionResult _EditJobShift(CompanyShiftModel model)
        {
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                try
                {
                    if (model.Id == 0)
                    {
                        _companyService.InsertNewCompanyShift(entity);
                    }
                    else
                    {
                        _companyService.UpdateCompanyShift(entity);
                    }
                    return Content("done");
                }
                catch (Exception ex)
                {
                    errorMessage = _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue;                  
                    _logger.Error("_EditJobShift()", ex, userAgent: Request.UserAgent);
                    return Content(errorMessage);
                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.ErrorMessage));
                errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                _logger.Error("_EditJobShift():" + errorMessage, userAgent: Request.UserAgent);
                return PartialView("_CreateEditJobShift", model);
            }
        }
        [HttpPost]
        public ActionResult _RemoveJobShift(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();
            if (Id > 0)
            {
                _companyService.DeleteCompanyShift(Id);
            }
            return new EmptyResult();
        }
        #endregion

        #region EditingShiftJobRole
        private AccountDropdownModel DefaultSupervisor()
        {
            return EngineContext.Current.Resolve<ICompanyContactService>().GetCompanyContactsByCompanyId(_workContext.CurrentAccount.CompanyId)
                .Select(x => new AccountDropdownModel { Id = x.Id, Name = x.FullName }).FirstOrDefault();
        }
        [HttpGet]
        public ActionResult _EditJobShiftRoles(int jobShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();
            var entity = _companyService.GetCompanyShiftById(jobShiftId);
            var jobRoles = _companyService.GetAllJobRolesByAccount(_workContext.CurrentAccount)
                .Where(x => x.CompanyLocationId == entity.CompanyDepartment.CompanyLocationId)
                .ToArray().Select(x => x.ToDropdownModel());
            ViewBag.DefaultJobRole = jobRoles.FirstOrDefault();
            ViewBag.DefaultSupervisor = DefaultSupervisor();
            ViewBag.CompanyJobRoles = jobRoles;
            ViewBag.JobShiftId = jobShiftId;
            return PartialView("_CreateEditJobShiftRoles");
        }
        public ActionResult EditingShiftJobRole([DataSourceRequest] DataSourceRequest request, int jobShiftId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageClientJobShift))
                return AccessDeniedView();
            var entities = _companyService.GetJobRolesOfShift(jobShiftId);
            var model = new List<CompanyShiftJobRoleModel>();
            var defaultSupervisor = DefaultSupervisor();
            foreach (var e in entities)
            {
                var entry = e.ToModel();
                if (entry.Supervisor == null)
                {
                    entry.Supervisor = defaultSupervisor;
                    entry.SupervisorId = defaultSupervisor.Id;
                }
                model.Add(entry);
            }
            return Json(model.ToDataSourceResult(request));
        }
        [HttpPost]
        public ActionResult EditingShiftJobRoleInsert([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<CompanyShiftJobRoleGridModel> jobRoles, int jobShiftId)
        {
            if (jobRoles != null && ModelState.IsValid)
            {
                var _jobRoles = jobRoles.Select(x => new CompanyShiftJobRoleModel
                {
                    CompanyShiftId = jobShiftId,
                    CompanyJobRoleId = x.CompanyJobRole.Id,
                    MandantoryRequiredCount = x.MandantoryRequiredCount,
                    ContingencyRequiredCount = x.ContingencyRequiredCount,
                    SupervisorId =  x.Supervisor.Id,
                });
                _companyService.InsertCompanyShiftJobRoles(jobShiftId, _jobRoles.Select(x => x.ToEntity()));
            }
            return Json(jobRoles.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult EditingShiftJobRoleUpdate([DataSourceRequest] DataSourceRequest request, 
            [Bind(Prefix = "models")]IEnumerable<CompanyShiftJobRoleGridModel> jobRoles, int jobShiftId)
        {
            if (jobRoles != null && ModelState.IsValid)
            {
                var _jobRoles = jobRoles.Select(x => new CompanyShiftJobRoleModel
                {
                    Id = x.Id,
                    CompanyShiftId = jobShiftId,
                    CompanyJobRoleId = x.CompanyJobRole.Id,
                    MandantoryRequiredCount = x.MandantoryRequiredCount,
                    ContingencyRequiredCount = x.ContingencyRequiredCount,
                    SupervisorId = x.Supervisor.Id,
                }); 
                _companyService.UpdateCompanyShiftJobRoles(jobShiftId, _jobRoles.Select(x => x.ToEntity()));
            }
            return Json(jobRoles.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult EditingShiftJobRoleDelete([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<CompanyShiftJobRoleGridModel> jobRoles)
        {
            if (jobRoles != null)
            {
                var _jobRoles = jobRoles.Select(x => new CompanyShiftJobRoleModel
                {
                    Id = x.Id,
                });
                _companyService.DeleteCompanyShiftJobRoles(_jobRoles.Select(x => x.ToEntity()));
            }
            return Json(jobRoles.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region JSON: _JobRolesByLocation

        [HttpPost]
        public ActionResult _JobRolesByLocation(int locationId)
        {
            var result = _companyService.GetAllJobRoles(_workContext.CurrentAccount.CompanyId)
                         .Where(x => x.CompanyLocationId == locationId).Select(x => new { Id = x.Id, Name = x.Name });

            return Json(result);
        }

        #endregion

        #region JSON: _SupervisorsByLocation

        [HttpPost]
        public ActionResult _SupervisorsByDepartment(int departmentId)
        {
            var result = _companyContactService.GetAllCompanyContactsByAccountAsQueryable(_workContext.CurrentAccount)
                         .Where(x => x.CompanyDepartmentId == departmentId && 
                                     x.AccountRoles.Any(y => y.SystemName != AccountRoleSystemNames.ClientAdministrators))
                         .Select(x => new { Id = x.Id, FullName = x.FirstName + " " + x.LastName });

            return Json(result);
        }

        #endregion

        #region ExportExcel
        public ActionResult ExportExcel(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        #endregion

    }
} 

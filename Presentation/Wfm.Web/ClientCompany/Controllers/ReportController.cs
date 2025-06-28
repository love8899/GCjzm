using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Client.Models.JobOrder;
using Wfm.Core;
using Wfm.Data;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Reports;
using Wfm.Services.Scheduling;
using Wfm.Services.Security;
using Wfm.Shared.Models.Search;
using Wfm.Web.Framework.Feature;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Report")]
    public class ReportController : BaseClientController
    {

        #region Fields

        private IReportService _reportService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyLocationService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly IOrgNameService _orgNameService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ISchedulingDemandService _schedulingDemandService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IDbContext _dbContext;

        #endregion


        #region Ctor

        public ReportController(
            IReportService reportService,
            ICompanyService companyService,
            ICompanyDivisionService companyLocationService,
            ICompanyDepartmentService companyDepartmentService,
            ICompanyContactService companyContactService,
            ICompanyVendorService companyVendorService,
            IOrgNameService orgNameService,
            IWorkContext workContext,
            IPermissionService permissionService,
            ISchedulingDemandService schedulingDemandService,
            IJobOrderService jobOrderService,
            IDbContext dbContext)
        {
            _reportService = reportService;
            _companyService = companyService;
            _companyLocationService = companyLocationService;
            _companyDepartmentService = companyDepartmentService;
            _companyContactService = companyContactService;
            _companyVendorService = companyVendorService;
            _orgNameService = orgNameService;
            _workContext = workContext;
            _permissionService = permissionService;
            _schedulingDemandService = schedulingDemandService;
            _jobOrderService = jobOrderService;
            _dbContext = dbContext;
        }

        #endregion


        // GET: Report
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            return View();
        }
        [HttpGet]
        public ActionResult GetTurnoverChart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        public ActionResult _TurnoverChart(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView("_TurnoverChart");
        }
        [HttpGet]
        public ActionResult GetComplianceReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        public ActionResult _ComplianceReport(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView("_ComplianceReport");
        }

        #region Daily Attendance
        [HttpGet]
        public ActionResult GetAttendanceReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            var companyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.Vendors = _companyVendorService.GetAllCompanyVendorsByCompanyIdAsSelectList(_workContext.CurrentAccount.CompanyId);
            ViewBag.Locations = _companyLocationService.GetAllCompanyLocationsByCompanyIdAsSelectList(companyId);
            ViewBag.Departments = _companyDepartmentService.GetAllCompanyDepartmentsByCompanyIdAsSelectList(companyId);
            ViewBag.Supervisors = _companyContactService.GetCompanyContactsByCompanyIdAsSelectList(companyId);

            return View();
        }

        [HttpPost]
        public ActionResult _AttendanceReport(string vendorIds, string locationIds, string departmentIds, string supervisorIds, string groupBy, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId.ToString();
            ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            ViewBag.VendorIds = vendorIds;
            ViewBag.LocationIds = locationIds;
            ViewBag.DepartmentIds = departmentIds;
            ViewBag.SupervisorIds = supervisorIds;

            ViewBag.GroupBy = groupBy;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return PartialView("_AttendanceReport");
        }

        #endregion


        #region Daily Time Sheet

        public ActionResult GetDailyTimeSheetReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult _DailyTimeSheetReport(DateTime startDate, DateTime endDate, string status)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId.ToString();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Status = status;
            ViewBag.HoursOnly = !_permissionService.Authorize(StandardPermissionProvider.ClientHRReports);

            return PartialView();
        }
        
        #endregion


        #region Weekly Cost Report
        [HttpGet]
        public ActionResult GetWeeklyReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();
            ViewBag.Franchises = _companyVendorService.GetAllCompanyVendorsByCompanyIdAsSelectList(_workContext.CurrentAccount.CompanyId);
            return View();
        }
        [HttpPost]
        public ActionResult _WeeklyCostReport(DateTime refDate, string vendorIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();

            ViewBag.refDate = refDate;
            ViewBag.vendorIds = vendorIds;
            ViewBag.companyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView();
        }
        #endregion


        #region Detailed Labour Cost Report
        public ActionResult GetDetailedCostReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();
            ViewBag.Franchises = _companyVendorService.GetAllCompanyVendorsByCompanyIdAsSelectList(_workContext.CurrentAccount.CompanyId);
            return View();
        }
        [HttpPost]
        public ActionResult _DetailedLabourCostReport(DateTime startDate, DateTime endDate, string vendorIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.VendorIds = vendorIds;
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId.ToString();
            ViewBag.companyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView();
        }
        #endregion


        #region Job Order Fill-in Rate
        [HttpGet]
        public ActionResult GetJobOrderFillInRate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _JobOrderFillInRate(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId.ToString();
            //ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView("_JobOrderFillInRate");
        }

        #endregion
        [HttpGet]
        public ActionResult GetEmployeesListReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        public ActionResult _EmployeesListReport(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.CompanyName = _companyService.GetCompanyById( _workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetTotalHoursBySupervisor()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();
            return View();
        }
        [HttpPost]
        public ActionResult _TotalHoursBySupervisor(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            //ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView("_TotalHoursBySupervisor");
        }

        [HttpGet]
        [FeatureAuthorize("Scheduling")]
        public ActionResult GetEmployeesScheduleReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();


            return View();
        }
        [HttpPost]
        public ActionResult _EmployeesScheduleReport(DateTime startDate, DateTime endDate, string employeeIds,bool breakPage) 
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.EmployeeIds = employeeIds;
            ViewBag.BreakPage = breakPage;
            ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView("_EmployeesScheduleReport");
        }
        [HttpGet]
        public ActionResult GetEmployeesForSchedule(DateTime startDate, DateTime endDate)
        {
            var result = _schedulingDemandService.GetEmployeeListForScheduleFilter().Distinct().ToList();
            var employeesDropDownList = new List<SelectListItem>();          
            foreach (var e in result)
            {
                var item = new SelectListItem()
                {
                    Text = string.Format("{0} {1} ({2})", e.FirstName, e.LastName,e.EmployeeId),
                    Value = e.Id.ToString()
                };
                employeesDropDownList.Add(item);
            }
            return Json(employeesDropDownList, JsonRequestBehavior.AllowGet);
        }


        #region Job orders
        [HttpGet]
        public ActionResult GetJobOrderReport() 
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            var companyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.Vendors = _companyVendorService.GetAllCompanyVendorsByCompanyIdAsSelectList(_workContext.CurrentAccount.CompanyId);
            ViewBag.Locations = _companyLocationService.GetAllCompanyLocationsByCompanyIdAsSelectList(companyId);      
            return View();
        }

        [HttpPost]
        public ActionResult _JobOrderReport(string vendorIds, string locationIds, string groupBy, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientReports))
                return AccessDeniedView();

            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId.ToString();
            ViewBag.VendorIds = vendorIds;
            ViewBag.LocationIds = locationIds;         

            ViewBag.GroupBy = groupBy;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyName = _companyService.GetCompanyById(_workContext.CurrentAccount.CompanyId).CompanyName;
            return PartialView("_JobOrderReport"); 
        }

        #endregion


        #region GetBudgetForcastingReport

        public ActionResult GetBudgetForcastingReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();

            var searchBL = new SearchBusinessLogic(_workContext, _orgNameService);

            // if using client opertation for grid, or search by name, set idVal = false !!!
            var model = searchBL.GetSearchPlacementModel(DateTime.Today, DateTime.Today, idVal: false);

            return View(model);
        }


        [HttpPost]
        public ActionResult GetAllBudgetForcasting([DataSourceRequest]DataSourceRequest request, SearchPlacementModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedJson();

            var bl = new BudgetForcastingModel_BL();
            var result = bl.GetAllBudgetForcastingModel(_jobOrderService, _workContext, _companyLocationService, _companyDepartmentService);

            //// if search by Id (for Location, Department,Position, Shift, etc.)
            //KendoHelper.CustomizePlacementBasedFilters(request, model,
            //    skip: new List<string>() { "Start", "End" },
            //    nameCols: new List<string>() { "CompanyLocationId", "CompanyDepartmentId", "PositionId", "ShiftId" });

            // if search by name (for Location, Department,Position, Shift, etc.)
            KendoHelper.CustomizePlacementBasedFilters(request, model);

            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult CalculateBudget([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<BudgetForcastingModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                BudgetForcastingModel_BL bl = new BudgetForcastingModel_BL();
                foreach (var model in models)
                {
                    //calculate the budget
                    bl.CalculateBudget(model, _dbContext);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region DNRListReport
        public ActionResult GetDNRListReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _DNRListReport(DateTime refDateStart, DateTime refDateEnd)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ClientHRReports))
                return AccessDeniedView();
            
            ViewBag.CompanyId = _workContext.CurrentAccount.CompanyId;
            ViewBag.RefDateStart = refDateStart;
            ViewBag.RefDateEnd = refDateEnd;

            
            return PartialView();
        }
        #endregion
    }
}

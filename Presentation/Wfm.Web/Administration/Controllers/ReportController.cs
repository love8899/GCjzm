using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Admin.Models.Report;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
//using Wfm.Services.Reports;
using System.Linq;
using Wfm.Services.ExportImport;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Web.Framework.Feature;
using Wfm.Services.Payroll;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Admin.Controllers
{
    public class ReportController : BaseAdminController
    {
        #region Fields

        //private IReportService _reportService;
        private readonly ICompanyService _companyService;
        private readonly IWorkContext _workContext;
        private readonly IFranchiseService _franchiseService;
        private readonly IPermissionService _permissionService;
        private readonly IPayGroupService _payGroupService;
        private readonly IPayrollCalendarService _payrollCalendarService;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IMessageCategoryService _messageCategoryService;
        private readonly IAccountService _accountService;
        private readonly IExportManager _exportManager;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ReportController(
            //IReportService reportService,
            ICompanyService companyService,
            IWorkContext workContext,
            IFranchiseService franchiseService,
             IPermissionService permissionService,
            IPayGroupService payGroupService,
            IPayrollCalendarService payrollCalendarService,
            IPaymentHistoryService paymentHistoryService,
            IRecruiterCompanyService recruiterCompanyService,
            IMessageCategoryService messageCategoryService,
            IAccountService accountService,
            IExportManager exportManager,
            IQueuedEmailService queuedEmailService,
            ILocalizationService localizationService,
            ILogger logger)
        {
           // _reportService = reportService;
            _companyService = companyService;
            _workContext = workContext;
            _franchiseService = franchiseService;
            _permissionService = permissionService;
            _payGroupService = payGroupService;
            _payrollCalendarService = payrollCalendarService;
            _paymentHistoryService = paymentHistoryService;
            _recruiterCompanyService = recruiterCompanyService;
            _messageCategoryService = messageCategoryService;
            _accountService = accountService;
            _exportManager = exportManager;
            _queuedEmailService = queuedEmailService;
            _localizationService = localizationService;
            _logger = logger;
        }

        #endregion

        #region Helper Methods

        private void SetVendorInViewBag(int vendorId)
        {
            if (_workContext.CurrentAccount.IsLimitedToFranchises)
            {
                ViewBag.VendorId = _workContext.CurrentAccount.FranchiseId;
                ViewBag.VendorName = _workContext.CurrentFranchise.FranchiseName;
            }
            else
            {
                var vendor = _franchiseService.GetFranchiseById(vendorId);
                if (vendor != null)
                {
                    ViewBag.VendorName = vendor.FranchiseName;
                    ViewBag.VendorId = vendor.Id;
                }
                else
                {
                    ViewBag.VendorName = String.Empty;
                    ViewBag.VendorId = 0;
                }
            }
        }


        /// <summary>
        /// Get company Id list as string, to be passed to report SP; for multi select
        /// </summary>
        private string _GetCompanyIds(string companyIds)
        {
            return !String.IsNullOrWhiteSpace(companyIds) ? companyIds : _GetCompanyIds(_workContext.CurrentAccount);
        }

        /// <summary>
        /// Get company Id list as string, to be passed to report SP; for dropdown list
        /// </summary>
        private string _GetCompanyIds(int? companyId)
        {
            return companyId.HasValue && companyId.Value > 0 ? companyId.Value.ToString() : _GetCompanyIds(_workContext.CurrentAccount);
        }


        private string _GetCompanyIds(Account account)
        {
            var assigned = _recruiterCompanyService.GetCompanyIdsByRecruiterId(_workContext.CurrentAccount.Id);
            var accessible = _companyService.Secure_GetAllCompanies(_workContext.CurrentAccount).Select(x => x.Id);

            return assigned.Any() ? String.Join(",", assigned) : (accessible.Any() ? null : "-1");
        }

        #endregion

        public ActionResult Index()
        {
            return AccessDeniedView();
            //_reportService = new ReportService();
            //var reportInst = _reportService.LoadReportMethodBySystemName("Reports.BillingChart");
            ////var reportInst = _reportService.LoadReportMethodBySystemName("JobOrder.AttendantList");
            //if (reportInst == null)
            //{
            //    throw new Exception("Loading Fail");
            //}

            //var ReportInfoModel = PrepareReportInfoModel(reportInst);
            //return View(ReportInfoModel);
        }

        [HttpGet]
        public ActionResult GetTotalHoursByCandidate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _TotalHoursByCandidate(DateTime startDate, DateTime endDate, int companyId = 0, int threshold = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyIds = _GetCompanyIds(companyId);
            ViewBag.Threshold = threshold;

            return PartialView("_TotalHoursByCandidate");
        }


        [HttpGet]
        public ActionResult GetTotalHoursBySupervisor()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _TotalHoursBySupervisor(DateTime startDate, DateTime endDate, string ids)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Ids = ids;
            return PartialView("_TotalHoursBySupervisor");
        }

        [HttpGet]
        public ActionResult GetJobOrderFillInRate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _JobOrderFillInRate(DateTime startDate, DateTime endDate, string companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = _GetCompanyIds(companyId);

            return PartialView("_JobOrderFillInRate");
        }

        [HttpGet]
        public ActionResult GetAvailableEmployees()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _AvailableEmployeesReport(DateTime refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.RefDate = refDate;
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetTurnoverChart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _TurnoverChart(DateTime startDate, DateTime endDate, string companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(Convert.ToInt32(companyId)).CompanyName;
            return PartialView("_TurnoverChart");
        }

        [HttpGet]
        public ActionResult GetComplianceReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _ComplianceReport(DateTime startDate, DateTime endDate, string companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(Convert.ToInt32(companyId)).CompanyName;
            return PartialView("_ComplianceReport");
        }


        #region Client Temporary Staff Assignment

        [HttpGet]
        public ActionResult GetClientTemporaryStaffAssignment()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _ClientTemporaryStaffAssignment(int franchiseId, int companyId, DateTime? refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.RefDate = (refDate ?? DateTime.Today).ToShortDateString();
            ViewBag.FranchiseId = franchiseId;
            ViewBag.FranchiseName = _franchiseService.GetFranchiseById(franchiseId).FranchiseName;
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(companyId).CompanyName;

            return PartialView();
        }

        #endregion


        #region Daily Time Sheets With Rate

        [HttpGet]
        public ActionResult GetDailyTimeSheetsWithRate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _DailyTimeSheetsWithRate(int franchiseId, int companyId, DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.FranchiseId = franchiseId;
            ViewBag.FranchiseName = _franchiseService.GetFranchiseById(franchiseId).FranchiseName;
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(companyId).CompanyName;
            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();

            return PartialView();
        }

        #endregion


        [HttpGet]
        public ActionResult GetCandidateJobOrderStatusHistoryReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _CandidateJobOrderStatusHistoryReport(DateTime startDate, DateTime endDate, string companyIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.FranchiseId = _workContext.CurrentAccount.IsLimitedToFranchises ? _workContext.CurrentAccount.FranchiseId : 0;
            ViewBag.CompanyIds = _GetCompanyIds(companyIds);

            return PartialView();
        }


        [HttpGet]
        public ActionResult GetWeeklyReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeeklyCostReport))
                return AccessDeniedView();

            ViewBag.Franchises = _franchiseService.GetAllFranchisesAsSelectList(_workContext.CurrentAccount);

            return View();
        }

        [HttpPost]
        public ActionResult _WeeklyCostReport(DateTime refDate, string vendorIds, string companyIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.WeeklyCostReport))
                return AccessDeniedView();

            ViewBag.refDate = refDate;
            ViewBag.vendorIds = vendorIds;
            ViewBag.companyIds = companyIds;
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetTimeSheetsValidationsReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _TimeSheetsValidationReport(DateTime startDate, DateTime endDate, string companyIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyIds = _GetCompanyIds(companyIds);

            return PartialView();
        }

        [HttpGet]
        public ActionResult GetEmployeesListReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EmployeeListReport))
                return AccessDeniedView();

            ViewBag.Companies = _companyService.Secure_GetAllCompanies(_workContext.CurrentAccount).Select(x => new SelectListItem() { Text = x.CompanyName, Value = x.Id.ToString() }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult _EmployeesListReport(DateTime startDate, DateTime endDate, string companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EmployeeListReport))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = _companyService.GetCompanyById(Convert.ToInt32(companyId)).CompanyName;
            return PartialView();
        }

        #region ClockTime Exceptions

        [HttpGet]
        public ActionResult GetClockTimeExceptionReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _ClockTimeExceptionReport(int companyId, string locationIds, string vendorIds, DateTime fromDate, DateTime toDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.CompanyId = companyId.ToString();
            ViewBag.LocationIds = locationIds;
            ViewBag.VendorIds = vendorIds;

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return PartialView("_ClockTimeExceptionReport");
        }

        #endregion

        [HttpGet]
        public ActionResult GetJobOrderPlacementSummaryByRecruiter()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.JobOrderPlacementByRecruiterReport))
                return AccessDeniedView();

            ViewBag.Franchises = _franchiseService.GetAllFranchisesAsSelectList(_workContext.CurrentAccount);
            return View();
        }

        [HttpPost]
        public ActionResult _JobOrderPlacementSummaryByRecruiter(DateTime startDate, DateTime endDate, string franchiseId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.JobOrderPlacementByRecruiterReport))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            this.SetVendorInViewBag(Convert.ToInt32(franchiseId));

            return PartialView("_JobOrderPlacementSummaryByRecruiter");
        }

        [HttpGet]
        public ActionResult GetDailyAttendanceByRecruiter()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DailyAttendanceByRecruiterReport))
                return AccessDeniedView();


            ViewBag.Franchises = _franchiseService.GetAllFranchisesAsSelectList(_workContext.CurrentAccount);
            return View();
        }

        [HttpPost]
        public ActionResult _DailyAttendanceByRecruiter(DateTime workDate, string franchiseId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DailyAttendanceByRecruiterReport))
                return AccessDeniedView();


            ViewBag.WorkDate = workDate;
            this.SetVendorInViewBag(Convert.ToInt32(franchiseId));
            ViewBag.CompanyIds = _GetCompanyIds(string.Empty);

            return PartialView("_DailyAttendanceByRecruiter");
        }

        [HttpGet]
        public ActionResult GetAttendanceReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _AttendanceReport(DateTime startDate, DateTime endDate, string companyIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyIds = _GetCompanyIds(companyIds);
            ViewBag.FranchiseId = _workContext.CurrentAccount.IsLimitedToFranchises ? _workContext.CurrentAccount.FranchiseId.ToString() : string.Empty;

            return PartialView("_AttendanceReport");
        }

        [HttpGet]
        public ActionResult GetBillingRatesAuditLogReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BillingRatesAuditLogReport))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult _BillingRatesAuditLogReport(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.BillingRatesAuditLogReport))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return PartialView("_BillingRatesAuditLogReport");
        }

        public ActionResult GetConfirmationEmailReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ConfirmationEmailReport))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _ConfirmationEmailReport(DateTime startDate, DateTime endDate, string companyIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ConfirmationEmailReport))
                return AccessDeniedView();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyIds = _GetCompanyIds(companyIds);
            ViewBag.AccountId = _workContext.CurrentAccount.Id;

            return PartialView("_ConfirmationEmailReport");
        }

        public ActionResult GetTestResultReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TestResultReport))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _TestResultReport(int? companyId, int? jobOrderId, DateTime refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.TestResultReport))
                return AccessDeniedView();

            ViewBag.FranchiseId = _workContext.CurrentFranchise.IsDefaultManagedServiceProvider ? 0 : _workContext.CurrentFranchise.Id;
            ViewBag.CompanyIds = _GetCompanyIds(companyId);
            ViewBag.JobOrderId = jobOrderId.HasValue ? jobOrderId.Value : 0;
            ViewBag.RefDate = refDate;

            return PartialView("_TestResultReport");
        }

        public ActionResult GetOneWeekFollowUpReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _OneWeekFollowUpReport(DateTime refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();
            ViewBag.RefDate = refDate;
            if (_workContext.CurrentAccount.IsRecruiterOrRecruiterSupervisor())
                ViewBag.AccountId = _workContext.CurrentAccount.Id;
            else
                ViewBag.AccountId = 0;
            return PartialView("_OneWeekFollowUpReport");
        }

        public ActionResult GetDNRListReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _DNRListReport(int? companyId, DateTime refDateStart, DateTime refDateEnd)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();
            
            ViewBag.CompanyId = companyId.HasValue ? companyId.Value : 0;
            ViewBag.RefDateStart = refDateStart;
            ViewBag.RefDateEnd = refDateEnd;

            return PartialView();
        }

        [FeatureAuthorize(featureName: "Invoicing")]
        public ActionResult GetMarkupReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            return View();
        }

        [FeatureAuthorize(featureName: "Invoicing")]
        [HttpPost]
        public ActionResult _MarkupReport(DateTime startDate, DateTime endDate, string companyIds, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.CompanyIds = String.IsNullOrWhiteSpace(companyIds) ? null : companyIds;
            this.SetVendorInViewBag(vendorId);

            return PartialView();
        }

        public ActionResult GetGoveronmentRemittance()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _GoveronmentRemittanceReport(DateTime startDate, DateTime endDate, int year, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = endDate.ToString("MM/dd/yyyy");
            ViewBag.Year = year;

            this.SetVendorInViewBag(vendorId);

            return PartialView();
        }

        public ActionResult GetSINExpiryReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _SINExpiryReport(DateTime refDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            ViewBag.RefDate = refDate;
            ViewBag.FranchiseId = _workContext.CurrentFranchise.Id;
            return PartialView();
        }

        public ActionResult GetEmployeePaySummaryReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _EmployeePaySummaryReport(DateTime fromDate, DateTime toDate, string companyIds, string paygroups, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();

            ViewBag.StartDate = fromDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = toDate.ToString("MM/dd/yyyy");
            ViewBag.CompanyIds = companyIds;
            ViewBag.PayGroups = paygroups;
            this.SetVendorInViewBag(vendorId);

            if (String.IsNullOrWhiteSpace(paygroups))
            {
                ViewBag.ShowPayGroup = true;
                ViewBag.PayGroup = "All Pay Groups";
            }
            else
            {
                string[] paygroupsStr = paygroups.Split(new char[] { ',' });
                if (paygroupsStr.Count() > 1)
                    ViewBag.ShowPayGroup = true;
                else
                    ViewBag.ShowPayGroup = false;
                var payGroupNames = _payGroupService.GetAllPayGroupsByFranchiseId(vendorId)
                                    .Where(x => paygroupsStr.Contains(x.Id.ToString())).Select(x => x.Name);
                ViewBag.PayGroup = string.Join(",", payGroupNames);
            }

            return PartialView();
        }

        public ActionResult GetEmployeeInfoReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _EmployeeInfoReport(DateTime fromDate, DateTime toDate, string companyIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            ViewBag.StartDate = fromDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = toDate.ToString("MM/dd/yyyy");
            ViewBag.CompanyIds = companyIds;
            ViewBag.FranchiseName = _workContext.CurrentFranchise.FranchiseName;
            ViewBag.FranchiseId = _workContext.CurrentFranchise.Id;
            return PartialView();
        }

        public ActionResult GetPayrollSummaryReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult _PayrollSummaryReport(string batchIds, int payCalendarId, string paygroups, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePayrollReports))
                return AccessDeniedView();

            this.SetVendorInViewBag(vendorId);

            if (String.IsNullOrWhiteSpace(paygroups))
            {
                ViewBag.PayGroup = "All Pay Groups";
            }
            else
            {
                string[] paygroupsStr = paygroups.Split(new char[] { ',' });
                var payGroupNames = _payGroupService.GetAllPayGroupsByFranchiseId(vendorId)
                                    .Where(x => paygroupsStr.Contains(x.Id.ToString())).Select(x => x.Name);
                ViewBag.PayGroup = string.Join(",", payGroupNames);

            }
            ViewBag.BatchIds = batchIds;
            var payrollCalendar = _payrollCalendarService.GetPayrollCalendarByIdNoTracking(payCalendarId);
            ViewBag.StartDate = payrollCalendar.PayPeriodStartDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = payrollCalendar.PayPeriodEndDate.ToString("MM/dd/yyyy");

            return PartialView();
        }


        #region Candidate Sources

        public ActionResult GetCandidateSources()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult _CandidateSources(DateTime startDate, DateTime endDate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return PartialView();
        }

        #endregion


        #region Employee Seniority

        [HttpGet]
        public ActionResult EmployeeSeniority()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult EmployeeSeniority([DataSourceRequest] DataSourceRequest request, string dateField, DateTime fromDate, DateTime toDate, bool exlcudePlaced, DateTime placedFrom, DateTime placedTo, int? companyId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AdminReports))
                return AccessDeniedView();

            var result = _paymentHistoryService.GetEmployeeSeniorityReport(dateField, fromDate, toDate, exlcudePlaced, placedFrom, placedTo, _GetCompanyIds(companyId)).ToList();

            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult ExportEmployeeSeniority(string _dateField, DateTime _fromDate, DateTime _toDate, bool _exlcudePlaced, DateTime _placedFrom, DateTime _placedTo, int? _companyId)
        {
            try
            {
                byte[] bytes = null;
                var fileName = string.Empty;
                var employees = _paymentHistoryService.GetEmployeeSeniorityReport(_dateField, _fromDate, _toDate, _exlcudePlaced, _placedFrom, _placedTo, _GetCompanyIds(_companyId));
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportEmployeeSeniorityReportToXlsx(stream, employees, _dateField);
                    bytes = stream.ToArray();
                }

                return File(bytes, "text/xls", fileName);
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

            return Redirect(Request.UrlReferrer.ToString());
        }


        public ActionResult _PreviewEmployeeSeniorityAlert(string dateField, DateTime fromDate, DateTime toDate, bool exlcudePlaced, DateTime placedFrom, DateTime placedTo, int? companyId)
        {
            var account = _workContext.CurrentAccount;
            var category = _messageCategoryService.GetMessageCategoryByName("Candidate");
            var recruiters = _recruiterCompanyService.GetAllRecruitersByCompanyId(companyId.Value).Select(x => x.Account.Email);
            var model = new QueuedEmailModel()
            {
                EmailAccountId = 3,
                Priority = 5,
                From = account.Email,
                FromName = account.FullName,
                FromAccountId = account.Id,
                To = String.Join(";", recruiters),
                Subject = _localizationService.GetResource("Admin.EmployeeSeniority"),
                Body = string.Empty,
                MessageCategoryId = category != null ? category.Id : 0
            };

            var employees = _paymentHistoryService.GetEmployeeSeniorityReport(dateField, fromDate, toDate, exlcudePlaced, placedFrom, placedTo, _GetCompanyIds(companyId));
            using (var stream = new MemoryStream())
            {
                model.AttachmentFileName = _exportManager.ExportEmployeeSeniorityReportToXlsx(stream, employees, dateField);
                model.AttachmentFile = stream.ToArray();
            }

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _SendEmployeeSeniorityAlert([Bind(Exclude = "Id")]QueuedEmailModel model)
        {
            var errorMessage = new StringBuilder();

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var error = _SendEmployeeSeniorityAlertEmail(model);
                if (!String.IsNullOrWhiteSpace(error))
                    errorMessage.AppendLine(error);
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
            }

            return Json(new { Result = errorMessage.Length == 0, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }


        private string _SendEmployeeSeniorityAlertEmail(QueuedEmailModel model)
        {
            var result = string.Empty;
            var email = model.ToEntity();

            // set FROM
            var sender = _accountService.GetAccountByEmail(email.From);
            if (sender == null)
                result = String.Format("From address [{0}] is invalid.", email.From);
            else
            {
                email.FromName = sender.FullName;
                email.FromAccountId = sender.Id;
            }

            // set TO
            var receivers = email.To.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (receivers.Count() == 1)
            {
                var receiver = _accountService.GetAccountByEmail(receivers.First());
                if (receiver != null)
                {
                    email.ToName = receiver.FullName;
                    email.ToAccountId = receiver.Id;
                }
            }

            if (!String.IsNullOrWhiteSpace(result))
                return result;

            // set ReplyTo
            email.ReplyTo = email.From;
            email.ReplyToName = email.FromName;

            // attachment
            email.AttachmentFile = email.AttachmentFile == null || email.AttachmentFile.Length == 0 ? null : email.AttachmentFile;
            email.AttachmentFile2 = null;
            email.AttachmentFile3 = null;

            email.CreatedOnUtc = email.UpdatedOnUtc = DateTime.UtcNow;
            _queuedEmailService.InsertQueuedEmail(email);

            return result;
        }

        #endregion
    }
}

using System.Collections.Generic;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Security;

namespace Wfm.Services.Security
{
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {

        #region Define Permissions
        // admin area permissions
        // ----------------------------------
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord ManageVendors = new PermissionRecord { Name = "Admin area. Manage Vendors", SystemName = "ManageVendors", Category = "Vendor" };
        public static readonly PermissionRecord MassEmailToVendors = new PermissionRecord { Name = "Admin area. Mass Email To Vendors", SystemName = "MassEmailToVendors", Category = "Vendor" };
        public static readonly PermissionRecord ManageCandidates = new PermissionRecord { Name = "Admin area. Manage Candidates", SystemName = "ManageCandidates", Category = "Candidate" };
        public static readonly PermissionRecord ManageEmployees = new PermissionRecord { Name = "Admin area. Manage Employees", SystemName = "ManageEmployees", Category = "Employee" };
        public static readonly PermissionRecord ManageCandidateBlacklist = new PermissionRecord { Name = "Admin area. Manage Candidate Blacklist", SystemName = "ManageCandidateBlacklist", Category = "Candidate" };
        public static readonly PermissionRecord MassEmailToCandidates = new PermissionRecord { Name = "Admin area. Mass Email To Candidates", SystemName = "MassEmailToCandidates", Category = "Candidate" };
        public static readonly PermissionRecord ManageCompanies = new PermissionRecord { Name = "Admin area. Manage Companies", SystemName = "ManageCompanies", Category = "Company" };
        public static readonly PermissionRecord ManageContacts = new PermissionRecord { Name = "Admin area. Manage Contacts", SystemName = "ManageContacts", Category = "Contact" };
        public static readonly PermissionRecord ManageJobOrders = new PermissionRecord { Name = "Admin area. Manage Job Orders", SystemName = "ManageJobOrders", Category = "JobOrder" };
        public static readonly PermissionRecord ManageBlog = new PermissionRecord { Name = "Admin area. Manage Blog", SystemName = "ManageBlog", Category = "Blog" };
        public static readonly PermissionRecord ManageForums = new PermissionRecord { Name = "Admin area. Manage eForums", SystemName = "ManageForums", Category = "Forum" };
        public static readonly PermissionRecord ManageConfiguration = new PermissionRecord { Name = "Admin area. Manage Configuration", SystemName = "ManageConfiguration", Category = "Configuration" };
        public static readonly PermissionRecord ManageAccounts = new PermissionRecord { Name = "Admin area. Manage Accounts", SystemName = "ManageAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Admin area. Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageStateProvinces = new PermissionRecord { Name = "Admin area. Manage State Provinces", SystemName = "ManageStateProvinces", Category = "Configuration" };
        public static readonly PermissionRecord ManageCities = new PermissionRecord { Name = "Admin area. Manage Cities", SystemName = "ManageCities", Category = "Configuration" };
        public static readonly PermissionRecord ManageLocalStringResource = new PermissionRecord { Name = "Admin area. Manage Local String Resource", SystemName = "ManageLocalStringResource", Category = "Configuration" };
        public static readonly PermissionRecord ManageCandidateSmartCards = new PermissionRecord { Name = "Admin area. Manage Candidate Smart Cards", SystemName = "ManageCandidateSmartCards", Category = "Configuration" };
        public static readonly PermissionRecord ManageCompanyClockDevices = new PermissionRecord { Name = "Admin area. Manage Company Clock Devices", SystemName = "ManageCompanyClockDevices", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageJobCategories = new PermissionRecord { Name = "Admin area. Manage Job Categories", SystemName = "ManageJobCategories", Category = "Configuration" };
        public static readonly PermissionRecord ManageAccessLog = new PermissionRecord { Name = "Admin area. Manage Access Log", SystemName = "ManageAccessLog", Category = "Logging" };
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Logging" };
        public static readonly PermissionRecord ManageCandidateActivityLog = new PermissionRecord { Name = "Admin area. Manage Candidate Activity Log", SystemName = "ManageCandidateActivityLog", Category = "Logging" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Logging" };
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageHistory = new PermissionRecord { Name = "Admin area. Manage Message History", SystemName = "ManageMessageHistory", Category = "Logging" };
        public static readonly PermissionRecord ManageClockTimes = new PermissionRecord { Name = "Admin area. Manage Clock Times", SystemName = "ManageClockTimes", Category = "ClockTime" };
        public static readonly PermissionRecord SelectClockTimeCandidate = new PermissionRecord { Name = "Admin area. Select ClockTime Candidate", SystemName = "SelectClockTimeCandidate", Category = "Candidate" };
        public static readonly PermissionRecord ManageTests = new PermissionRecord { Name = "Admin area. Manage Tests", SystemName = "ManageTests", Category = "Test" };
        public static readonly PermissionRecord ManageAttachments = new PermissionRecord { Name = "Admin area. Manage Attachments", SystemName = "ManageAttachments", Category = "Attachment" };
        public static readonly PermissionRecord ManageCompanyBillings = new PermissionRecord { Name = "Admin area. Manage Company Billings", SystemName = "ManageCompanyBillingRates", Category = "CompanyBilling" };
        public static readonly PermissionRecord ManageIntersections = new PermissionRecord { Name = "Admin area. Manage Intersections", SystemName = "ManageIntersections", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Admin area. Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Setting" };
        public static readonly PermissionRecord ManageShifts = new PermissionRecord { Name = "Admin area. Manage Shifts", SystemName = "ManageShifts", Category = "Configuration" };
        public static readonly PermissionRecord ManageSkills = new PermissionRecord { Name = "Admin area. Manage Skills", SystemName = "ManageSkills", Category = "Configuration" };

        public static readonly PermissionRecord ManageCandidateWorkTime = new PermissionRecord { Name = "Admin area. Manage Candidate Work Time", SystemName = "ManageCandidateWorkTime", Category = "CandidateWorkTime" };
        public static readonly PermissionRecord ApproveTimeSheet = new PermissionRecord { Name = "Admin area. Approve Time Sheet", SystemName = "ApproveTimeSheet", Category = "CandidateWorkTime" };
        public static readonly PermissionRecord SubmitMissingHour = new PermissionRecord { Name = "Admin area. Submit Missing Hour", SystemName = "SubmitMissingHour", Category = "CandidateWorkTime" };
        public static readonly PermissionRecord ProcessMissingHour = new PermissionRecord { Name = "Admin area. Process Missing Hour", SystemName = "ProcessMissingHour", Category = "CandidateWorkTime" };
        public static readonly PermissionRecord ManageCandidateTimeSheet = new PermissionRecord { Name = "Admin area. Manage Candidate Time Sheet", SystemName = "ManageCandidateTimeSheet", Category = "CandidateTimeSheet" };

        public static readonly PermissionRecord ManageScheduledTasks = new PermissionRecord { Name = "Admin area. Manage Scheduled Tasks", SystemName = "ManageScheduledTasks", Category = "Configuration" };
        public static readonly PermissionRecord ManagePolicies = new PermissionRecord { Name = "Admin area. Manage Policies", SystemName = "ManagePolicies", Category = "Configuration" };
        public static readonly PermissionRecord ImportTimeSheets = new PermissionRecord { Name = "Admin area. Import Time Sheets", SystemName = "ImportTimeSheets", Category = "Payroll" };
        public static readonly PermissionRecord ManageCompanyLocations = new PermissionRecord { Name = "Admin area. Manage Company Locations", SystemName = "ManageCompanyLocations", Category = "Company" };
        public static readonly PermissionRecord ManageCompanyDepartments = new PermissionRecord { Name = "Admin area. Manage Company Departments", SystemName = "ManageCompanyDepartments", Category = "Company" };
        public static readonly PermissionRecord ManageCompanySettings = new PermissionRecord { Name = "Admin area. Manage Company Settings", SystemName = "ManageCompanySettings", Category = "Company" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "Admin area. Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageCandidateBankAccounts = new PermissionRecord { Name = "Admin area. Manage Candidate Bank Accounts", SystemName = "ManageCandidateBankAccounts", Category = "Candidate" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Admin area. Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageIncidentCategory = new PermissionRecord { Name = "Admin area. Manage Incident Category", SystemName = "ManageIncidentCategory", Category = "Configuration" };
        public static readonly PermissionRecord ManageTimeoffType = new PermissionRecord { Name = "Admin area. Manage Timeoff Type", SystemName = "ManageTimeoffType", Category = "Configuration" };
        public static readonly PermissionRecord HtmlEditorManagePictures = new PermissionRecord { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };
        public static readonly PermissionRecord UpdateJobOrder = new PermissionRecord { Name = "Admin area. Update Job Order", SystemName = "UpdateJobOrder", Category = "JobOrder" };
        public static readonly PermissionRecord ManageRecruiters = new PermissionRecord { Name = "Admin area. Manage Recruiters", SystemName = "ManageRecruiters", Category = "Company" };
        public static readonly PermissionRecord ManageOvertimeRule = new PermissionRecord { Name = "Admin area. Manage Overtime Rule", SystemName = "ManageOvertimeRule", Category = "Company" };
        public static readonly PermissionRecord AllowResetCandidatePassword = new PermissionRecord { Name = "Admin area. Allow Reset Candidate Password", SystemName = "AllowResetCandidatePassword", Category = "Candidate" };
        public static readonly PermissionRecord ManageDocumentTypes = new PermissionRecord { Name = "Admin area. Manage Document Types", SystemName = "ManageDocumentTypes", Category = "Configuration" };
        public static readonly PermissionRecord ManageCandidatePlacement = new PermissionRecord { Name = "Admin area. Manage Candidate Placement", SystemName = "ManageCandidatePlacement", Category = "Candidate" };
        public static readonly PermissionRecord AllowAddCandidates = new PermissionRecord { Name = "Admin area. Allow Add Candidates", SystemName = "AllowAddCandidates", Category = "Candidate" };
        public static readonly PermissionRecord ManageMessageCategory = new PermissionRecord { Name = "Admin area. Manage Message Category", SystemName = "ManageMessageCategory", Category = "Configuration" };
        public static readonly PermissionRecord ManagePositions = new PermissionRecord { Name = "Admin area. Manage Positions", SystemName = "ManagePositions", Category = "Configuration" };
        public static readonly PermissionRecord ManageFeatures = new PermissionRecord { Name = "Admin area. Manage Features", SystemName = "ManageFeatures", Category = "Feature" };
        public static readonly PermissionRecord AdminReports = new PermissionRecord { Name = "Admin area. Reports", SystemName = "AdminReports", Category = "Reports" };
        public static readonly PermissionRecord WeeklyCostReport = new PermissionRecord { Name = "Admin area.Weekly Cost Report", SystemName = "WeeklyCostReport", Category = "Reports" };
        public static readonly PermissionRecord EmployeeListReport = new PermissionRecord { Name = "Admin area.Employee List Report", SystemName = "EmployeeListReport", Category = "Reports" };
        public static readonly PermissionRecord ManageAnnouncements = new PermissionRecord { Name = "Admin area.Manage Announcements", SystemName = "ManageAnnouncements", Category = "Configuration" };
        public static readonly PermissionRecord ManageDirectHireJobOrders = new PermissionRecord { Name = "Admin area. Manage Direct Hire Job Orders", SystemName = "ManageDirectHireJobOrders", Category = "JobOrder" };
        public static readonly PermissionRecord JobOrderPlacementByRecruiterReport = new PermissionRecord { Name = "Admin area.JobOrder Placement By Recruiter Report", SystemName = "JobOrderPlacementByRecruiterReport", Category = "Reports" };
        public static readonly PermissionRecord DailyAttendanceByRecruiterReport = new PermissionRecord { Name = "Admin area.Daily Attendance By Recruiter Report", SystemName = "DailyAttendanceByRecruiterReport", Category = "Reports" };
        public static readonly PermissionRecord BillingRatesAuditLogReport = new PermissionRecord { Name = "Admin area.Billing Rates Audit Log  Report", SystemName = "BillingRatesAuditLogReport", Category = "Reports" };
        public static readonly PermissionRecord ConfirmationEmailReport = new PermissionRecord { Name = "Admin area.Confirmation Email Report", SystemName = "ConfirmationEmailReport", Category = "Reports" };
        public static readonly PermissionRecord UploadCompanyAttachment = new PermissionRecord { Name = "Admin area.Upload Company Attachment", SystemName = "UploadCompanyAttachment", Category = "Company" };
        public static readonly PermissionRecord TestResultReport = new PermissionRecord { Name = "Admin area.Test Result Report", SystemName = "TestResultReport", Category = "Reports" };
        public static readonly PermissionRecord ManageVendorEmailSetting = new PermissionRecord { Name = "Admin area.Manage Vendor Email Setting", SystemName = "ManageVendorEmailSetting", Category = "Vendor" };
        public static readonly PermissionRecord ManageVendorPayrollSetting = new PermissionRecord { Name = "Admin area.Manage Vendor Payroll Setting", SystemName = "ManageVendorPayrollSetting", Category = "Vendor" };
        public static readonly PermissionRecord ManagePayrollReports = new PermissionRecord { Name = "Admin area.Manage Payroll Reports", SystemName = "ManagePayrollReports", Category = "Reports" };
        public static readonly PermissionRecord ManageJobBoards = new PermissionRecord { Name = "Admin area.Manage Job Boards", SystemName = "ManageJobBoards", Category = "JobOrder" };
        // view permissions 
        public static readonly PermissionRecord ViewCompanies = new PermissionRecord { Name = "Admin area. View Companies", SystemName = "ViewCompanies", Category = "Company" };
        public static readonly PermissionRecord ViewContacts = new PermissionRecord { Name = "Admin area. View Contacts", SystemName = "ViewContacts", Category = "Contact" };
        public static readonly PermissionRecord ViewCompanyPayRates = new PermissionRecord { Name = "Admin area. View Company Pay Rates", SystemName = "ViewCompanyPayRates", Category = "Configuration" };
        public static readonly PermissionRecord ViewCompanyBillingRates = new PermissionRecord { Name = "Admin area. View Company Billing Rates", SystemName = "ViewCompanyBillingRates", Category = "Configuration" };
        public static readonly PermissionRecord ViewCandidateSmartCards = new PermissionRecord { Name = "Admin area. View Candidate Smart Cards", SystemName = "ViewCandidateSmartCards", Category = "SmartCards" };
        public static readonly PermissionRecord ViewCandidateSIN = new PermissionRecord { Name = "Admin area. View Candidate SIN", SystemName = "ViewCandidateSIN", Category = "SIN" };
        public static readonly PermissionRecord ViewVendorEmailSetting = new PermissionRecord { Name = "Admin area. View Vendor Email Setting", SystemName = "ViewVendorEmailSetting", Category = "Vendor" };

        // Payroll module permissions
        public static readonly PermissionRecord ManagePayGroups = new PermissionRecord { Name = "Admin area. Manage Pay Groups", SystemName = "ManagePayGroups", Category = "PayrollConfiguration" };

        // client area permissions
        // ----------------------------------
        public static readonly PermissionRecord AccessClientPanel = new PermissionRecord { Name = "Access client area", SystemName = "AccessClientPanel", Category = "Standard" };
        public static readonly PermissionRecord ManageClientCompanies = new PermissionRecord { Name = "Admin area. Manage Companies", SystemName = "ManageClientCompanies", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientCompanyContacts = new PermissionRecord { Name = "Admin area. Manage Company Contacts", SystemName = "ManageClientCompanyContacts", Category = "ClientContact" };
        public static readonly PermissionRecord ManageClientCompanyJobOrders = new PermissionRecord { Name = "Client area. Manage Client Company Job Orders", SystemName = "ManageClientCompanyJobOrders", Category = "ClientJobOrder" };
        public static readonly PermissionRecord ManageClientCompanyTimeSheets = new PermissionRecord { Name = "Client area. Manage Client Company Time Sheets", SystemName = "ManageClientCompanyTimeSheets", Category = "ClientTimeSheet" };
        public static readonly PermissionRecord ManageClientCompanyIncidents = new PermissionRecord { Name = "Client area. Manage Client Company Incidents", SystemName = "ManageClientCompanyIncidents", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientCompanyBillingRates = new PermissionRecord { Name = "Client area. Manage Client Company Billing Rates", SystemName = "ManageClientCompanyBillingRates", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientJobPosting = new PermissionRecord { Name = "Client area. Manage Job Posting", SystemName = "ManageClientJobPosting", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientEmployee = new PermissionRecord { Name = "Client area. Manage Employee", SystemName = "ManageClientEmployee", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientJobRole = new PermissionRecord { Name = "Client area. Manage Job Role", SystemName = "ManageClientJobRole", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientJobShift = new PermissionRecord { Name = "Client area. Manage Job Shift", SystemName = "ManageClientJobShift", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageClientScheduling = new PermissionRecord { Name = "Client area. Schedule workforce", SystemName = "ManageClientScheduling", Category = "ClientCompany" };
        public static readonly PermissionRecord ManageSchedulingPlacement = new PermissionRecord { Name = "Client area. Scheduling Placement", SystemName = "ManageSchedulingPlacement", Category = "ClientCompany" };
        public static readonly PermissionRecord ClientReports = new PermissionRecord { Name = "Client area. Reports", SystemName = "ClientReports", Category = "Reports" };
        public static readonly PermissionRecord ManageClientVendors = new PermissionRecord { Name = "Client area. Manage Client Vendors", SystemName = "ManageClientVendors", Category = "ClientCompany" };
        public static readonly PermissionRecord ClientHRReports = new PermissionRecord { Name = "Client area. Client HR Report", SystemName = "ClientHRReports", Category = "Reports" };
        #endregion


        #region Methods

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminPanel,
                ManageVendors,
                MassEmailToVendors,
                ManageCandidates,
                ManageEmployees,
                ManageCandidateBlacklist,
                MassEmailToCandidates,
                ManageCompanies,
                ManageContacts,
                ManageJobOrders,
               
                ManageBlog,
                ManageForums,
                ManageConfiguration,
                ManageAccounts,
                ManageCountries,
                ManageStateProvinces,
                ManageCities,
                ManageLocalStringResource,
                ManageCandidateSmartCards,
                ManageCompanyClockDevices,
                ManageLanguages,
                ManageJobCategories,
                ManageAccessLog,
                ManageActivityLog,
                ManageCandidateActivityLog,
                ManageSystemLog,
                ManageAcl,
                ManageMessageHistory,
                ManageClockTimes,
                SelectClockTimeCandidate,
                ManageTests,
                ManageAttachments,
                ManageCompanyBillings,
                ManageIntersections,
                ManageMessageTemplates,
                ManageSettings,
                ManageShifts,
                ManageSkills,
                ManageCandidateWorkTime,
                ApproveTimeSheet,
                SubmitMissingHour,
                ProcessMissingHour,
                ManageCandidateTimeSheet,
                ManageScheduledTasks,
                ManagePolicies,
                ImportTimeSheets,
                ManageCompanyLocations,
                ManageCompanyDepartments,
                ManageCompanySettings,
                ManageMessageQueue,
                ManageCandidateBankAccounts,
                ManageWidgets,
                ManageTimeoffType,
                ManageFeatures,

                ViewCompanies,
                ViewContacts,
                UpdateJobOrder,
                ManageRecruiters,
                ManageOvertimeRule,
                ManageDocumentTypes
            };
        }


        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    AccountRoleSystemName = AccountRoleSystemNames.Administrators,
                    PermissionRecords = new[]
                    {
                        AccessAdminPanel,
                        ManageVendors,
                        MassEmailToVendors,
                        ManageCandidates,
                        ManageCandidateBlacklist,
                        MassEmailToCandidates,
                        ManageCompanies,
                        ManageContacts,
                        ManageJobOrders,
                       
                        ManageBlog,
                        ManageForums,
                        ManageConfiguration,
                        ManageAccounts,
                        ManageCountries,
                        ManageStateProvinces,
                        ManageCities,
                        ManageLocalStringResource,
                        ManageCandidateSmartCards,
                        ManageCompanyClockDevices,
                        ManageLanguages,
                        ManageJobCategories,
                        ManageAccessLog,
                        ManageActivityLog,
                        ManageCandidateActivityLog,
                        ManageSystemLog,
                        ManageAcl,
                        ManageMessageHistory,
                        ManageClockTimes,
                        SelectClockTimeCandidate,
                        ManageTests,
                        ManageAttachments,
                        ManageCompanyBillings,
                        ManageIntersections,
                        ManageMessageTemplates,
                        ManageSettings,
                        ManageShifts,
                        ManageSkills,
                        ManageCandidateWorkTime,
                        ManageCandidateTimeSheet,
                        ManageScheduledTasks,
                        ManagePolicies,
                        ImportTimeSheets,
                        ManageCompanyLocations,
                        ManageCompanyDepartments,
                        ManageCompanySettings,
                        ManageMessageQueue,
                        ManageCandidateBankAccounts,
                        ManageWidgets,
                        ManageFeatures,

                        ViewCompanies,
                        ViewContacts,
                        ManageDocumentTypes,
                    }
                },
                new DefaultPermissionRecord
                {
                    AccountRoleSystemName = AccountRoleSystemNames.Recruiters,
                    PermissionRecords = new[]
                    {
                        AccessAdminPanel,
                        ManageCandidates,
                        ManageJobOrders,
                       
                        ManageClockTimes,
                        ManageCandidateWorkTime,

                        ViewCompanies,
                        ViewContacts,
                      
                    }
                }
            };
        }

        #endregion

    }
}

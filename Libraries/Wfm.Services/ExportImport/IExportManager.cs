using System;
using System.Collections.Generic;
using System.IO;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Services.ExportImport
{
    public partial interface IExportManager
    {
        #region Export AttandentList

        string ExportAttandentListToXlsxByDateRange(Stream stream, JobOrder jobOrder, DateTime startDate, DateTime endDate);

        string ExportAtetanceListForJobOrders(Stream stream, string jobOrderIds, DateTime startDate, DateTime endDate, int companyId);

        string ExportDailyAttendanceConfirmationListToXlsx(Stream stream, int companyId, DateTime refDate, int[] candidateJobOrderIds);

        string ExportDialyAttendantListToXlsx(Stream stream, int companyId, DateTime refDate, int[] candidateJobOrderIds);

        #endregion

        #region Export CandidateWorkTime

        void ExportCandidateWorkTimeToXlsx(Stream stream, IList<CandidateWorkTime> candidateWorkTime);

        string ExportWorkTimeChangesAfterInvoiceToXlsx(Stream stream, IList<InvoiceUpdateDetail> candidateWorkTime);

        string ExportMissingHourToXlsx(Stream stream, IEnumerable<CandidateMissingHour> missingHours, bool toClient = false);

        #endregion

        #region Export EmployeeTimeChartHistory
        string ExportEmployeeTimeChartToXlsxForAdmin(Stream stream, IList<EmployeeTimeChartHistory> employeeTimeChartHistory, DateTime startDate, DateTime endDate, int companyId = 0, int vendorId = 0, bool withRates = false);
        string ExportEmployeeTimeChartToXlsxForClient(Stream stream, IList<EmployeeTimeChartHistory> employeeTimeChartHistory);
        string ExportEmployeeTimeChartToXlsxForReminder(Stream stream, IList<EmployeeTimeChartHistory> employeeTimeChartHistory);
        #endregion


        #region Export Company Billing Rate
        string ExportCompanyBillingRate(Stream stream, IList<CompanyBillingRate> companyBillingRates);
        #endregion

        #region Export CompanyCanidate

        string ExportCompanyCandidateToXlsx(Stream stream, int companyId, int vendorId, IList<CompanyCandidate> companyCandidates);

        #endregion

        #region Export Client Daily Attendance List
        string ExportDailyAttendanceListForClient(Stream stream, IEnumerable<DailyAttendanceList> attendanceList, DateTime refDate);
        #endregion

        #region Direct Placement Invoice

        void DirectPlacementInvoiceXlsx(Stream stream, IList<CandidateDirectHireStatusHistory> placements);

        #endregion

        #region Export One Week Follow Up excel
        string GenerateOneWeekFollowUpExcel(Stream stream, List<OneWeekFollowUpReportData> data);
        #endregion

        #region Export Employee Total Hours

        string ExportEmployeeTotalHoursToXlsx(Stream stream, IEnumerable<EmployeeTotalHours> totalHours);

        #endregion


        #region Employee Seneiority

        // for alert
        string ExportEmployeeSeniorityReportToXlsx(Stream stream, IEnumerable<EmployeeSeniority> employees, decimal years);

        // for report
        string ExportEmployeeSeniorityReportToXlsx(Stream stream, IEnumerable<EmployeeSeniority> employees, string dateField);

        #endregion


        #region Employee Availability

        string ExportAvailableToXlsx(Stream stream, Guid companyGuid, DateTime refDate);

        #endregion
    }
}
 
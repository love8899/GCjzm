using System.Collections.Generic;
using System.IO;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Payroll;
using Wfm.Core.Domain.Companies;
using System;


namespace Wfm.Services.Common
{
    /// <summary>
    /// Pdf service interface
    /// </summary>
    public partial interface IPdfService
    {
        #region Print Candidate

        void PrintCandidatesToPdf(Stream output, IList<Candidate> candidates);

        #endregion

        #region Print CandidateWorkTime

        void PrintCandidateWorkTimesToPdf(Stream output, IList<CandidateWorkTime> workTimes);

        #endregion

        #region Print EmployeeTimeChartHistory
        void PrintEmployeeTimeChartHistoryToPdfForAdmin(Stream stream, IEnumerable<EmployeeTimeChartHistory> timeCharts, bool withRates = false);
        void PrintEmployeeTimeChartHistoryToPdfForClient(Stream stream, IEnumerable<EmployeeTimeChartHistory> timeCharts);
        #endregion

        #region Export Company Billing Rate to PDF
        void ExportCompanyBillingRateToPDF(Stream stream, IEnumerable<CompanyBillingRate> companyBillingRates);        
        #endregion

        void PrintDailyAttendanceListToPdfForClient(Stream stream, IEnumerable<DailyAttendanceList> attendanceList, DateTime refDate, bool hideTotalHours);


        #region Print Smart Card Bitmap to PDF

        void PrintBitmapsToPdf(System.Drawing.Image frontImg, System.Drawing.Image backImg, Stream pdfStream);

        #endregion


        #region Encrypt

        byte[] SecurePDF(byte[] bytes, string password);

        #endregion
    }
}

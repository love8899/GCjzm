using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Services.Candidates
{
    public partial interface IPaymentHistoryService
    {

        #region Payment History
        
        IQueryable<PaymentHistory> GetPaymentHistoryByCandidatId(int candidateId, int franchiseId, bool excludeReversed = true);

        IQueryable<PaymentHistory> GetPaymentHistoryByCandidatIdAndDate(int candidateId, int franchiseId, DateTime startDate);

        IQueryable<PaymentHistory> GetPaymentHistoryByCandidatIdAndYear(int candidateId, int franchiseId, int year);

        IEnumerable<PaymentDetail> GetPaymentDetails(int historyId);

        void UdpatePaymentHistory(int id, bool? isEmailed = null, bool? isPrinted = null, string note = null);

        byte[] GetPaystub(int historyId);

        byte[] GetPaystub(Guid historyGuid);

        #endregion


        #region Payment History with pay stubs

        IEnumerable<PaymentHistoryWithPayStub> GetAllPaymentHistoryWithPayStubByCandidateId(int candidateId);
        PaymentHistoryWithPayStub GetPayStubByPaymentGuid(Guid? guid, int candidateId);
        bool PayStub_Password(int EmployeeId, out string password);
        string SecurePayStubPDFFile(int candidateId, byte[] bytes);

        #endregion


        #region Payroll Batch
        IQueryable<Payroll_Batch> GetPayrollBatchByPayCalendarId(int payCalendarId);
        #endregion

        #region Seniority

        IEnumerable<EmployeeSeniority> GetEmployeeSeniorityReport(DateTime refDate, int scope, decimal threshold);

        IEnumerable<EmployeeSeniority> GetEmployeeSeniorityReport(string dateField, DateTime fromDate, DateTime toDate, bool exlcudePlaced, DateTime placedFrom, DateTime placedTo, string companyIds = null);

        #endregion
    }
}

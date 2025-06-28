using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Services.Candidates
{
    public partial interface IEmployeeTimeChartHistoryService
    {
        #region Methods
        IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByDate(DateTime start, DateTime end, string status, string weeklyStatus = "0,1", int companyId = 0);

        IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByDateAndAccount(DateTime start, DateTime end, Account account, string status,string weeklyStatus = "0,1");

        IEnumerable<EmployeeTimeChartHistory> FilterTimeSheetHistoryByAccount(IEnumerable<EmployeeTimeChartHistory> table, Account account = null);

        IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByIds(int[] ids, DateTime start, DateTime end,string status,string weeklyStatus);
        IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByIds(int[] ids, DateTime start, DateTime end,Account account);

        IList<CandidateWorkTimeStatusClass> GetAllCandidateWorkTimeStatusFromEnum();

        IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByCandidateId(int CanidateId, DateTime? start = null, DateTime? end = null);
        
        IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryForInvoice(DateTime start, DateTime end, int companyId, int vendorId);

        IList<InvoiceUpdateDetail> GetInvoiceUpdatesFromDailyWorkTime(string ids);
        #endregion
    }
}

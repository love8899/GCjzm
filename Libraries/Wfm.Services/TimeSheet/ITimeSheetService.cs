using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Services.TimeSheet
{
    public partial interface ITimeSheetService
    {

        #region TimeSheetAttachment

        int[] GetCompaniesWithWorkTimePendingForApproval(DateTime weekStartDate);

        string GetTimeSheetAttachmentPath();

        string GetTimeSheetAttachment(Account account, DateTime startDate, DateTime endDate, out byte[] attachmentFile, int? vendorId);

        IList<TimeSheetDetails> GetAllTimeSheetDetails(int companyId, int vendorId, DateTime startDate, DateTime endDate);
        IList<OneWeekFollowUpReportData> GetAllOneWeekFollowUpData(DateTime refDate, int accountId);
        #endregion

    }
}

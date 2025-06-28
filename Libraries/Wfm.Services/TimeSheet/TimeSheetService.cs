using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Data;
using Wfm.Services.Candidates;
using Wfm.Services.Configuration;
using Wfm.Services.ExportImport;
using Wfm.Services.Logging;


namespace Wfm.Services.TimeSheet
{
    public partial class TimeSheetService : ITimeSheetService
    {

        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IEmployeeTimeChartHistoryService _employeeTimeSheetHistoryService;
        private readonly ISettingService _settingService;
        private readonly IExportManager _exportManager;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;

        #endregion 

        #region Ctor

        public TimeSheetService(
            IDbContext dbContext,
            IEmployeeTimeChartHistoryService employeeTimeSheetHistoryService,
            ISettingService settingService,
            IExportManager exportManager,
            ILogger logger,
            IWebHelper webHelper)
        {
            _dbContext = dbContext;
            _employeeTimeSheetHistoryService = employeeTimeSheetHistoryService;
            _settingService = settingService;
            _exportManager = exportManager;
            _logger = logger;
            _webHelper = webHelper;
        }

        #endregion

        #region TimeSheetAttachment

        public int[] GetCompaniesWithWorkTimePendingForApproval(DateTime weekStartDate)
        {
            var year = CommonHelper.GetYearAndWeekNumber(weekStartDate, out int weekOfYear);
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("Year", year);
            paras[1] = new SqlParameter("WeekOfYear", weekOfYear);

            var result = _dbContext.SqlQuery<TimeSheetSummary>("EXEC [dbo].[GetTimeSheetSummary] @Year, @WeekOfYear", paras)
                         .Where(x => x.SubmittedHours > 0)
                         .Select(x => x.CompanyId).Distinct()
                         .ToArray();

            return result;
        }


        public string GetTimeSheetAttachmentPath()
        {
            string rootDirectory = _webHelper.GetRootDirectory();

            var attachmentFolder = _settingService.GetSettingByKey<string>("TimeSheetSettings.TimeSheetAttachmentLocation");
            if (String.IsNullOrWhiteSpace(attachmentFolder))
                attachmentFolder = @"Export\TimeSheetAttachment";

            string attachmentPath = Path.Combine(rootDirectory, attachmentFolder);
            if (!Directory.Exists(attachmentPath))
            {
                Directory.CreateDirectory(attachmentPath);
            }

            return attachmentPath;
        }


        public string GetTimeSheetAttachment(Account account, DateTime startDate, DateTime endDate, out byte[] attachmentFile, int? vendorId)
        {
            string fileName = string.Empty;
            string status = String.Format("{0},{1}", (int)CandidateWorkTimeStatus.PendingApproval, (int)CandidateWorkTimeStatus.Approved);
            string weeklyStatus = "0";//only send unapproved hours
            attachmentFile = null;

            var timeCharts = _employeeTimeSheetHistoryService.GetAllEmployeeTimeSheetHistoryByDateAndAccount(startDate, endDate, account, status, weeklyStatus);
            if (vendorId.HasValue)
                timeCharts = timeCharts.Where(x => x.FranchiseId == vendorId.Value).ToList();
            fileName = GenerateTimeSheetFile(timeCharts, out attachmentFile);

            return fileName;
        }


        private string GenerateTimeSheetFile(IList<EmployeeTimeChartHistory> timeCharts, out byte[] attachmentFile)
        {
            string fileName = String.Empty;
            attachmentFile = null;
            if (timeCharts.Count() > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        fileName = _exportManager.ExportEmployeeTimeChartToXlsxForReminder(stream, timeCharts);
                        attachmentFile = stream.ToArray();
                    }
                }
                catch (Exception exc)
                {
                    _logger.Error(string.Format("Error occurred while exporting time sheet for approval reminder. Error message : {0}", exc.Message), exc);
                }
            }

            return fileName;
        }


        public IList<TimeSheetDetails> GetAllTimeSheetDetails(int companyId, int vendorId, DateTime startDate, DateTime endDate)
        {
            SqlParameter[] paras = new SqlParameter[4];
            paras[0] = new SqlParameter("companyId", companyId);
            paras[1] = new SqlParameter("vendor", vendorId);
            paras[2] = new SqlParameter("StartDate", startDate);
            paras[3] = new SqlParameter("EndDate", endDate);
            StringBuilder query = new StringBuilder(@"Exec [GetTimeSheetDetails] @StartDate, @EndDate, @companyId, @vendor");
            var result = _dbContext.SqlQuery<TimeSheetDetails>(query.ToString(), paras).ToList();
            return result;
        }

        public IList<OneWeekFollowUpReportData> GetAllOneWeekFollowUpData(DateTime refDate,int accountId)
        {
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@ref", refDate);
            paras[1] = new SqlParameter("@accountId", accountId);
            StringBuilder query = new StringBuilder(@"Exec [OneWeekFollowUpReport] @ref, @accountId");
            var result = _dbContext.SqlQuery<OneWeekFollowUpReportData>(query.ToString(), paras).ToList();
            return result;
        }
        #endregion
    }
} 

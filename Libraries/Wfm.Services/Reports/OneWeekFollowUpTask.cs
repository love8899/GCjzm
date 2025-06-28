using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.ExportImport;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;
using Wfm.Services.TimeSheet;

namespace Wfm.Services.Reports
{

    public class OneWeekFollowUpTask : IScheduledTask
    {
        #region Field
        private readonly ITimeSheetService _timeSheetService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IAccountService _accountServive;
        private readonly IExportManager _exportManager;
        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public OneWeekFollowUpTask(ITimeSheetService timeSheetService,
                                    IWorkflowMessageService workflowMessageService,
                                    IAccountService accountServive,
                                    IExportManager exportManager,
                                    ILogger logger)
        {
            _timeSheetService = timeSheetService;
            _workflowMessageService = workflowMessageService;
            _accountServive = accountServive;
            _exportManager = exportManager;
            _logger = logger;
        }
        #endregion
        public virtual void Execute()
        {
            var data = _timeSheetService.GetAllOneWeekFollowUpData(DateTime.Today,0);
            Dictionary<int,List<OneWeekFollowUpReportData>> groupable = new Dictionary<int,List<OneWeekFollowUpReportData>>();
            foreach(var d in data)
            {
                if(groupable.Keys.Contains(d.RecruiterId))
                    groupable[d.RecruiterId].Add(d);
                else
                    groupable.Add(d.RecruiterId, new List<OneWeekFollowUpReportData>() { d });

                if (d.OwnerId != d.RecruiterId)
                {
                    if (groupable.Keys.Contains(d.OwnerId))
                        groupable[d.OwnerId].Add(d);
                    else
                        groupable.Add(d.OwnerId, new List<OneWeekFollowUpReportData>() { d });
                }
                

            }
            foreach (var group in groupable)
            {
                var account = _accountServive.GetAccountById(group.Key);
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.GenerateOneWeekFollowUpExcel(stream, group.Value);
                    bytes = stream.ToArray();
                }
                if (bytes != null)
                    _workflowMessageService.SendOneWeekFollowUpReminderToRecruiter(account, fileName, bytes);
                else
                {
                    _logger.Error("GenerateOneWeekFollowUpExcel():Cannot generate the excel file!");   
                }
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}

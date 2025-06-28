using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Configuration;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;


namespace Wfm.Services.TimeSheet
{
    public partial class PendingApprovalReminderAlertTask : IScheduledTask
    {
        #region Fields

        private readonly IWorkTimeService _workTimeService;
        private readonly ITimeSheetService _timeSheetService;
        private readonly ILogger _logger;
        private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public PendingApprovalReminderAlertTask(
            IWorkTimeService workTimeService,
            ITimeSheetService timeSheetService,
            CandidateWorkTimeSettings candidateWorkTimeSettings,
            IAccountService accountService,
            IWorkContext workContext,
            ILogger logger,
            IWorkflowMessageService workflowMessageService)
        {
            this._workTimeService = workTimeService;
            this._timeSheetService = timeSheetService;
            this._logger = logger;
            this._candidateWorkTimeSettings = candidateWorkTimeSettings;
            this._accountService = accountService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var reminderDay = DayOfWeek.Sunday + _candidateWorkTimeSettings.PendingApprovalReminderDay;
                var reminderTime = DateTime.MinValue.AddHours(9);                                       // 09:00
                DateTime.TryParse(_candidateWorkTimeSettings.PendingApprovalReminderTime, out reminderTime);

                var clock = DateTime.Now;
                // it is time to send reminder
                if (clock.DayOfWeek == reminderDay && clock.Hour == reminderTime.Hour)
                {
                    var lastSunday = clock.AddDays(DayOfWeek.Sunday - clock.DayOfWeek - 7).Date;
                    var lastSaturday = lastSunday.AddDays(6);
                    var todayNow = clock.Date + reminderTime.TimeOfDay;
                    var dueDay = lastSaturday.AddDays(1 + _candidateWorkTimeSettings.PendingApprovalDueDay);
                    var dueTime = DateTime.MinValue.AddHours(14);                                       // 14:00
                    DateTime.TryParse(_candidateWorkTimeSettings.PendingApprovalDueTime, out dueTime);
                    dueTime = dueDay.Date + dueTime.TimeOfDay;

                    this.SendPendingApprovalReminderToClient(lastSunday, lastSaturday, todayNow, dueTime);
                }
            }

            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while sending approval reminder. Error message : {0}", exc.Message), exc);
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private void SendPendingApprovalReminderToClient(DateTime startDate, DateTime endDate, DateTime todayNow, DateTime dueTime)
        {
            byte[] attachmentFile;
            string attachmentFileName;
            
            var ids = _timeSheetService.GetCompaniesWithWorkTimePendingForApproval(startDate);

            // by company
            foreach (var companyId in ids)
            {
                // HR managers, in single message
                var receivers = _accountService.GetAllCompanyHrAccountsByCompany(companyId).ToList();
                if (receivers.Any())
                {
                    var account = receivers.FirstOrDefault();       // first or any HR
                    attachmentFileName = _timeSheetService.GetTimeSheetAttachment(account, startDate, endDate, out attachmentFile, null);
                    if (!String.IsNullOrWhiteSpace(attachmentFileName) && attachmentFile.Length > 0)
                        _workflowMessageService.SendPendingApprovalReminder(1, receivers, todayNow, dueTime, startDate, endDate, attachmentFileName, attachmentFile);
                }

                // supervisors, in individual messges
                var supervisorIds = _workTimeService.GetSupervisorIdsWithPendingApproval(startDate, endDate, companyId);
                receivers = _accountService.GetAllClientAccountForTask().Where(x => supervisorIds.Contains(x.Id)).ToList();
                foreach (var account in receivers)
                {
                    attachmentFileName = _timeSheetService.GetTimeSheetAttachment(account, startDate, endDate, out attachmentFile,null);
                    if (!String.IsNullOrWhiteSpace(attachmentFileName) && attachmentFile.Length > 0)
                        _workflowMessageService.SendPendingApprovalReminder(1, new List<Account>() { account }, todayNow, dueTime, startDate, endDate, attachmentFileName, attachmentFile);
                }
            }
        }

    }

}

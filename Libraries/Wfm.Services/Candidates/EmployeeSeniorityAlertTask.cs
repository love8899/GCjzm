using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Configuration;
using Wfm.Services.ExportImport;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;


namespace Wfm.Services.Candidates
{
    public partial class EmployeeSeniorityAlertTask : IScheduledTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly CandidateSettings _candidateSettings;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly IExportManager _exportManager;

        #endregion

        #region Ctor

        public EmployeeSeniorityAlertTask(
            CandidateSettings candidateSettings,
            IAccountService accountService,
            IWorkContext workContext,
            ILogger logger,
            IWorkflowMessageService workflowMessageService,
            IPaymentHistoryService paymentHistoryService,
            IExportManager exportManager)
        {
            this._logger = logger;
            this._candidateSettings = candidateSettings;
            this._accountService = accountService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._paymentHistoryService = paymentHistoryService;
            this._exportManager = exportManager;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var alertDay = DayOfWeek.Sunday + _candidateSettings.EmployeeSeniorityAlertDay;
                var alertTime = DateTime.MinValue.AddHours(9);
                DateTime.TryParse(_candidateSettings.EmployeeSeniorityAlertTime, out alertTime);

                var clock = DateTime.Now;
                if (clock.DayOfWeek == alertDay && clock.Hour == alertTime.Hour)
                {
                    if (!String.IsNullOrWhiteSpace(_candidateSettings.EmployeeSeniorityAlertRecipient))
                        this._SendEmployeeSeniorityAlert(clock.Date, _candidateSettings.EmployeeSeniorityAlertScope, _candidateSettings.EmployeeSeniorityAlertThreshold, _candidateSettings.EmployeeSeniorityAlertRecipient);
                    else
                        _logger.Error("Error occurred while sending employee seniority alert: alert recipient is no defined.");
                }
            }
            catch (Exception exc)
            {
                _logger.Error(String.Format("Error occurred while sending employee seniority alert. Error message : {0}", exc.Message), exc);
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private void _SendEmployeeSeniorityAlert(DateTime refDate, int scope, decimal threshold, string recipient)
        {
            byte[] attachmentFile = null;
            string attachmentFileName = null;

            var employees = _paymentHistoryService.GetEmployeeSeniorityReport(refDate, scope, threshold);
            if (employees.Any())
            {
                using (var stream = new MemoryStream())
                {
                    attachmentFileName = _exportManager.ExportEmployeeSeniorityReportToXlsx(stream, employees, threshold);
                    attachmentFile = stream.ToArray();
                }
            }

            _workflowMessageService.SendEmployeeSeniorityAlert(refDate, scope, threshold, recipient, attachmentFileName, attachmentFile);
        }
    }
}

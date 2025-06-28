using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Configuration;
using Wfm.Services.ExportImport;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;


namespace Wfm.Services.TimeSheet
{
    public partial class MissingHourFollowUpAlertTask : IScheduledTask
    {
        #region Fields

        private readonly CandidateMissingHourSettings _candidateMissingHourSettings;
        private readonly IMissingHourService _missingHourService;
        private readonly IExportManager _exportManager;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public MissingHourFollowUpAlertTask(
            CandidateMissingHourSettings candidateMissingHourSettings,
            IMissingHourService missingHourService,
            IExportManager exportManager,
            IWorkflowMessageService workflowMessageService,
            IAccountService accountService,
            ILogger logger)
        {
            this._candidateMissingHourSettings = candidateMissingHourSettings;
            this._missingHourService = missingHourService;
            this._exportManager = exportManager;
            this._workflowMessageService = workflowMessageService;
            this._accountService = accountService;
            this._logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var refDateTime = DateTime.UtcNow.AddDays(-_candidateMissingHourSettings.FollowUpIntervalInDays);
                var missingHours = _missingHourService.GetAllCandidateMissingHourAsQueryable(isAccountBased: false)
                                   .Where(x => x.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.PendingApproval)
                                   .Where(x => !x.LastAskForApprovalOnUtc.HasValue || x.LastAskForApprovalOnUtc.Value < refDateTime);

                var recruiterIds = missingHours.Select(x => x.EnteredBy).Distinct();
                var recruiters = _accountService.GetAllAccountsAsQueryable().Where(x => recruiterIds.Contains(x.Id));
                foreach (var recruiter in recruiters.ToList())
                {
                    var missingHoursByRecruiter = missingHours.Where(x => x.EnteredBy == recruiter.Id);
                    byte[] attachmentBytes = null;
                    var attachmentName = _GetMissingHourExcelFile(_exportManager, missingHoursByRecruiter, out attachmentBytes);

                    _workflowMessageService.SendMissingHourFollowUpAlert(recruiter, attachmentName, attachmentBytes);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Failed to send missing hour follow-up alert. Error message : {0}", exc.Message), exc);
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private string _GetMissingHourExcelFile(IExportManager _exportManager, IEnumerable<CandidateMissingHour> missingHours, out byte[] bytes)
        {
            var fileName = String.Empty;
            using (var stream = new MemoryStream())
            {
                fileName = _exportManager.ExportMissingHourToXlsx(stream, missingHours, toClient: false);
                bytes = stream.ToArray();
            }

            return fileName;
        }

    }

}

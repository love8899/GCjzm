using System;
using System.Threading.Tasks;
using Wfm.Services.Configuration;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;
using Wfm.Services.TimeSheet;

namespace Wfm.Services.JobOrders
{
    public partial class AlertMissingClockInOutTask : IScheduledTask
    {
        #region Fields

        private readonly IWorkTimeService _workTimeService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public AlertMissingClockInOutTask(
            IWorkTimeService workTimeService,
            ISettingService settingService,
            ILogger logger)
        {
            this._workTimeService = workTimeService;
            this._logger = logger;
            this._settingService = settingService;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {

                _workTimeService.AlertMissingClockInOut();

            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while checking candidate clock in/out. Error message : {0}", exc.Message), exc);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}

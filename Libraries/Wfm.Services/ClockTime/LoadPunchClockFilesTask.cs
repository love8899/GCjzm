using System;
using System.Threading.Tasks;
using Wfm.Services.Configuration;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;

namespace Wfm.Services.ClockTime
{
    public partial class LoadPunchClockFilesTask : IScheduledTask
    {
        #region Fields

        private readonly IClockTimeService _clockTimeService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public LoadPunchClockFilesTask(
            IClockTimeService clockTimeService,
            ISettingService settingService,
            ILogger logger)
        {
            this._clockTimeService = clockTimeService;
            this._logger = logger;
            this._settingService = settingService;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                // Load clock times
                _clockTimeService.LoadPunchClockTime();
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while executing task and loading punch clock files into database. Error message : {0}", exc.Message), exc);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }

    }
}

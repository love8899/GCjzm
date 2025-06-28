using System;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Services.ClockTime
{
    public partial class SyncClockTimeTask : IScheduledTask
    {
        #region Fields

        private readonly TimeClockSettings _timeClockSettings;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly ILogger _logger;

        #endregion


        #region Ctor

        public SyncClockTimeTask(
            TimeClockSettings timeClockSettings,
            IClockDeviceService clockDeviceService,
            ILogger logger)
        {
            this._timeClockSettings = timeClockSettings;
            this._clockDeviceService = clockDeviceService;
            this._logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            var timeNow = DateTime.Now;
            if (timeNow.Hour == _timeClockSettings.SyncHour)
            {
                var clocks = _clockDeviceService.GetAllClockDevicesWithIPAddress(excludeEnrolment: false).ToList();
                foreach (var clock in clocks)
                {
                    using (var hr = new HandReader(clock.IPAddress))
                    {
                        if (hr != null && hr.TryConnect())
                        {
                            var minDrift = _timeClockSettings.MinDrift;
                            var maxDrift = _timeClockSettings.MaxDrift > 0 ? _timeClockSettings.MaxDrift : (int?)null;
                            var synced = hr.SyncClockTime(minDrift, maxDrift);

                            var dstSet = hr.SetDayLightSavingTimes(timeNow.Year);
                        }
                        else
                            _logger.Error(String.Concat("Error occurred while sync time of clock ", clock.ClockDeviceUid, ": ", "The clock is not ready."));
                    }
                }
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}

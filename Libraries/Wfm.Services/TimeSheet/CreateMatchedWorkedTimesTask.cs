using System;
using System.Threading.Tasks;
using Wfm.Services.Configuration;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;

namespace Wfm.Services.TimeSheet
{
    public partial class CreateMatchedWorkedTimesTask : IScheduledTask
    {
        #region Fields

        private readonly IWorkTimeService _workTimeService;
        private readonly ILogger _logger;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;

        #endregion

        public CreateMatchedWorkedTimesTask(
            IWorkTimeService workTimeService,
            ISettingService settingService,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            ILogger logger)

        {
            this._workTimeService = workTimeService;
            this._logger = logger;
            this._activityLogService = activityLogService;
            this._localizationService = localizationService;
        }

        public virtual void Execute()
        {
            try
            {
                // Calculate work time previous day and today (including over night shift)
                var endDateTime = DateTime.Now; // new DateTime(2016, 09, 07); //
                var startDateTime = endDateTime.AddDays(-1);
                var errors = _workTimeService.CaptureCandidateWorkTime(startDateTime, endDateTime, null, false, true);

                //activity log
                _activityLogService.InsertActivityLog("CreateMatchedWorkedTimes", _localizationService.GetResource("ActivityLog.CreateMatchedWorkedTimes"),
                    startDateTime.ToString("yyyy-MMM-dd HH:mm") + " - " + endDateTime.ToString("yyyy-MMM-dd HH:mm") + " : " + String.Join(", ", errors));
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while creating matched work time records. Error message : {0}", exc.Message), exc);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}

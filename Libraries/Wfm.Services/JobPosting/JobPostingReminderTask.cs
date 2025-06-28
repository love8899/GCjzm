using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Services.Configuration;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;


namespace Wfm.Services.JobPosting
{
    public partial class JobPostingReminderTask : IScheduledTask
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IJobPostService _jobPostingService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public JobPostingReminderTask(
            IWorkContext workContext,
            ILogger logger,
            ISettingService settingService,
            IJobPostService jobPostingService,
            IWorkflowMessageService workflowMessageService)
        {
            _workContext = workContext;
            _logger = logger;
            _settingService = settingService;
            _jobPostingService = jobPostingService;
            this._workflowMessageService = workflowMessageService;
        }

        #endregion


        public void Execute()
        {
            try {
                int dueHours;
                var dueHoursString = _settingService.GetSettingByKey<string>("JobPostingSettings.SubmissionDueHours");
                if (!int.TryParse(dueHoursString, out dueHours))
                    dueHours = 24;

                var TimeNow = DateTime.Now;

                var jobPsotings = _jobPostingService.GetAllUnsubmittedJobPosts()
                                  .Where(x => DbFunctions.DiffHours(TimeNow,
                                                                    DbFunctions.CreateDateTime(x.StartDate.Year, x.StartDate.Month, x.StartDate.Day,
                                                                                               x.StartTime.Hour, x.StartTime.Minute, x.StartTime.Second))
                                              <= dueHours);

                foreach (var jobPosting in jobPsotings)
                    _workflowMessageService.SendJobPostingSubmissionReminder(jobPosting);
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while sending job posting submission reminder. Error message : {0}", exc.Message), exc);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }

    }

}

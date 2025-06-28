using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Services.ClockTime
{
    public partial class RefreshClockCandidateTask : IScheduledTask
    {
        #region Fields

        private readonly IClockDeviceService _clockDeviceService;
        private readonly ISmartCardService _smartCardService;
        private readonly IHandTemplateService _handTemplateService;
        private readonly IClockCandidateService _clockCandidateService;
        private readonly IClockTimeService _clockTimeService;
        private readonly ICandidateJobOrderService _placementService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        #endregion


        #region Ctor

        public RefreshClockCandidateTask(
            IClockDeviceService clockDeviceService,
            ISmartCardService smartCardService,
            IHandTemplateService handTemplateService,
            IClockCandidateService clockCandidateService,
            IClockTimeService clockTimeService,
            ICandidateJobOrderService placementService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IAccountService accountService,
            ILogger logger)
        {
            this._clockDeviceService = clockDeviceService;
            this._smartCardService = smartCardService;
            this._handTemplateService = handTemplateService;
            this._clockCandidateService = clockCandidateService;
            this._clockTimeService = clockTimeService;
            this._placementService = placementService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._accountService = accountService;
            this._logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            var today = DateTime.Today;
            var clocks = _clockDeviceService.GetAllClockDevicesWithIPAddress(excludeEnrolment: false).ToList();     // include enrolment clock
            foreach (var clock in clocks)
            {
                var candidates = _clockCandidateService.GetAllClockCandidatesByClock(clock.Id);

                // TODO: refresh only on specified hour (local or UTC???)
                if (true || !clock.RefreshHour.HasValue || DateTime.Now.Hour == clock.RefreshHour)
                {
                    var error = string.Empty;

                    if (clock.AddOnEnroll)  // exclude enrolment only clocks
                    {
                        // add those placed for the company
                        var nextDay = today.AddDays(1);
                        var placed = _placementService.GetAllCandidateJobOrdersByDateRangeAsQueryable(today, nextDay)
                            .Where(x => x.JobOrder.CompanyId == clock.CompanyLocation.CompanyId).Select(x => x.CandidateId).Distinct();
                        // exclude added aleady while card & hand not updated
                        var smartCards = _smartCardService.GetAllSmartCardsAsQueryable(showInactive: false);
                        var handTemplates = _handTemplateService.GetAllHandTemplatesAsQueryable(showInactive: false);
                        var addedAndNotUpdated = from c in candidates
                                                 join sc in smartCards on c.CandidateId equals sc.CandidateId
                                                 join ht in handTemplates on c.CandidateId equals ht.CandidateId
                                                 where c.AddedOnUtc > sc.UpdatedOnUtc && c.AddedOnUtc > ht.UpdatedOnUtc
                                                 select c.CandidateId;
                        var toBeAdded = placed.Except(addedAndNotUpdated);
                        if (toBeAdded.Any())
                        {
                            error = _AddCandidates(clock, toBeAdded);
                            if (!String.IsNullOrWhiteSpace(error))
                                _logger.Error(String.Concat("Error occurred while adding candidates to clock ", clock.ClockDeviceUid, ": ", error));
                        }
                    }

                    // remove those recently not worked
                    var daysAgo = today.AddDays(0 - (clock.ExpiryDays ?? 90));
                    var daysAgoUtc = daysAgo.ToUniversalTime();
                    var addedEarlierThanDaysAgo = candidates.Where(x => x.AddedOnUtc < daysAgoUtc).Select(x => x.CandidateId);
                    var workedRecently = _clockTimeService.GetAllCandidateClockTimesAsQueryable().Where(x => x.ClockDeviceUid == clock.ClockDeviceUid)
                        .Where(x => x.CandidateId.HasValue && x.ClockInOut >= daysAgo).Select(x => x.CandidateId.Value).Distinct();
                    var toBeRemoved = addedEarlierThanDaysAgo.Except(workedRecently);
                    if (toBeRemoved.Any())
                    {
                        error = _RemoveCandidates(clock, toBeRemoved);
                        if (!String.IsNullOrWhiteSpace(error))
                            _logger.Error(String.Concat("Error occurred while removing candidates from clock ", clock.ClockDeviceUid, ": ", error));
                    }
                }
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private string _RemoveCandidates(CompanyClockDevice clock, IEnumerable<int> candidateIds)
        {
            int done = 0, failed = 0;
            var errors = new StringBuilder();

            using (var hr = new HandReader(clock.IPAddress))
            {
                if (hr != null && hr.TryConnect())
                {
                    foreach (var id in candidateIds.ToList())
                    {
                        try
                        {
                            if (_clockCandidateService.AddOrRemoveCandidate(clock, hr, "remove", id))
                                done++;
                            else
                                failed++;
                        }
                        catch (Exception e)
                        {
                            errors.AppendLine(String.Concat(e.Message, " ", e.InnerException != null ? e.InnerException.Message : string.Empty));
                            failed++;
                        }
                    }
                }
                else
                    errors.AppendLine("The clock is not ready.");
            }

            return errors.ToString();
        }


        private string _AddCandidates(CompanyClockDevice clock, IEnumerable<int> candidateIds)
        {
            int done = 0, failed = 0;
            var failedIds = new List<int>();
            var errors = new StringBuilder();

            using (var hr = new HandReader(clock.IPAddress))
            {
                if (hr != null && hr.TryConnect())
                {
                    foreach (var id in candidateIds.ToList())
                    {
                        try
                        {
                            if (_clockCandidateService.AddOrRemoveCandidate(clock, hr, "add", id))
                                done++;
                            else
                            {
                                failed++;
                                failedIds.Add(id);
                            }
                        }
                        catch (Exception e)
                        {
                            errors.AppendLine(e.Message);
                            failed++;
                        }
                    }
                }
                else
                    errors.AppendLine("The clock is not ready.");
            }

            if (failedIds.Any())
                _SendClockCandidateReminder(clock, failedIds);

            return errors.ToString();
        }


        private void _SendClockCandidateReminder(CompanyClockDevice clock, List<int> candidateIds)
        {
            var subject = "Cannot add hand template to punch clock";
            var actions = "Please double check their smart cards and hand templates, then try to add hand templates manually.";

            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var messages = _placementService.GetAllCandidateJobOrdersByDateRangeAsQueryable(today, nextDay)
                .Where(x => x.JobOrder.CompanyId == clock.CompanyLocation.CompanyId && candidateIds.Contains(x.CandidateId))
                .Select(x => new
                {
                    x.JobOrder.RecruiterId,
                    x.CandidateId,
                }).Distinct().AsEnumerable().GroupBy(x => x.RecruiterId)
                .Select(g => new
                {
                    RecruiterId = g.Key,
                    Message = g.Aggregate(subject + ", for candidate:", (a, b) => a + " " + b.CandidateId),
                });

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            foreach (var msg in messages)
            {
                var recruiter = _accountService.GetAccountById(msg.RecruiterId);
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail()
                {
                    Priority = 3,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = recruiter.FullName,
                    To = recruiter.Email,
                    Bcc = _emailAccountSettings.DefaultBccEmailAddress,
                    Subject = subject,
                    Body = String.Concat(msg.Message, Environment.NewLine, Environment.NewLine, actions),
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                });
            }
        }
    }
}

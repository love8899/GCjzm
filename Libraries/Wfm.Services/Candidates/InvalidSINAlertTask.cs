using System;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Accounts;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;


namespace Wfm.Services.Candidates
{
    public partial class InvalidSINAlertTask : IScheduledTask
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IAccountService _accountService;
        private readonly ICandidateService _candidateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public InvalidSINAlertTask(EmailAccountSettings emailAccountSettings,
            IAccountService accountService,
            ICandidateService candidateService,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            ILogger logger)
        {
            this._emailAccountSettings = emailAccountSettings;
            this._accountService = accountService;
            this._candidateService = candidateService;
            this._emailAccountService = emailAccountService;
            this._queuedEmailService = queuedEmailService;
            this._logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var subject = String.Format("Invalid SIN ({0})", DateTime.Today.ToString("yyyy-MM-dd"));
                var starter = String.Concat("Invalid SIN (Social Insurance Number):", Environment.NewLine);
                var actions = "Please collect correct SIN info and update in the system ASAP.";

                var messages = _candidateService.GetAllCandidatesAsQueryable()
                    .Where(x => x.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started)
                    .Where(x => x.SocialInsuranceNumber == null || x.SocialInsuranceNumber == "000000000")
                    .AsEnumerable().GroupBy(x => x.OwnerId).Select(g => new
                    {
                        RecruiterId = g.Key,
                        Message = g.Aggregate(starter, (a, b) => 
                            String.Concat(a, Environment.NewLine, b.Id, ", ", b.FirstName, " ", b.LastName, ", ", b.SocialInsuranceNumber))
                    });

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                foreach (var msg in messages)
                {
                    var recruiter = _accountService.GetAccountById(msg.RecruiterId);
                    if (recruiter.IsActive && !recruiter.IsDeleted)
                    {
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
            catch (Exception exc)
            {
                _logger.Error(string.Format("Failed to send invalid SIN alert. Error message: {0}", exc.Message), exc);
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}

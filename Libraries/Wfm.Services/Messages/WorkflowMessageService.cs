using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Forums;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Incident;
using Wfm.Core.Domain.JobOrders;
using JP_Core = Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.Messages;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Franchises;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Media;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Scheduling;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Logging;
using Wfm.Services.Test;
using Wfm.Core.Domain.Tests;
using Wfm.Services.Payroll;


namespace Wfm.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IClientNotificationService _clientNotificationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IMessageHistoryService _messageHistoryService;
        private readonly ILanguageService _languageService;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IFranchiseService _franchiseService;
        private readonly IAccountService _accountService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IRepository<CandidateWorkTime> _workTimeRepository;
        private readonly IWorkContext _workContext;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IAttachmentTypeService _attachmentTypeService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IPayrollCalendarService _calendarService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly ILogger _logger;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IAttachmentService _attachmentService;
        private readonly IWebHelper _webHelper;
        private readonly ICompanyEmailTemplateService _companyEmailTemplateService;
        private readonly ICandidateService _candidateService;
        private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly ITestService _testService;
        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IClientNotificationService clientNotificationService,
            IQueuedEmailService queuedEmailService,
            IMessageHistoryService messageHistoryService,
            ILanguageService languageService,
            ITokenizer tokenizer,
            IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            IFranchiseService franchiseService,
            IAccountService accountService,
            IJobOrderService jobOrderService,
            IRepository<CandidateWorkTime> workTimeRepository,
            IWorkContext workContext,
            EmailAccountSettings emailAccountSettings,
            ICompanyDivisionService companyDivisionService,
            ICompanyBillingService companyBillingService,
            IPayrollCalendarService calendarService,
            IAttachmentTypeService attachmentTypeService,
            IRecruiterCompanyService recruiterCompanyService,
            ICompanyVendorService companyVendorService,
            ICandidateJobOrderService candidateJobOrderService,
            ILogger logger,
            IAttachmentService attachmentService,
            IWebHelper webHelper,
            ICompanyEmailTemplateService companyEmailTemplateService,
            CandidateWorkTimeSettings candidateWorkTimeSettings,
            ICandidateService candidateService,
            ITestService testService)
          
        {
            this._messageTemplateService = messageTemplateService;
            this._clientNotificationService = clientNotificationService;
            this._queuedEmailService = queuedEmailService;
            this._messageHistoryService = messageHistoryService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._franchiseService = franchiseService;
            this._accountService = accountService;
            this._emailAccountSettings = emailAccountSettings;
            this._attachmentTypeService = attachmentTypeService;
            this._jobOrderService = jobOrderService;
            this._workTimeRepository = workTimeRepository;
            this._workContext = workContext;
            this._recruiterCompanyService = recruiterCompanyService;
            this._companyDivisionService = companyDivisionService;
            this._companyBillingService = companyBillingService;
            this._calendarService = calendarService;
            this._companyVendorService = companyVendorService;
            this._candidateJobOrderService = candidateJobOrderService;
            this._logger = logger;
            this._attachmentService = attachmentService;
            this._webHelper = webHelper;
            this._companyEmailTemplateService = companyEmailTemplateService;
            _candidateWorkTimeSettings = candidateWorkTimeSettings;
            this._candidateService = candidateService;
            this._testService = testService;
        }

        #endregion

        #region Utilities

        private string TruncateEmailAddress(string address, int maxLength = 500)
        {
            if (String.IsNullOrWhiteSpace(address))
                return address;

            if (address.Length > maxLength)
            {
                address = address.Substring(0, maxLength);
                address = address.Substring(0, address.LastIndexOf(';'));
            }

            return address;
        }

        private int SendNotification(QueuedEmail email)
        {
            _queuedEmailService.InsertQueuedEmail(email);

            return email.Id;
        }

        private void SendNotification(IList<QueuedEmail> email)
        {
            _queuedEmailService.InsertQueuedEmail(email);
        }

        protected virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName, int toAccountId, string cc = null,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            var email = GenerateQueuedEmail(messageTemplate, emailAccount, languageId, tokens,
                                            toEmailAddress, toName, toAccountId, cc,
                                            attachmentFilePath, attachmentFileName);

            return this.SendNotification(email);
        }

        protected virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string fromEmailAddress, string fromName, int fromAccountId,
            string toEmailAddress, string toName, int toAccountId, string cc = null,
            string attachmentFilePath = null, string attachmentFileName = null, byte[] attachmentFile = null,
            string attachmentFileName2 = null, byte[] attachmentFile2 = null, 
            string attachmentFileName3 = null, byte[] attachmentFile3 = null,
            string replyTo = null, string replyToName = null)
        {
            var email = GenerateQueuedEmail(messageTemplate, emailAccount, languageId, tokens,
                                            fromEmailAddress, fromName, fromAccountId, toEmailAddress, toName, toAccountId, cc,
                                            attachmentFilePath, attachmentFileName, attachmentFile,attachmentFileName2,attachmentFile2,attachmentFileName3,attachmentFile3,
                                            replyTo: replyTo, replyToName: replyToName);

            return this.SendNotification(email);
        }


        public QueuedEmail GenerateQueuedEmail(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName, int toAccountId, string cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            string attachmentFileName2 = null, byte[] attachmentFile2 = null, 
            string attachmentFileName3 = null, byte[] attachmentFile3 = null)
        {
            var fromEmailAddress = emailAccount.Email;
            var fromName = emailAccount.DisplayName;
            var fromAccountId = emailAccount.Id;

            return GenerateQueuedEmail(messageTemplate, emailAccount, languageId, tokens,
                                       fromEmailAddress, fromName, fromAccountId, toEmailAddress, toName, toAccountId, cc,
                                       attachmentFilePath, attachmentFileName);
        }


        public QueuedEmail GenerateQueuedEmail(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string fromEmailAddress, string fromName, int fromAccountId,
            string toEmailAddress, string toName, int toAccountId, string cc = null,
            string attachmentFilePath = null, string attachmentFileName = null, byte[] attachmentFile = null,
            string attachmentFileName2 = null, byte[] attachmentFile2 = null, 
            string attachmentFileName3 = null, byte[] attachmentFile3 = null,
            bool includeLogo = false,int? confirmationEmailLinkId=null,
            string replyTo = null, string replyToName = null)
        {
            //retrieve localized message template data
            if (String.IsNullOrEmpty(cc))
                cc = messageTemplate.CCEmailAddresses;
            var bcc = messageTemplate.BccEmailAddresses;
            var subject = messageTemplate.Subject;
            var body = messageTemplate.Body;

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            var attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(attachmentFileName));
            var attachmentType2 = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(attachmentFileName2));
            var attachmentType3 = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(attachmentFileName3));
            var email = new QueuedEmail()
            {
                Priority = 5,
                From = fromEmailAddress,
                FromName = fromName,
                FromAccountId = fromAccountId,
                To = TruncateEmailAddress(toEmailAddress),
                ToName = toName,
                ToAccountId = toAccountId,
                ReplyTo = replyTo,
                ReplyToName = replyToName,
                CC = TruncateEmailAddress(cc),
                Bcc = TruncateEmailAddress(bcc),
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachmentFile = attachmentFile,
                AttachmentFile2=attachmentFile2,
                AttachmentFileName2=attachmentFileName2,
                AttachmentFile3=attachmentFile3,
                AttachmentFileName3 =attachmentFileName3,
                AttachmentTypeId2 = attachmentType2 != null ? attachmentType2.Id : (int?)null,
                AttachmentTypeId3 = attachmentType3 != null ? attachmentType3.Id : (int?)null,
                AttachmentTypeId = attachmentType != null ? attachmentType.Id : (int?)null,
                MessageCategoryId = messageTemplate.MessageCategoryId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                IncludeLogo = includeLogo,
                ConfirmationEmailLinkId = confirmationEmailLinkId
            };
            return email;
        }


        public virtual MessageTemplate GetActiveMessageTemplate(string messageTemplateName, int franchaseId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName, franchaseId);

            //no template found or it not active
            if (messageTemplate == null || !messageTemplate.IsActive)
                return null;

            return messageTemplate;
        }

        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccounId = messageTemplate.EmailAccountId;
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccounId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;

        }

        protected virtual int EnsureLanguageIsActive(int languageId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.IsActive)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");
            return language.Id;
        }

        protected virtual Franchise GetFranchise(int franchiseId = 0)
        {
            var franchise = _franchiseService.GetFranchiseById(franchiseId);

            if (franchise == null)
                //throw new Exception("No franchise could be loaded");
                franchise = new Franchise();

            return franchise;
        }



        #endregion

        #region Account workflow

        /// <summary>
        /// Sends 'New account' notification message to a franchise owner
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountRegisteredNotificationMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            //var franchise = _franchiseContext.CurrentFranchise;
            //languageId = EnsureLanguageIsActive(languageId, franchise.Id);

            var messageTemplate = GetActiveMessageTemplate("NewAccount.Notification", 1);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(account.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddAccountTokens(tokens, account);


            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            var toAccountId = emailAccount.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends a welcome message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountWelcomeMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");


            var messageTemplate = GetActiveMessageTemplate("Account.WelcomeMessage", 1);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(account.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddAccountTokens(tokens, account);


            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends an email validation message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountEmailValidationMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Account.EmailValidationMessage", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(account.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddAccountTokens(tokens, account);


            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends password recovery message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAccountPasswordRecoveryMessage(Account account, int languageId)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Account.PasswordRecovery", 0);
            if (messageTemplate == null)
                return 0;

            //email account for sending email
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(account.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddAccountTokens(tokens, account);


            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, toAccountId);
        }

        #endregion

        #region Candidate workflow

        /// <summary>
        /// Sends 'New candidate' notification message to the franchise
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCandidateRegisteredNotificationMessage(Candidate candidate, int languageId)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            //var franchise = _franchiseContext.CurrentFranchise;
            //languageId = EnsureLanguageIsActive(languageId, franchise.Id);

            var messageTemplate = GetActiveMessageTemplate("Candidate.NewCandidateNotification", 1);
            if (messageTemplate == null)
                return 0;

            //email candidate
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(candidate.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddCandidateTokens(tokens, candidate);

            //var toEmail = emailAccount.Email;
            //var toName = emailAccount.DisplayName;
            string toEmail = _emailAccountSettings.DefaultToEmailAddress;
            string toName = null;
            var defaultAccount = _emailAccountService.GetAllEmailAccounts().Where(x => x.Email == toEmail).FirstOrDefault();
            var toAccountId = defaultAccount == null ? 0 : defaultAccount.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends a welcome message to a candidate
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCandidateWelcomeMessage(Candidate candidate, int languageId)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            var messageTemplate = GetActiveMessageTemplate("Candidate.WelcomeMessage", 1);
            if (messageTemplate == null)
                return 0;

            //email candidate
            var emailCandidate = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(candidate.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddCandidateTokens(tokens, candidate);

            var toEmail = candidate.Email;
            var toName = candidate.GetFullName();
            var toAccountId = candidate.Id;
            var qrCodeBytes = CommonHelper.ToQrCode(_candidateService.GetCandidateQrCodeStr(candidate), text: candidate.Id.ToString("D8"));
            return SendNotification(messageTemplate, emailCandidate,
                languageId, tokens,
                emailCandidate.Email, emailCandidate.DisplayName, emailCandidate.Id,
                toEmail, toName, toAccountId,
                attachmentFileName: "ID_QR_CODE.png", attachmentFile: qrCodeBytes);
        }


        public virtual int SendCandidateIdQrCode(Candidate candidate, int languageId)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            var messageTemplate = GetActiveMessageTemplate("Candidate.IdQrCode", 1);
            if (messageTemplate == null)
                return 0;

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var franchise = GetFranchise(candidate.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);

            var toEmail = candidate.Email;
            var toName = candidate.GetFullName();
            var toAccountId = candidate.Id;
            var qrCodeBytes = CommonHelper.ToQrCode(_candidateService.GetCandidateQrCodeStr(candidate), text: candidate.Id.ToString("D8"));
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                emailAccount.Email, emailAccount.DisplayName, emailAccount.Id,
                toEmail, toName, toAccountId,
                attachmentFileName: "ID_QR_CODE.png", attachmentFile: qrCodeBytes);
        }


        public virtual int SendNotificationWhenNewIncidentCreatedByClient(IncidentReport incident, int languageId)
        {
            if (incident == null)
                throw new ArgumentNullException("incident");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Incident.NewIncidentCreated", _workContext.CurrentAccount.FranchiseId);
            if (messageTemplate == null)
                return 0;

            //email candidate
            var emailfrom = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            var from = _workContext.CurrentAccount;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddIncidentReportTokens(tokens, incident);

            //StringBuilder toEmails = new StringBuilder(_accountService.GetAllMSPEmails());
            StringBuilder toEmails = new StringBuilder();
            if (incident.JobOrderId == null)
            {
                toEmails.Append(_recruiterCompanyService.GetAllRecruitersEmailByCompanyId(incident.CompanyId, incident.Candidate.FranchiseId));
            }
            else
            {
                JobOrder jobOrder = _jobOrderService.GetJobOrderById(incident.JobOrderId.Value);
                int recuiterId = jobOrder.RecruiterId;
                int ownerId = jobOrder.OwnerId;
                if (recuiterId > 0)
                {
                    string recruiterEmail = _accountService.GetAccountById(recuiterId).Email;
                    toEmails.Append(recruiterEmail);
                    toEmails.Append(";");
                }
                if (ownerId > 0)
                {
                    string ownerEmail = _accountService.GetAccountById(ownerId).Email;
                    toEmails.Append(ownerEmail);
                    toEmails.Append(";");
                }
            }

            //MSP emails 
            string MSPEmails = _accountService.GetAllMSPEmails();
            if (MSPEmails.Length > 0)
            {
                toEmails.Append(MSPEmails);
            }
            var toName = "";
            var toAccountId = 0;

            return SendNotification(messageTemplate, emailfrom, languageId, tokens,
                                 from.Email, from.FullName, from.Id,
                                 toEmails.ToString(), toName, toAccountId);
        }

        /// <summary>
        /// Sends an email validation message to a candidate
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCandidateEmailValidationMessage(Candidate candidate, int languageId)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Candidate.EmailValidationMessage", 0);
            if (messageTemplate == null)
                return 0;

            //email candidate
            var emailCandidate = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(candidate.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddCandidateTokens(tokens, candidate);

            var toEmail = candidate.Email;
            var toName = candidate.GetFullName();
            var toAccountId = candidate.Id;
            return SendNotification(messageTemplate, emailCandidate,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends password recovery message to a candidate
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCandidatePasswordRecoveryMessage(Candidate candidate, int languageId)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Candidate.PasswordRecovery", 0);
            if (messageTemplate == null)
                return 0;

            //email candidate
            var emailCandidate = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(candidate.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddCandidateTokens(tokens, candidate);

            var toEmail = candidate.Email;
            var toName = candidate.GetFullName();
            var toAccountId = candidate.Id;
            return SendNotification(messageTemplate, emailCandidate,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        public QueuedEmail GetCandidateRequiredTestsEmail(Candidate candidate, int languageId, int franchiseId)
        {
            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Candidate.TestsMessage", franchiseId);
            if (messageTemplate == null)
            {
                _logger.Error(String.Format("GetCandidateRequiredTestsEmail() : Cannot find the 'Candidate.TestsMessage' template for franchise Id {0}", franchiseId));
                return null;
            }

            var categories = _testService.GetAllRequiredTestCategories();
            if (categories == null || categories.Count <= 0)
            {
                _logger.Error("GetCandidateRequiredTestsEmail() : There are no test categories marked as required for registration. Test email template cannot be generated.");
                return null;
            }

            //email candidate
            var emailCandidate = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var tokens = new List<Token>();
            string toEmail, toName;
            int toAccountId;

            if (candidate != null)
            {
                CandidateTestLink link = new CandidateTestLink() { CandidateId = candidate.Id, Candidate = candidate, EnteredBy = 0, IsUsed = false, ValidBefore = DateTime.Now.AddDays(2) };
                _messageTokenProvider.AddCandidateTokens(tokens, candidate);
                _messageTokenProvider.AddCandidateTestsLinkTokens(tokens, link, categories);
                toEmail = candidate.Email;
                toName = candidate.GetFullName();
                toAccountId = candidate.Id;
            }
            else
            {
                // this is only valid for the Preview screen in Mass Email page
                toEmail = String.Empty;
                toName = String.Empty;
                toAccountId = 0;
            }

            var emailSender = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            QueuedEmail email = email = GenerateQueuedEmail(messageTemplate, emailSender, languageId, tokens, toEmail, toName, toAccountId);
            return email;
        }

        public int SendCandidateTestsMessage(Candidate candidate, int languageId)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            var email = GetCandidateRequiredTestsEmail(candidate, languageId, 0);
            if (email == null)
                return 0;
            return SendNotification(email);
        }
        #endregion

        #region JobOrder workflow

        /// <summary>
        /// Sends an order placed notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendJobOrderPlacedCandidateNotification(JobOrder jobOrder, Candidate candidate, int languageId)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            if (candidate == null)
                throw new ArgumentNullException("candidate");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("JobOrderPlaced.CandidateNotification", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder, languageId);
            _messageTokenProvider.AddCandidateTokens(tokens, candidate);

            var toEmail = candidate.Email;
            var toName = candidate.GetFullName();
            var toAccountId = candidate.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends an order placed notification to a account
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendJobOrderPlacedRecruiterNotification(JobOrder jobOrder, Account account, int languageId)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("JobOrderPlaced.AccountNotification", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(account.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddAccountTokens(tokens, account);
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder, languageId);

            //event notification

            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }


        /// <summary>
        /// Sends the job order applied recruiter notification.
        /// </summary>
        /// <param name="jobOrder">The job order.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="languageId">The language identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// jobOrder
        /// or
        /// candidate
        /// </exception>
        public virtual int SendJobOrderAppliedRecruiterNotification(JobOrder jobOrder, Candidate candidate, int languageId)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            if (candidate == null)
                throw new ArgumentNullException("candidate");


            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("JobOrderApplied.AccountNotification", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string from = emailAccount.Email;
            if (!String.IsNullOrWhiteSpace(candidate.Email))
                from = candidate.Email;

            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder, languageId);
            _messageTokenProvider.AddCandidateTokens(tokens, candidate);

            //recruitor information
            Account account = _accountService.GetAccountById(jobOrder.RecruiterId);
            if (account == null)
                return 0;

            CandidateAttachment attachment = _attachmentService.GetCandidateResumeByCandidateGuid(candidate.CandidateGuid);
            if (attachment == null)
                return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                   from, candidate.GetFullName(), candidate.Id,
                                   account.Email, account.FullName, account.Id);
            if (!String.IsNullOrEmpty(attachment.StoredPath))
            {
                string contentType = attachment.ContentType;
                string originalFileName = attachment.OriginalFileName;
                string basePath = _webHelper.GetRootDirectory();
                string folderPath = Path.Combine(basePath, attachment.StoredPath);

                string filePath = Path.Combine(folderPath, attachment.StoredFileName);

                if (System.IO.File.Exists(filePath))
                    return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                    from, candidate.GetFullName(), candidate.Id,
                                    account.Email, account.FullName, account.Id, null, folderPath, attachment.StoredFileName, null);
                else
                    return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                    from, candidate.GetFullName(), candidate.Id,
                                    account.Email, account.FullName, account.Id);
            }
            else
            {
                return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                    from, candidate.GetFullName(), candidate.Id,
                                    account.Email, account.FullName, account.Id,null,null,attachment.OriginalFileName,attachment.AttachmentFile);
            }
            
        }

        public int SendRemindEmailPlacementNotification(JobOrder jobOrder, DateTime refDate, int languageId)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("JobOrder.RemindSendingPlacement", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            tokens.Add(new Token("RefDate.Time", refDate.ToString("hh:mm tt")));
            tokens.Add(new Token("RefDate.Date", refDate.ToString("MM/dd/yyyy")));
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder, languageId);

            //recruitor information
            Account account = _accountService.GetAccountById(jobOrder.RecruiterId);
            var allow = _clientNotificationService.AllowSendingEmailToAccount(messageTemplate.Id, jobOrder.CompanyId,account);
            
            if (account == null||!allow)
                return 0;

            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        #endregion

        #region ClockTime workflow

        public virtual int SendCandidateMissedClockingRecruiterNotification(Company company, JobOrder jobOrder, Account contact, List<Candidate> candidateList, int languageId)
        {
            if (company == null)
                throw new ArgumentNullException("company");

            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            if (candidateList == null)
                throw new ArgumentNullException("candidate");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("CandidateMissedClocking.AccountNotification", 0);
            if (messageTemplate == null)
                return 0;

            //email copy
            var ccAddresses = new List<string>();
            if (!String.IsNullOrEmpty(messageTemplate.CCEmailAddresses))
                ccAddresses = messageTemplate.CCEmailAddresses.Split(';').ToList();
            if (!String.IsNullOrEmpty(_emailAccountSettings.DefaultCCEmailAddress))
                ccAddresses = ccAddresses.Union(_emailAccountSettings.DefaultCCEmailAddress.Split(';')).ToList();

            //email sender account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddCompanyTokens(tokens, company);
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder, languageId);
            _messageTokenProvider.AddCompanyContactTokens(tokens, contact);
            _messageTokenProvider.AddCandidateListTokens(tokens, candidateList);

            //email to recruitor
            Account recruiter = _accountService.GetAccountById(jobOrder.RecruiterId);
            if (recruiter == null)
                return 0;
            var toEmail = recruiter.Email;
            var toName = recruiter.FullName;
            var toAccountId = recruiter.Id;

            //copy email to owner
            Account owner = _accountService.GetAccountById(jobOrder.OwnerId);
            if (owner != null && !String.IsNullOrEmpty(owner.Email))
                ccAddresses.Add(owner.Email);

            // copy to client recipients
            var allowedAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, company.Id, jobOrder.CompanyLocationId, jobOrder.CompanyDepartmentId)
                                  .Where(x => !x.IsCompanyDepartmentSupervisor() || x.Id == jobOrder.CompanyContactId).ToList();
            if (allowedAccounts.Count > 0)
            {             
                //tokens
                var clientTokens = new List<Token>();
                _messageTokenProvider.AddFranchiseTokens(clientTokens, franchise);
                _messageTokenProvider.AddCompanyTokens(clientTokens, company);
                _messageTokenProvider.AddJobOrderTokens(clientTokens, jobOrder, languageId);
                _messageTokenProvider.AddCompanyContactTokens(clientTokens, contact);
                _messageTokenProvider.AddCandidateListTokensForClient(clientTokens, candidateList);              
                string toClientEmail = allowedAccounts.Count == 1 ? allowedAccounts.Select(x => x.Email).FirstOrDefault() : String.Join(";", allowedAccounts.Select(x => x.Email));
                 SendNotification(messageTemplate, emailAccount, languageId, clientTokens, toClientEmail, company.CompanyName, 0);            
            }

            string msp = _accountService.GetAllMSPEmails();
            if (msp.Length > 0)
                ccAddresses = ccAddresses.Union(msp.Split(';')).ToList();

            //From 
            //if (jobOrder.CompanyContactId>0)
            //{
            //    int fromAccountId = jobOrder.CompanyContactId;
            //    //Account from= _accountService.GetAccountById(fromAccountId);
            //    string fromEmail = emailAccount.Email; // from.Email;
            //    string fromName = emailAccount.DisplayName; // from.FullName;
            //    return SendNotification(messageTemplate, emailAccount,
            //                            languageId, tokens,fromEmail,fromName,fromAccountId,
            //                            toEmail, toName, toAccountId, String.Join(";", ccAddresses));
            //}
            //else
            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, toAccountId, String.Join(";", ccAddresses));
        }

        public void SendWorkTimeRejectionRecruiterNotification(CandidateWorkTime candidateWorkTime, int languageId, Account currentAccount)
        {
            languageId = EnsureLanguageIsActive(languageId);

            //email to recruitor
            Account recruiter = _accountService.GetAccountById(candidateWorkTime.JobOrder.RecruiterId);
            if (recruiter == null)
            {
                // if no recruiter is assigned then select owner.
                recruiter = _accountService.GetAccountById(candidateWorkTime.JobOrder.OwnerId);
            }
            if (recruiter != null)
            {
                var tokens = new List<Token>();
                var toEmail = recruiter.Email;
                var toName = recruiter.FullName;
                var toAccountId = recruiter.Id;
                var from = currentAccount;

                _messageTokenProvider.AddWorktimeRejectionTokens(tokens, candidateWorkTime, from.FullName);
                var messageTemplate = GetActiveMessageTemplate("CandidateWorkTime.RejectWorkTimeNotification", 0);

                //email sender account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                try
                {
                SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                  from.Email, from.FullName, from.Id,
                                  toEmail, toName, toAccountId);
            }
                catch (Exception ex)
                {
                    _logger.Error("SendWorkTimeRejectionRecruiterNotification(): Failed to send the rejection alert.", ex, currentAccount);
                }
            }
            else
            {
                // Unable to send the email. Log the issue
                _logger.Warning(String.Format("SendWorkTimeRejectionRecruiterNotification(): Unable to send the rejection alert because job order {0} has no recruiter or owner.", candidateWorkTime.JobOrder));
            }
        }

        public void SendNotOnBoardedPunchRecruiterNotification(CandidateClockTime candidateClockTime, Candidate candidate, int languageId)
        {
            if (candidateClockTime.CompanyId != null && candidate != null)
            {
                languageId = EnsureLanguageIsActive(languageId);
                var tokens = new List<Token>();
                StringBuilder toEmails = new StringBuilder();
                string[] emails = _accountService.GetAllRecruitersByCompanyIdAndVendorId(Convert.ToInt32(candidateClockTime.CompanyId), candidate.FranchiseId, true)
                                                      .Select(x => x.Email).Distinct().ToArray();
                if (emails.Length > 0)
                {
                    toEmails.Append(String.Join(";", emails));
                    _messageTokenProvider.AddCandidateNotOnBoardedPunchTokens(tokens, candidateClockTime);
                    var messageTemplate = GetActiveMessageTemplate("CandidateClockTime.NotOnboarded", 0);
                    //email sender account
                    var emailfrom = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                    var toName = "";
                    var toAccountId = 0;
                    SendNotification(messageTemplate, emailfrom, languageId, tokens, toEmails.ToString(), toName, toAccountId);
                }
            }
        }

        public void SendUnexpectedPunchNotification(CandidateClockTime candidateClockTime,Candidate candidate, int languageId)
        {
            if (candidateClockTime.CompanyId != null && candidate != null)
            {
              
                int companyId = Convert.ToInt32(candidateClockTime.CompanyId);
                int placedStatus=Convert.ToInt32(CandidateJobOrderStatusEnum.Placed);
                DateTime previousDate = candidateClockTime.ClockInOut.Date.AddMinutes(-1);               
                var allowedClockTimeDate=candidateClockTime.ClockInOut.AddMinutes(- _candidateWorkTimeSettings.EndScanWindowSpanInMinutes).Date;
                var cj = _candidateJobOrderService.GetCandidateJobOrderByCandidateIdAsQueryable(candidate.Id)
                                                                     .Where(c => c.CandidateJobOrderStatusId == placedStatus
                                                                                  && c.JobOrder.CompanyId == companyId
                                                                               && c.StartDate <= candidateClockTime.ClockInOut.Date
                                                                               && (!c.EndDate.HasValue || (c.JobOrder.StartTime > c.JobOrder.EndTime 
                                                                                    ? c.EndDate.Value >= previousDate.Date :
                                                                                     c.EndDate.Value >=allowedClockTimeDate)
                                                                     )).OrderByDescending(c => c.CreatedOnUtc).FirstOrDefault();



             
                bool sendEmail = false;
                if (cj == null)
                    sendEmail = true;
                else 
                {

                    // *** Scan clock time ***
                    DateTime startDate = candidateClockTime.ClockInOut.Date;
                    DateTime endDate = candidateClockTime.ClockInOut.Date;

                    DateTime startTime = cj.JobOrder.StartTime;
                    DateTime endTime = cj.JobOrder.EndTime;

                    // Check if it's night shift
                    // --------------------------
                    if (startTime.TimeOfDay >= endTime.TimeOfDay)
                    {
                        //if punch  is after start time then its the same day(its punch in)
                        if (candidateClockTime.ClockInOut.TimeOfDay > startTime.AddMinutes(-_candidateWorkTimeSettings.StartScanWindowSpanInMinutes).TimeOfDay)
                        {
                            endDate = endDate.AddDays(1);
                        }
                        else // else it is punch out in next day.
                        {
                            startDate = startDate.AddDays(-1);                            
                        }
                    }
                    // Define job start date time / end date time
                    DateTime jobStartDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hour, startTime.Minute, 0);
                    DateTime jobEndDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, endTime.Hour, endTime.Minute, 0);

                    // check if punch in date is working day in Job Order
                    if (jobStartDateTime.DayOfWeek == DayOfWeek.Sunday && !cj.JobOrder.SundaySwitch)
                        sendEmail = true;
                    else if (jobStartDateTime.DayOfWeek == DayOfWeek.Monday && !cj.JobOrder.MondaySwitch)
                        sendEmail = true;
                    else if (jobStartDateTime.DayOfWeek == DayOfWeek.Tuesday && !cj.JobOrder.TuesdaySwitch)
                        sendEmail = true;
                    else if (jobStartDateTime.DayOfWeek == DayOfWeek.Wednesday && !cj.JobOrder.WednesdaySwitch)
                        sendEmail = true;
                    else if (jobStartDateTime.DayOfWeek == DayOfWeek.Thursday && !cj.JobOrder.ThursdaySwitch)
                        sendEmail = true;
                    else if (jobStartDateTime.DayOfWeek == DayOfWeek.Friday && !cj.JobOrder.FridaySwitch)
                        sendEmail = true;
                    else if (jobStartDateTime.DayOfWeek == DayOfWeek.Saturday && !cj.JobOrder.SaturdaySwitch)
                        sendEmail = true;
                    if (!sendEmail)
                    {
                        DateTime scanStartDateTime = jobStartDateTime.AddMinutes(-_candidateWorkTimeSettings.StartScanWindowSpanInMinutes);
                        DateTime scanEndDateTime = jobEndDateTime.AddMinutes(_candidateWorkTimeSettings.EndScanWindowSpanInMinutes);

                        if (!(scanStartDateTime <= candidateClockTime.ClockInOut && scanEndDateTime >= candidateClockTime.ClockInOut))
                            sendEmail = true;
                    }
                    
                }
                if (sendEmail)
                {
                    languageId = EnsureLanguageIsActive(languageId);
                    var tokens = new List<Token>();
                    StringBuilder toEmails = new StringBuilder();
                    List<string> emails = _accountService.GetAllRecruitersByCompanyIdAndVendorId(Convert.ToInt32(candidateClockTime.CompanyId), candidate.FranchiseId, true)
                                                         .Select(x => x.Email).Distinct().ToList();                  
                    var messageTemplate = GetActiveMessageTemplate("CandidateClockTime.UnexpectedPunchIn", 0);
                    var allowedClientAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, Convert.ToInt32(candidateClockTime.CompanyId)).ToList();                    
                    if (allowedClientAccounts.Count>0)
                        emails.AddRange(allowedClientAccounts.Select(x => x.Email));                   
                    //email copy                   
                    string mspccAddresses = _accountService.GetAllMSPEmails();
                    if (emails.Count > 0)
                    {
                        toEmails.Append(String.Join(";", emails));
                        _messageTokenProvider.AddUnexpectedPunchTokens(tokens, candidateClockTime, candidate);
                        //email sender account
                        var emailfrom = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                        var toName = "";
                        var toAccountId = 0;
                        SendNotification(messageTemplate, emailfrom, languageId, tokens, toEmails.ToString(), toName, toAccountId, mspccAddresses);
                    }
                }
            }
        }


        public int SendPendingApprovalReminder(int languageId, IList<Account> accounts, DateTime todayNow, DateTime dueTime, DateTime startDate, DateTime endDate, 
                                               string attachmentFileName = null, byte[] attachmentFile = null)
        {
            if (attachmentFileName == null || attachmentFile == null)
                return 0;

            var messageTemplate = GetActiveMessageTemplate("TimeSheet.PendingApprovalReminder", 1);
            if (messageTemplate == null)
                return 0;

            languageId = EnsureLanguageIsActive(languageId);

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var from = emailAccount;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, GetFranchise(1));
            _messageTokenProvider.AddPendingApprovalTokens(tokens, todayNow, dueTime, startDate, endDate);
            _messageTokenProvider.AddHostTokens(tokens);

            // if allowed
            var account = accounts.FirstOrDefault();    // all accounts in list have to be the same role
            var allowedAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, account.CompanyId).ToList();
            if (accounts.Except(allowedAccounts).Any())
                return 0;

            // To
            var toEmailAddresses = accounts.Count == 1 ? account.Email : String.Join(";", accounts.Select(x => x.Email));
            var toName = accounts.Count == 1 ? account.FullName : String.Empty;
            var toAccountId = accounts.Count == 1 ? account.Id : 0;

            // CC
            var otherAccounts = new List<Account>();
            if (account.IsCompanyHrManager())                       // copy to company level
                otherAccounts = allowedAccounts.Where(x => x.IsCompanyAdministrator()).ToList();
            else if (account.IsCompanyDepartmentSupervisor())       // copy to non company level
            {
                // remove default CC
                // A new object (not entity object) needed here
                Mapper.CreateMap<MessageTemplate, MessageTemplate>();
                messageTemplate = Mapper.Map<MessageTemplate, MessageTemplate>(messageTemplate);
                messageTemplate.CCEmailAddresses = null;
                otherAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, account.CompanyId, account.CompanyLocationId, account.CompanyDepartmentId)
                                .Where(x => !x.IsCompanyDepartmentSupervisor() && !x.IsCompanyHrManager() && !x.IsCompanyAdministrator()).ToList();
            }
            var cc = otherAccounts.Any() ? String.Join(";", otherAccounts.Select(x => x.Email)) : null;
            if (account.IsCompanyHrManager() && !String.IsNullOrWhiteSpace(messageTemplate.CCEmailAddresses))       // copy to default CC
                cc = !String.IsNullOrWhiteSpace(cc) ? String.Concat(cc, ";", messageTemplate.CCEmailAddresses) : messageTemplate.CCEmailAddresses;

            SendNotification(messageTemplate, emailAccount, languageId, tokens,
                             from.Email, from.DisplayName, from.Id,
                             toEmailAddresses, toName, toAccountId, cc,
                             null, attachmentFileName, attachmentFile,
                             replyTo: messageTemplate.CCEmailAddresses);

            return 0;
        }


        public void SendWorkTimeAdjustmentRecruiterNotification(CandidateWorkTime candidateWorkTime, int languageId, Account sender)
        {

            languageId = EnsureLanguageIsActive(languageId);
            //tokens 
            var tokens = new List<Token>();

            //email to recruitor
            Account recruiter = _accountService.GetAccountById(candidateWorkTime.JobOrder.RecruiterId);
            if (recruiter == null)
            {// if no recruiter is assigned then select owner.
                recruiter = _accountService.GetAccountById(candidateWorkTime.JobOrder.OwnerId);
            }
            if (recruiter != null)
            {
                var toEmail = recruiter.Email;
                var toName = recruiter.FullName;
                var toAccountId = recruiter.Id;

                var messageTemplate = GetActiveMessageTemplate("CandidateWorkTime.AdjustWorkTimeNotification", 0);

                //email copy
                var ccAddresses = new List<string>();
                if (!String.IsNullOrEmpty(messageTemplate.CCEmailAddresses))
                    ccAddresses = messageTemplate.CCEmailAddresses.Split(';').ToList();
                if (!String.IsNullOrEmpty(_emailAccountSettings.DefaultCCEmailAddress))
                    ccAddresses = ccAddresses.Union(_emailAccountSettings.DefaultCCEmailAddress.Split(';')).ToList();

                //email sender account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var from = sender;
                _messageTokenProvider.AddWorktimeAdjustmentTokens(tokens, candidateWorkTime, from.FullName);

                string msp = _accountService.GetAllMSPEmails();
                if (msp.Length > 0)
                    ccAddresses = ccAddresses.Union(msp.Split(';')).ToList();

                SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                from.Email, from.FullName, from.Id,
                                toEmail, toName, toAccountId, String.Join(";", ccAddresses));
            }
        }


        public void SendMissingHourFollowUpAlert(Account account, string attachmentName, byte[] attachmentBytes)
        {
            var messageTemplate = GetActiveMessageTemplate("MissingHour.FollowUpAlert", 1);
            if (messageTemplate == null)
                return;

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, 1);
            var from = emailAccount;

            SendNotification(messageTemplate, emailAccount, 1, Enumerable.Empty<Token>(),
                             from.Email, from.DisplayName, from.Id,
                             account.Email, account.FullName, account.Id, null,
                             null, attachmentName, attachmentBytes);
        }


        public void SendTimeSheetImportPlacementValidationAlert(string companyName, IList<int> bannedIds, IList<string> duplicates, IList<int> receivers, string importedBy)
        {
            var messageTemplate = GetActiveMessageTemplate("TimeSheetImport.PlacementValidationAlert", 1);
            if (messageTemplate == null)
                return;

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, 1);
            var from = emailAccount;

            var recruiters = _accountService.GetAllAccountsAsQueryable().Where(x => x.IsActive && !x.IsDeleted).Where(x => receivers.Contains(x.Id));
            if (recruiters.Any())
            {
                var toEmail = String.Join(";", recruiters.Select(x => x.Email));
                var toName = recruiters.Count() == 1 ? recruiters.First().FullName : null;
                var toAccountId = recruiters.Count() == 1 ? recruiters.First().Id : 0;

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddTimeSheetImportPlacementValidationAlertTokens(tokens, companyName, bannedIds, duplicates);

                SendNotification(messageTemplate, emailAccount, 1, tokens,
                                 from.Email, from.DisplayName, from.Id,
                                 toEmail, toName, toAccountId, importedBy);
            }
        }

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="candidate">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="jobOrder">Product instance</param>
        /// <param name="accountEmail">Account's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendJobOrderEmailAFriendMessage(Candidate candidate, int languageId,
            JobOrder jobOrder, string accountEmail, string friendsEmail, string personalMessage)
        {
            //if (candidate == null)
            //    throw new ArgumentNullException("candidate");

            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Service.EmailAFriend", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder, languageId);
            tokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("EmailAFriend.Email", accountEmail));

            var toEmail = friendsEmail;
            var toName = "";
            var toAccountId = 0;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("NewsLetterSubscription.ActivationMessage", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);

            var toEmail = subscription.Email;
            var toName = "";
            var toAccountId = 0;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumTopicMessage(Account account,
            ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            //var franchise = _franchiseContext.CurrentFranchise;

            var messageTemplate = GetActiveMessageTemplate("Forums.NewForumTopic", 1);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            //_messageTokenProvider.AddFranchiseTokens(tokens, franchise, emailAccount);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);

            //event notification
            //_eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, toAccountId);
        }

        /// <summary>
        /// Sends a forum subscription message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumPostMessage(Account account,
            ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            //var franchise = _franchiseContext.CurrentFranchise;

            var messageTemplate = GetActiveMessageTemplate("Forums.NewForumPost", 1);
            if (messageTemplate == null)
            {
                return 0;
            }

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            //_messageTokenProvider.AddFranchiseTokens(tokens, franchise, emailAccount);
            _messageTokenProvider.AddForumPostTokens(tokens, forumPost);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumPost.ForumTopic,
                friendlyForumTopicPageIndex, forumPost.Id);
            _messageTokenProvider.AddForumTokens(tokens, forumPost.ForumTopic.Forum);

            //event notification
            //_eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, toAccountId);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Sends a blog comment notification message to a franchise owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            if (blogComment == null)
                throw new ArgumentNullException("blogComment");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Blog.BlogComment", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            var toAccountId = emailAccount.Id;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName, toAccountId);
        }

        #endregion

        #region Reject Placement

        public int SendNotificationToRecruiterForPlacmentRejection(CandidateJobOrder cjo, string reason, string comment, Account account, int languageId, DateTime? FromDate = null)
        {
            if (cjo == null)
                throw new ArgumentNullException("cjo");

            if (String.IsNullOrEmpty(reason))
                throw new ArgumentNullException("reason");

            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate = GetActiveMessageTemplate("PlacementRejection.RecruiterNotification", 0);
            if (messageTemplate == null)
                return 0;

            // email sender account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddPlacementRejectiontokens(tokens, cjo, reason, comment, account, FromDate);

            // email to recruitor
            Account recruiter = _accountService.GetAccountById(cjo.JobOrder.RecruiterId);
            if (recruiter == null)
                return 0;

            // copy email to owner
            Account owner = _accountService.GetAccountById(cjo.JobOrder.OwnerId);
            if (owner != null)
                messageTemplate.CCEmailAddresses = owner.Email;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                    account.Email, account.FullName, account.Id,
                                    recruiter.Email, recruiter.FullName, recruiter.Id);
        }

        #endregion

        #region Job Posting       

        public void SendJobPostingCreateEditNotification(JP_Core.JobPosting jobPosting, int languageId, bool isCreateNotification)
        {
            string templateName = "JobPosting.JobPostingUpdated";
            if (isCreateNotification)
                templateName = "JobPosting.NewJobPostingCreated";

            languageId = EnsureLanguageIsActive(languageId);
            //tokens  
            var tokens = new List<Token>();
            _messageTokenProvider.AddJobPostingTokens(tokens, jobPosting);
            var messageTemplate = GetActiveMessageTemplate(templateName, 0);

            ////email sender account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //MSP emails 
            string MSPEmails = _accountService.GetAllMSPEmails();
            var toName = "";
            var toAccountId = 0;
            var ccAddresses = new List<string>();

            if (MSPEmails.Length > 0)
            {
                string toemail;
                ccAddresses = MSPEmails.Split(';').ToList();
                toemail = ccAddresses.ElementAt(0);
                var from = _workContext.CurrentAccount;

                if (ccAddresses.Count == 1)
                {
                    SendNotification(messageTemplate, emailAccount, languageId, tokens,
                               from.Email, from.FullName, from.Id,
                               toemail, toName, toAccountId);
                }
                else if (ccAddresses.Count > 1)
                {
                    ccAddresses.RemoveAt(0);
                    SendNotification(messageTemplate, emailAccount, languageId, tokens,
                            from.Email, from.FullName, from.Id,
                            toemail, toName, toAccountId, String.Join(";", ccAddresses));
                }
            }
        }


        public void SendJobPostingPublishNotification(JP_Core.JobPosting jobPosting, List<string> jobOrderIds, int languageId)
        {

            if (jobPosting == null)
                throw new ArgumentNullException("jobPosting");

            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate = GetActiveMessageTemplate("JobPosting.PublishJobPosting", 0);
            if (messageTemplate == null)
                return;

            // email sender account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddJobPostingPublishTokens(tokens, jobOrderIds, jobPosting);

            var from = _workContext.CurrentAccount;

            // email to owner
            Account owner = _accountService.GetAccountById(jobPosting.EnteredBy);
            if (owner != null && !owner.Franchise.IsDefaultManagedServiceProvider)
            {
                // if allowed
                var allowedAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, jobPosting.CompanyId).ToList();
                if (!allowedAccounts.Contains(owner))
                    return;

                var toEmail = owner.Email;
                var toName = owner.FullName;
                var toAccountId = owner.Id;

                // copy to other allowed recipients
                var otherAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, jobPosting.CompanyId, jobPosting.CompanyLocationId, jobPosting.CompanyDepartmentId ?? 0)
                                    .Where(x => !x.IsCompanyDepartmentSupervisor()).ToList();
                var cc = otherAccounts.Count > 0 ? String.Join(";", otherAccounts.Select(x => x.Email)) : null;

                SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                 from.Email, from.FullName, from.Id,
                                 toEmail, toName, toAccountId, cc);
            }
        }


        public void SendJobPostingPublishNotificationToVendors(List<string> jobOrderIds, int languageId = 1)
        {
            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate = GetActiveMessageTemplate("JobPosting.PublishNotificationToVendor", 0);
            if (messageTemplate == null)
                return;

            // email sender account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            foreach (var id in jobOrderIds)
            {
                var jobOrder = _jobOrderService.GetJobOrderById(Convert.ToInt32(id));
                if (jobOrder != null)
                {
                    var to = _accountService.GetAccountById(jobOrder.OwnerId);

                    var tokens = new List<Token>();
                    _messageTokenProvider.AddTokensForJobOrderFromJobPosting(tokens, jobOrder);

                    var from = _workContext.CurrentAccount;
                    var recruiterEmails = _accountService.GetAllRecruitersByCompanyIdAndVendorId(jobOrder.CompanyId, jobOrder.FranchiseId, includeAdmin: true)
                                          .Select(x => x.Email).ToList();
                    recruiterEmails.Remove(to.Email);
                    var cc = String.Join(";", recruiterEmails);

                    SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                     from.Email, from.FullName, from.Id,
                                     to.Email, to.FullName, to.Id, cc);
                }
            }
        }


        public void SendJobPostingSubmissionReminder(JP_Core.JobPosting jobPosting, int languageId = 1)
        {
            if (jobPosting != null && jobPosting.EnteredBy > 0)
            {
                languageId = EnsureLanguageIsActive(languageId);
                var messageTemplate = GetActiveMessageTemplate("JobPosting.SubmissionReminder", 0);
                if (messageTemplate == null)
                    return;

                var subject = messageTemplate.Subject.Replace("%JobPosting.Id%", jobPosting.Id.ToString());
                if (!_messageHistoryService.MessageSentOrNot(subject))
                {
                    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                    var from = emailAccount;
                    var to = _accountService.GetAccountById(jobPosting.EnteredBy);
                    var contact = jobPosting.CompanyContactId > 0 ? _accountService.GetAccountById(jobPosting.CompanyContactId) : null;

                    // if allowed
                    var allowedAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, jobPosting.CompanyId).ToList();
                    if (!allowedAccounts.Contains(to))
                        return;
                    // copy to other allowed recipients
                    var otherAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, jobPosting.CompanyId, jobPosting.CompanyLocationId, jobPosting.CompanyDepartmentId ?? 0)
                                        .Where(x => !x.IsCompanyDepartmentSupervisor()).ToList();
                    if (contact != null) otherAccounts.Add(contact);
                    var cc = otherAccounts.Count > 0 ? String.Join(";", otherAccounts.Select(x => x.Email)) : null;

                    var tokens = new List<Token>();
                    _messageTokenProvider.AddJobPostingTokens(tokens, jobPosting, includeCompanyName: false);

                    SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                        from.Email, from.DisplayName, from.Id,
                                        to.Email, to.FullName, to.Id, cc);
                }
            }
        }

        #endregion

        #region Send JobOrder Placement Notification
        public QueuedEmail GetJobOrderPlacementEmail(JobOrder jo, DateTime refDate, Franchise franchise, int languageId, string fileName, string filePath, string note, out string error)
        {
            error = string.Empty;
            var HR = _accountService.GetClientCompanyHRAccount(jo.CompanyId);
            if (jo.CompanyContactId <= 0 && HR == null)
            {
                error = "The job order does not have a supervisor!";
                return null;
            }
            string templateName = "JobOrderPlacementNotification";
            languageId = EnsureLanguageIsActive(languageId);
            var tokens = new List<Token>();
            _messageTokenProvider.AddJobOrderTokens(tokens, jo, languageId);
            _messageTokenProvider.AddFranchiseTokens(tokens, franchise);
            tokens.Add(new Token("Datetime.RefDate", refDate.ToString("MM/dd/yyyy")));
            tokens.Add(new Token("Note", note));

            var messageTemplate = GetActiveMessageTemplate(templateName, franchise.Id);
            var emailSender = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            var to = jo.CompanyContactId > 0 ? jo.CompanyContact : HR;
            string toEmail = to.Email;
            string toName = to.FullName;
            int toAccountId = to.Id;
            tokens.Add(new Token("JobOrder.SupervisorName", toName));

            // if allowed
            var allowedAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, jo.CompanyId).ToList();
            if (!allowedAccounts.Contains(to))
                return null;
            // copy to other allowed recipients
            var otherAccounts = _clientNotificationService.GetClientRecipientsAllowed(messageTemplate.Id, jo.CompanyId, jo.CompanyLocationId, jo.CompanyDepartmentId)
                                .Where(x => !x.IsCompanyDepartmentSupervisor()).ToList();
            var cc = otherAccounts.Count > 0 ? String.Join(";", otherAccounts.Select(x => x.Email)) : null;

            string fromEmail = _workContext.CurrentAccount.Email;
            string fromName = _workContext.CurrentAccount.FullName;
            int fromAccountId = _workContext.CurrentAccount.Id;
            var email = GenerateQueuedEmail(messageTemplate, emailSender, languageId, tokens, fromEmail, fromName, fromAccountId, toEmail, toName, toAccountId, cc, filePath, fileName);
            return email;

        }
        #endregion


        #region Send Cancel JobPosting Email
        public void SendCancelJobPostingEmail(JP_Core.JobPosting jobPosting, List<JobOrder> jobOrders, Account clientAccount, int languageId, string reason, out StringBuilder error)
        {
            languageId = EnsureLanguageIsActive(languageId);
            error = new StringBuilder();
            string toEmail = string.Empty;
            int fromAccountId = clientAccount.Id;
            string fromEmail = clientAccount.Email;
            string fromName = clientAccount.FullName;
            EmailAccount emailAccount = new EmailAccount();
            MessageTemplate messageTemplate = new MessageTemplate();
            string templateName = "JobPosting.CancelNotification";
            if (jobOrders.Count <= 0)
            {
                templateName = "JobPosting.CancelNotification.OnlyMSP";
                toEmail = _accountService.GetAllMSPEmails();
                if (toEmail.Length > 0)
                {
                    var tokens = new List<Token>();
                    _messageTokenProvider.AddJobPostingTokens(tokens, jobPosting);
                    tokens.Add(new Token("Reason", reason));
                    messageTemplate = GetActiveMessageTemplate(templateName, 0);
                    emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                    SendNotification(messageTemplate, emailAccount, languageId, tokens, fromEmail, fromName, fromAccountId, toEmail, null, 0);
                }
                else
                {
                    error.AppendLine("There is no MSP account!");
                }
                return;
            }


            foreach (JobOrder jo in jobOrders)
            {
                int toAccountId = 0;
                string ccEmail = string.Empty;
                //tokens  
                var tokens = new List<Token>();
                _messageTokenProvider.AddJobPostingTokens(tokens, jobPosting);
                _messageTokenProvider.AddJobOrderTokens(tokens, jo, languageId);
                tokens.Add(new Token("Reason", reason));
                messageTemplate = GetActiveMessageTemplate(templateName, 0);

                ////email sender account
                emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //MSP emails 
                toEmail = GetContactEmailForJobOrder(jo, out toAccountId, out ccEmail);
                if (string.IsNullOrEmpty(toEmail))
                {
                    error.AppendLine(String.Format("The job order {0} has no recruiter or owner!", jo.Id));
                    continue;
                }
                string MSPEmails = _accountService.GetAllMSPEmails();
                var toName = _accountService.GetAccountById(toAccountId).FullName;
                var ccAddresses = new List<string>();

                if (MSPEmails.Length > 0)
                {
                    ccAddresses = MSPEmails.Split(';').ToList();
                    if (!string.IsNullOrEmpty(ccEmail))
                        ccAddresses.Add(ccEmail);
                }
                SendNotification(messageTemplate, emailAccount, languageId, tokens, fromEmail, fromName, fromAccountId, toEmail, toName, toAccountId, String.Join(";", ccAddresses));
            }
        }
        #endregion

        #region Get Contact Email for a job order
        public string GetContactEmailForJobOrder(JobOrder jo, out int toAccountId, out string ccEmail)
        {
            toAccountId = 0;
            ccEmail = string.Empty;
            if (jo.RecruiterId > 0)
            {
                toAccountId = jo.RecruiterId;
                if (jo.OwnerId > 0)
                    ccEmail = _accountService.GetAccountById(jo.OwnerId).Email;
                return _accountService.GetAccountById(jo.RecruiterId).Email;
            }
            else
            {
                if (jo.OwnerId > 0)
                {
                    toAccountId = jo.OwnerId;
                    return _accountService.GetAccountById(jo.OwnerId).Email;
                }
                else
                    return null;
            }
        }
        #endregion

        #region Vendor Certificate Expire Reminder
        public void SendVendorCertificateExpireReminder(string certificateName, int franchiseId, int languageId)
        {
            var franchise = _franchiseService.GetFranchiseById(franchiseId);
            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate = GetActiveMessageTemplate("VendorCertificate.ExpireNotification", 0);
            if (messageTemplate == null)
                return;
            // email sender account
            var from = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            //send email to Client HR only
            List<string> to = new List<string>();
            var companies = _companyVendorService.GetAllCompaniesByVendorId(franchiseId);
            foreach (var company in companies)
            {
                var HR = _accountService.GetClientCompanyHRAccount(company.CompanyId);
                if (HR != null)
                    to.Add(HR.Email);
            }
            string toEmails = String.Join(";", to);
            var cc = _accountService.GetAllMSPEmails();
            var tokens = new List<Token>();
            tokens.Add(new Token("Certificate.Name", certificateName));
            tokens.Add(new Token("Franchise.Name", franchise.FranchiseName));
            SendNotification(messageTemplate, from, languageId, tokens,
                    from.Email, from.DisplayName, from.Id,
                    toEmails, null, 0, cc);

        }
        #endregion

        #region Scheduling Notification
        public void SendRescheduleTimeoffBookingNotification(EmployeeSchedule schedule, EmployeeTimeoffBooking timeoff, int languageId = 1)
        {
            if (schedule != null && timeoff != null)
            {
                languageId = EnsureLanguageIsActive(languageId);
                var messageTemplate = GetActiveMessageTemplate("Schedule.Reschedule.TimeoffBooking", 0);
                if (messageTemplate == null)
                    return;

                var tokens = new List<Token>();
                _messageTokenProvider.AddScheduleTimeOffTokens(tokens, schedule, timeoff);
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                var from = emailAccount;
                var companyShiftJobRole = schedule.CompanyShift.CompanyShiftJobRoles.FirstOrDefault(x => x.SupervisorId.HasValue
                        && x.CompanyJobRoleId == schedule.JobRoleId) ?? schedule.CompanyShift.CompanyShiftJobRoles.FirstOrDefault(x => x.SupervisorId.HasValue);
                if (companyShiftJobRole != null)
                {
                    var to = _accountService.GetAccountById(companyShiftJobRole.SupervisorId.Value);
                    SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                        from.Email, from.DisplayName, from.Id,
                                        to.Email, to.FullName, to.Id);
                }
            }
        }
        #endregion


        #region Send confirmation Email to Employees

        public void SendConfirmationToEmployeeNotification(QueuedEmail email)
        {
            if (email != null)
            {
                if (email.AttachmentFile != null && email.AttachmentFile.Length <= 0)
                    email.AttachmentFile = null;
                if (email.AttachmentFile2 != null && email.AttachmentFile2.Length <= 0)
                    email.AttachmentFile2 = null;
                if (email.AttachmentFile3 != null && email.AttachmentFile3.Length <= 0)
                    email.AttachmentFile3 = null;

                email.ReplyTo = email.From;
                email.ReplyToName = email.FromName;

                _queuedEmailService.InsertQueuedEmail(email);
            }

        }


        public string SendConfirmationToEmployeeNotification(int jobOrderId, int candidateId, DateTime start, DateTime? end, int type, int languageId = 1)
        {
            string result = string.Empty;

            var email = GetCandidateConfirmationEmail(jobOrderId, candidateId, start, end, type, out result);
            if (String.IsNullOrEmpty(result))
               this.SendConfirmationToEmployeeNotification(email);

            return result;
        }


        public QueuedEmail GetCandidateConfirmationEmail(int jobOrderId, int candidateId, DateTime start, DateTime? end, 
            int type, out string result, int languageId = 1, bool forMassMessagePreview = false)
        {
            result = string.Empty;
            try
            {
                string attachmentFileName = string.Empty;
                string attachmentFileName2 = string.Empty;
                string attachmentFileName3 = string.Empty;
                byte[] file = null;
                byte[] file2 = null;
                byte[] file3 = null;
                
                var jo = _jobOrderService.GetJobOrderById(jobOrderId);
                if (jo == null)
                {
                    result = String.Format("Invalid job order id {0} ", jobOrderId);
                    _logger.Error(String.Concat("GetCandidateConfirmationEmail(): ", result));
                    return null;
                }

                if ((_workContext.CurrentAccount.Id != jo.RecruiterId && _workContext.CurrentAccount.Id != jo.OwnerId) &&
                    !_workContext.CurrentAccount.IsClientAccount)
                {
                    result =String.Concat("You cannot send this confirmation email to the candidate since you are not the recruiter or owner of job order ",jobOrderId,"!");
                    return null;
                }
                var ca = _candidateService.GetCandidateById(candidateId);
                languageId = EnsureLanguageIsActive(languageId);
                var emailTemplate = _companyEmailTemplateService.GetEmailTemplate(type, jo.CompanyId, jo.CompanyLocationId, jo.CompanyDepartmentId);
                MessageTemplate messageTmeplate = _messageTemplateService.GetMessageTemplateByName("Candidate.ConfirmationEmailDefaultMessage", 0);
                if (emailTemplate == null && messageTmeplate == null)
                {
                    result = "Failed to email this placement to the employee, since there is no template for confirmation email!";
                    return null;
                }
                if (emailTemplate != null)
                {
                    messageTmeplate.Body = emailTemplate.Body;
                    messageTmeplate.Subject = emailTemplate.Subject;
                    if (!String.IsNullOrEmpty(emailTemplate.AttachmentFileName))
                    {
                        attachmentFileName = emailTemplate.AttachmentFileName;
                        file = emailTemplate.AttachmentFile;
                    }
                    if (!String.IsNullOrEmpty(emailTemplate.AttachmentFileName2))
                    {
                        attachmentFileName2 = emailTemplate.AttachmentFileName2;
                        file2 = emailTemplate.AttachmentFile2;
                    }
                    if (!String.IsNullOrEmpty(emailTemplate.AttachmentFileName3))
                    {
                        attachmentFileName3 = emailTemplate.AttachmentFileName3;
                        file3 = emailTemplate.AttachmentFile3;
                    }
                }


                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                var recruiter = _accountService.GetAccountById(jo.RecruiterId);
                var from = _workContext.CurrentAccount.IsClientAccount ? recruiter : _workContext.CurrentAccount;
                var to = ca;
                
                ConfirmationEmailLink link = new ConfirmationEmailLink() { JobOrderId = jo.Id, CandidateId = forMassMessagePreview?0: ca.Id, StartDate = start, EndDate = end, EnteredBy = _workContext.CurrentAccount.Id };
                var tokens = new List<Token>();
                tokens.Add(new Token("ConfirmationEmail.StartDate", start.ToString("ddd MMMM dd yyyy")));
                if (end.HasValue)
                    tokens.Add(new Token("ConfirmationEmail.EndDate", string.Concat(" Ending time: ", end.Value.ToString("ddd MMMM dd yyyy"))));
                else
                    tokens.Add(new Token("ConfirmationEmail.EndDate", ""));

                _messageTokenProvider.AddCompanyTokens(tokens, jo.Company);
                if(!forMassMessagePreview)
                    _messageTokenProvider.AddCandidateTokens(tokens, ca);
                _messageTokenProvider.AddJobOrderTokens(tokens, jo);
                _messageTokenProvider.AddFranchiseTokens(tokens, _workContext.CurrentFranchise);
                //get next pay day
                var payroll_calendar = _calendarService.GetNextPayrollCalendar(DateTime.Today, _workContext.CurrentFranchise.Id);
                if (payroll_calendar == null)
                {
                    result = "Next pay day has not been set yet, please contact our payroll department!";
                    return null;
                }
                else
                    tokens.Add(new Token("PayrollCalendar.PayDay", payroll_calendar.PayPeriodPayDate.ToString("ddd MMMM dd yyyy")));
                //JobOrder.PayRate
                var billingRate = _companyBillingService.GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(jo.CompanyId, jo.CompanyLocationId, jo.BillingRateCode, start);
                if (billingRate != null)
                {
                    var regPayRate = billingRate.RegularPayRate;
                    tokens.Add(new Token("JobOrder.RegPayRate", regPayRate.ToString("C")));
                    //tokens.Add(new Token("JobOrder.VacationPayRate", (regPayRate * (decimal)0.04).ToString("C")));
                    tokens.Add(new Token("JobOrder.PayRate", (regPayRate * (decimal)1.04).ToString("C")));
                }
                else
                {
                    result = String.Format("The job order {0} has no billing rate!", jo.Id);
                    return null;
                }
                //recruiter token
                tokens.Add(new Token("JobOrder.RecruiterPhone", from.FormatPhoneNumber(recruiter.WorkPhone)));
                tokens.Add(new Token("JobOrder.RecruiterMobilePhone", from.FormatPhoneNumber(recruiter.MobilePhone)));
                tokens.Add(new Token("JobOrder.Recruiter", from.FullName));
                tokens.Add(new Token("JobOrder.RecruiterEmail", from.Email));
                tokens.Add(new Token("JobOrder.Recruiter.Title", from.AccountRoles.FirstOrDefault().AccountRoleName));

                //add logo 
               // tokens.Add(new Token("LogoPath", Path.Combine(_webHelper.MapPath("~/Content/Images/"), "logo.png"))); //Note: We leave the logo token as is and replace it when the email is actually being sent, so we can include the image file as an inline attachment
                if(!forMassMessagePreview)
                    _messageTokenProvider.AddConfirmationEmailLinkTokens(tokens, link);

                //SendNotification(messageTmeplate, emailAccount, languageId, tokens, from.Email, from.FullName, from.Id, to.Email, to.GetFullName(), to.Id);
                return GenerateQueuedEmail(messageTmeplate, emailAccount, languageId, tokens, from.Email, from.FullName,
                    from.Id,forMassMessagePreview?null:to.Email, forMassMessagePreview?null:to.GetFullName(), 
                    forMassMessagePreview?0:to.Id,null,null,attachmentFileName,file,
                    attachmentFileName2,file2,attachmentFileName3,file3, true,link.Id);
            }
            catch (Exception ex)
            {
                result = String.Concat("Failed to email this placement to the employee, please contact administrator!");
                _logger.Error("SendConfirmationToEmployeeNotification():", ex);
                return null;
            }
        }

        #endregion


        #region candidate confirm back message

        public virtual int SendConfirmationFeedBackMessage(int jobOrderId, int candidateId, bool accept, int receiverId,string note, int languageId=1)
        {
            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            var candidate = _candidateService.GetCandidateById(candidateId);
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            if (candidate == null)
                throw new ArgumentNullException("candidate");


            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Candidate.ConfirmBackMessage", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string from = emailAccount.Email;
            if (!String.IsNullOrWhiteSpace(candidate.Email))
                from = candidate.Email;

            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            tokens.Add(new Token("Candidate.Id", candidate.Id.ToString()));
            tokens.Add(new Token("Candidate.Email", candidate.Email));
            tokens.Add(new Token("Candidate.FullName", candidate.GetFullName()));
            tokens.Add(new Token("JobOrder.Id", jobOrder.Id.ToString()));
            tokens.Add(new Token("JobOrder.Title", jobOrder.JobTitle));
            tokens.Add(new Token("ConfirmInformation.AcceptOrDecline", accept ? "accepted" : "declined"));
            tokens.Add(new Token("Note", note));
            //reply to the one who sent the information
            Account account = _accountService.GetAccountById(receiverId);
            if (account == null)
                return 0;
            return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                   from, candidate.GetFullName(), candidate.Id,
                                   account.Email, account.FullName, account.Id);
        }

        #endregion


        #region Send daily confirmation to candidates

        public string SendDailyConfirmationToCandidate(JobOrder jobOrder, DateTime refDate, out int done,
            List<int> statusList = null, bool skipContacted = true, int languageId = 1)
        {
            done = 0;
            var errors = new StringBuilder();

            if ((_workContext.CurrentAccount.Id != jobOrder.RecruiterId && _workContext.CurrentAccount.Id != jobOrder.OwnerId) 
                && !_workContext.CurrentAccount.IsClientAccount)
                return string.Concat("You are not the recruiter or owner of job order ", jobOrder.Id, "!");

            languageId = EnsureLanguageIsActive(languageId);
            var messageTmeplate = _messageTemplateService.GetMessageTemplateByName("Candidate.DailyConfirmationMessage", 0);
            if (messageTmeplate == null)
                return "No template for daily confirmation email!";

            var tokens = new List<Token>();
            tokens.Add(new Token("ConfirmationEmail.RefDate", refDate.ToString("ddd MMMM dd yyyy")));

            _messageTokenProvider.AddCompanyTokens(tokens, jobOrder.Company);
            _messageTokenProvider.AddJobOrderTokens(tokens, jobOrder);
            _messageTokenProvider.AddFranchiseTokens(tokens, _workContext.CurrentFranchise);

            var recruiter = _accountService.GetAccountById(jobOrder.RecruiterId);
            var from = _workContext.CurrentAccount.IsClientAccount ? recruiter : _workContext.CurrentAccount;
            tokens.Add(new Token("JobOrder.RecruiterPhone", from.FormatPhoneNumber(recruiter.WorkPhone)));
            tokens.Add(new Token("JobOrder.RecruiterMobilePhone", from.FormatPhoneNumber(recruiter.MobilePhone)));
            tokens.Add(new Token("JobOrder.Recruiter", from.FullName));
            tokens.Add(new Token("JobOrder.RecruiterEmail", from.Email));
            tokens.Add(new Token("JobOrder.Recruiter.Title", from.AccountRoles.FirstOrDefault().AccountRoleName));

            var statusNotSpecified = statusList == null;
            var pipeline = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable(refDate)
                .Where(x => x.JobOrderId == jobOrder.Id);
            if (statusList != null && statusList.Any())
                pipeline = pipeline.Where(x => statusList.Contains(x.CandidateJobOrderStatusId));
            if (skipContacted)
                pipeline = pipeline.Where(x => x.CandidateJobOrderStatusId != (int)CandidateJobOrderStatusEnum.Contacted)
                    .Where(x => x.CandidateJobOrderStatusId != (int)CandidateJobOrderStatusEnum.Refused);

            foreach (var placement in pipeline.ToList())
            {
                if (placement.Candidate == null || placement.Candidate.IsActive)
                {
                    var email = GetDailyConfirmationEmail(messageTmeplate, from, jobOrder, tokens, 
                        placement.CandidateId, refDate, out string error, languageId);
                    if (email != null && string.IsNullOrEmpty(error))
                        SendConfirmationToEmployeeNotification(email);

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        errors.AppendLine(string.Concat("Failed to send daily confirmation to ", placement.CandidateId.ToString(),
                            ". Error: ", error));
                        continue;
                    }

                    error = _candidateJobOrderService.SetCandidateJobOrderToStandbyStatus(placement.CandidateId, jobOrder.Id,
                        placement.CandidateJobOrderStatusId, refDate, refDate, (int)CandidateJobOrderStatusEnum.Contacted);
                    if (!string.IsNullOrWhiteSpace(error))
                        errors.AppendLine(error);
                    else
                        done++;
                }
            }

            return errors.ToString();
        }


        private QueuedEmail GetDailyConfirmationEmail(MessageTemplate messageTmeplate, Account from, 
            JobOrder jo, List<Token> commonTokens, int candidateId, DateTime refDate, out string result, int languageId = 1)
        {
            result = string.Empty;

            try
            {
                var tokens = new List<Token>();

                var candidate = _candidateService.GetCandidateById(candidateId);
                if (candidate == null)
                {
                    result = string.Concat("Cannot find candidate: ", candidateId);
                    return null;
                }
                tokens.Add(new Token("Candidate.FullName", candidate.GetFullName()));

                var link = new ConfirmationEmailLink()
                {
                    JobOrderId = jo.Id,
                    CandidateId = candidateId,
                    StartDate = refDate,
                    EndDate = refDate,
                    EnteredBy = _workContext.CurrentAccount.Id
                };
                _messageTokenProvider.AddConfirmationEmailLinkTokens(tokens, link);

                tokens.AddRange(commonTokens);

                var emailAccount = _emailAccountService.GetEmailAccountById(messageTmeplate.EmailAccountId);

                return GenerateQueuedEmail(messageTmeplate, emailAccount, languageId, tokens, 
                    from.Email, from.FullName, from.Id, 
                    candidate.Email, candidate.GetFullName(), candidate.Id);
            }
            catch (Exception ex)
            {
                result = string.Concat("Failed to send daily confirmation email to candidate: ", candidateId);
                return null;
            }
        }

        #endregion


        #region SendCandidateOutOfJobOrderMessage
        public int SendCandidateOutOfJobOrderMessage(int jobOrderId, int candidateId, DateTime moveOutDate, int languageId=1)
        {
            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            var candidate = _candidateService.GetCandidateById(candidateId);
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            if (candidate == null)
                throw new ArgumentNullException("candidate");


            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetActiveMessageTemplate("Candidate.MoveOutOfJobOrderMessage", 0);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string from = emailAccount.Email;
            string fromName = emailAccount.FriendlyName;
            int fromAccountId = emailAccount.Id;



            //franchise
            var franchise = GetFranchise(jobOrder.FranchiseId);

            //tokens
            var tokens = new List<Token>();
            tokens.Add(new Token("Candidate.Id", candidate.Id.ToString()));
            tokens.Add(new Token("Candidate.FullName", candidate.GetFullName()));
            tokens.Add(new Token("JobOrder.Id", jobOrder.Id.ToString()));
            tokens.Add(new Token("JobOrder.Title", jobOrder.JobTitle));
            tokens.Add(new Token("TwoDaysBeforeLastWorkingDate", moveOutDate.ToShortDateString()));

            //recruitor information
            var re_account = _accountService.GetAccountById(jobOrder.RecruiterId);
            var own_account = _accountService.GetAccountById(jobOrder.OwnerId);
            //var supervisor_account = _accountService.GetAccountById(jobOrder.CompanyContactId);
            List<string> receiver = new List<string>();
            if (re_account != null)
                receiver.Add(re_account.Email);
            if (own_account != null)
                receiver.Add(own_account.Email);
            //if (supervisor_account != null)
            //    receiver.Add(supervisor_account.Email);

            if (receiver.Distinct().Count()<=0)
                return 0;
            return SendNotification(messageTemplate, emailAccount, languageId, tokens,
                                   from, fromName, fromAccountId,
                                   String.Join(";",receiver.Distinct()), null, 0);


        }
        #endregion


        #region Rescheduling Alerts

        public int SendCandidateReschedulingMessage(int origId, int newId, int candidateId, DateTime startDate, DateTime endDate, int languageId = 1)
        {
            var origJobOrder = _jobOrderService.GetJobOrderById(origId);
            var newJobOrder = _jobOrderService.GetJobOrderById(newId);
            var candidate = _candidateService.GetCandidateById(candidateId);

            if (origJobOrder == null || newJobOrder == null)
                throw new ArgumentNullException("jobOrder");
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate1 = GetActiveMessageTemplate("Candidate.ReschedulingMessageToEmployee", 0);
            var messageTemplate2 = GetActiveMessageTemplate("Candidate.ReschedulingMessageToRecruiter", 0);
            if (messageTemplate1 == null || messageTemplate2 == null)
                return 0;

            //email account
            var emailAccount1 = GetEmailAccountOfMessageTemplate(messageTemplate1, languageId);
            var emailAccount2 = GetEmailAccountOfMessageTemplate(messageTemplate2, languageId);

            //tokens
            var tokens = new List<Token>();
            int result = 0;

            try
            {
                _messageTokenProvider.AddResschedulingTokens(tokens, candidate, startDate, endDate, origJobOrder, newJobOrder);

                #region To Employee

                result = SendNotification(messageTemplate1, emailAccount1, languageId, tokens,
                                       emailAccount1.Email, emailAccount1.FriendlyName, emailAccount1.Id,
                                       candidate.Email, candidate.GetFullName(), 0);

                #endregion

                #region To Recruiter

                // recruitor
                var re_account = _accountService.GetAccountById(newJobOrder.RecruiterId);
                var own_account = _accountService.GetAccountById(newJobOrder.OwnerId);
                var receiver = new List<string>();
                if (re_account != null)
                    receiver.Add(re_account.Email);
                if (own_account != null)
                    receiver.Add(own_account.Email);
                if (!receiver.Distinct().Any())
                    return 0;

                result = SendNotification(messageTemplate2, emailAccount2, languageId, tokens,
                                       emailAccount2.Email, emailAccount2.FriendlyName, emailAccount2.Id,
                                       String.Join(";", receiver.Distinct()), null, 0);

                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, _workContext.CurrentAccount);
                result = -1;
            }

            return result;
        }

        #endregion

        public int SendGeneralEmail(int templateId,int? jobOrderId,Candidate candidate,DateTime? start,DateTime? end,int languageId,out string result)
        {
            result = String.Empty;
            var message = _messageTemplateService.GetMessageTemplateById(templateId);
            switch (message.TagName)
            {
                case "Candidate.ConfirmationEmailDefaultMessage": 
                    {
                        if(!jobOrderId.HasValue)
                        {
                            result = "JobOrder Id is required!";
                            return 0;
                        }
                        if (candidate==null)
                        {
                            result = "Candidate is required!";
                            return 0;
                        }
                        if (!start.HasValue)
                        {
                            result = "Start date is required!";
                            return 0;
                        }
                        result = SendConfirmationToEmployeeNotification(jobOrderId.Value, candidate.Id, start.Value, end, (int)CompanyEmailTemplateType.Confirmation,languageId);
                        break;
                    }
                case "Candidate.TestsMessage":
                    {
                        if (candidate==null)
                        {
                            result = "Candidate is required!";
                            return 0;
                        }
                        return SendCandidateTestsMessage(candidate,languageId);
                    }
                default: result = "Current we dont support send this general email!"; break;

            }
            if (String.IsNullOrEmpty(result))
                return 0;
            else
                return 1;
        }


        #region Send One Week Follow Up report to recruiter
        public int SendOneWeekFollowUpReminderToRecruiter(Account account, string fileName, byte[] file,int languageId=1)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            //var franchise = _franchiseContext.CurrentFranchise;

            var messageTemplate = GetActiveMessageTemplate("Account.OneWeekFollowUpReminder", 1);
            if (messageTemplate == null)
                return 0;
            //tokens
            var tokens = new List<Token>();
            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            var toEmail = account.Email;
            var toName = account.FullName;
            var toAccountId = account.Id;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, emailAccount.Email,emailAccount.FriendlyName,emailAccount.Id
                            ,toEmail, toName, toAccountId,null,null,fileName,file);
        }
        #endregion


        #region Payroll

        public string SendPaystubToEmployee(Candidate candidate, byte[] paystub, DateTime payPeriodStart, DateTime payPeriodEnd, string fileName)
        {
            var from = _emailAccountService.GetAllEmailAccounts().Where(x => x.FranchiseId == candidate.FranchiseId && x.Code == "PayStub").FirstOrDefault();
            if (from == null || String.IsNullOrWhiteSpace(from.EmailSubject) || String.IsNullOrWhiteSpace(from.EmailBody))
                return "Email account for paystub is not setup correctly";

            var subject = from.EmailSubject.Replace("[Pay Period]",
                String.Format("{0} - {1}", payPeriodStart.ToString("MMM dd yyyy"), payPeriodEnd.ToString("MMM dd yyyy")));

            var email = new QueuedEmail()
            {
                EmailAccountId = from.Id,
                Priority = 5,
                From = from.Email,
                FromName = from.DisplayName,
                FromAccountId = from.Id,
                To = candidate.Email,
                ToName = candidate.GetFullName(),
                ToAccountId = candidate.Id,
                Subject = subject,
                Body = from.EmailBody,
                AttachmentFileName = fileName,
                AttachmentFile = paystub,
                AttachmentFile2 = null,
                AttachmentFile3 = null,
                MessageCategoryId = 0,
            };

            _queuedEmailService.InsertQueuedEmail(email);

            return string.Empty;
        }

        #endregion

        #region Send Time Off Request to Manager
        public int SendTimeOffRequestToManager(Account account, EmployeeTimeoffBooking timeOff, string action,int languageId = 1)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            var messageTemplate = GetActiveMessageTemplate("Account.RequestTimeOff", 1);
            if (messageTemplate == null)
            {
                _logger.Error(String.Concat("SendTimeOffRequestToManager():The message template with tag 'Account.RequestTimeOff' does not exist!"));
                return 0;
            }
        
            var manager = _accountService.GetAccountById(account.ManagerId);
            if (manager==null)
            {
                _logger.Error(String.Concat("SendTimeOffRequestToManager():The account ", account.Id, " does not have a manager!"));
                return 0;
            }
            //tokens
            var tokens = new List<Token>();
            tokens.Add(new Token("Account.Name", account.FullName));
            tokens.Add(new Token("Account.Manager.Name", manager.FullName));
            tokens.Add(new Token("TimeOff.Type", timeOff.EmployeeTimeoffType.Name));
            tokens.Add(new Token("StartDate", timeOff.TimeOffStartDateTime.ToShortDateString()));
            tokens.Add(new Token("EndDate", timeOff.TimeOffEndDateTime.ToShortDateString()));
            tokens.Add(new Token("TimeOff.Action", action));

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var from = account.Email;
            var fromName = account.FullName;
            var toEmail = manager.Email;
            var toName = manager.FullName;
            var toAccountId = manager.Id;
            return SendNotification(messageTemplate, emailAccount, languageId, tokens, from, fromName,account.Id
                , toEmail, toName, toAccountId);
        }

        public int SendTimeOffRequestFeedBackToAccount(Account account, EmployeeTimeoffBooking timeOff, int languageId = 1, string result = null)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            var messageTemplate = GetActiveMessageTemplate("Account.TimeOffRequestFeedBack", 1);
            if (messageTemplate == null)
            {
                _logger.Error(String.Concat("SendTimeOffRequestFeedBackToAccount():The message template with tag 'Account.TimeOffRequestFeedBack' does not exist!"));
                return 0;
            }
            string rejectMessage = string.Empty;
            string rejectReason = string.Empty;
            if (timeOff.IsRejected.HasValue && timeOff.IsRejected.Value)
            {
                rejectMessage = "rejected";
                rejectReason = String.Concat("Reason: ", timeOff.Note);
            }
            else
            {
                rejectMessage = result == null ? "approved" : result;
            }

            //tokens
            var tokens = new List<Token>();
            tokens.Add(new Token("Account.Name", account.FullName));
            tokens.Add(new Token("Account.EmployeeName", timeOff.BookedByAccount.FullName));
            tokens.Add(new Token("TimeOff.FeedBack", rejectMessage));
            tokens.Add(new Token("TimeOff.RejectReason", rejectReason));

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var from = account.Email;
            var fromName = account.FullName;
            var toEmail = timeOff.BookedByAccount.Email;
            var toName = timeOff.BookedByAccount.FullName;
            var toAccountId = timeOff.BookedByAccount.Id;
            return SendNotification(messageTemplate, emailAccount, languageId, tokens, from, fromName, account.Id
                , toEmail, toName, toAccountId);
        }
        #endregion


        #region Max Annual Hours Reached

        public void SendMaxAnnualHoursReachedAlert(IList<string> recipents, string attachmentName, byte[] attachmentBytes, 
                                                   string companyName, DateTime startDate, DateTime endDate, int maxHours)
        {
            var messageTemplate = GetActiveMessageTemplate("TimeSheet.MaxAnnualHoursReachedAlert", 1);
            if (messageTemplate == null)
                return;

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, 1);
            var from = emailAccount;

            var msp = _accountService.GetAllMSPEmails();
            var toEmail = String.Join(";", recipents);
            string cc = null;
            if (String.IsNullOrWhiteSpace(toEmail))
                toEmail = msp;
            else
                cc = msp;

            var tokens = new List<Token>();
            tokens.Add(new Token("CompanyName", companyName));
            tokens.Add(new Token("StartDate", startDate.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("EndDate", endDate.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("MaxHours", maxHours.ToString()));
            tokens.Add(new Token("NewStartDate", endDate.AddDays(1).ToString("yyyy-MM-dd")));

            SendNotification(messageTemplate, emailAccount, 1, tokens,
                             from.Email, from.DisplayName, from.Id,
                             toEmail, null, 0, cc,
                             null, attachmentName, attachmentBytes);
        }

        #endregion


        #region Employee Seniority

        public int SendEmployeeSeniorityAlert(DateTime refDate, int scope, decimal threshold, string recipient, string attachmentFileName = null, byte[] attachmentFile = null, int languageId = 1)
        {
            var templateName = !String.IsNullOrWhiteSpace(attachmentFileName) ? "Candidate.EmployeeSeniorityAlert" : "Candidate.EmployeeSeniorityAlert.None";
            var messageTemplate = GetActiveMessageTemplate(templateName, 1);
            if (messageTemplate == null)
                return 0;

            languageId = EnsureLanguageIsActive(languageId);
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var from = emailAccount;

            var tokens = new List<Token>()
            {
                new Token("threshold", threshold.ToString()),
                new Token("scope", scope.ToString()),
            };

            var toEmailAddresses = recipient;
            var cc = messageTemplate.CCEmailAddresses;
            var bcc = messageTemplate.BccEmailAddresses;

            SendNotification(messageTemplate, emailAccount, languageId, tokens,
                             from.Email, from.DisplayName, from.Id,
                             toEmailAddresses, null, 0, cc,
                             null, attachmentFileName, attachmentFile);

            return 1;
        }

        #endregion
    }
}

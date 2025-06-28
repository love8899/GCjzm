using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Wfm.Core;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Forums;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Messages;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Common;
using Wfm.Services.Forums;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Franchises;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Wfm.Services.Seo;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Incident;
using Wfm.Services.Companies;
using Wfm.Services.Incident;
using JP_Core = Wfm.Core.Domain.JobPosting;
using System.Text.RegularExpressions;
using Wfm.Services.JobOrders;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Scheduling;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Tests;
using Wfm.Services.Test;

namespace Wfm.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IFranchiseService _franchiseService;

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        private readonly CommonSettings _commonSettings;
        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly ICompanyService _companyService;
        private readonly IAccountService _accountService;
        private readonly ICandidateService _candidateService;
        private readonly ICompanyDivisionService _companyLocationService;
        private readonly IIncidentService _incidentService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IShiftService _shiftService;
        private readonly IConfirmationEmailLinkService _confirmationEmailService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICandidateTestLinkService _candidateTestLinkService;
        private readonly ITestService _testService;
        private readonly ICompanyContactService _companyContactService;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IFranchiseService franchiseService,
            IWorkContext workContext,
            CommonSettings commonSettings,
            ICompanyService companyService,
            IAccountService accountService,
            ICandidateService candidateService,
            ICompanyDivisionService companyLocationService,
            IIncidentService incidentService,
            MessageTemplatesSettings templatesSettings,
            IJobOrderService jobOrderService,
            IShiftService shiftService,
            IConfirmationEmailLinkService confirmationEmailService,
            ICompanyDepartmentService companyDepartmentService,
            ICandidateTestLinkService candidateTestLinkService,
            ITestService testService,
            ICompanyContactService companyContactService
            )
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._franchiseService = franchiseService;
            this._dateTimeHelper = dateTimeHelper;
            this._workContext = workContext;

            this._commonSettings = commonSettings;
            this._templatesSettings = templatesSettings;
            this._companyService = companyService;
            this._accountService = accountService;
            this._candidateService = candidateService;
            this._incidentService = incidentService;
            this._companyLocationService = companyLocationService;
            this._jobOrderService = jobOrderService;
            this._shiftService = shiftService;
            this._confirmationEmailService = confirmationEmailService;
            this._companyDepartmentService = companyDepartmentService;
            this._candidateTestLinkService = candidateTestLinkService;
            this._testService = testService;
            this._companyContactService = companyContactService;
        }

        #endregion

        #region Utilities

        protected virtual Franchise GetFranchise(int franchiseId = 0)
        {
            var franchise = _franchiseService.GetFranchiseById(franchiseId);

            if (franchise == null)
                throw new Exception("No franchise could be loaded");

            return franchise;
        }

        /// <summary>
        /// Get franchise URL
        /// </summary>
        /// <param name="franchiseId">Franchise identifier; Pass 0 to load URL of the current franchise</param>
        /// <param name="useSsl">Use SSL</param>
        /// <returns></returns>
        protected virtual string GetFranchiseUrl(int franchiseId = 0, bool useSsl = false)
        {
            //var franchise = _franchiseService.GetFranchiseById(franchiseId) ?? _franchiseContext.CurrentFranchise;

            //if (franchise == null)
            //    throw new Exception("No franchise could be loaded");

            //return useSsl ? franchise.SecureUrl : franchise.Url;

            // Temp solution
            return _commonSettings.HostUrl;
        }

        #endregion

        #region Methods

        public void AddHostTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("HostUrl", _commonSettings.HostUrl));
        }


        public virtual void AddFranchiseTokens(IList<Token> tokens, Franchise franchise)
        {
            var emailAccount = new EmailAccount { Email = "" };
            AddFranchiseTokens(tokens, franchise, emailAccount);
        }

        public virtual void AddFranchiseTokens(IList<Token> tokens, Franchise franchise, EmailAccount emailAccount)
        {
            if (franchise == null)
                throw new ArgumentNullException("franchise");

            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            tokens.Add(new Token("Franchise.Id", franchise.Id.ToString()));
            tokens.Add(new Token("Franchise.Name", franchise.FranchiseName));
            tokens.Add(new Token("Franchise.URL", franchise.WebSite, true));
            tokens.Add(new Token("Franchise.Email", emailAccount.Email));

        }

        public virtual void AddCompanyTokens(IList<Token> tokens, Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");

            tokens.Add(new Token("Company.Id", company.Id.ToString()));
            tokens.Add(new Token("Company.Name", company.CompanyName));
            tokens.Add(new Token("Company.URL", company.WebSite, true));
            tokens.Add(new Token("Company.Address", company.CompanyLocations.Where(x=>x.IsActive&&!x.IsDeleted).FirstOrDefault().LocationFullAddress, true));
            
        }

        public virtual void AddAccountTokens(IList<Token> tokens, Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            tokens.Add(new Token("Account.Email", account.Email));
            tokens.Add(new Token("Account.Username", account.Username));
            tokens.Add(new Token("Account.FullName", account.FullName));
            tokens.Add(new Token("Account.FirstName", account.FirstName));
            tokens.Add(new Token("Account.LastName", account.LastName));
            tokens.Add(new Token("Account.WorkPhone", account.WorkPhone));

            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string passwordRecoveryUrl = string.Format("{0}/user/passwordrecovery/confirm?token={1}&email={2}", GetFranchiseUrl(), account.GetAttribute<string>(SystemAccountAttributeNames.PasswordRecoveryToken), HttpUtility.UrlEncode(account.Email));
            string accountActivationUrl = string.Format("{0}/user/activation?token={1}&email={2}", GetFranchiseUrl(), account.GetAttribute<string>(SystemAccountAttributeNames.AccountActivationToken), HttpUtility.UrlEncode(account.Email));

            //var wishlistUrl = string.Format("{0}wishlist/{1}", GetFranchiseUrl(), account.AccountGuid);
            tokens.Add(new Token("Account.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Account.AccountActivationURL", accountActivationUrl, true));

        }


        public virtual void AddCompanyContactTokens(IList<Token> tokens, Account account)
        {
            //if (account == null)
            //    throw new ArgumentNullException("account");

            // Contact is optional in some cases like job order
            if (account != null) {
                tokens.Add(new Token("CompanyContact.FullName", account.FullName));
                tokens.Add(new Token("CompanyContact.WorkPhone", account.WorkPhone));
                tokens.Add(new Token("CompanyContact.Email", account.Email));
            }
            else
            {
                tokens.Add(new Token("CompanyContact.FullName", "N/A"));
                tokens.Add(new Token("CompanyContact.WorkPhone", "N/A"));
                tokens.Add(new Token("CompanyContact.Email", "N/A"));
            }

        }

        public virtual void AddCompanyBillingRateTokens(IList<Token> tokens, CompanyBillingRate rate)
        {
            if (rate == null)
                throw new ArgumentNullException("rate");

            tokens.Add(new Token("CompanyBillingRate.RegPayRate",rate.RegularPayRate.ToString() ));
            //tokens.Add(new Token("CompanyBillingRate.104RegPayRate", ((decimal)1.04*rate.RegularPayRate).ToString()));

            tokens.Add(new Token("CompanyBillingRate.OTPayRate", rate.OvertimePayRate.ToString()));
            tokens.Add(new Token("CompanyBillingRate.RegBillingRate", rate.RegularBillingRate.ToString()));
            tokens.Add(new Token("CompanyBillingRate.OTBillingRate", rate.OvertimeBillingRate.ToString()));

        }
        public virtual void AddJobOrderTokens(IList<Token> tokens, JobOrder jobOrder, int languageId=1)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            tokens.Add(new Token("JobOrder.Id", jobOrder.Id.ToString()));
            var companyName = jobOrder.Company != null ? jobOrder.Company.CompanyName : _companyService.GetCompanyByIdForScheduleTask(jobOrder.CompanyId).CompanyName;
            tokens.Add(new Token("JobOrder.CompanyName", companyName));
            var shiftName = jobOrder.Shift != null ? jobOrder.Shift.ShiftName : _shiftService.GetShiftById(jobOrder.ShiftId).ShiftName;
            tokens.Add(new Token("JobOrder.ShiftName", shiftName));
            tokens.Add(new Token("JobOrder.Title", jobOrder.JobTitle));
            tokens.Add(new Token("JobOrder.StartDate", jobOrder.StartDate.ToString("MMMM dd yyyy")));
            tokens.Add(new Token("JobOrder.EndDate", jobOrder.EndTime.ToString("MMMM dd yyyy")));
            tokens.Add(new Token("JobOrder.StartTime", jobOrder.StartTime.ToString("hh:mm tt")));
            tokens.Add(new Token("JobOrder.EndTime", jobOrder.EndTime.ToString("hh:mm tt")));

            string noHTML = Regex.Replace(jobOrder.JobDescription, @"<[^>]+>|&nbsp;", "").Trim();
            string description = Regex.Replace(noHTML, @"\s{2,}", " ");
            tokens.Add(new Token("JobOrder.Description", description));

            var jobOrderUrl = string.Format("{0}/jobs/{1}/{2}", GetFranchiseUrl(), jobOrder.Id, jobOrder.JobTitle.ToSeoFriendlyUrl());
            tokens.Add(new Token("JobOrder.JobOrderURLForCandidate", jobOrderUrl, true));
            
            tokens.Add(new Token("JobOrder.Supervisor", jobOrder.CompanyContact == null ? "" : jobOrder.CompanyContact.FullName));
            tokens.Add(new Token("JobOrder.SupervisorPhone", jobOrder.CompanyContact == null 
                ? "" : jobOrder.CompanyContact.FormatPhoneNumber(jobOrder.CompanyContact.WorkPhone)));

            var locationName = string.Empty;
            if (jobOrder.CompanyLocationId != 0)
            {
                var location = _companyLocationService.GetCompanyLocationById(jobOrder.CompanyLocationId);
                locationName = location.LocationName;
            }
            tokens.Add(new Token("JobOrder.Location", locationName));

            var departmentName = string.Empty;
            if (jobOrder.CompanyDepartmentId!=0)
            {
                var department = _companyDepartmentService.GetCompanyDepartmentById(jobOrder.CompanyDepartmentId);
                departmentName = department.DepartmentName;
            }
            tokens.Add(new Token("JobOrder.Department", departmentName));
            
            tokens.Add(new Token("JobOrder.SafetyShoes", jobOrder.RequireSafetyShoe?"Required":"No"));
            tokens.Add(new Token("JobOrder.SafetyEquipment", jobOrder.RequireSafeEquipment ? "Required" : "No"));
            
            var workingDay = new List<string>();
            if (jobOrder.MondaySwitch)
                workingDay.Add("Monday");
            if (jobOrder.TuesdaySwitch)
                workingDay.Add("Tuesday");
            if (jobOrder.WednesdaySwitch)
                workingDay.Add("Wednesday");
            if (jobOrder.ThursdaySwitch)
                workingDay.Add("Thursday");
            if (jobOrder.FridaySwitch)
                workingDay.Add("Friday");
            if (jobOrder.SaturdaySwitch)
                workingDay.Add("Saturday");
            if (jobOrder.SundaySwitch)
                workingDay.Add("Sunday");
            if (jobOrder.IncludeHolidays)
                workingDay.Add("(Including Holidays)");
            tokens.Add(new Token("JobOrder.WorkingDay", string.Join(",",workingDay)));
        }

        public virtual void AddCandidateTokens(IList<Token> tokens, Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            tokens.Add(new Token("Candidate.Id", candidate.Id.ToString()));
            tokens.Add(new Token("Candidate.Email", candidate.Email));
            tokens.Add(new Token("Candidate.Username", candidate.Username));
            tokens.Add(new Token("Candidate.FullName", candidate.GetFullName()));
            tokens.Add(new Token("Candidate.FirstName", candidate.FirstName));
            tokens.Add(new Token("Candidate.LastName", candidate.LastName));
            tokens.Add(new Token("Candidate.HomePhone", candidate.HomePhone));
            tokens.Add(new Token("Candidate.MobilePhone", candidate.MobilePhone));
            tokens.Add(new Token("Candidate.EmergencyPhone", candidate.EmergencyPhone));

            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string passwordRecoveryUrl = string.Format("{0}/passwordrecovery/confirm?token={1}&email={2}", GetFranchiseUrl(), candidate.GetAttribute<string>(SystemCandidateAttributeNames.PasswordRecoveryToken), HttpUtility.UrlEncode(candidate.Email));
            string candidateActivationUrl = string.Format("{0}/candidate/activation?token={1}&email={2}", GetFranchiseUrl(), candidate.GetAttribute<string>(SystemCandidateAttributeNames.CandidateActivationToken), HttpUtility.UrlEncode(candidate.Email));

            //var wishlistUrl = string.Format("{0}wishlist/{1}", GetFranchiseUrl(), candidate.CandidateGuid);
            tokens.Add(new Token("Candidate.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Candidate.CandidateActivationURL", candidateActivationUrl, true));

        }

        public virtual void AddCandidateListTokens(IList<Token> tokens, List<Candidate> candidateList)
        {
            if (candidateList == null)
                throw new ArgumentNullException("candidateList");

            StringBuilder sbCandidate = new StringBuilder();          
            sbCandidate.AppendLine("<table><tr><td>Emp #</td><td>Phone</td><td>Name</td></tr>");
            foreach (Candidate candidate in candidateList)
            {
                sbCandidate.Append("<tr>");
                sbCandidate.Append("<td>");
                sbCandidate.Append(candidate.Id.ToString("00000000"));
                sbCandidate.Append("</td>");              
                sbCandidate.Append("<td>");
                if (candidate.HomePhone != null && candidate.HomePhone.Trim().Length==10)
                    sbCandidate.Append(String.Format("{0:(###) ###-####}", double.Parse(candidate.HomePhone.Trim())));
                else
                sbCandidate.Append(candidate.HomePhone);
                sbCandidate.Append("</td>");
                sbCandidate.Append("<td>");
                sbCandidate.Append(candidate.GetFullName());
                sbCandidate.Append("</td>");
                sbCandidate.Append("</tr>");
            }
            sbCandidate.AppendLine("</table>");

            // Passing true to not encode html.
            tokens.Add(new Token("Candidate.List", sbCandidate.ToString(),true));
        }
        public virtual void AddCandidateListTokensForClient(IList<Token> tokens, List<Candidate> candidateList)
        {
            if (candidateList == null) 
                throw new ArgumentNullException("candidateList");

            StringBuilder sbCandidate = new StringBuilder();
            sbCandidate.AppendLine("<table><tr><td>Emp #</td><td>Name</td></tr>");
            foreach (Candidate candidate in candidateList)
            {
                sbCandidate.Append("<tr>");
                sbCandidate.Append("<td>");
                sbCandidate.Append(candidate.Id.ToString("00000000"));
                sbCandidate.Append("</td>");               
                sbCandidate.Append("<td>");
                sbCandidate.Append(candidate.GetFullName());
                sbCandidate.Append("</td>");
                sbCandidate.Append("</tr>");
            }
            sbCandidate.AppendLine("</table>");

            // Passing true to not encode html.
            tokens.Add(new Token("Candidate.List", sbCandidate.ToString(), true));
        }
        public virtual void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));


            //const string urlFormat = "{0}newsletter/subscriptionactivation/{1}/{2}";

            //var activationUrl = String.Format(urlFormat, GetStoreUrl(), subscription.NewsLetterSubscriptionGuid, "true");
            //tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            //var deActivationUrl = String.Format(urlFormat, GetStoreUrl(), subscription.NewsLetterSubscriptionGuid, "false");
            //tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deActivationUrl, true));

        }


        public virtual void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment)
        {
            tokens.Add(new Token("BlogComment.BlogPostTitle", blogComment.BlogPost.Title));

        }


        public virtual void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic, 
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null)
        {
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string topicUrl = null;
            //if (friendlyForumTopicPageIndex.HasValue && friendlyForumTopicPageIndex.Value > 1)
            //    topicUrl = string.Format("{0}boards/topic/{1}/{2}/page/{3}", GetStoreUrl(), forumTopic.Id, forumTopic.GetSeName(), friendlyForumTopicPageIndex.Value);
            //else
            //    topicUrl = string.Format("{0}boards/topic/{1}/{2}", GetStoreUrl(), forumTopic.Id, forumTopic.GetSeName());
            if (appendedPostIdentifierAnchor.HasValue && appendedPostIdentifierAnchor.Value > 0)
                topicUrl = string.Format("{0}#{1}", topicUrl, appendedPostIdentifierAnchor.Value);
            tokens.Add(new Token("Forums.TopicURL", topicUrl, true));
            tokens.Add(new Token("Forums.TopicName", forumTopic.Subject));

            //event notification
            //_eventPublisher.EntityTokensAdded(forumTopic, tokens);
        }

        public virtual void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost)
        {
            tokens.Add(new Token("Forums.PostAuthor", forumPost.Account.FormatUserName()));
            tokens.Add(new Token("Forums.PostBody", forumPost.FormatPostText(), true));

            //event notification
            //_eventPublisher.EntityTokensAdded(forumPost, tokens);
        }

        public virtual void AddForumTokens(IList<Token> tokens, Forum forum)
        {
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            //var forumUrl = string.Format("{0}boards/forum/{1}/{2}", GetStoreUrl(), forum.Id, forum.GetSeName());
            //tokens.Add(new Token("Forums.ForumURL", forumUrl, true));
            tokens.Add(new Token("Forums.ForumName", forum.Name));

            //event notification
            //_eventPublisher.EntityTokensAdded(forum, tokens);
        }

        public virtual void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage)
        {
            tokens.Add(new Token("PrivateMessage.Subject", privateMessage.Subject));
            tokens.Add(new Token("PrivateMessage.Text",  privateMessage.FormatPrivateMessageText(), true));

            //event notification
            //_eventPublisher.EntityTokensAdded(privateMessage, tokens);
        }

        /// <summary>
        /// Gets list of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>List of allowed (supported) message tokens for campaigns</returns>
        public virtual string[] GetListOfCampaignAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Franchise.Name%",
                "%Franchise.URL%",
                "%Franchise.Email%",
                "%NewsLetterSubscription.Email%",
                "%NewsLetterSubscription.ActivationUrl%",
                "%NewsLetterSubscription.DeactivationUrl%"
            };
            return allowedTokens.ToArray();
        }

        public virtual string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Franchise.Name%",
                "%Franchise.URL%",
                "%Franchise.Email%",
                "%Account.Email%", 
                "%Account.Username%",
                "%Account.FullName%",
                "%Account.FirstName%",
                "%Account.LastName%",
                "%Account.VatNumber%",
                "%Account.VatNumberStatus%", 
                "%Account.PasswordRecoveryURL%", 
                "%Account.AccountActivationURL%",
                "%Candidate.Email%", 
                "%Candidate.Username%",
                "%Candidate.FullName%",
                "%Candidate.FirstName%",
                "%Candidate.LastName%",
                "%Candidate.PasswordRecoveryURL%", 
                "%Candidate.AccountActivationURL%",

                "%Wishlist.URLForAccount%", 
                "%NewsLetterSubscription.Email%", 
                "%NewsLetterSubscription.ActivationUrl%",
                "%NewsLetterSubscription.DeactivationUrl%", 
                "%BlogComment.BlogPostTitle%", 
                "%NewsComment.NewsTitle%",
                "%Forums.TopicURL%",
                "%Forums.TopicName%", 
                "%Forums.PostAuthor%",
                "%Forums.PostBody%",
                "%Forums.ForumURL%", 
                "%Forums.ForumName%", 
                "%PrivateMessage.Subject%", 
                "%PrivateMessage.Text%",
            };
            return allowedTokens.ToArray();
        }

        public virtual void AddPendingApprovalTokens(IList<Token> tokens, DateTime todayNow, DateTime dueTime, DateTime startDate, DateTime endDate)
        {
            var lastSunday = todayNow.AddDays(DayOfWeek.Sunday - todayNow.DayOfWeek - 7);
            tokens.Add(new Token("TodayNow", todayNow.ToString("yyyy-MM-dd HH:mm")));
            tokens.Add(new Token("DueTime", dueTime.ToString("yyyy-MM-dd HH:mm")));
            tokens.Add(new Token("StartDate", startDate.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("EndDate", endDate.ToString("yyyy-MM-dd")));
        }

        public virtual void AddWorktimeRejectionTokens(IList<Token> tokens, CandidateWorkTime candidateWorktime, string supervisorName)
        {
            tokens.Add(new Token("WorkTimeDate", candidateWorktime.JobStartDateTime.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("CandidateId", candidateWorktime.CandidateId.ToString()));
            tokens.Add(new Token("JobOrderId", candidateWorktime.JobOrderId.ToString()));
            tokens.Add(new Token("NetWorkTimeInHours", candidateWorktime.NetWorkTimeInHours.ToString()));
            tokens.Add(new Token("Reason", candidateWorktime.Note));
            tokens.Add(new Token("Supervisor", supervisorName));
        }
        public virtual void AddCandidateNotOnBoardedPunchTokens(IList<Token> tokens, CandidateClockTime candidateClockTime)
        {
            tokens.Add(new Token("PunchDateTime", candidateClockTime.ClockInOut.ToString("yyyy-MM-dd HH:mm")));
            tokens.Add(new Token("CandidateId", candidateClockTime.CandidateId.ToString()));
            tokens.Add(new Token("Company.Id", candidateClockTime.CompanyId.ToString()));         
            tokens.Add(new Token("Company.Name", candidateClockTime.CompanyName));
         
        }
        public virtual void AddUnexpectedPunchTokens(IList<Token> tokens, CandidateClockTime candidateClockTime, Candidate candidate)
        {
                 
            tokens.Add(new Token("PunchDateTime", candidateClockTime.ClockInOut.ToString("yyyy-MM-dd HH:mm")));
            tokens.Add(new Token("CandidateId", candidateClockTime.CandidateId.ToString()));           
            tokens.Add(new Token("CandidateName", candidate.FirstName + " " + candidate.LastName));
            tokens.Add(new Token("Company.Name", candidateClockTime.CompanyName)); 
        } 

        public virtual void AddWorktimeAdjustmentTokens(IList<Token> tokens, CandidateWorkTime candidateWorktime,string supervisorName)
        {
            tokens.Add(new Token("WorkTimeDate", candidateWorktime.JobStartDateTime.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("CandidateId", candidateWorktime.CandidateId.ToString()));
            tokens.Add(new Token("JobOrderId", candidateWorktime.JobOrderId.ToString()));
            tokens.Add(new Token("NetWorkTimeInHours", candidateWorktime.NetWorkTimeInHours.ToString()));
            tokens.Add(new Token("AdjustmentInMinutes", candidateWorktime.AdjustmentInMinutes.ToString()));
            tokens.Add(new Token("Note", candidateWorktime.Note));
            tokens.Add(new Token("OriginalHours", candidateWorktime.GrossWorkTimeInHours.ToString()));
            tokens.Add(new Token("Supervisor", supervisorName));
        }

        public void AddIncidentReportTokens(IList<Token> tokens, IncidentReport incidentReport)
        {
            string companyName = _companyService.GetCompanyById(incidentReport.CompanyId).CompanyName;
            string reportName = _accountService.GetAccountById(incidentReport.ReportedByAccountId).FullName;
            string createdDate = incidentReport.CreatedOnUtc.Value.ToLocalTime().ToShortDateString();
            string candidateName = _candidateService.GetCandidateById(incidentReport.CandidateId).GetFullName();
            string location= String.Empty;
            if(incidentReport.LocationId.HasValue)
                location = _companyLocationService.GetCompanyLocationById(incidentReport.LocationId.Value).LocationName;
            string categoryName = _incidentService.GetIncidentCategoryByCategoryId(incidentReport.IncidentCategoryId).Description;
            string details = incidentReport.Note;
            string incidentDatetime = incidentReport.IncidentDateTimeUtc.ToString("MM/dd/yyyy hh:mm tt");
            tokens.Add(new Token("Incident.CompanyName", companyName));
            tokens.Add(new Token("Incident.ReportedBy", reportName));
            tokens.Add(new Token("Incident.CreatedOn", createdDate));
            tokens.Add(new Token("Incident.CandidateName", candidateName));
            tokens.Add(new Token("Incident.Location", location));
            tokens.Add(new Token("Incident.Category", categoryName));
            tokens.Add(new Token("Incident.Note", details));
            tokens.Add(new Token("Incident.IncidentDateTime", incidentDatetime));
            tokens.Add(new Token("Incident.CandidateId", Convert.ToString(incidentReport.CandidateId)));
        }


        public void AddPlacementRejectiontokens(IList<Token> tokens, CandidateJobOrder cjo, string reason, string comment, Account account,DateTime? FromDate=null)
        {
            var candidate = cjo.Candidate;
            var jobOrder = cjo.JobOrder;

            tokens.Add(new Token("RejectOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
            tokens.Add(new Token("AccountName", account.FirstName + " " + account.LastName));
            tokens.Add(new Token("CompanyName", account.IsClientAccount ? jobOrder.Company.CompanyName : account.Franchise.FranchiseName));
            tokens.Add(new Token("CandidateId", candidate.Id.ToString()));
            tokens.Add(new Token("CandidateName", candidate.FirstName + " " + candidate.LastName));
            tokens.Add(new Token("JobOrderId", jobOrder.Id.ToString()));
            tokens.Add(new Token("JobTitle", jobOrder.JobTitle));
            if (FromDate!=null)
                tokens.Add(new Token("StartDate", Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd")));
            else
            tokens.Add(new Token("StartDate", cjo.StartDate.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("Reason", reason));
            tokens.Add(new Token("Comment", comment));
        }


        public void AddJobPostingTokens(IList<Token> tokens, JP_Core.JobPosting jobPosting, bool includeCompanyName = true) 
        {
            if (includeCompanyName)
            {
                string companyName = _companyService.GetCompanyByIdForScheduleTask(jobPosting.CompanyId).CompanyName;
                tokens.Add(new Token("JobPosting.CompanyName", companyName));
            }
            tokens.Add(new Token("JobPosting.Id", jobPosting.Id.ToString()));
            tokens.Add(new Token("JobPosting.StartDate", jobPosting.StartDate.ToString("yyyy-MM-dd")));
            tokens.Add(new Token("JobPosting.JobTitle", jobPosting.JobTitle));
            tokens.Add(new Token("JobPosting.NumberOfOpenings", jobPosting.NumberOfOpenings.ToString()));
            tokens.Add(new Token("JobPosting.JobDescription", jobPosting.JobDescription));
            tokens.Add(new Token("JobPosting.Shift", jobPosting.Shift.ShiftName));
        }


        public virtual void AddJobPostingPublishTokens(IList<Token> tokens, IList<string> jobOrderIds, JP_Core.JobPosting jobPosting) 
        {
            StringBuilder JoborderIdsText = new StringBuilder();
            foreach (var id in jobOrderIds)
            {
              int franchiseId=  _jobOrderService.GetJobOrderById(Convert.ToInt32(id)).FranchiseId;
              string vendorName= _franchiseService.GetFranchiseById(franchiseId).FranchiseName;
                JoborderIdsText.Append( id + " for "+ vendorName +", ");
            }
            tokens.Add(new Token("JobPosting.JobOrderIds", JoborderIdsText.ToString()));
            tokens.Add(new Token("JobPosting.Id", jobPosting.Id.ToString()));  
        }


        public virtual void AddTokensForJobOrderFromJobPosting(IList<Token> tokens, JobOrder jobOrder)
        {
            tokens.Add(new Token("JobOrder.Id", jobOrder.Id.ToString()));
            tokens.Add(new Token("Client.Name", jobOrder.Company.CompanyName));
            tokens.Add(new Token("JobOrder.Title", jobOrder.JobTitle));
            tokens.Add(new Token("JobOrder.Shift", jobOrder.Shift.ShiftName));
            tokens.Add(new Token("JobOrder.StartDate", jobOrder.StartDate.ToString("yyyy-MM-dd")));

            var opening = jobOrder.JobOrderOpenings.FirstOrDefault().OpeningNumber;
            tokens.Add(new Token("JobOrder.Opening", opening.ToString()));
        }

        #endregion

        #region Schedule related messages
        public virtual void AddScheduleTimeOffTokens(IList<Token> tokens, EmployeeSchedule schedule, EmployeeTimeoffBooking timeoff)
        {
            tokens.Add(new Token("Schedule.JobRoleName", schedule.JobRole.Name));
            tokens.Add(new Token("Schedule.ShiftName", schedule.CompanyShift.Shift.ShiftName));
            tokens.Add(new Token("Schedule.EmployeeName", schedule.Employee.FirstName + " " + schedule.Employee.LastName));
            tokens.Add(new Token("Timeoff.TimeoffType", timeoff.EmployeeTimeoffType.Name));
            tokens.Add(new Token("Timeoff.StartDate", timeoff.TimeOffStartDateTime.ToString("yyyy-MM-dd HH:mm")));
            tokens.Add(new Token("Timeoff.EndDate", timeoff.TimeOffEndDateTime.ToString("yyyy-MM-dd HH:mm")));
        }
        #endregion

        #region Add ConfirmationEmailLink Tokens
        public void AddConfirmationEmailLinkTokens(IList<Token> tokens, ConfirmationEmailLink link)
        {
            //save the link into table
            link.ValidBefore = DateTime.Now.AddDays(1);
            _confirmationEmailService.Create(link);
            //use Guid as link parameter
            string accpetUrl = string.Format("{0}/candidate/confirm?guid={1}&accept={2}", GetFranchiseUrl(),link.ConfirmationEmailLinkGuid,"true" );
            string declineUrl = string.Format("{0}/candidate/confirm?guid={1}&accept={2}", GetFranchiseUrl(), link.ConfirmationEmailLinkGuid,"false");
            tokens.Add(new Token("ConfirmationEmail.Accept.Url", accpetUrl));
            tokens.Add(new Token("ConfirmationEmail.Decline.Url", declineUrl));
        }
        #endregion

        #region Add Candidate Test LInk
        public void AddCandidateTestsLinkTokens(IList<Token> tokens, CandidateTestLink link,IList<TestCategory> categories)
        {
            StringBuilder testsLink = new StringBuilder();
            //save the link into table
            //var categories = _testService.GetAllRequiredTestCategories();
            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    link.TestCategoryId = category.Id;
                    _candidateTestLinkService.Create(link);
                    testsLink.AppendLine(String.Format(@"{0}: <a href='{1}/candidate/test?guid={2}'>click here</a></p><p>", category.TestCategoryName, GetFranchiseUrl(), link.CandidateTestLinkGuid));
                }
            }
            tokens.Add(new Token("Test.Link", testsLink.ToString(),true));

        }
        #endregion


        #region Tiemsheet Import Placement Validation Alert

        public virtual void AddTimeSheetImportPlacementValidationAlertTokens(IList<Token> tokens, string companyName, IList<int> bannedIds, IList<string> duplicates)
        {
            tokens.Add(new Token("CompanyName", companyName, true));

            var candidates = bannedIds.Any() ? String.Join(",", bannedIds) : "None";
            tokens.Add(new Token("Candidate.List", candidates, true));

            var conflicts = duplicates.Any() ? String.Join("<br />", duplicates) : "None";
            tokens.Add(new Token("Conflict.List", conflicts, true));
        }

        #endregion


        #region Rescheduling

        public void AddResschedulingTokens(IList<Token> tokens, Candidate candidate, DateTime startDate, DateTime endDate, JobOrder origJobOrder, JobOrder newJobOrder)
        {
            tokens.Add(new Token("Candidate.Id", candidate.Id.ToString()));
            tokens.Add(new Token("Candidate.FullName", candidate.GetFullName()));
            tokens.Add(new Token("Candidate.StartDate", startDate.ToShortDateString()));
            tokens.Add(new Token("Candidate.EndDate", endDate.ToShortDateString()));
            tokens.Add(new Token("JobOrder.Supervisor", _workContext.CurrentAccount.FullName));

            tokens.Add(new Token("JobOrder.OrigId", origJobOrder.Id.ToString()));
            tokens.Add(new Token("JobOrder.OrigJobTitle", origJobOrder.JobTitle));

            var location = _companyLocationService.GetCompanyLocationById(origJobOrder.CompanyLocationId);
            var department = _companyDepartmentService.GetCompanyDepartmentById(origJobOrder.CompanyDepartmentId);

            tokens.Add(new Token("JobOrder.OrigLocation", location != null ? location.LocationName : string.Empty));
            tokens.Add(new Token("JobOrder.OrigDepartment", department != null ? department.DepartmentName : string.Empty));
            tokens.Add(new Token("JobOrder.OrigSupervisor", origJobOrder.CompanyContact.FullName));
            tokens.Add(new Token("JobOrder.OrigStartTime", origJobOrder.StartTime.ToShortTimeString()));
            tokens.Add(new Token("JobOrder.OrigEndTime", origJobOrder.EndTime.ToShortTimeString()));

            tokens.Add(new Token("JobOrder.NewId", newJobOrder.Id.ToString()));
            tokens.Add(new Token("JobOrder.NewJobTitle", newJobOrder.JobTitle));

            location = _companyLocationService.GetCompanyLocationById(newJobOrder.CompanyLocationId);
            department = _companyDepartmentService.GetCompanyDepartmentById(newJobOrder.CompanyDepartmentId);
            if (newJobOrder.CompanyContact == null && newJobOrder.CompanyContactId > 0)
                newJobOrder.CompanyContact = _companyContactService.GetCompanyContactById(newJobOrder.CompanyContactId);

            tokens.Add(new Token("JobOrder.NewLocation", location != null ? location.LocationName : string.Empty));
            tokens.Add(new Token("JobOrder.NewDepartment", department != null ? department.DepartmentName : string.Empty));
            tokens.Add(new Token("JobOrder.NewSupervisor", newJobOrder.CompanyContact != null? newJobOrder.CompanyContact.FullName : string.Empty));
            tokens.Add(new Token("JobOrder.NewStartTime", newJobOrder.StartTime.ToShortTimeString()));
            tokens.Add(new Token("JobOrder.NewEndTime", newJobOrder.EndTime.ToShortTimeString()));
        }

        #endregion
    }
}

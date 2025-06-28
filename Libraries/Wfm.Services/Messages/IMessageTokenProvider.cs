using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Forums;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Messages;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Incident;
using JP_Core = Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Scheduling;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Tests;

namespace Wfm.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddHostTokens(IList<Token> tokens);

        void AddFranchiseTokens(IList<Token> tokens, Franchise franchise);

        void AddFranchiseTokens(IList<Token> tokens, Franchise franchise, EmailAccount emailAccount);

        void AddCompanyTokens(IList<Token> tokens, Company company);

        void AddAccountTokens(IList<Token> tokens, Account account);

        void AddCompanyContactTokens(IList<Token> tokens, Account account);

        void AddJobOrderTokens(IList<Token> tokens, JobOrder jobOrder, int languageId=1);

        void AddCandidateTokens(IList<Token> tokens, Candidate candidate);
        void AddCandidateListTokens(IList<Token> tokens, List<Candidate> candidateList);
        void AddCandidateListTokensForClient(IList<Token> tokens, List<Candidate> candidateList); 

        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);

        void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment);


        void AddForumTokens(IList<Token> tokens, Forum forum);

        void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic, int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost);

        void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage);

        //string[] GetListOfCampaignAllowedTokens();

        //string[] GetListOfAllowedTokens();

        void AddPendingApprovalTokens(IList<Token> tokens, DateTime todayNow, DateTime dueTime, DateTime startDate, DateTime endDate);

        void AddWorktimeRejectionTokens(IList<Token> tokens, CandidateWorkTime candidateWorktime,string supervisorName);
        void AddIncidentReportTokens(IList<Token> tokens, IncidentReport incidentReport);

        void AddPlacementRejectiontokens(IList<Token> tokens, CandidateJobOrder cjo, string reason, string comment, Account account,DateTime? FromDate=null);

        void AddJobPostingTokens(IList<Token> tokens, JP_Core.JobPosting jobPosting, bool includeCompanyName = true);
        void AddWorktimeAdjustmentTokens(IList<Token> tokens, CandidateWorkTime candidateWorktime,string supervisorName);

        void AddJobPostingPublishTokens(IList<Token> tokens, IList<string> jobOrderIds, JP_Core.JobPosting jobPosting);

        void AddTokensForJobOrderFromJobPosting(IList<Token> tokens, JobOrder jobOrder);
        void AddCandidateNotOnBoardedPunchTokens(IList<Token> tokens, CandidateClockTime candidateClockTime);
        void AddScheduleTimeOffTokens(IList<Token> tokens, EmployeeSchedule schedule, EmployeeTimeoffBooking timeoff);
        void AddUnexpectedPunchTokens(IList<Token> tokens, CandidateClockTime candidateClockTime, Candidate candidate);

        void AddConfirmationEmailLinkTokens(IList<Token> tokens, ConfirmationEmailLink link);
        void AddCandidateTestsLinkTokens(IList<Token> tokens, CandidateTestLink link, IList<TestCategory> categories);


        #region Tiemsheet Import Placement Validation Alert

        void AddTimeSheetImportPlacementValidationAlertTokens(IList<Token> tokens, string companyName, IList<int> bannedIds, IList<string> duplicates);

        #endregion


        #region Rescheduling

        void AddResschedulingTokens(IList<Token> tokens, Candidate candidate, DateTime startDate, DateTime endDate, JobOrder origJobOrder, JobOrder newJobOrder);

        #endregion
    }
}

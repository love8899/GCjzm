using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Blogs;
using Wfm.Core.Domain.Forums;
using Wfm.Core.Domain.Messages;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Domain.Incident;
using JP_Core = Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.Franchises;
using System.Text;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Services.Messages
{
    public partial interface IWorkflowMessageService
    {
        MessageTemplate GetActiveMessageTemplate(string messageTemplateName, int franchaseId);

        #region Account workflow

        /// <summary>
        /// Sends 'New account' notification message to a franchise owner
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountRegisteredNotificationMessage(Account account, int languageId);

        /// <summary>
        /// Sends a welcome message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountWelcomeMessage(Account account, int languageId);

        /// <summary>
        /// Sends an email validation message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountEmailValidationMessage(Account account, int languageId);

        /// <summary>
        /// Sends password recovery message to a account
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendAccountPasswordRecoveryMessage(Account account, int languageId);

        #endregion

        #region Candidate workflow

        /// <summary>
        /// Sends 'New candidate' notification message to a franchise owner
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCandidateRegisteredNotificationMessage(Candidate candidate, int languageId);

        /// <summary>
        /// Sends a welcome message to a candidate
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCandidateWelcomeMessage(Candidate candidate, int languageId);

        int SendCandidateIdQrCode(Candidate candidate, int languageId);

        /// <summary>
        /// Sends an email validation message to a candidate
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCandidateEmailValidationMessage(Candidate candidate, int languageId);

        /// <summary>
        /// Sends password recovery message to a candidate
        /// </summary>
        /// <param name="candidate">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCandidatePasswordRecoveryMessage(Candidate candidate, int languageId);

        QueuedEmail GetCandidateRequiredTestsEmail(Candidate candidate, int languageId, int franchiseId);
        int SendCandidateTestsMessage(Candidate candidate, int languageId);
        #endregion

        #region JobOrder workflow

        /// <summary>
        /// Sends a job order placed notification to a Candidate
        /// </summary>
        /// <param name="order">JobOrder instance</param>
        /// <param name="vendor">Candidate instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendJobOrderPlacedCandidateNotification(JobOrder jobOrder, Candidate candidate, int languageId);

        /// <summary>
        /// Sends a job order placed notification to a Recruiter
        /// </summary>
        /// <param name="order">JobOrder instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendJobOrderPlacedRecruiterNotification(JobOrder jobOrder, Account account, int languageId);

        /// <summary>
        /// Sends the job order applied recruiter notification.
        /// </summary>
        /// <param name="jobOrder">The job order.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="languageId">The language identifier.</param>
        /// <returns></returns>
        int SendJobOrderAppliedRecruiterNotification(JobOrder jobOrder, Candidate candidate, int languageId);

        int SendRemindEmailPlacementNotification(JobOrder jobOrder,DateTime refDate, int languageId);
        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId);

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="account">Account instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="accountEmail">Account's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        int SendJobOrderEmailAFriendMessage(Candidate candidate, int languageId,
            JobOrder jobOrder, string accountEmail, string friendsEmail, string personalMessage);

        #endregion

        #region TimeSheet workflow

        int SendCandidateMissedClockingRecruiterNotification(Company company, JobOrder jobOrder, Account contact, List<Candidate> candidateList, int languageId);

        int SendPendingApprovalReminder(int languageId, IList<Account> accounts, DateTime todayNow, DateTime dueTime, DateTime startDate, DateTime endDate, 
                                        string attachmentFileName = null, byte[] attachmentFile = null);

        void SendWorkTimeRejectionRecruiterNotification(CandidateWorkTime candidateWorkTime, int languageId, Account currentAccount);

        void SendWorkTimeAdjustmentRecruiterNotification(CandidateWorkTime candidateWorkTime, int languageId, Account sender );
        void SendNotOnBoardedPunchRecruiterNotification(CandidateClockTime candidateClockTime, Candidate candidate, int languageId);

        void SendUnexpectedPunchNotification(CandidateClockTime candidateClockTime, Candidate candidate, int languageId);

        void SendMissingHourFollowUpAlert(Account account, string attachmentName, byte[] attachmentBytes);

        void SendTimeSheetImportPlacementValidationAlert(string companyName, IList<int> bannedIds, IList<string> duplicates, IList<int> receivers, string importedBy);

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
        int SendNewForumTopicMessage(Account account,
            ForumTopic forumTopic, Forum forum, int languageId);

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
        int SendNewForumPostMessage(Account account,
            ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, 
            int languageId);

        #endregion
        
        #region Misc

        /// <summary>
        /// Sends a blog comment notification message to a franchise owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId);

        #endregion

        int SendNotificationWhenNewIncidentCreatedByClient(IncidentReport incident, int languageId);
        string GetContactEmailForJobOrder(JobOrder jo, out int toAccountId, out string ccEmail);
        void SendCancelJobPostingEmail(JP_Core.JobPosting jobPosting, List<JobOrder> jobOrders,Account clientAccount ,int languageId, string reason, out StringBuilder error);
        QueuedEmail GetJobOrderPlacementEmail(JobOrder jo, DateTime refDate, Franchise franchise, int languageId, string fileName,string filePath,string note,out string error);

        #region Reject Placement

        int SendNotificationToRecruiterForPlacmentRejection(CandidateJobOrder cjo, string reason, string comment, Account account, int languageId, DateTime? FromDate = null);

        #endregion

        #region JobPosting

        void SendJobPostingCreateEditNotification(JP_Core.JobPosting jobPosting, int languageId, bool isCreateNotification);
        
        void SendJobPostingPublishNotification(JP_Core.JobPosting jobPosting, List<string> jobOrderIds, int languageId);

        void SendJobPostingPublishNotificationToVendors(List<string> jobOrderIds, int languageId = 1);

        void SendJobPostingSubmissionReminder(JP_Core.JobPosting jobPosting, int languageId = 1);

        #endregion

        #region Vendor Certificate Expire reminder
        void SendVendorCertificateExpireReminder(string certificateName, int franchiseId, int languageId);
        #endregion

        #region Scheduling
        void SendRescheduleTimeoffBookingNotification(EmployeeSchedule schedule, EmployeeTimeoffBooking timeoff, int languageId = 1);
        #endregion


        #region Send Email to Employees when they are placed in the job order

        string SendConfirmationToEmployeeNotification(int jobOrderId, int candidateId,DateTime start,DateTime? end, int type, int languageId = 1);

        void SendConfirmationToEmployeeNotification(QueuedEmail email);

        QueuedEmail GetCandidateConfirmationEmail(int jobOrderId, int candidateId, DateTime start, DateTime? end, 
            int type, out string result, int languageId = 1, bool forMassMessagePreview = false);

        string SendDailyConfirmationToCandidate(JobOrder jobOrder, DateTime refDate, out int done, 
            List<int> statusList = null, bool skipContacted = true, int languageId = 1);

        #endregion


        #region candidate confirm back message
        int SendConfirmationFeedBackMessage(int jobOrderId, int candidateId, bool accept, int receiverId,string note, int languageId=1);
        #endregion

        #region SendCandidateOutOfJobOrderMessage
        int SendCandidateOutOfJobOrderMessage(int jobOrderId, int candidateId, DateTime moveOutDate, int languageId = 1);
        #endregion

        #region Rescheduling Alerts

        int SendCandidateReschedulingMessage(int origId, int newId, int candidateId, DateTime startDate, DateTime endDate, int languageId = 1);

        #endregion

        #region Send General Email
        int SendGeneralEmail(int templateId, int? jobOrderId, Candidate candidate, DateTime? start, DateTime? end, int languageId, out string result);
        #endregion

        #region Send One Week Follow Up report to recruiter
        int SendOneWeekFollowUpReminderToRecruiter(Account account, string fileName, byte[] file, int languageId=1);
        #endregion


        #region Payroll

        string SendPaystubToEmployee(Candidate candidate, byte[] paystub, DateTime payPeriodStart, DateTime payPeriodEnd, string fileName);

        #endregion

        #region Time Off email
        int SendTimeOffRequestToManager(Account account, EmployeeTimeoffBooking timeOff, string action, int languageId = 1);
        int SendTimeOffRequestFeedBackToAccount(Account account, EmployeeTimeoffBooking timeOff, int languageId = 1, string result = null);
        #endregion


        #region Max Annual Hours Reached

        void SendMaxAnnualHoursReachedAlert(IList<string> recipents, string attachmentName, byte[] attachmentBytes, string companyName, DateTime startDate, DateTime endDate, int maxHours);

        #endregion


        #region Employee Seniority

        int SendEmployeeSeniorityAlert(DateTime refDate, int scope, decimal threshold, string recipient, string attachmentFileName = null, byte[] attachmentFile = null, int languageId = 1);

        #endregion
    }
}

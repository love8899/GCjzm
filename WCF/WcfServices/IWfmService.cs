using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Wfm.Core.Domain.TimeSheet;
using WcfServices.TimeSheets;


namespace WcfServices
{
    [ServiceContract]
    public interface IWfmService
    {
        /// 
        ///Returns a constant string ("Service is running.")
        ///
        [WebGet]
        [OperationContract]
        string PingWfmService();

        [OperationContract(IsOneWay = true)]
        [WebInvoke(Method = "POST", UriTemplate = "SendMassEmail?p1={User}&p2={Password}&p3={SentBy}&p4={FranchiseId}&p5={SelectedIds}&p6={targetType}&p7={From}&p8={FromName}&p9={Subject}&p10={Message}&p11={MessageCategoryId}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SendMassEmail(string User, string Password, string SentBy, int FranchiseId, string SelectedIds, string targetType, string From, string FromName, string Subject, string Message, int MessageCategoryId);


        [OperationContract(IsOneWay = true)]
        [WebInvoke(Method = "POST", UriTemplate = "SendMassMessage?p1={User}&p2={Password}&p3={SelectedIds}&p4={Body}&p5={From}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SendMassMessage(string User, string Password, string SelectedIds, string Body, string From);


        /// <summary>
        /// Authenticates the user credentials and if valid, creates a new session for the user. User account should be active and a client account.
        /// </summary>
        /// <param name="accountUsername">User name</param>
        /// <param name="accountPassword">Password</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "AuthenticateUser/?p1={accountUsername}&p2={accountPassword}", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        AuthenticationResult AuthenticateUser(string accountUsername, string accountPassword);

        /// <summary>
        /// Refreshes the session by getting the last valid session id and issueing a new session id
        /// </summary>
        /// <param name="sessionId"></param>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RefreshSession/?p1={userName}&p2={sessionId}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        AuthenticationResult RefreshSession(string userName, string sessionId);

        /// <summary>
        /// Closes the session
        /// </summary>
        /// <param name="sessionId"></param>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Logout/?p1={sessionId}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void LogOut(string sessionId);

        // [OperationContract]
        List<string> GetAccountSecuirtyQuestions(string User, string Password, string email);

        /// <summary>
        /// When user clicks on 'Forgot Password', he/she should provide an email address. If the email exists in our DB, an email with links to password reset page will be sent to the user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>false if it encounters any errors (exceptions), otherwise it returns true (even if the email does not exist in the DB)</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool SendPasswordResetMessage(string email);

        /// <summary>
        /// Returns the data for Daily Time Sheets Page
        /// </summary>
        /// <param name="SessionId">An active session ID</param>
        /// <param name="submittedOnly">False:The service includes both approved and pending records.  True: The service will only return pending records(excludes the approved timesheets) </param>
        /// <param name="pageSize">Maximum number of rows that will be returned per request</param>
        /// <param name="pageIndex"></param>
        /// <param name="sortBy">One of the followings (case insensitive):
        /// EmployeeNumber
        /// JobTitle
        /// JobShift
        /// Location
        /// Department
        /// FirstName
        /// LastName
        /// JobOrder
        /// ContactName
        /// VendorName
        /// JobStartDateTime
        /// NOTE: If no value is defined for sort parameters, the 'JobStartDateTime desc' will be used
        /// </param>
        /// <param name="sortOrder">valid value is 'asc' or 'desc' (case insensitive, without the single quotes)</param>
        /// <param name="filterBy">One of the followings:
        /// EmployeeNumber
        /// JobTitle
        /// JobShift
        /// Location
        /// Department
        /// FirstName
        /// LastName
        /// JobOrder
        /// ContactName
        /// VendorName
        /// JobStartDateTime
        /// </param>
        ///  <param name="filterCondition">
        ///  valid values are:
        ///   "like" 
        ///   "eq" (maps into "=")
        ///   "gt" (maps into ">")
        ///   "lt" (maps into "<")
        ///   "gteq" (maps into ">="
        ///   "lteq" (maps into "<=")
        ///  </param>
        /// <param name="filterValue">Value to filter data on</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetDailyTimeSheet?p1={sessionId}&p2={submittedOnly}&p3={pagesize}&p4={pageindex}&p5={sortby}&p6={sortorder}&p7={filterBy}&p8={filterCondition}&p9={filterValue}", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        [ServiceKnownType(typeof(PagedListResult<TimeSheet>))]
        PagedListResult<TimeSheet> GetDailyTimeSheet(string SessionId, bool submittedOnly, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue);

        /// <summary>
        /// Returns the data for Time Sheet History Page
        /// </summary>
        /// <param name="SessionId">An active session ID</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageSize">Maximum number of rows that will be returned per request</param>
        /// <param name="pageIndex"></param>
        /// <param name="sortBy">One of the followings (case insensitive):
        /// EmployeeNumber
        /// FirstName
        /// LastName
        /// Department
        /// Location
        /// JobTitle
        /// JobOrder
        /// ContactName
        /// VendorName
        /// ApprovedBy
        /// NOTE: If no value is defined for the sort parameters 'EmployeeNumber asc' will be used
        /// </param>
        /// <param name="sortOrder">valid value is 'asc' or 'desc' (case insensitive, without the single quotes)</param>
        /// <param name="filterBy">One of the followings:
        /// EmployeeNumber
        /// FirstName
        /// LastName
        /// Department
        /// Location
        /// JobTitle
        /// JobOrder
        /// ContactName
        /// VendorName
        /// ApprovedBy
        /// </param>
        ///  <param name="filterCondition">
        ///  valid values are:
        ///   "like" 
        ///   "eq" (maps into "=")
        ///   "gt" (maps into ">")
        ///   "lt" (maps into "<")
        ///   "gteq" (maps into ">="
        ///   "lteq" (maps into "<=")
        ///  </param>
        /// <param name="filterValue">Value to filter data on</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetEmployeeTimeSheetHistoryByDate/?p1={sessionId}&p2={start}&p3={end}&p4={pagesize}&p5={pageindex}&p6={sortby}&p7={sortorder}&p8={filterBy}&p9={filterCondition}&p10={filterValue}", ResponseFormat = WebMessageFormat.Json)]
        [ServiceKnownType(typeof(PagedListResult<SimpleEmployeeTimeChartHistory>))]
        PagedListResult<SimpleEmployeeTimeChartHistory> GetEmployeeTimeSheetHistoryByDate(string sessionId, DateTime start, DateTime end, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SessionId">An active session ID</param>
        /// <param name="weekStartDate">This date has to be Sunday </param>
        /// <param name="submittedOnly">False:The service include both approved and pending records.  True: The service will only return pending records(excludes the approved timesheets) </param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortBy">One of the followings (case insensitive):
        /// EmployeeNumber
        /// JobTitle
        /// Location
        /// Department
        /// FirstName
        /// LastName
        /// JobShift
        /// ContactName
        /// VendorName
        /// JobStartDateTime
        /// NOTE: If no value is defined for sort parameters, the 'JobStartDateTime desc' will be used
        /// </param>
        /// <param name="sortOrder">valid value is 'asc' or 'desc' (case insentitive, without the single quotes)</param>
        /// <param name="filterBy">One of the followings:
        /// EmployeeNumber
        /// JobTitle
        /// Location
        /// Department
        /// FirstName
        /// LastName
        /// JobShift
        /// ContactName
        /// VendorName
        /// JobStartDateTime
        /// </param>
        ///  <param name="filterCondition">
        ///  valid values are:
        ///   "like" 
        ///   "eq" (maps into "=")
        ///   "gt" (maps into ">")
        ///   "lt" (maps into "<")
        ///   "gteq" (maps into ">="
        ///   "lteq" (maps into "<=")
        ///  </param>
        /// <param name="filterValue">Value to filter data on</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetTimeSheetForApproval?p1={sessionId}&p2={weekStartDate}&p3={submittedOnly}&p4={pagesize}&p5={pageindex}&p6={sortby}&p7={sortorder}&p8={filterBy}&p9={filterCondition}&p10={filterValue}", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        PagedListResult<TimeSheet> GetTimeSheetForApproval(string SessionId, DateTime weekStartDate, bool submittedOnly, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SessionId"></param>
        /// <param name="candidateTimeSheet">'Note' is a required field for updating a time sheet and cannot be empty</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        TimeSheetOperationResult UpdateTimeSheet(string SessionId, TimeSheet candidateTimeSheet);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json)]
        TimeSheetOperationResult ApproveTimeSheet(TimeSheetAndSession input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="timeSheets"></param>
        /// <returns>The number of time sheets that were successfully approved.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        PagedListResult<TimeSheet> ApproveTimeSheets(string sessionId, TimeSheetSlim[] timeSheets);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">'Note' is a required field for rejecting a time sheet and cannot be empty</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json)]
        TimeSheetOperationResult RejectTimeSheet(TimeSheetAndSession input);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        TimeSheetOperationResult AddCandidateWorkTime(string sessionId, WorkTime workTime);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetAllDailyAttendanceList?p1={sessionId}&p2={refDate}&p3={pagesize}&p4={pageindex}&p5={sortby}&p6={sortorder}&p7={filterBy}&p8={filterCondition}&p9={filterValue}", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        PagedListResult<DailyAttendance> GetAllDailyAttendanceList(string sessionId, DateTime refDate, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        BasicResult SubmitMobilePunch(string sessionId, MobilePunchEntry mobilePunch);
    }

    [DataContract]
    public class TimeSheetAndSession
    {
        [DataMember]
        public string SessionId {get; set;}
        [DataMember]
        public TimeSheetSlim CandidateTimeSheet { get; set; }
    }

    [DataContract]
    public class BasicResult
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class TimeSheetOperationResult
    {
        [DataMember]
        public BasicResult Result { get; set; }
        [DataMember]
        public TimeSheet TimeSheet { get; set; }
    }

    public interface ICollectionWrapper<T>
    {
        long TotalCount { get; set; }
        IList<T> Items { get; set; }
    }

    [KnownType(typeof(TimeSheet))]
    [KnownType(typeof(SimpleEmployeeTimeChartHistory))]
    public class PagedListResult<T> : ICollectionWrapper<T>
    {
        [DataMember]
        public long TotalCount { get; set; }
        [DataMember]
        public IList<T> Items { get; set; }
    }

    [DataContract]
    public class AuthenticationResult
    {
        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public bool IsPasswordExpired { get; set; }

        [DataMember]
        public bool IsDelegate { get; set; }

        [DataMember]
        public string SessionId { get; set; }
    }

    [DataContract]
    public class WorkTime
    {
        [DataMember]
        public string CandidateGuid { get; set; }
        [DataMember]
        public string JobOrderGuid { get; set; }
        [DataMember]
        public DateTime WorkDate { get; set; }
        [DataMember]
        public decimal NetWorkTimeInHours { get; set; }
        [DataMember]
        public string Note { get; set; }
    }

    [DataContract]
    public class TimeSheet
    {
        [DataMember]
        public int TimeSheetId { get; set; }

        [DataMember]
        public string CandidateGuid { get; set; }

        [DataMember]
        public int FranchiseId { get; set; }

        [DataMember]
        public string VendorName { get; set; }

        [DataMember]
        public int CandidateId { get; set; }

        [DataMember]
        public int JobOrderId { get; set; }

        [DataMember]
        public string JobOrderGuid { get; set; }

        [DataMember]
        public int CandidateWorkTimeStatusId { get; set; }

        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public string EmployeeFirstName { get; set; }

        [DataMember]
        public string EmployeeLastName { get; set; }

        [DataMember]
        public string JobTitle { get; set; }

        [DataMember]
        public string JobShift { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public int CompanyLocationId { get; set; }

        [DataMember]
        public string LocationName { get; set; }

        [DataMember]
        public string DepartmentName { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public DateTime JobStartDateTime { get; set; }

        [DataMember]
        public DateTime JobEndDateTime { get; set; }

        [DataMember]
        public int Year { get; set; }

        [DataMember]
        public int WeekOfYear { get; set; }

        [DataMember]
        public DateTime? ClockIn { get; set; }

        [DataMember]
        public DateTime? ClockOut { get; set; }

        [DataMember]
        public decimal JobOrderDurationInHours { get; set; }

        [DataMember]
        public decimal ClockTimeInHours { get; set; }

        [DataMember]
        public decimal NetWorkTimeInMinutes { get; set; }

        [DataMember]
        public decimal NetWorkTimeInHours { get; set; }

        [DataMember]
        public int Payroll_BatchId { get; set; }

        [DataMember]
        public DateTime? CreatedOnUtc { get; set; }

        [DataMember]
        public DateTime? UpdatedOnUtc { get; set; }

        [DataMember]
        public decimal AdjustmentInMinutes { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public bool AllowSuperVisorModifyWorkTime { get; set; }

        [DataMember]
        public string OvertimeMessage { get; set; }
        [DataMember]
        public string OtherAlerts { get; set; }
       
        [DataMember]
        public bool IsCurrentWeek { get; set; }

        [DataMember]
        public string ApprovedBy { get; set; }
    }

    [DataContract]
    public class TimeSheetSlim
    {
        [DataMember]
        public int TimeSheetId { get; set; }

        [DataMember]
        public string CandidateGuid { get; set; }

        [DataMember]
        public string Note { get; set; }
    }

    [DataContract]
    public class DailyAttendance
    {
        [DataMember]
        public int CandidateJobOrderId { get; set; }
        [DataMember]
        public Guid CandidateGuid { get; set; }
        [DataMember]
        public int CandidateId { get; set; }
        [DataMember]
        public string EmployeeId { get; set; }
        [DataMember]
        public string EmployeeFirstName { get; set; }
        [DataMember]
        public string EmployeeLastName { get; set; }
        [DataMember]
        public int JobOrderId { get; set; }
        [DataMember]
        public string JobTitle { get; set; }
        [DataMember]
        public string JobTitleAndId { get; set; }
        [DataMember]
        public DateTime ShiftStartTime { get; set; }
        [DataMember]
        public DateTime ShiftEndTime { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]        
        public string Location { get; set; }
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public string VendorName { get; set; } 
    }

    [DataContract]
    public class MobilePunchEntry
    {
        [DataMember]
        public Guid CandidateGuid { get; set; }
        [DataMember]
        public int CandidateId { get; set; }
        [DataMember]
        public DateTime Punch { get; set; }
    }

}
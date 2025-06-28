using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Wfm.Core;
using WcfServices.Accounts;
using WcfServices.Messages;
using WcfServices.TimeSheets;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Security;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Core.Infrastructure;
using Wfm.Services.TimeSheet;
using Wfm.Services.Security;
using WcfServices.BL.TimeSheets;
using Wfm.Services.JobOrders;
using Wfm.Services.Candidates;
using Wfm.Services.Messages;
using Wfm.Core.Caching;
using WcfServices.BL.MobilePunch;


namespace WcfServices
{
    public class WfmService : IWfmService
    {
        public static SqlDatabase WfmDataBase = new SqlDatabase(Common.GetConnectionString());
       // private static string _svcCacheName = "WcfServiceCache";
        private static MemoryCacheManager _svcCache = new MemoryCacheManager();


        #region Private helper methods
        private WcfSession ValidateSession(string sessionId)
        {
            WcfSession _session =  _svcCache.Get<WcfSession>(sessionId);
            if (_session == null)
            {
                WcfSessionManager _sessionBL = new WcfSessionManager();
                _session = _sessionBL.ValidateSession(sessionId);
                if (_session == null || _session.SessionId == null || !_session.UserAccount.IsClientAccount)
                    return null;
                else
                {
                    _svcCache.Set(_session.SessionId, _session, 120);
                    return _session;
                }
            }
            else
            {
                return _session;
            }
        }


        private IWorkContext InitilizeWorkContext(WcfSession session)
        {
            var _workContext = EngineContext.Current.Resolve<IWorkContext>();
            _workContext.CurrentAccount = session.UserAccount;

            using (DbCommand command = WfmDataBase.GetSqlStringCommand(@"
                    select ar.IsClientRole, ar.IsVendorRole, ar.SystemName, AccountRole_Id
                    from Account_AccountRole_Mapping  arm
                        inner join AccountRole ar on arm.AccountRole_Id = ar.Id and ar.IsActive = 1
                    where arm.Account_id = @id"))
            {
                WfmDataBase.AddInParameter(command, "Id", DbType.String, session.AccountId);
                WfmDataBase.AddInParameter(command, "accountRoleId", DbType.String, 0); // the value will change in the loop

                using (var objReader = WfmDataBase.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
                {
                    while (objReader.Read()) // (objReader.ReadAsync())
                    {
                        var _accountRole = new AccountRole();
                        

                        _accountRole.IsActive = true;
                        _accountRole.Id = Convert.ToInt32(objReader["AccountRole_Id"]);
                        _accountRole.IsClientRole = Convert.ToBoolean(objReader["IsClientRole"]);
                        _accountRole.IsVendorRole = Convert.ToBoolean(objReader["IsVendorRole"]);
                        _accountRole.SystemName = Convert.ToString(objReader["SystemName"]);

                        
                        // Get permissions
                        command.CommandText = @"Select Name, SystemName,Category From PermissionRecord_Role_Mapping  prm
                                                 inner join PermissionRecord pr on prm.PermissionRecord_Id = pr.Id
                                                Where AccountRole_Id = @accountRoleId";
                        WfmDataBase.SetParameterValue(command, "accountRoleId", _accountRole.Id);

                        using (var objReader2 = WfmDataBase.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
                        {
                            while (objReader2.Read()) // (objReader.ReadAsync())
                            {
                                var _permission = new PermissionRecord();
                                _permission.Name = Convert.ToString(objReader2["Name"]); 
                                _permission.SystemName = Convert.ToString(objReader2["SystemName"]); 
                                _permission.Category = Convert.ToString(objReader2["Category"]);

                                if (_permission.SystemName == "CompanyAdministrators")
                                    _workContext.IsCompanyAdministrator = true;

                                _accountRole.PermissionRecords.Add(_permission);
                            }
                        }

                        session.UserAccount.AccountRoles.Add(_accountRole);
                        break;
                    }
                }
            }
            //_workContext.IsAdmin = false;
            //_workContext.OriginalAccountIfImpersonate = null;
            return _workContext;
        }

        #endregion


        #region Services that do NOT require any session
        public string PingWfmService()
        {
            return "Service is running.";
        }

        public void SendMassEmail(string User, string Password, string SentBy, int FranchiseId, string SelectedIds, string targetType, string FromEmail, string FromName, string Subject, string Message, int MessageCategoryId)
        {
            Common _common = new Common();

            // Authenticate the account
            if (!_common.AuthenticateWebServiceCredentials(User, Password))
                return;

            #region validate input parameters

            if (String.IsNullOrWhiteSpace(FromEmail))
                throw new ArgumentNullException("From");

            if (String.IsNullOrWhiteSpace(Message))
                throw new ArgumentNullException("Message");


            // SentBy must be an existing GUID in the Account table. We validate this GUID and make sure the associated user has permission to send mass emails
            Wfm.Core.Domain.Accounts.Account userAccount = null;
            if (String.Equals(targetType, "Candidate", StringComparison.OrdinalIgnoreCase))
            {
                userAccount = _common.ValidateUserAccount(SentBy, FranchiseId, "MassEmailToCandidates");
                if (userAccount == null || userAccount.IsClientAccount)
                    return;
            }
            else if (String.Equals(targetType, "Vendor", StringComparison.OrdinalIgnoreCase))
            {
                userAccount = _common.ValidateUserAccount(SentBy, FranchiseId, "MassEmailToVendors");
                if (userAccount == null || userAccount.IsClientAccount)
                    return;
            }
            else
                return; //invalid target type

            #endregion

            int batchSize, bccMaxLength, prioriyId, fromEmailAccountId;
            string query;
            if (String.Equals(targetType, "Candidate", StringComparison.OrdinalIgnoreCase))
            {
                var massEmailSettings = _common.LoadSettings<CandidateMassEmailSettings>().GetAwaiter().GetResult();
                batchSize = massEmailSettings.BatchSize;
                bccMaxLength = massEmailSettings.BccMaxLength;
                prioriyId = massEmailSettings.PriorityId;
                fromEmailAccountId = massEmailSettings.FromEmailAccountId;

                // Send mass emails to candidates
                query =  String.Format(@"Select Email from Candidate 
                                                left outer join Employee_Type et on Candidate.EmployeeTypeId = et.Id and et.Code <> 'REG'
                                         Where IsDeleted = 0 
                                           and Email Is not null
                                           and candidate.Id in ({0})", SelectedIds);
            }
            else
            {
                var massEmailSettings = _common.LoadSettings<VendorMassEmailSettings>().GetAwaiter().GetResult();
                batchSize = massEmailSettings.BatchSize;
                bccMaxLength = massEmailSettings.BccMaxLength;
                prioriyId = massEmailSettings.PriorityId;
                fromEmailAccountId = massEmailSettings.FromEmailAccountId;

                // send mass emails to vendors
                query = String.Format(@"Select Email from Account 
                                         Where IsDeleted = 0 
                                           and IsActive = 1
                                           and IsClientAccount = 0 
                                           and IsSystemAccount = 0
                                           and Email Is not null
                                           and Account.Id in ({0})", SelectedIds);
            }

            if (batchSize == 0) throw new ArgumentNullException("batchSize");
            // 254 = Max Email address length
            if (bccMaxLength < 254 || bccMaxLength > 4000) throw new ArgumentNullException("bccMaxLength");

            if (userAccount.IsLimitedToFranchises) // IsLimitedToFranchises -- it is vendor 
                query = String.Concat(query, " and FranchiseId = @franchiseId ");

            List<string> emails = new List<string>();

            SqlDatabase database = new SqlDatabase(Common.GetConnectionString());

            // default category is Other
            if (MessageCategoryId == 0)
            {
                using (DbCommand command = database.GetSqlStringCommand("Select Id from MessageCategory Where CategoryName = 'Other' and IsActive = 1 "))
                {
                    using (var objReader = database.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
                    {
                        while (objReader.Read()) // (objReader.ReadAsync())
                        {
                            MessageCategoryId = Convert.ToInt32(objReader["Id"]);
                            break;
                        }
                    }
                }
            }

            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                if (userAccount.IsLimitedToFranchises)
                    database.AddInParameter(command, "franchiseId", DbType.Int32, FranchiseId);

                using (var objReader = database.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
                {
                    while (objReader.Read()) // (objReader.ReadAsync())
                    {
                        emails.Add(objReader["Email"].ToString());
                    }
                }
            }

            const string insertSql = @"INSERT INTO [QueuedEmail]
                                       (EmailAccountId, Priority, [From], [FromName], [To], [ToName], Bcc, Subject, Body, SentTries, CreatedOnUtc, UpdatedOnUtc, fromAccountId, ToAccountId, MessageCategoryId)
                                       Values
                                       (@EmailAccountId, @Priority, @From, @fromName, @To, @toName, @Bcc, @Subject, @Body, 0, GetUtcDate(), GetUtcDate(), @fromUserAccountId, @toUserAccountId, @messageCategoryId )";

            using (DbCommand command = database.GetSqlStringCommand(insertSql))
            {
                // in mass email, the 'from' and 'to' are the same
                database.AddInParameter(command, "from", DbType.String, FromEmail);
                database.AddInParameter(command, "fromName", DbType.String, FromName);
                database.AddInParameter(command, "to", DbType.String, FromEmail);
                database.AddInParameter(command, "toName", DbType.String, FromName);
                database.AddInParameter(command, "fromUserAccountId", DbType.Int32, userAccount.Id);
                database.AddInParameter(command, "toUserAccountId", DbType.Int32, userAccount.Id);

                database.AddInParameter(command, "bcc", DbType.String, String.Empty);
                database.AddInParameter(command, "subject", DbType.String, Subject);
                database.AddInParameter(command, "body", DbType.String, Message);
                database.AddInParameter(command, "emailAccountId", DbType.Int32, fromEmailAccountId);
                database.AddInParameter(command, "priority", DbType.Int32, prioriyId);
                database.AddInParameter(command, "messageCategoryId", DbType.Int32, MessageCategoryId);
                //todo: add replyTo

                int sentNum = 0;
                while (sentNum < emails.Count)
                {
                    var addrList = emails.Skip(sentNum).Take(batchSize).ToList();

                    int batchNum = 0, i = 0;
                    var sb = new StringBuilder();
                    while (i < addrList.Count && (sb.Length + addrList[i].Length) < bccMaxLength)
                    {
                        sb.Append(addrList[i]).Append(";");
                        batchNum++;
                        i++;
                    }
                        
                    var bcc = sb.ToString();
                    // remove last semicolon
                    bcc = bcc.Substring(0, bcc.LastIndexOf(';'));

                    if (String.IsNullOrWhiteSpace(bcc))
                        throw new ArgumentNullException("bcc");
                    database.SetParameterValue(command, "bcc", bcc);
                    database.ExecuteNonQuery(command);

                    sentNum += Math.Min(batchNum, batchSize);
                }
            }

        }


        public void SendMassMessage(string User, string Password, string SelectedIds, string Body, string From)
        {
            Common _common = new Common();
            if (!_common.AuthenticateWebServiceCredentials(User, Password))
                return;

            if (String.IsNullOrWhiteSpace(Body))
                throw new ArgumentNullException("Body");

            var messageBL = new MessageBL();
            messageBL.SendMassMessage(SelectedIds, Body, From);
        }


        public bool SendPasswordResetMessage(string email)
        {
            var _passwordRecoveryBL = new PasswordRecoveryBL();
            return _passwordRecoveryBL.SendPasswordResetMessage(email);
        }

        #endregion

        public AuthenticationResult AuthenticateUser(string accountUsername, string accountPassword)
        {
            var result = new AuthenticationResult() { IsValid = false, SessionId = null };

            #region validate input parameters

            if (String.IsNullOrWhiteSpace(accountUsername) || String.IsNullOrWhiteSpace(accountPassword))
            {
                WcfLogger.LogError("Authentication failed.", "WCF - AuthenticateUser(): User name or password are empty.", null);
                return result;
            }

            #endregion

            bool isPasswordExpired, isDelegate;
            WcfSession session;
  
            Login _loginBL = new Login();
            result.IsValid = _loginBL.ValidateAccountLogin(accountUsername.Trim(), accountPassword.Trim(), out isPasswordExpired, out isDelegate, out session);
            result.IsPasswordExpired = isPasswordExpired;
            if (session != null)
                result.SessionId = session.SessionId;
            result.IsDelegate = isDelegate;

            if (result.IsValid)
                _svcCache.Set(session.SessionId, session, 120);

            return result;
        }

        public AuthenticationResult RefreshSession(string userName, string sessionId)
        {
            var result = new AuthenticationResult() { IsValid = false, SessionId = null };

            if (!String.IsNullOrWhiteSpace(sessionId) && !String.IsNullOrWhiteSpace(userName) )
            {
                WcfSessionManager _sessionBL = new WcfSessionManager();
                
                bool isPasswordExpired, isDelegate;
                WcfSession session = _sessionBL.RefreshSession(userName, sessionId, out isPasswordExpired, out isDelegate);

                if (session != null)
                {
                    result.IsPasswordExpired = isPasswordExpired;
                    result.SessionId = session.SessionId;
                    result.IsDelegate = isDelegate;
                    result.IsValid = true;

                    _svcCache.Set(session.SessionId, session, 120);
                }
            }

            return result;
        }

        public void LogOut(string sessionId)
        {
            if (!String.IsNullOrWhiteSpace(sessionId))
            {
                WcfSessionManager _sessionBL = new WcfSessionManager();
                _sessionBL.CloseSession(sessionId, true);
                //_svcCache.Remove(OperationContext.Current.SessionId);
                _svcCache.Remove(sessionId);
            }
        }

        public List<string> GetAccountSecuirtyQuestions(string User, string Password, string email)
        {
            var result = new List<string>();

            Common _common = new Common();
            if (!_common.AuthenticateWebServiceCredentials(User, Password))
                return result;

            #region validate input parameters

            if (String.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("email");

            #endregion

            var query = String.Format(@"select top 1 a.Id, email, sq1.Question as Question1, sq2.Question as Question2 from account as a
                                        left join SecurityQuestion as sq1 on a.SecurityQuestion1Id = sq1.Id
                                        left join SecurityQuestion as sq2 on a.SecurityQuestion2Id = sq2.Id
                                        where email = '{0}'", email);

            SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                using (var objReader = database.ExecuteReader(command))
                {
                    objReader.Read();
                    var q1 = objReader["Question1"].ToString();
                    var q2 = objReader["Question2"].ToString();
                    if (!String.IsNullOrWhiteSpace(q1) && !String.IsNullOrWhiteSpace(q2))
                    {
                        result.Add(q1);
                        result.Add(q2);
                    }
                }
            }

            return result;
        }

        public PagedListResult<TimeSheet> GetDailyTimeSheet(string SessionId, bool submittedOnly, int pageSize, int pageIndex, string sortBy, string sortOrder,string filterBy,string filterCondition,string filterValue)
        {
            var _session = this.ValidateSession(SessionId);
            if (_session == null)
                return null;

            PagedListResult<TimeSheet> result = new PagedListResult<TimeSheet>();
            result.Items = new List<TimeSheet>();

            var _workContext = InitilizeWorkContext(_session);
            var account = _workContext.CurrentAccount;

            // Validate user's Permission and timesheet data- check the company id, user's permission, data completeness, etc.
            if (!Common.UserHasPermission(StandardPermissionProvider.ManageClientCompanyTimeSheets))
            {
                WcfLogger.LogError("Access Denied", "WCF - GetDailyTimeSheet(): User does not have permission to get the timesheets.", _session.AccountId);
                return result;
            }

            if (string.IsNullOrWhiteSpace(sortBy))
                sortBy = "JobStartDateTime";
            if (string.IsNullOrWhiteSpace(sortOrder))
                sortOrder = "desc";

            StringBuilder query = new StringBuilder();
            query.Append(@"Select * from (Select COUNT(*) OVER() as TotalCount, CWT.Id as TimeSheetId, CA.CandidateGuid, CWT.Year, CWT.WeekOfYear, CWT.Payroll_BatchId, CA.EmployeeId, CWT.CandidateId, CWT.JobOrderId
                                    ,JO.JobTitle,CWT.CompanyId,CWT.CompanyLocationId,Isnull(CL.LocationName,'')LocationName, Isnull(CD.DepartmentName,'')DepartmentName,Franchise.FranchiseName as VendorName
                                    ,CA.FirstName,CA.LastName,Shift.ShiftName as JobShift,CWT.JobStartDateTime,CWT.JobEndDateTime,CWT.ClockIn,CWT.ClockOut,CWT.NetWorkTimeInHours
                                    ,CWT.AdjustmentInMinutes,CWT.ClockTimeInHours,CWT.CandidateWorkTimeStatusId,CWT.UpdatedOnUtc,CWT.CreatedOnUtc,JO.AllowSuperVisorModifyWorkTime 
                                    ,Isnull(Account.FirstName,'')+' '+Isnull(Account.LastName,'')  as ContactName, CWT.FranchiseId
                                    ,IsNull(ApprovedUser.FirstName,'')+ ' '+ IsNull(ApprovedUser.LastName,'') as ApprovedBy                                        
                                From CandidateWorkTime CWT Join Joborder Jo on Jo.Id =CWT.JobOrderId 
                                       left Join CompanyLocation CL on CWT.CompanyLocationId=CL.Id 
                                       left  Join CompanyDepartment CD on CWT.CompanyDepartmentId=CD.Id 
                                       Join Candidate CA On CWT.CandidateId =CA.Id 
                                       left Join Account on Jo.CompanyContactId=Account.Id 
                                       left join Account ApprovedUser on CWT.ApprovedBy = ApprovedUser.ID
                                       join Franchise on CWT.FranchiseId=Franchise.Id   
                                       left join Shift on CWT.ShiftId = Shift.Id                                                                            
                                   Where CWT.CompanyId =@companyId ");

            if (submittedOnly == true)
                query.Append(" AND CWT.CandidateWorkTimeStatusId = @candidateWorkTimeStatusSubmitted");

            // except Voided, Matched, and Rejected
            query.Append(" AND CWT.CandidateWorkTimeStatusId not in(@candidateWorkTimeStatusMatched,@candidateWorkTimeStatusRejected,@candidateWorkTimeStatusVoided)");

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query.Append(" AND cwt.CompanyLocationId > 0 AND cwt.CompanyLocationId =@companyLocationId ");

            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query.Append(@" AND cwt.CompanyLocationId > 0 AND cwt.CompanyLocationId =@companyLocationId 
                                AND cwt.CompanyDepartmentId > 0 AND cwt.CompanyDepartmentId =@companyDepartmentId 
                                  And cwt.CompanyContactId = @accountId");

            else if (account.IsCompanyDepartmentManager())
                query.Append(@" AND cwt.CompanyLocationId > 0 AND cwt.CompanyLocationId =@companyLocationId 
                               AND cwt.CompanyDepartmentId > 0 AND cwt.CompanyDepartmentId =@companyDepartmentId");

            // Apply filter. Example:    query.Append(Common.GetFilterStatement("DailyTimeSheet", "EmployeeNumber", "like", "4"));
            query.Append(Common.GetFilterStatement("DailyTimeSheet", filterBy,filterCondition, filterValue));

            query.Append(String.Concat(@") tbl Order by " , Common.GetSortStatement("DailyTimeSheet", sortBy, sortOrder) , "  OFFSET ((@pageIndex) * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY "));

            try
            {
                SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = database.GetSqlStringCommand(query.ToString()))
                {
                    database.AddInParameter(command, "companyId", DbType.Int32, _session.UserAccount.CompanyId);
                    database.AddInParameter(command, "pageSize", DbType.Int32, pageSize);
                    database.AddInParameter(command, "pageIndex", DbType.Int32, pageIndex);
                    database.AddInParameter(command, "candidateWorkTimeStatusSubmitted", DbType.Int32, (int)CandidateWorkTimeStatus.PendingApproval);
                    database.AddInParameter(command, "candidateWorkTimeStatusMatched", DbType.Int32, (int)CandidateWorkTimeStatus.Matched);
                    database.AddInParameter(command, "candidateWorkTimeStatusRejected", DbType.Int32, (int)CandidateWorkTimeStatus.Rejected);
                    database.AddInParameter(command, "candidateWorkTimeStatusVoided", DbType.Int32, (int)CandidateWorkTimeStatus.Voided);
                    database.AddInParameter(command, "companyLocationId", DbType.Int32, account.CompanyLocationId);
                    database.AddInParameter(command, "companyDepartmentId", DbType.Int32, account.CompanyDepartmentId);
                    database.AddInParameter(command, "accountId", DbType.Int32, account.Id);

                    using (var objReader = database.ExecuteReader(command))
                    {
                        while (objReader.Read())
                        {
                            TimeSheet objCandidateTimeSheet = new TimeSheet();
                            objCandidateTimeSheet.TimeSheetId = Convert.ToInt32(objReader["TimeSheetId"]);
                            objCandidateTimeSheet.FranchiseId = Convert.ToInt32(objReader["FranchiseId"]);
                            objCandidateTimeSheet.CandidateGuid = Convert.ToString(objReader["CandidateGuid"]);
                            objCandidateTimeSheet.Year = Convert.ToInt32(objReader["Year"]);
                            objCandidateTimeSheet.WeekOfYear = Convert.ToInt32(objReader["WeekOfYear"]);
                            if (objReader["Payroll_BatchId"] != DBNull.Value)
                                objCandidateTimeSheet.Payroll_BatchId = Convert.ToInt32(objReader["Payroll_BatchId"]);
                            objCandidateTimeSheet.EmployeeFirstName = Convert.ToString(objReader["FirstName"]);
                            objCandidateTimeSheet.EmployeeLastName = Convert.ToString(objReader["LastName"]);
                            objCandidateTimeSheet.EmployeeId = Convert.ToString(objReader["EmployeeId"]);
                            objCandidateTimeSheet.CandidateId = Convert.ToInt32(objReader["CandidateId"]);
                            objCandidateTimeSheet.JobOrderId = Convert.ToInt32(objReader["JobOrderId"]);
                            objCandidateTimeSheet.JobTitle = Convert.ToString(objReader["JobTitle"]);
                            objCandidateTimeSheet.CompanyLocationId = Convert.ToInt32(objReader["CompanyLocationId"]);
                            objCandidateTimeSheet.LocationName = Convert.ToString(objReader["LocationName"]);
                            objCandidateTimeSheet.DepartmentName = Convert.ToString(objReader["DepartmentName"]);
                            objCandidateTimeSheet.ContactName = Convert.ToString(objReader["ContactName"]);
                            objCandidateTimeSheet.VendorName = Convert.ToString(objReader["VendorName"]);
                            objCandidateTimeSheet.JobShift = Convert.ToString(objReader["JobShift"]);
                            objCandidateTimeSheet.JobStartDateTime = Convert.ToDateTime(objReader["JobStartDateTime"]);
                            objCandidateTimeSheet.JobEndDateTime = Convert.ToDateTime(objReader["JobEndDateTime"]);
                            if (objReader["ClockIn"] != DBNull.Value)
                                objCandidateTimeSheet.ClockIn = Convert.ToDateTime(objReader["ClockIn"]);
                            if (objReader["ClockOut"] != DBNull.Value)
                                objCandidateTimeSheet.ClockOut = Convert.ToDateTime(objReader["ClockOut"]);
                            objCandidateTimeSheet.NetWorkTimeInHours = Convert.ToDecimal(objReader["NetWorkTimeInHours"]);
                            objCandidateTimeSheet.ClockTimeInHours = Convert.ToDecimal(objReader["ClockTimeInHours"]);
                            objCandidateTimeSheet.CandidateWorkTimeStatusId = Convert.ToInt32(objReader["CandidateWorkTimeStatusId"]);
                            objCandidateTimeSheet.ApprovedBy = Convert.ToString(objReader["ApprovedBy"]);

                            if (objReader["UpdatedOnUtc"] != DBNull.Value)
                                objCandidateTimeSheet.UpdatedOnUtc = Convert.ToDateTime(objReader["UpdatedOnUtc"]);
                            if (objReader["CreatedOnUtc"] != DBNull.Value)
                                objCandidateTimeSheet.CreatedOnUtc = Convert.ToDateTime(objReader["CreatedOnUtc"]);

                            result.Items.Add(objCandidateTimeSheet);

                            if (result.Items.Count == 1)
                                result.TotalCount = Convert.ToInt64(objReader["TotalCount"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WcfLogger.LogError(ex.Message, ex.StackTrace, null);
            }
            return result;
        }

        public PagedListResult<SimpleEmployeeTimeChartHistory> GetEmployeeTimeSheetHistoryByDate(string sessionId, DateTime start, DateTime end, 
                                                                                                 int pageSize, int pageIndex, 
                                                                                                 string sortBy, string sortOrder, 
                                                                                                 string filterBy, string filterCondition, string filterValue)
        {
            var _session = this.ValidateSession(sessionId);
            if (_session == null)
                return null;

            var _timeSheetHistoryBL = new TimeSheetHistoryBL();
            return _timeSheetHistoryBL.GetEmployeeTimeSheetHistoryByDate(_session.AccountId, _session.UserAccount.CompanyId, start, end, 
                                                                         pageSize, pageIndex, sortBy, sortOrder, filterBy, filterCondition, filterValue);
        }

        
        public PagedListResult<TimeSheet> GetTimeSheetForApproval(string SessionId, DateTime weekStartDate, bool submittedOnly, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue)
        {
            var _session = this.ValidateSession(SessionId);
            if (_session == null)
                return null;

            PagedListResult<TimeSheet> result = new PagedListResult<TimeSheet>();
            result.Items = new List<TimeSheet>();

            weekStartDate = Common.GetFirstDayOfWeek(weekStartDate); // make sure the date is actually start of the week

            DateTime weekEndDate = weekStartDate.Date.AddDays(7).AddMinutes(-1);
            var _workContext = InitilizeWorkContext(_session);
            var account = _workContext.CurrentAccount;

            // Validate user's Permission and timesheet data- check the company id, user's permission, data completeness, etc.
            if (!Common.UserHasPermission(StandardPermissionProvider.ManageClientCompanyTimeSheets))
            {
                WcfLogger.LogError("Access Denied", "WCF - GetTimeSheetForApproval(): User does not have permission to read the timesheets.", _session.AccountId);
                return result;
            }

            StringBuilder query = new StringBuilder();
            query.Append(String.Concat(" Select *  from (Select  COUNT(*) OVER() as TotalCount, ", TimeSheetBL.SQL_TIME_SHEET_QUERY, " Where CWT.CompanyId =@companyId and JobStartDateTime Between @weekStartDate and @weekEndDate"));

            if (submittedOnly==true)
                query.Append(" AND CWT.CandidateWorkTimeStatusId = @candidateWorkTimeStatusSubmitted");
            else
                // except Voided, Matched
                query.Append(" AND CWT.CandidateWorkTimeStatusId not in(@candidateWorkTimeStatusMatched, @candidateWorkTimeStatusVoided)");


           
            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                query.Append(" AND cwt.CompanyLocationId > 0 AND cwt.CompanyLocationId =@companyLocationId ");
               
            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query.Append(@" AND cwt.CompanyLocationId > 0 AND cwt.CompanyLocationId =@companyLocationId 
                                AND cwt.CompanyDepartmentId > 0 AND cwt.CompanyDepartmentId =@companyDepartmentId 
                                And cwt.CompanyContactId = @accountId 
                              ");             
                
            else if (account.IsCompanyDepartmentManager())
                query.Append(@" AND cwt.CompanyLocationId > 0 AND cwt.CompanyLocationId =@companyLocationId 
                                AND cwt.CompanyDepartmentId > 0 AND cwt.CompanyDepartmentId =@companyDepartmentId");

            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "JobStartDateTime";
            if (String.IsNullOrWhiteSpace(sortOrder))
                sortOrder = "desc";

            query.Append(Common.GetFilterStatement("TimeSheetApproval", filterBy, filterCondition, filterValue));

            query.Append(String.Concat(@") tbl Order by ", Common.GetSortStatement("TimeSheetApproval", sortBy, sortOrder), "  OFFSET ((@pageIndex) * @pageSize) ROWS FETCH NEXT @pageSize ROWS ONLY "));

            try
            {
                SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = database.GetSqlStringCommand(query.ToString()))
                {
                    database.AddInParameter(command, "companyId", DbType.Int32, _session.UserAccount.CompanyId);
                    database.AddInParameter(command, "weekStartDate", DbType.DateTime, weekStartDate);
                    database.AddInParameter(command, "weekEndDate", DbType.DateTime, weekEndDate);
                    database.AddInParameter(command, "pageSize", DbType.Int32, pageSize);
                    database.AddInParameter(command, "pageIndex", DbType.Int32, pageIndex);
                    database.AddInParameter(command, "candidateWorkTimeStatusSubmitted", DbType.Int32, (int)CandidateWorkTimeStatus.PendingApproval);
                    database.AddInParameter(command, "candidateWorkTimeStatusMatched", DbType.Int32, (int)CandidateWorkTimeStatus.Matched);
                    database.AddInParameter(command, "candidateWorkTimeStatusRejected", DbType.Int32, (int)CandidateWorkTimeStatus.Rejected);
                    database.AddInParameter(command, "candidateWorkTimeStatusVoided", DbType.Int32, (int)CandidateWorkTimeStatus.Voided);
                    database.AddInParameter(command, "companyLocationId", DbType.Int32, account.CompanyLocationId);
                    database.AddInParameter(command, "companyDepartmentId", DbType.Int32, account.CompanyDepartmentId);
                    database.AddInParameter(command, "accountId", DbType.Int32, account.Id);

                    using (var objReader = database.ExecuteReader(command))
                    {
                        TimeSheetBL _bl = new TimeSheetBL();

                        while (objReader.Read())
                        {
                            var objEmployeeWorkTimeApproval = _bl.ReadOneTimeSheet(objReader);

                            result.Items.Add(objEmployeeWorkTimeApproval);

                            if (result.Items.Count == 1)
                                result.TotalCount = Convert.ToInt64(objReader["TotalCount"]);
                        }
                    }
                }              
            }
            catch (Exception ex)
            {
                WcfLogger.LogError(ex.Message, ex.StackTrace, _session.AccountId);
            }
            return  result;
        }


        private TimeSheetOperationResult UpdateTimeSheetStatus(TimeSheetAndSession input, TimeSheetOperation operation)
        {
            var result = new TimeSheetOperationResult() { Result = new BasicResult() { Success = false } };

            if (input == null || input.SessionId == null || input.CandidateTimeSheet == null)
            {
                result.Result.Message = "Invalid input parameters: null values are received.";
                return result;
            }

            var _session = this.ValidateSession(input.SessionId);
            if (_session == null)
            {
                result.Result.Message = "Invalid session id or session is expired.";
                return result;
            }

            var _workContext = InitilizeWorkContext(_session);

            // Validate user's Permission and timesheet data- check the company id, user's permission, data completeness, etc.
            if (!Common.UserHasPermission(StandardPermissionProvider.ManageClientCompanyTimeSheets))
            {
                WcfLogger.LogError("Access Denied", String.Format("WCF - {0}TimeSheet(): User does not have permission to {0} the timesheets.", operation.ToString()), _session.AccountId);
                result.Result.Message = "Access Denied.";
                return result;
            }

            var _workTimeService = EngineContext.Current.Resolve<IWorkTimeService>();
            CandidateWorkTime cwt = _workTimeService.GetWorkTimeById(input.CandidateTimeSheet.TimeSheetId);


            TimeSheetBL _bl = new TimeSheetBL();
            if (_bl.ValidateTimeSheet(cwt, input.CandidateTimeSheet.CandidateGuid, _session.UserAccount.CompanyId, _session.AccountId, operation))
            {

                try
                {
                    switch (operation)
                    {
                        case TimeSheetOperation.Approve:
                            string err;
                            result.Result.Success = _workTimeService.ApproveWorkTimeEntry(cwt.Id, _session.UserAccount, out err);
                            result.Result.Message = err;
                            break;
                        case TimeSheetOperation.Reject:
                            result.Result.Success = _workTimeService.RejectWorkTimeEntry(cwt.Id, input.CandidateTimeSheet.Note, _session.UserAccount);
                            if (!result.Result.Success)
                                result.Result.Message = "Unexpected error!";
                            break;
                    }


                    // return the updated time sheet
                    if (result.Result.Success)
                        result.TimeSheet = _bl.GetTimeSheetById(cwt.Id);
                    

                    return result;
                }
                catch (Exception ex)
                {
                    WcfLogger.LogError(String.Format("WCF - {0}TimeSheet(): {1}", operation.ToString(), ex.Message), ex.StackTrace, _session.AccountId);
                    result.Result.Message = "Unexpected error!";
                    return result;
                }
            }

            return result;
        }


        public TimeSheetOperationResult ApproveTimeSheet(TimeSheetAndSession input)
        {
            return this.UpdateTimeSheetStatus(input, TimeSheetOperation.Approve);
        }

        public PagedListResult<TimeSheet> ApproveTimeSheets(string sessionId, TimeSheetSlim[] candidateTimeSheets)
        {
            PagedListResult<TimeSheet> result = new PagedListResult<TimeSheet>();
            result.Items = new List<TimeSheet>();

            if (candidateTimeSheets == null || sessionId == null || candidateTimeSheets.Length == 0)
            {
                WcfLogger.LogError("WCF - Error in ApproveTimeSheets()", "Invalid input parameters: null values are received.", null);
                return result; 
            }

            var _session = this.ValidateSession(sessionId);
            if (_session == null)
            {
                WcfLogger.LogError("WCF - Error in ApproveTimeSheets()", "Invalid session id or session is expired.", null);
                return result;
            }

            //
            var _workContext = InitilizeWorkContext(_session);

            // Validate user's Permission and timesheet data- check the company id, user's permission, data completeness, etc.
            if (!Common.UserHasPermission(StandardPermissionProvider.ManageClientCompanyTimeSheets))
            {
                WcfLogger.LogError("Access Denied", "WCF - ApproveTimeSheets(): User does not have permission to approve the timesheets.", _session.AccountId);
                return result; 
            }

            int counter = 0;
            string err;
            var _workTimeService = EngineContext.Current.Resolve<IWorkTimeService>();
            foreach (var _timesheet in candidateTimeSheets)
            {
                CandidateWorkTime cwt = _workTimeService.GetWorkTimeById(_timesheet.TimeSheetId);
                TimeSheetBL _bl = new TimeSheetBL();
                if (_bl.ValidateTimeSheet(cwt, _timesheet.CandidateGuid, _session.UserAccount.CompanyId, _session.AccountId, TimeSheetOperation.Approve))
                {

                    try
                    {
                        if (_workTimeService.ApproveWorkTimeEntry(cwt.Id, _workContext.CurrentAccount, out err))
                        {
                            counter++;
                            result.Items.Add(_bl.GetTimeSheetById(cwt.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        WcfLogger.LogError(String.Format("WCF - ApproveTimeSheets(): {0}", ex.Message), ex.StackTrace, _session.AccountId);
                        return result; 
                    }
                }
            }

            result.TotalCount = counter;
            return result;
        }

        public TimeSheetOperationResult RejectTimeSheet(TimeSheetAndSession input)
        {
            if (input == null || String.IsNullOrWhiteSpace(input.CandidateTimeSheet.Note))
            {
                var result = new TimeSheetOperationResult() { Result = new BasicResult() { Success = false } };
                result.Result.Message = "Invalid input parameters: null values are received.";
                return result;
            }
            else
                return this.UpdateTimeSheetStatus(input, TimeSheetOperation.Reject);
        }


        public TimeSheetOperationResult UpdateTimeSheet(string sessionId, TimeSheet candidateTimeSheet)
        {
            var result = new TimeSheetOperationResult() { Result = new BasicResult() { Success = false } };

            if (sessionId == null || candidateTimeSheet == null || String.IsNullOrWhiteSpace(candidateTimeSheet.Note) || candidateTimeSheet.NetWorkTimeInMinutes < 0)
            {
                result.Result.Message = "Invalid input parameters: null values are received.";
                return result;
            }

            var _session = this.ValidateSession(sessionId);
            if (_session == null || candidateTimeSheet == null)
            {
                result.Result.Message = "Invalid session id or session is expired.";
                return result;
            }

            //
            var _workContext = InitilizeWorkContext(_session);

            if (!Common.UserHasPermission(StandardPermissionProvider.ManageClientCompanyTimeSheets))
            {
                WcfLogger.LogError("Access Denied", "WCF - UpdateTimeSheet(): User does not have permission to edit the timesheets.", _session.AccountId);
                result.Result.Message = "Access Denied.";
                return result;
            }


            var _workTimeService = EngineContext.Current.Resolve<IWorkTimeService>();
            CandidateWorkTime cwt = _workTimeService.GetWorkTimeById(candidateTimeSheet.TimeSheetId);


            TimeSheetBL _bl = new TimeSheetBL();
            if (_bl.ValidateTimeSheet(cwt, candidateTimeSheet.CandidateGuid, _session.UserAccount.CompanyId, _session.AccountId, TimeSheetOperation.Adjust))
            {
                try
                {
                    result.Result.Success = _workTimeService.ManualAdjustCandidateWorkTime(cwt.CandidateId, cwt.JobOrderId, cwt.JobStartDateTime, cwt.JobOrderId, candidateTimeSheet.AdjustmentInMinutes, candidateTimeSheet.NetWorkTimeInHours, candidateTimeSheet.Note);

                    if (result.Result.Success)
                    {
                        var _workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                        _workflowMessageService.SendWorkTimeAdjustmentRecruiterNotification(cwt, 1, _workContext.CurrentAccount);

                        // return the updated time sheet
                        if (result.Result.Success)
                        {
                            result.TimeSheet = _bl.GetTimeSheetById(cwt.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WcfLogger.LogError(String.Format("WCF - UpdateTimeSheet(): {0}", ex.Message), ex.StackTrace, _session.AccountId);
                    result.Result.Message = "Unexpected error!";
                    return result;
                }

            }
            

            return result;
        }

        public TimeSheetOperationResult AddCandidateWorkTime(string sessionId, WorkTime workTime)
        {
            var result = new TimeSheetOperationResult() { Result = new BasicResult() { Success = false } };

            if (sessionId == null || workTime == null || workTime.NetWorkTimeInHours <= 0 || String.IsNullOrWhiteSpace(workTime.Note) || String.IsNullOrWhiteSpace(workTime.CandidateGuid) || String.IsNullOrWhiteSpace(workTime.JobOrderGuid))
            {
                result.Result.Message = "Invalid input parameters: null values are received.";
                return result;
            }

            var _session = this.ValidateSession(sessionId);
            if (_session == null)
            {
                result.Result.Message = "Invalid session id or session is expired.";
                return result;
            }

            var _workContext = InitilizeWorkContext(_session);

            if (!Common.UserHasPermission(StandardPermissionProvider.ManageClientCompanyTimeSheets))
            {
                WcfLogger.LogError("Access Denied", "WCF - AddCandidateWorkTime(): User does not have permission to edit the timesheets.", _session.AccountId);
                result.Result.Message = "Access Denied.";
                return result;
            }

            var _jobOrderService = EngineContext.Current.Resolve<IJobOrderService>();
            var jobOrder = _jobOrderService.GetJobOrderByGuid(Guid.Parse(workTime.JobOrderGuid));
            if (jobOrder == null || jobOrder.CompanyId != _session.UserAccount.CompanyId)
            {
                WcfLogger.LogError("Failed to add the new time sheet","Job Order does not exist!", _session.AccountId);
                result.Result.Message = "Job Order does not exist or is blocked.";
                return result;
            }

            if (jobOrder.AllowSuperVisorModifyWorkTime)
            {
                var _candidateService = EngineContext.Current.Resolve<ICandidateService>();
                var candidate = _candidateService.GetCandidateByGuid(Guid.Parse(workTime.CandidateGuid));
                if (candidate == null)
                {
                    WcfLogger.LogError("Failed to add the new time sheet", "Candidate does not exist!", _session.AccountId);
                    result.Result.Message = "Candidate does not exist.";
                    return result;
                }

                try
                {
                    CandidateWorkTime newWorkTime = new CandidateWorkTime();
                    newWorkTime.CandidateId =candidate.Id;
                    newWorkTime.JobOrderId = jobOrder.Id;
                    newWorkTime.JobStartDateTime = workTime.WorkDate;
                    newWorkTime.NetWorkTimeInHours = workTime.NetWorkTimeInHours;
                    newWorkTime.Note = workTime.Note;

                    var _workTimeService = EngineContext.Current.Resolve<IWorkTimeService>();
                    int newId = _workTimeService.SaveManualCandidateWorkTime(newWorkTime, WorkTimeSource.Manual);

                    // return the updated time sheet
                    if (newId > 0)
                    {
                        TimeSheetBL _bl = new TimeSheetBL();
                        result.TimeSheet = _bl.GetTimeSheetById(newId);
                    }

                    result.Result.Success = true;
                    return result;
                }
                catch (Exception ex)
                {
                    WcfLogger.LogError("SaveManualCandidateWorkTime():",ex.Message,_session.AccountId);
                    result.Result.Message = "Unexpected error!";
                    return result;
                }
            }
            else
            {
                WcfLogger.LogError(String.Format("Job Order {0} does not allow to add or modify worktimes.", jobOrder.Id),"",_session.AccountId);
                result.Result.Message = "Job Order is read-only.";
                return result;
            }
        }

        public PagedListResult<DailyAttendance> GetAllDailyAttendanceList(string sessionId, DateTime refDate, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue)
        {
            PagedListResult<DailyAttendance> result = new PagedListResult<DailyAttendance>();
            result.Items = new List<DailyAttendance>();

            if (sessionId == null )
            {
                WcfLogger.LogError("WCF - Error in GetAllDailyAttendanceList()", "Invalid input parameters: null values are received.", null);
                return result;
            }

            var _session = this.ValidateSession(sessionId);
            if (_session == null)
            {
                WcfLogger.LogError("WCF - Error in GetAllDailyAttendanceList()", "Invalid session id or session is expired.", null);
                return result;
            }

            //
            var _workContext = InitilizeWorkContext(_session);

            var _candidateJobOrderService = EngineContext.Current.Resolve<ICandidateJobOrderService>();
            var candidateAttendanceList = _candidateJobOrderService.GetDailyAttendanceList(refDate, DateTime.Now, _workContext.CurrentAccount);
            var bl = new TimeSheetBL();
            result = bl.GetDailyAttendanceFromDB(candidateAttendanceList,pageSize,pageIndex,sortBy,sortOrder,filterBy,filterCondition,filterValue);
            return result;
        }

        public BasicResult SubmitMobilePunch(string sessionId, MobilePunchEntry mobilePunch)
        {
            var result = new BasicResult() { Success = false };

            if (sessionId == null)
            {
                WcfLogger.LogError("WCF - Error in SubmitMobilePunch()", "Invalid input parameters: sessionId is null.", null);
                result.Message = "Invalid session";
                return result;
            }

            var _session = this.ValidateSession(sessionId);
            if (_session == null)
            {
                WcfLogger.LogError("WCF - Error in SubmitMobilePunch()", "Invalid session id or session is expired.", null);
                result.Message = "Invalid session";
                return result;
            }

            var bl = new MobilePunchBL();
            bl.ProcessMobilePunch(_session, mobilePunch, result);

            return result;
        }
    }
}

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Wfm.Core.Domain.Accounts;

namespace WcfServices
{
    public class WcfSession
    {
        public string SessionId { get; set; }
        public int AccountId { get; set; }
        public char SessionStatus { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; }

        public Account UserAccount { get; set; }
    }

    public class WcfSessionManager
    {
        const int ExpirationTimeInMinutes = 30; 

        #region Methods
        public WcfSession CreateSession(Account account)
        {
            SessionIDManager manager = new SessionIDManager();
            string newSessionId = manager.CreateSessionID(HttpContext.Current);
            
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1, /*version*/
                account.Username,
                now,
                now.Add(new TimeSpan(0,ExpirationTimeInMinutes,0)),
                false, //createPersistentCookie,
                newSessionId, // or new GUID
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            if (encryptedTicket.Length > 250)
               encryptedTicket = encryptedTicket.Substring(0,250);


            // Save the session
            string IpAddress = null;
            var props = OperationContext.Current.IncomingMessageProperties;
            var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (endpointProperty != null)
            {
                IpAddress = endpointProperty.Address;
            }


            const string query = @"INSERT INTO [Session] ([SessionId] ,[AccountId] ,[IsClientAccount] ,[CompanyId] ,[SessionStatus] ,[LoginTime] ,[LogoutTime] ,[LastActivityTime], [IpAddress])
                                   VALUES
                                   (@SessionId, @AccountId, @IsClientAccount, @CompanyId, 'A', GetUtcDate(), null, GetUtcDate(), @IpAddress)";
            Database database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "SessionId", DbType.String, encryptedTicket);
                database.AddInParameter(command, "AccountId", DbType.Int32, account.Id);
                database.AddInParameter(command, "IsClientAccount", DbType.Boolean, account.IsClientAccount);
                database.AddInParameter(command, "CompanyId", DbType.Int32, account.CompanyId);
                database.AddInParameter(command, "IpAddress", DbType.String, IpAddress);
                
                database.ExecuteNonQuery(command);
            }

            WcfSession result = new WcfSession() { AccountId = account.Id, SessionId = encryptedTicket, SessionStatus = 'A' };
            result.UserAccount = new Account()
            {
                CompanyId = account.CompanyId,
                IsClientAccount = account.IsClientAccount,
                FirstName = account.FirstName,
                LastName = account.LastName,
                FranchiseId = account.FranchiseId,
                CompanyDepartmentId = account.CompanyDepartmentId,
                CompanyLocationId = account.CompanyLocationId,
                Id = account.Id,
                LastIpAddress = IpAddress,
                Email = account.Email,
                IsActive = true,
                IsDeleted = false
            };

            return result; 
        }

        public WcfSession ValidateSession(string sessionId)
        {
           // var ticket = FormsAuthentication.Decrypt(sessionId);

            WcfSession result = null;
           // Database db = DatabaseFactory.CreateDatabase();
            Database database = new SqlDatabase(Common.GetConnectionString());
            
            using (DbCommand command = database.GetSqlStringCommand(@"
                    Select AccountId, LastActivityTime, a.CompanyId, a.IsClientAccount, a.FirstName, a.LastName, a.FranchiseId, a.CompanyLocationId, a.CompanyDepartmentId, a.Email
                    from session s
                      inner join Account a on s.AccountId = a.Id and a.IsActive = 1 and a.IsDeleted = 0 
                    where SessionStatus = 'A' and SessionId = @sessionId"))
            {
                database.AddInParameter(command, "SessionId", DbType.String, sessionId);

                using (var objReader = database.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
                {
                    while (objReader.Read()) // (objReader.ReadAsync())
                    {
                        //Check the lastactivity time to make sure it is not expired
                        if (Convert.ToDateTime(objReader["LastActivityTime"]) >= DateTime.UtcNow.AddMinutes(-ExpirationTimeInMinutes) )
                        {
                            result = new WcfSession();
                            result.UserAccount = new Account();

                            result.AccountId = Convert.ToInt32(objReader["AccountId"]);
                            result.UserAccount.CompanyId = Convert.ToInt32(objReader["CompanyId"]);
                            result.UserAccount.IsClientAccount = Convert.ToBoolean(objReader["IsClientAccount"]);
                            result.UserAccount.FirstName = Convert.ToString(objReader["FirstName"]);
                            result.UserAccount.LastName = Convert.ToString(objReader["LastName"]);
                            result.UserAccount.FranchiseId = Convert.ToInt32(objReader["FranchiseId"]);
                            result.UserAccount.CompanyDepartmentId = Convert.ToInt32(objReader["CompanyDepartmentId"]);
                            result.UserAccount.CompanyLocationId = Convert.ToInt32(objReader["CompanyLocationId"]);
                            result.UserAccount.Id = result.AccountId;
                            result.UserAccount.IsActive = true;
                            result.UserAccount.Email = Convert.ToString(objReader["Email"]);

                            result.SessionId = sessionId;
                            result.SessionStatus = 'A';

                            // update the lastactivity time
                            command.CommandText = @"Update Session Set LastActivityTime = GetUtcDate() where SessionStatus = 'A' and SessionId = @sessionId";
                            database.ExecuteNonQuery(command);
                        }
                        else
                        {
                            // update session status
                            command.CommandText = @"Update Session Set SessionStatus = 'X' where SessionStatus = 'A' and SessionId = @sessionId";
                            database.ExecuteNonQuery(command);
                        }

                        break;
                    }
                }
            }

            return result;
        }

        public void CloseSession(string sessionId, bool isLogOut)
        {
            Database database = new SqlDatabase(Common.GetConnectionString());

            string cmd;
            if (isLogOut)
                cmd = @"Update Session Set SessionStatus = 'C', LogOutTime = GetUTCDate() where SessionStatus = 'A' and SessionId = @sessionId";
            else
                cmd = @"Update Session Set SessionStatus = 'C'  where SessionStatus = 'A' and SessionId = @sessionId";

            using (DbCommand command = database.GetSqlStringCommand(cmd))
            {
                database.AddInParameter(command, "SessionId", DbType.String, sessionId);
                database.ExecuteNonQuery(command);
            }
        }

        public WcfSession RefreshSession(string userName, string sessionId, out bool isPasswordExpired, out bool isDelegate)
        {
            WcfSession result = null;
            isPasswordExpired = false;
            isDelegate = false;

            Database database = new SqlDatabase(Common.GetConnectionString());

            // First, check if the last session id that we issued to the user matches the input session 
            using (DbCommand command = database.GetSqlStringCommand(@"
                    Select Top 1 AccountId, LastActivityTime, a.CompanyId, a.IsClientAccount, a.FranchiseId, SessionId,
                                a.FirstName, a.LastName, a.CompanyLocationId, a.CompanyDepartmentId, a.Email
                    from session s
                      inner join Account a on s.AccountId = a.Id and a.IsActive = 1 and a.IsDeleted = 0 
                    where SessionStatus <> 'C' and a.UserName = @userName 
                    order by LoginTime desc
                    "))
            {
                database.AddInParameter(command, "userName", DbType.String, userName);

                using (var objReader = database.ExecuteReader(command)) 
                {
                    if (objReader.Read()) // (objReader.ReadAsync())
                    {
                        if (Convert.ToString(objReader["SessionId"]) == sessionId)
                        {
                            // If the input session id is valid, create a new session Id
                            Login _loginBL = new Login();
                            Account account = _loginBL.GetAccountByUsername(userName);
                            if (account != null)
                            {
                                result = this.CreateSession(account);
                                this.CloseSession(sessionId, false); //Close the previous session

                                _loginBL.UpdateLastLogin(account.Id);
                                WcfLogger.InsertAccessLog(userName, true, "User logged in", account);
                            }
                        }
                        else
                        {
                            WcfLogger.LogInfo("RefreshSession(): Invalid Session Id.", 
                                              String.Format("Session is closed or it is not the last sessionId that was assigned to the user {0}.", userName),
                                              null);
                        }

                    }
                }
            }

            return result;
        }
        #endregion

    }
}
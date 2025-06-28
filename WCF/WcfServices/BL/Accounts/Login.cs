using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Policies;
using Wfm.Core.Domain.Security;
using Wfm.Services.Security;

namespace WcfServices
{
    public class Login
    {

        #region Login
//        public void GetCurrentUserAccountInfo(WcfSession session)
//        {
           
//            _workContext.CurrentAccount = session.UserAccount;

//            using (DbCommand command = WfmDataBase.GetSqlStringCommand(@"
//                    select ar.IsClientRole, ar.IsVendorRole, ar.SystemName, AccountRole_Id
//                    from Account_AccountRole_Mapping  arm
//                        inner join AccountRole ar on arm.AccountRole_Id = ar.Id and ar.IsActive = 1
//                    where arm.Account_id = @id"))
//            {
//                WfmDataBase.AddInParameter(command, "Id", DbType.String, session.AccountId);
//                WfmDataBase.AddInParameter(command, "accountRoleId", DbType.String, 0); // the value will change in the loop

//                using (var objReader = WfmDataBase.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
//                {
//                    while (objReader.Read()) // (objReader.ReadAsync())
//                    {
//                        var _accountRole = new AccountRole();


//                        _accountRole.IsActive = true;
//                        _accountRole.Id = Convert.ToInt32(objReader["AccountRole_Id"]);
//                        _accountRole.IsClientRole = Convert.ToBoolean(objReader["IsClientRole"]);
//                        _accountRole.IsVendorRole = Convert.ToBoolean(objReader["IsVendorRole"]);
//                        _accountRole.SystemName = Convert.ToString(objReader["SystemName"]);


//                        // Get permissions
//                        command.CommandText = @"Select Name, SystemName,Category From PermissionRecord_Role_Mapping  prm
//                                                 inner join PermissionRecord pr on prm.PermissionRecord_Id = pr.Id
//                                                Where AccountRole_Id = @accountRoleId";
//                        WfmDataBase.SetParameterValue(command, "accountRoleId", _accountRole.Id);

//                        using (var objReader2 = WfmDataBase.ExecuteReader(command)) //command.ExecuteReaderAsync().GetAwaiter().GetResult())
//                        {
//                            while (objReader2.Read()) // (objReader.ReadAsync())
//                            {
//                                var _permission = new PermissionRecord();
//                                _permission.Name = Convert.ToString(objReader2["Name"]);
//                                _permission.SystemName = Convert.ToString(objReader2["SystemName"]);
//                                _permission.Category = Convert.ToString(objReader2["Category"]);

//                                if (_permission.SystemName == "CompanyAdministrators")
//                                    _workContext.IsCompanyAdministrator = true;

//                                _accountRole.PermissionRecords.Add(_permission);
//                            }
//                        }

//                        session.UserAccount.AccountRoles.Add(_accountRole);
//                        break;
//                    }
//                }
//            }
//            //_workContext.IsAdmin = false;
//            //_workContext.OriginalAccountIfImpersonate = null;
//            return _workContext;
//        }
        public Account GetAccountByUsername(string userName)
        {
            Account result = null;

            const string query = @"Select Account.Id, UserName, Email, IsClientAccount, PasswordSalt, IsSystemAccount, IsLimitedToFranchises,
                                          Password, PasswordFormatId, LastPasswordUpdateDate, LastPasswordUpdateDate, FranchiseId, CompanyId,
                                          FirstName, LastName, CompanyLocationId, CompanyDepartmentId
                                   From Account 
                                   Where UserName =@userName and Account.IsActive = 1 and Account.IsDeleted = 0
                                  ";

            try
            {
                //SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = WfmService.WfmDataBase.GetSqlStringCommand(query))
                {
                    WfmService.WfmDataBase.AddInParameter(command, "userName", DbType.String, userName);

                    using (var objReader = WfmService.WfmDataBase.ExecuteReader(command))
                    {
                        while (objReader.Read())
                        {
                            result = new Account();
                            result.Id = Convert.ToInt32(objReader["Id"]);
                            result.CompanyId = Convert.ToInt32(objReader["CompanyId"]);
                            result.FranchiseId = Convert.ToInt32(objReader["FranchiseId"]);
                            result.Username = objReader["UserName"].ToString();
                            result.FirstName = objReader["FirstName"].ToString();
                            result.LastName = objReader["LastName"].ToString();
                            result.Email = objReader["Email"].ToString();
                            result.Password = objReader["Password"].ToString();
                            result.PasswordFormatId = Convert.ToInt32(objReader["PasswordFormatId"]);
                            result.PasswordSalt = objReader["PasswordSalt"].ToString();
                            result.IsClientAccount = Convert.ToBoolean(objReader["IsClientAccount"]);
                            result.IsSystemAccount = Convert.ToBoolean(objReader["IsSystemAccount"]);
                            result.IsLimitedToFranchises = Convert.ToBoolean(objReader["IsLimitedToFranchises"]);
                            result.LastPasswordUpdateDate = Convert.ToDateTime(objReader["LastPasswordUpdateDate"]);
                            result.CompanyLocationId = Convert.ToInt32(objReader["CompanyLocationId"]);
                            result.CompanyDepartmentId = Convert.ToInt32(objReader["CompanyDepartmentId"]);
                            break;
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
        public PasswordPolicy GetPasswordPolicyByAccountType(string accountType)
        {
            PasswordPolicy result = null;

            const string query = @"Select PP.PasswordLifeTime    
                                   From PasswordPolicy PP join AccountType_PasswordPolicy_Mapping APM on PP.Id=APM.PasswordPolicyId
                                   Where APM.AccountType =@accountType";

            try
            {
                SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = database.GetSqlStringCommand(query))
                {
                    database.AddInParameter(command, "accountType", DbType.String, accountType);

                    using (var objReader = database.ExecuteReader(command))
                    {
                        while (objReader.Read())
                        {
                            result = new PasswordPolicy();
                            result.PasswordLifeTime = Convert.ToInt32(objReader["PasswordLifeTime"]);
                            break;
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

        private bool CheckIfSelectedAsDelegate(int accounId)
        {
            bool result = false;
            var refDate = DateTime.Today;
            const string query = @"Select Count(1)     
                                   From AccountDelegate
                                   Where DelegateAccountId =@accounId and @refDate Between StartDate and EndDate";

            try
            {
                SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = database.GetSqlStringCommand(query))
                {
                    database.AddInParameter(command, "accounId", DbType.Int32, accounId);
                    database.AddInParameter(command, "refDate", DbType.Date, refDate);
                    if ((int)database.ExecuteScalar(command) > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WcfLogger.LogError(ex.Message, ex.StackTrace, null);
            }

            return result;
        }

        public void UpdateLastLogin(int accountId)
        {
            string IpAddress = null;
            var props = OperationContext.Current.IncomingMessageProperties;
            var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (endpointProperty != null)
            {
                IpAddress = endpointProperty.Address;
            }
            const string query = @"Update account Set LastIpAddress=@IpAddress,LastLoginDateUtc=GetUtcDate() Where Id= @accountId";
            Database database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "IpAddress", DbType.String, IpAddress);
                database.AddInParameter(command, "accountId", DbType.Int32, accountId);
                database.ExecuteNonQuery(command);
            }
        }

        public bool ValidateAccountLogin(string username, string password, out bool isPasswordExpired, out bool isDelegate, out WcfSession session)
        {
            bool isValid = false;
            isPasswordExpired = false;
            isDelegate = false;
            session = null;

            Account account = GetAccountByUsername(username);
            if (account == null)
            {
                WcfLogger.InsertAccessLog(username, false, "Entered wrong user name", account);
                return isValid;
            }

            Common _common = new Common();
            var securitySettings = _common.LoadSettings<SecuritySettings>().GetAwaiter().GetResult();
            var encryptionService = new EncryptionService(securitySettings);

            if (account.IsClientAccount)
            {
                var accountSettings = _common.LoadSettings<AccountSettings>().GetAwaiter().GetResult();
                string pwd = encryptionService.ConvertPassword(account.PasswordFormat, password, account.PasswordSalt);

                isValid = pwd == account.Password;
                if (isValid)
                {
                    PasswordPolicy policy = new PasswordPolicy();
                    policy = this.GetPasswordPolicyByAccountType("Client");
                    if (policy.PasswordLifeTime != 0)
                    {
                        if (account.LastPasswordUpdateDate.AddDays(policy.PasswordLifeTime) <= DateTime.UtcNow)
                        {
                            WcfLogger.InsertAccessLog(username, true, "Password expired", account);
                            isPasswordExpired = true;
                        }
                    }

                    if (CheckIfSelectedAsDelegate(account.Id))
                    {
                        isDelegate = true;
                    }

                    var _sessionManager = new WcfSessionManager();
                    session = _sessionManager.CreateSession(account);

                    UpdateLastLogin(account.Id);
                    WcfLogger.InsertAccessLog(username, true, "User logged in", account);

                    return isValid;
                }
            }

            WcfLogger.InsertAccessLog(username, false, "Entered wrong credentials", account);
            return isValid;
        }

        #endregion
    }
}
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;

namespace WcfServices
{
    public class WcfLogger
    {
        public static void LogError(string ShortMessage, string FullMessage, int? AccountId)
        {
            WriteToLog(40, ShortMessage, FullMessage, AccountId);
        }

        public static void LogInfo(string ShortMessage, string FullMessage, int? AccountId)
        {
            WriteToLog(30, ShortMessage, FullMessage, AccountId);
        }

        private static void WriteToLog(int LogLevelId, string ShortMessage, string FullMessage, int? AccountId)
        {
            string IpAddress = null;
            string ServiceName = null;

            var props = OperationContext.Current.IncomingMessageProperties;
            var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (endpointProperty != null)
            {
                IpAddress = endpointProperty.Address;
            }

            var requestProperty = props[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            if (requestProperty != null && requestProperty.Headers != null && requestProperty.Headers.HasKeys() && requestProperty.Headers["SOAPAction"] != null)
            {
                ServiceName = requestProperty.Headers["SOAPAction"].ToString();
                FullMessage = String.Concat(FullMessage, "\n", requestProperty.Headers.ToString());
            }

            string UserAgent = WebOperationContext.Current.IncomingRequest.Headers["User-Agent"];

            const string query = @"INSERT INTO [Log]
            (LogLevelId, ShortMessage, FullMessage, IpAddress, AccountId, PageUrl, CreatedOnUtc, UpdatedOnUtc, UserAgent)
            VALUES
            (@LogLevelId, @ShortMessage, @FullMessage, @IpAddress, @AccountId, @PageUrl,  GetUtcDate(), GetUtcDate(), @UserAgent) ";

            Database database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "LogLevelId", DbType.String, LogLevelId);
                database.AddInParameter(command, "ShortMessage", DbType.String, ShortMessage);
                database.AddInParameter(command, "FullMessage", DbType.String, FullMessage);
                database.AddInParameter(command, "IpAddress", DbType.String, IpAddress);
                database.AddInParameter(command, "AccountId", DbType.Int32, AccountId);
                database.AddInParameter(command, "PageUrl", DbType.String, ServiceName);
                database.AddInParameter(command, "UserAgent", DbType.String, UserAgent);

                database.ExecuteNonQuery(command);
            }

        }

        public static void InsertAccessLog(string username, bool isSuccessful, string comment, Account account)
        {
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);
            string ipAddress = null;
            var props = OperationContext.Current.IncomingMessageProperties;
            var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (endpointProperty != null)
            {
                ipAddress = endpointProperty.Address;
            }
            string userAgent = WebOperationContext.Current.IncomingRequest.Headers["User-Agent"]; 

            const string query = @"INSERT INTO [AccessLog]
            (AccountId, Username, IsSuccessful, IpAdress, FranchiseId, Description, CreatedOnUtc, UpdatedOnUtc, UserAgent)
            VALUES
            (@AccountId, @Username, @IsSuccessful, @IpAddress, @FranchiseId, @Description,  GetUtcDate(), GetUtcDate(), @UserAgent) ";

            Database database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "AccountId", DbType.Int32, account == null ? 0 : account.Id);
                database.AddInParameter(command, "Username", DbType.String, username);
                database.AddInParameter(command, "IsSuccessful", DbType.Boolean, isSuccessful);
                database.AddInParameter(command, "IpAddress", DbType.String, ipAddress);
                database.AddInParameter(command, "FranchiseId", DbType.Int32, account == null ? 0 : account.FranchiseId);
                database.AddInParameter(command, "Description", DbType.String, comment);
                database.AddInParameter(command, "UserAgent", DbType.String, userAgent);
                database.ExecuteNonQuery(command);
            }
        }
    }
}
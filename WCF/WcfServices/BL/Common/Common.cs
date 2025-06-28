using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Configuration;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Security;
using Wfm.Core.Infrastructure;
using Wfm.Services.Security;

namespace WcfServices
{
    internal class Common
    {
        const int CACHE_SLIDING_EXPIRY_IN_MINUTES = 5;
        static ObjectCache cache = MemoryCache.Default;

        public static string GetConnectionString()
        {
            string dbConnection = cache["DB_ConnectionStr"] as string;

            if (dbConnection == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(CACHE_SLIDING_EXPIRY_IN_MINUTES) };

                var dataSettingsManager = new DataSettingsManager();
                var dataProviderSettings = dataSettingsManager.LoadSettings();
                dbConnection = dataProviderSettings.DataConnectionString;

                cache.Set("DB_ConnectionStr", dbConnection, policy);
            }

            return dbConnection;
        }

        public static IEngine GetWfmEngine()
        {
            return EngineContext.Current;
        }

        public async Task<T> LoadSettings<T>() where T : ISettings, new()
        {
            T result = new T();

            var key = String.Concat(typeof(T).Name, ".");
            if (cache[key] == null)
            {
                string query = String.Format(@"Select REPLACE(Name, '{0}', '') as PropertyName, Value from Setting
                                           Where Name like '{0}%' ", key);

                SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = database.GetSqlStringCommand(query))
                {
                    using (var objReader = await Task<IDataReader>.Factory
                                                                  .FromAsync<DbCommand>(database.BeginExecuteReader, database.EndExecuteReader, command, null))
                    {
                        while (objReader.Read())
                        {
                            PropertyInfo propertyInfo = result.GetType().GetProperty(objReader["PropertyName"].ToString());
                            if (propertyInfo != null)
                            {
                                switch (propertyInfo.PropertyType.ToString())
                                {
                                    case "System.Int32": propertyInfo.SetValue(result, Convert.ToInt32(objReader["Value"]), null); break;
                                    case "System.String": propertyInfo.SetValue(result, Convert.ToString(objReader["Value"]), null); break;
                                }
                            }

                        }
                    }
                }

                // Add the value to the cache
                CacheItemPolicy policy = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(CACHE_SLIDING_EXPIRY_IN_MINUTES) };
                cache.Set(key, result, policy);
            }
            else
                result = (T) cache[key];

            return result;
        }

        public static bool UserHasPermission(PermissionRecord _permission)
        {
            var _permissionService = EngineContext.Current.Resolve<IPermissionService>();
            return _permissionService.Authorize(_permission);
        }

        public Account ValidateUserAccount(string guid, int franchiseId, string permissionName)
        {
            if (String.IsNullOrWhiteSpace(guid) || franchiseId == 0 || String.IsNullOrWhiteSpace(permissionName))
            {
                WcfLogger.LogError("Invalid WCF user credentials deteced.", String.Format("ValidateUserAccount(): guid={0} , franchiseId={1}, permissionName={2}", guid, franchiseId, permissionName), null);
                return null;
            }


            Guid tmp;
            if (String.IsNullOrWhiteSpace(guid) || !Guid.TryParse(guid, out tmp))
            {
                WcfLogger.LogError("Invalid user account GUID deteced.", String.Format("ValidateUserAccount(): guid={0} , franchiseId={1}, permissionName={2}", guid, franchiseId, permissionName), null);
                return null;
            }

            Account result = null;

            const string query = @"Select Account.Id, UserName, Email, IsClientAccount, IsSystemAccount, IsLimitedToFranchises
                                   From Account 
                                     inner join Account_AccountRole_Mapping arm on Account.Id = arm.Account_Id
                                     inner join PermissionRecord_Role_Mapping prm on arm.AccountRole_Id = prm.AccountRole_Id
                                     inner join PermissionRecord pr on prm.PermissionRecord_Id = pr.Id
                                   Where AccountGuid = cast(@accountGuid as uniqueidentifier)  and Account.IsActive = 1 and Account.IsDeleted = 0 and pr.SystemName = @permissionName and Account.FranchiseId = @franchiseId
                                  ";

            try
            {
                SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
                using (DbCommand command = database.GetSqlStringCommand(query))
                {
                    database.AddInParameter(command, "accountGuid", DbType.String, guid);
                    database.AddInParameter(command, "permissionName", DbType.String, permissionName);
                    database.AddInParameter(command, "franchiseId", DbType.Int32, franchiseId);

                    using (var objReader = database.ExecuteReader(command))
                    {
                        while (objReader.Read())
                        {
                            result = new Account();
                            result.Id = Convert.ToInt32(objReader["Id"]);
                            result.Username = objReader["UserName"].ToString();
                            result.Email = objReader["Email"].ToString();
                            result.FranchiseId = franchiseId;
                            result.IsClientAccount = Convert.ToBoolean(objReader["IsClientAccount"]);
                            result.IsSystemAccount = Convert.ToBoolean(objReader["IsSystemAccount"]);
                            result.IsLimitedToFranchises = Convert.ToBoolean(objReader["IsLimitedToFranchises"]);
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

        public bool AuthenticateWebServiceCredentials(string userName, string password)
        {
            bool result = false;

            try
            {
                if (!String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(password))
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    string svcUserName = appSettings["WebServiceuserName"];
                    string svcPassword = appSettings["WebServicePassword"];
                    if (userName.Equals(svcUserName, StringComparison.OrdinalIgnoreCase) && password == svcPassword)
                        result = true;
                    else
                        WcfLogger.LogError("Invalid WCF user credentials deteced.", String.Format("AuthenticateWebServiceCredentials(): userName={0}, password={1}", userName, password), null);
                }
                else
                    WcfLogger.LogInfo("Missing configuration for WCF user or invalid service call.", String.Format("AuthenticateWebServiceCredentials(): userName={0}, password={1}", userName, password), null);
            }
            catch (Exception ex)
            {
                WcfLogger.LogError(ex.Message, ex.StackTrace, null);
            }

            return result;
        }

        public static string GetNotNullValue(string input)
        {
            if (input == null)
                return String.Empty;
            else
                return input;
        }

        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            int delta = DayOfWeek.Sunday - dayInWeek.DayOfWeek;
            return dayInWeek.AddDays(delta);
        }

        #region Filter Utility

        private class ColumnInfo
        {
            public string FilterName { get; set; }
            public string SortName { get; set; }
            public string ColumnType { get; set; }
        }

        static Dictionary<string, ColumnInfo> DailyTimeSheet_Columns = new Dictionary<string, ColumnInfo>()
        {
             { "VendorName", new ColumnInfo {FilterName="Franchise.FranchiseName", SortName="VendorName", ColumnType="S"}},
             { "EmployeeNumber", new ColumnInfo {FilterName="CA.EmployeeId", SortName="EmployeeId", ColumnType="S"}},
             { "Location", new ColumnInfo {FilterName="CL.LocationName", SortName="LocationName", ColumnType="S"}},
             { "Department", new ColumnInfo {FilterName="CD.DepartmentName", SortName="DepartmentName", ColumnType="S"}},
             { "FirstName", new ColumnInfo {FilterName="CA.FirstName", SortName="FirstName", ColumnType="S"}},
             { "LastName", new ColumnInfo {FilterName="CA.LastName", SortName="LastName", ColumnType="S"}},
             { "JobOrder", new ColumnInfo {FilterName="CWT.JobOrderId", SortName="JobOrderId", ColumnType="N"}},
             { "JobTitle", new ColumnInfo {FilterName="JO.JobTitle", SortName="JobTitle", ColumnType="S"}},
             { "JobShift", new ColumnInfo {FilterName="Shift.ShiftName", SortName="JobShift", ColumnType="S"}},
             { "ContactName", new ColumnInfo {FilterName="ContactName", SortName="ContactName", ColumnType="S"}},
             { "JobStartDateTime", new ColumnInfo {FilterName="JobStartDateTime", SortName="JobStartDateTime", ColumnType="D"}}
        };

        static Dictionary<string, ColumnInfo> TimeSheetHistory_Columns = new Dictionary<string, ColumnInfo>()
        {
             { "VendorName", new ColumnInfo {FilterName="FranchiseName", SortName="FranchiseName", ColumnType="S"}},
             { "EmployeeNumber", new ColumnInfo {FilterName="EmployeeNumber", SortName="EmployeeNumber", ColumnType="S"}},
             { "Location", new ColumnInfo {FilterName="LocationName", SortName="LocationName", ColumnType="S"}},
             { "Department", new ColumnInfo {FilterName="DepartmentName", SortName="DepartmentName", ColumnType="S"}},
             { "FirstName", new ColumnInfo {FilterName="EmployeeFirstName", SortName="EmployeeFirstName", ColumnType="S"}},
             { "LastName", new ColumnInfo {FilterName="EmployeeLastName", SortName="EmployeeLastName", ColumnType="S"}},
             { "ContactName", new ColumnInfo {FilterName="ContactName", SortName="ContactName", ColumnType="S"}},
             { "JobTitle", new ColumnInfo {FilterName="JobTitle", SortName="JobTitle", ColumnType="S"}},
             { "JobOrder", new ColumnInfo {FilterName="JobOrderId", SortName="JobOrderId", ColumnType="N"}},
             { "ApprovedBy", new ColumnInfo {FilterName="ApprovedBy", SortName="ApprovedBy", ColumnType="S"}}
        };

        static Dictionary<string, ColumnInfo> TimeSheetApproval_Columns = new Dictionary<string, ColumnInfo>()
        {
             { "VendorName", new ColumnInfo {FilterName="Franchise.FranchiseName", SortName="FranchiseName", ColumnType="S"}},
             { "EmployeeNumber", new ColumnInfo {FilterName="CA.EmployeeId", SortName="EmployeeId", ColumnType="S"}},
          //   { "JobOrder", new ColumnInfo {FilterName="CWT.JobOrderId", SortName="JobOrderId", ColumnType="N"}},
             { "Location", new ColumnInfo {FilterName="CL.LocationName", SortName="LocationName", ColumnType="S"}},
             { "Department", new ColumnInfo {FilterName="CD.DepartmentName", SortName="DepartmentName", ColumnType="S"}},
             { "FirstName", new ColumnInfo {FilterName="CA.FirstName", SortName="FirstName", ColumnType="S"}},
             { "LastName", new ColumnInfo {FilterName="CA.LastName", SortName="LastName", ColumnType="S"}},
             { "JobTitle", new ColumnInfo {FilterName="JO.JobTitle", SortName="JobTitle", ColumnType="S"}},
             { "JobShift", new ColumnInfo {FilterName="Shift.ShiftName", SortName="JobShift", ColumnType="S"}},
             { "ContactName", new ColumnInfo {FilterName="Isnull(Account.FirstName,'')+' '+Isnull(Account.LastName,'')", SortName="ContactName", ColumnType="S"}},
             { "JobStartDateTime", new ColumnInfo {FilterName="JobStartDateTime", SortName="JobStartDateTime", ColumnType="D"}}
        };
        static Dictionary<string, ColumnInfo> DailyAttendance_Columns = new Dictionary<string, ColumnInfo>()
        {
             { "EmployeeNumber", new ColumnInfo {FilterName="EmployeeId", SortName="EmployeeId", ColumnType="S"}},
             { "FirstName", new ColumnInfo {FilterName="EmployeeFirstName", SortName="EmployeeFirstName", ColumnType="S"}},
             { "LastName", new ColumnInfo {FilterName="EmployeeLastName", SortName="EmployeeLastName", ColumnType="S"}},
             { "JobTitle", new ColumnInfo {FilterName="JobTitle", SortName="JobTitle", ColumnType="S"}},
             { "Status", new ColumnInfo {FilterName="Status", SortName="Status", ColumnType="S"}},
             { "Location", new ColumnInfo {FilterName="Location", SortName="Location", ColumnType="S"}},
             { "Department", new ColumnInfo {FilterName="Department", SortName="Department", ColumnType="S"}},
             { "JobStartDateTime", new ColumnInfo {FilterName="ShiftStartTime", SortName="ShiftStartTime", ColumnType="D"}},
        };
        private static Dictionary<string, Dictionary<string, ColumnInfo>> COLUMN_ALIASES = new Dictionary<string, Dictionary<string, ColumnInfo>>()
        {
            { "DailyTimeSheet", DailyTimeSheet_Columns},
            { "TimeSheetHistory", TimeSheetHistory_Columns},
            { "TimeSheetApproval", TimeSheetApproval_Columns},
            {"DailyAttendanceList",DailyAttendance_Columns}
        };

        private static string GetDefaultSortStatement(string moduleName, string sortOrder)
        {
            switch (moduleName)
            {
                case "DailyTimeSheet": if (String.IsNullOrWhiteSpace(sortOrder)) sortOrder = "desc"; return String.Concat("JobStartDateTime", " ", sortOrder);
                case "TimeSheetApproval": if (String.IsNullOrWhiteSpace(sortOrder)) sortOrder = "desc"; return String.Concat("JobStartDateTime", " ", sortOrder);
                default: return " ";
            }
        }

        public static string GetSortStatement(string moduleName, string columnName, string sortOrder)
        {
            StringBuilder result = new StringBuilder("");

            if (!String.IsNullOrWhiteSpace(sortOrder) && !sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase) && !sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                sortOrder = "";

            // validate the input. If invalid, just return an empty string
            if (String.IsNullOrWhiteSpace(moduleName) || !COLUMN_ALIASES.ContainsKey(moduleName))
                return "";

            // Shoud we use the defaults?
            if (String.IsNullOrWhiteSpace(columnName) || !COLUMN_ALIASES[moduleName].ContainsKey(columnName))
                return GetDefaultSortStatement(moduleName, sortOrder);


            var sortBy = COLUMN_ALIASES[moduleName][columnName];

            result = result.Append(String.Concat(COLUMN_ALIASES[moduleName][columnName].SortName, ' ', sortOrder, ' '));

            return result.ToString();
        }

        //private static Expression GetMemberExpression(Expression expression, out ParameterExpression parameterExpression)
        //{
        //    parameterExpression = null;
        //    if (expression is MemberExpression)
        //    {
        //        var memberExpression = expression as MemberExpression;
        //        while (!(memberExpression.Expression is ParameterExpression))
        //            memberExpression = memberExpression.Expression as MemberExpression;
        //        parameterExpression = memberExpression.Expression as ParameterExpression;
        //        return expression as MemberExpression;
        //    }
        //    if (expression is MethodCallExpression)
        //    {
        //        var methodCallExpression = expression as MethodCallExpression;
        //        parameterExpression = methodCallExpression.Object as ParameterExpression;
        //        return methodCallExpression;
        //    }
        //    return null;
        //}


        /// <summary>
        /// Get Equal Binary Expression for Equal relational operator
        /// </summary>
        /// <param name="propertyAccess"></param>
        /// <param name="columnValue"></param>
        /// <returns></returns>
        static BinaryExpression GetEqualBinaryExpression(MemberExpression propertyAccess, string columnValue, string colType)
        {
            switch (colType)
            {
                case "S":
                    return Expression.Equal(GetCompareExpression(propertyAccess, columnValue), Expression.Constant(0));
                default:
                    return Expression.Equal(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
            }
        }

        static BinaryExpression GetGreaterThanOrEqualExp(MemberExpression propertyAccess, string columnValue, string colType)
        {
            switch (colType)
            {
                case "S":
                    return Expression.GreaterThanOrEqual(GetCompareExpression(propertyAccess, columnValue), Expression.Constant(0));
                default:
                    return Expression.GreaterThanOrEqual(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
            }
        }

        static Expression GetGreaterThanExp(MemberExpression propertyAccess, string columnValue, string colType)
        {
            switch (colType)
            {
                case "S":
                    return Expression.GreaterThan(GetCompareExpression(propertyAccess, columnValue), Expression.Constant(0));
                default:
                    return Expression.GreaterThan(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
            }
        }

        //public Expression<Func<T, bool>> constructaPredicate<T>(ExpressionType operation, string fieldName, string value)
        //{
        //    var type = typeof(T);
        //    var parameter = Expression.Parameter(type);
        //    var member = Expression.PropertyOrField(parameter, fieldName);

        //    Expression comparison = null;

        //    //if (value.Type == typeof(string))
        //    //{
        //        if (operation == ExpressionType.GreaterThanOrEqual ||
        //            operation == ExpressionType.GreaterThan ||
        //            operation == ExpressionType.LessThanOrEqual ||
        //            operation == ExpressionType.LessThan)
        //        {
        //            var method = typeof(String).GetMethod("CompareTo", new[] { typeof(string) });
        //            var zero = Expression.Constant(0);

        //            var result = Expression.Call(member, method, value);

        //            comparison = Expression.MakeBinary(operation, result, zero);
        //        }
        //    //}

        //    if (comparison == null)
        //        comparison = Expression.MakeBinary(operation, member, value);

        //    return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        //}


        static BinaryExpression GetLessThanExp(MemberExpression propertyAccess, string columnValue, string colType)
        {
            switch (colType)
            {
                case "S":
                    return Expression.LessThan(GetCompareExpression(propertyAccess, columnValue), Expression.Constant(0));
                default:
                    return Expression.LessThan(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
            }
        }

        static BinaryExpression GetLessThanOrEqualExp(MemberExpression propertyAccess, string columnValue, string colType)
        {
            switch (colType)
            {
                case "S":
                    return Expression.LessThanOrEqual(GetCompareExpression(propertyAccess, columnValue), Expression.Constant(0));
                default:
                    return Expression.LessThanOrEqual(GetLowerCasePropertyAccess(propertyAccess), Expression.Constant(columnValue.ToLower()));
            }
        }

        /// <summary>                              
        /// Get Lower Case Property Access
        /// </summary>
        /// <param name="propertyAccess"></param>
        /// <returns></returns>
        static MethodCallExpression GetLowerCasePropertyAccess(MemberExpression propertyAccess)
        {
            // todo: http://stackoverflow.com/questions/28948920/c-sharp-lambda-null-check-expression-tree
            return Expression.Call(Expression.Call(propertyAccess, "ToString", new Type[0]), typeof(string).GetMethod("ToLower", new Type[0]));
        }

        static MethodCallExpression GetCompareExpression(MemberExpression propertyAccess, string columnValue)
        {
            MethodCallExpression methodCallExpression = Expression.Call(GetLowerCasePropertyAccess(propertyAccess), CompareMethod, Expression.Constant(columnValue.ToLower()));
            return methodCallExpression;
        }

        /// <summary>
        /// Get Method Call Expression for Like/Contains relational operator
        /// </summary>
        /// <param name="propertyAccess"></param>
        /// <param name="columnValue"></param>
        /// <returns></returns>
        static MethodCallExpression GetLikeExpression(MemberExpression propertyAccess, string columnValue)
        {
            MethodCallExpression methodCallExpression = Expression.Call(GetLowerCasePropertyAccess(propertyAccess), ContainsMethod, Expression.Constant(columnValue.ToLower()));
            return methodCallExpression;
        }


        #region Readonly Fields

        private static readonly MethodInfo CompareMethod = typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) });

        private static readonly MethodInfo ContainsMethod = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });

        private static readonly MethodInfo StartsWithMethod = typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) });

        private static readonly MethodInfo EndsWithMethod = typeof(String).GetMethod("EndsWith", new Type[] { typeof(String) });

        #endregion 


        public static Func<T, bool> GetLinqFilterStatement<T>(string moduleName, string columnName, string filterCondition, string filterValue)
        {
            // validate the input. If invalid, just return an empty string
            if (String.IsNullOrWhiteSpace(columnName) || String.IsNullOrWhiteSpace(filterCondition) || String.IsNullOrWhiteSpace(filterValue) || String.IsNullOrWhiteSpace(moduleName) ||
                !COLUMN_ALIASES.ContainsKey(moduleName) || !COLUMN_ALIASES[moduleName].ContainsKey(columnName))
                return null;

            var filterOn = COLUMN_ALIASES[moduleName][columnName];

            Type expType;
            switch (filterOn.ColumnType)
            {
                case "S": expType = typeof(string); break;
                case "D": expType = typeof(DateTime); break;
                case "N": expType = typeof(int); break;
                default: expType = typeof(string); break;
            }

            ParameterExpression expParam = Expression.Parameter(typeof(T), "dataContainer");

            var property = typeof(T).GetProperty(filterOn.FilterName); // Assign filter column name 
            //var propertyType = property.PropertyType;

            var propertyAccess = Expression.MakeMemberAccess(expParam, property); 

           // ConstantExpression expVal = Expression.Constant(filterValue, expType);
            Expression exp;

            switch (filterCondition)
            {
                case "like":
                    exp = GetLikeExpression(propertyAccess, filterValue);
                    break;
                case "eq":  exp = GetEqualBinaryExpression(propertyAccess, filterValue, filterOn.ColumnType); break;
                case "gt":  exp = GetGreaterThanExp(propertyAccess, filterValue, filterOn.ColumnType); break;
                case "lt":  exp = GetLessThanExp(propertyAccess, filterValue, filterOn.ColumnType); break;
                case "lteq": exp = GetLessThanOrEqualExp(propertyAccess, filterValue, filterOn.ColumnType); break;
                case "gteq": exp = GetGreaterThanOrEqualExp(propertyAccess, filterValue, filterOn.ColumnType); break;
                default :
                    return null;
            }

            var pred = Expression.Lambda<Func<T, bool>>(exp, new[] { expParam });
            Func<T, bool> compiled = pred.Compile();
            return compiled; 
        }

        public static string GetFilterStatement(string moduleName, string columnName, string filterCondition, string filterValue)
        {
            StringBuilder result = new StringBuilder("");

            // validate the input. If invalid, just return an empty string
            if (String.IsNullOrWhiteSpace(columnName) || String.IsNullOrWhiteSpace(filterCondition) || String.IsNullOrWhiteSpace(filterValue) || String.IsNullOrWhiteSpace(moduleName) ||
                !COLUMN_ALIASES.ContainsKey(moduleName) || !COLUMN_ALIASES[moduleName].ContainsKey(columnName))
                return "";

            var filterOn = COLUMN_ALIASES[moduleName][columnName];

            string filterOperator;

            filterCondition = filterCondition.Trim().ToLower();
            
            if (filterCondition == "like")
            {
                    filterOperator = "like";
                    if (filterOn.ColumnType != "D")
                        filterValue = String.Concat("'%", filterValue, "%'");
                    else
                        return "";
            }
            else
            {
                if (filterCondition == "eq")    
                 filterOperator = "=";
                else if (filterCondition == "gt")
                    filterOperator = ">";
                else if (filterCondition == "lt")
                    filterOperator = "<";
                else if (filterCondition == "lteq")
                    filterOperator = "<=";
                else if (filterCondition == "gteq")
                    filterOperator = ">=";
                else return "";

                if (filterOn.ColumnType == "S" || filterOn.ColumnType == "D")
                    filterValue = String.Concat("'", filterValue, "'");
            }

            result = result.Append(String.Concat(" and ", COLUMN_ALIASES[moduleName][columnName].FilterName, ' ', filterOperator, ' ', filterValue));

            return result.ToString();
        }

        #endregion
    }
}
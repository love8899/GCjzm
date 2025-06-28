using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Wfm.Core.Domain.TimeSheet;
using System.Linq;
using System.Linq.Dynamic;

namespace WcfServices.BL.TimeSheets
{
    public enum TimeSheetOperation
    {
        Approve,
        Reject,
        Adjust
    }

    public class TimeSheetBL
    {
        public const string SQL_TIME_SHEET_QUERY = @" 
            CWT.Id, CA.CandidateGuid, CWT.Year, CWT.WeekOfYear, CWT.Payroll_BatchId, CA.EmployeeId, CWT.CandidateId, 
            CWT.JobOrderId, Isnull(cwt.note,'') Note
            ,JO.JobTitle, JO.AllowSuperVisorModifyWorkTime, JO.JobOrderGuid, CWT.CompanyId, CWT.CompanyLocationId, Isnull(CL.LocationName,'') LocationName, 
            Isnull(CD.DepartmentName,'') DepartmentName
            ,CA.FirstName, CA.LastName, Shift.ShiftName as JobShift, CWT.JobStartDateTime, CWT.JobEndDateTime, CWT.ClockIn, CWT.ClockOut, CWT.NetWorkTimeInHours
            ,CWT.AdjustmentInMinutes, CWT.CandidateWorkTimeStatusId, CWT.UpdatedOnUtc, CWT.CreatedOnUtc 
            ,Isnull(Account.FirstName,'')+' '+Isnull(Account.LastName,'')  as ContactName , CWT.ClockTimeInHours,
            Franchise.FranchiseName	,'Calculated overtime hours in this period is '+Convert(nvarchar(10), (CWot.OvertimeHours))+' hours' as OvertimeMessage
		    ,Alerts.Message as OtherAlerts 
        From CandidateWorkTime CWT Join Joborder Jo on Jo.Id =CWT.JobOrderId 
            Left Join CompanyLocation CL on CWT.CompanyLocationId=CL.Id 
            Left Join CompanyDepartment CD on CWT.CompanyDepartmentId=CD.Id 
            Join Candidate CA On CWT.CandidateId =CA.Id 
            Left  Join Account on Jo.CompanyContactId=Account.Id
            inner join Franchise on CWT.FranchiseId = Franchise.Id
            Left join CandidateWorkOverTime CWOT on CWOT.CandidateId=CWT.CandidateId And CWOT.Year=CWT.Year AND CWOT.WeekOfYear=CWT.WeekOfYear
			Left join Alerts  on Alerts.CandidateId=CWT.CandidateId And Alerts.Year=CWT.Year AND Alerts.WeekOfYear=CWT.WeekOfYear 
            Left join Shift on CWT.ShiftId = Shift.Id
        ";

        private bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }

        public bool ValidateTimeSheet(CandidateWorkTime WorkTimeRecord, string CandidateGuid, int CompanyId, int UserAccountId, TimeSheetOperation Operation)
        {
            if (WorkTimeRecord == null || !String.Equals(WorkTimeRecord.Candidate.CandidateGuid.ToString(), CandidateGuid, StringComparison.OrdinalIgnoreCase) || WorkTimeRecord.CompanyId != CompanyId)
            {
                WcfLogger.LogError("Access Denied",
                                   String.Concat("WCF - User is trying to approve/reject/edit a timesheet that belongs to wrong candidate or another company.", Environment.StackTrace),
                                   UserAccountId);
                return false;
            }

            if (WorkTimeRecord.Payroll_BatchId.HasValue && WorkTimeRecord.Payroll_BatchId>0)
            {
                WcfLogger.LogError("Access Denied",
                                   String.Concat("WCF - User is trying to approve/reject/edit a timesheet that is locked by Payroll or already paid.", Environment.StackTrace),
                                   UserAccountId);
                return false;
            }

            if (Operation == TimeSheetOperation.Approve)
            {
                if (WorkTimeRecord.NetWorkTimeInMinutes <= 0)
                {
                    WcfLogger.LogError("Cannot approve the selected time sheet",
                                       String.Concat("WCF - Time sheet's NetWorkTimeInMinutes <= 0", Environment.StackTrace),
                                       UserAccountId);
                    return false;
                }

                if (WorkTimeRecord.CandidateWorkTimeStatus == CandidateWorkTimeStatus.Approved)
                {
                    WcfLogger.LogError("Cannot approve the selected time sheet",
                                       String.Concat("WCF - Time sheet is already approved.", Environment.StackTrace),
                                       UserAccountId);
                    return false;
                }

                if (WorkTimeRecord.CandidateWorkTimeStatus == CandidateWorkTimeStatus.Rejected)
                {
                    // check if the time sheet belongs to the current week
                    if (!DatesAreInTheSameWeek(WorkTimeRecord.JobStartDateTime, DateTime.Now))
                    {
                        WcfLogger.LogError("Cannot approve the selected time sheet",
                                           String.Concat("WCF - Time sheet is already rejected.", Environment.StackTrace),
                                           UserAccountId);
                        return false;
                    }
                }
            }
            else if (Operation == TimeSheetOperation.Reject)
            {
                if (WorkTimeRecord.CandidateWorkTimeStatus == CandidateWorkTimeStatus.Rejected)
                {
                    WcfLogger.LogError("Cannot reject the selected time sheet",
                                       String.Concat("WCF - Time sheet is already rejected by another user.", Environment.StackTrace),
                                       UserAccountId);
                    return false;
                }
            }
            else if (Operation == TimeSheetOperation.Adjust)
            {
                if (WorkTimeRecord == null || !String.Equals(WorkTimeRecord.Candidate.CandidateGuid.ToString(), CandidateGuid, StringComparison.OrdinalIgnoreCase) ||
                    WorkTimeRecord.CompanyId != CompanyId || !WorkTimeRecord.JobOrder.AllowSuperVisorModifyWorkTime)
                {
                    WcfLogger.LogError("Access Denied", "WCF - UpdateTimeSheet(): Job order is not allowing the edit for supervisors or user is trying to edit a timesheet that belongs to wrong candidate or another company.", UserAccountId);
                    return false;
                }

                if ((WorkTimeRecord.CandidateWorkTimeStatus == CandidateWorkTimeStatus.Approved || WorkTimeRecord.CandidateWorkTimeStatus == CandidateWorkTimeStatus.Rejected) 
                    && !DatesAreInTheSameWeek(WorkTimeRecord.JobStartDateTime, DateTime.Now))
                {
                    WcfLogger.LogError("Cannot edit the selected time sheet", "WCF - UpdateTimeSheet(): time sheet is already approved or rejected by another user.", UserAccountId);
                    return false;
                }
            }

            return true;
        }

        public TimeSheet ReadOneTimeSheet(IDataReader _reader)
        {
            TimeSheet result = new TimeSheet();

            result.TimeSheetId = Convert.ToInt32(_reader["Id"]);
            result.CandidateGuid = Convert.ToString(_reader["CandidateGuid"]);
            result.Year = Convert.ToInt32(_reader["Year"]);
            result.WeekOfYear = Convert.ToInt32(_reader["WeekOfYear"]);
            if (_reader["Payroll_BatchId"] != DBNull.Value)
                result.Payroll_BatchId = Convert.ToInt32(_reader["Payroll_BatchId"]);
            result.EmployeeFirstName = Convert.ToString(_reader["FirstName"]);
            result.EmployeeLastName = Convert.ToString(_reader["LastName"]);
            result.EmployeeId = Convert.ToString(_reader["EmployeeId"]);
            result.CandidateId = Convert.ToInt32(_reader["CandidateId"]);
            result.JobOrderId = Convert.ToInt32(_reader["JobOrderId"]);
            result.JobOrderGuid = Convert.ToString(_reader["JobOrderGuid"]);
            result.JobTitle = Convert.ToString(_reader["JobTitle"]);
            result.CompanyLocationId = Convert.ToInt32(_reader["CompanyLocationId"]);
            result.LocationName = Convert.ToString(_reader["LocationName"]);
            result.DepartmentName = Convert.ToString(_reader["DepartmentName"]);
            result.ContactName = Convert.ToString(_reader["ContactName"]);
            result.JobShift = Convert.ToString(_reader["JobShift"]);
            result.JobStartDateTime = Convert.ToDateTime(_reader["JobStartDateTime"]);
            result.JobEndDateTime = Convert.ToDateTime(_reader["JobEndDateTime"]);
            if (_reader["ClockIn"] != DBNull.Value)
                result.ClockIn = Convert.ToDateTime(_reader["ClockIn"]);
            if (_reader["ClockOut"] != DBNull.Value)
                result.ClockOut = Convert.ToDateTime(_reader["ClockOut"]);
            result.NetWorkTimeInHours = Convert.ToDecimal(_reader["NetWorkTimeInHours"]);
            result.AdjustmentInMinutes = Convert.ToDecimal(_reader["AdjustmentInMinutes"]);
            result.ClockTimeInHours = Convert.ToDecimal(_reader["ClockTimeInHours"]);
            result.CandidateWorkTimeStatusId = Convert.ToInt32(_reader["CandidateWorkTimeStatusId"]);
            if (_reader["UpdatedOnUtc"] != DBNull.Value)
                result.UpdatedOnUtc = Convert.ToDateTime(_reader["UpdatedOnUtc"]);
            if (_reader["CreatedOnUtc"] != DBNull.Value)
                result.CreatedOnUtc = Convert.ToDateTime(_reader["CreatedOnUtc"]);
            result.AllowSuperVisorModifyWorkTime = Convert.ToBoolean(_reader["AllowSuperVisorModifyWorkTime"]);
            result.Note = Convert.ToString(_reader["Note"]);
            result.VendorName = Convert.ToString(_reader["FranchiseName"]);

            result.IsCurrentWeek = DateTime.Today.AddDays(7 - (int)(DateTime.Today.DayOfWeek)).Equals( // next Sunday 
                                                          result.JobStartDateTime.Date.AddDays(7 - (int)(result.JobStartDateTime.Date.DayOfWeek)));

            result.OvertimeMessage = Convert.ToString(_reader["OvertimeMessage"]);
            if (string.IsNullOrEmpty(result.OvertimeMessage))
            {
                result.OtherAlerts = Convert.ToString(_reader["OtherAlerts"]);
            }

            return result;
        }

        public TimeSheet GetTimeSheetById(int id)
        {
            if (id == 0)
                return null;

            string query = String.Concat("SELECT ", SQL_TIME_SHEET_QUERY, "Where CWT.Id =@Id ");
            TimeSheet result = null;

            SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "Id", DbType.Int32, id);

                using (var objReader = database.ExecuteReader(command))
                {
                    while (objReader.Read())
                    {
                        result = this.ReadOneTimeSheet(objReader);
                        break;
                    }
                }
            }

            return result;
        }

        public PagedListResult<DailyAttendance> GetDailyAttendanceFromDB(IList<DailyAttendanceList> list, int pageSize, int pageIndex, string sortBy, string sortOrder, string filterBy, string filterCondition, string filterValue)
        {
            int total = list.Count;
            // filtering
            var filter = Common.GetLinqFilterStatement<DailyAttendanceList>("DailyAttendanceList", filterBy, filterCondition, filterValue);
            if (filter != null)
                list = list.Where(filter).ToList();

            // sorting and filtering bu user input
            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "EmployeeId";
            if (String.IsNullOrWhiteSpace(sortOrder))
                sortOrder = "desc";

            string _sortStr = Common.GetSortStatement("DailyAttendanceList", sortBy, sortOrder).Replace(" asc ", "").Replace(" desc ", " descending ");
            if(!String.IsNullOrWhiteSpace(_sortStr))
                list = list.OrderBy(_sortStr).ToList();
            list = list.Skip(pageSize * pageIndex).Take(pageSize).ToList();

            PagedListResult<DailyAttendance> result = new PagedListResult<DailyAttendance>();
            result.TotalCount = total;
            result.Items = new List<DailyAttendance>();
            if (list.Count <= 0)
                return result;
            foreach (var item in list)
            {
                DailyAttendance instance = new DailyAttendance();
                instance.CandidateGuid = item.CandidateGuid;
                instance.CandidateId = item.CandidateId;
                instance.CandidateJobOrderId = item.CandidateJobOrderId;
                instance.Department = item.Department;
                instance.EmployeeFirstName = item.EmployeeFirstName;
                instance.EmployeeId = item.EmployeeId;
                instance.EmployeeLastName = item.EmployeeLastName;
                instance.JobOrderId = item.JobOrderId;
                instance.JobTitle = item.JobTitle;
                instance.JobTitleAndId = item.JobTitleAndId;
                instance.Location = item.Location;
                instance.ShiftEndTime = item.ShiftEndTime;
                instance.ShiftStartTime = item.ShiftStartTime;
                instance.Status = item.Status;
                instance.VendorName = item.VendorName;
                result.Items.Add(instance);
            }
            return result;



        }

    }
}
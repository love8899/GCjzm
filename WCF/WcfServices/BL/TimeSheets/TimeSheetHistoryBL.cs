using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Wfm.Core.Infrastructure;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;


namespace WcfServices.TimeSheets
{
    public class TimeSheetHistoryBL
    {
        public PagedListResult<SimpleEmployeeTimeChartHistory> GetEmployeeTimeSheetHistoryByDate(int accountId, int companyId, DateTime start, DateTime end, 
                                                                                                 int pageSize, int pageIndex, 
                                                                                                 string sortBy, string sortOrder, 
                                                                                                 string filterBy, string filterCondition, string filterValue)
        {
            IEnumerable<EmployeeTimeChartHistory> sheets;

            SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
            using (var dataSet = database.ExecuteDataSet("Timesheet_Weekly", start, end, ((Int32)CandidateWorkTimeStatus.Approved).ToString(), "1", companyId))
            {
                sheets = dataSet.Tables[0].AsEnumerable()
                                        .Select(x => new EmployeeTimeChartHistory
                                        {
                                            EmployeeNumber = x.Field<string>("EmployeeNumber"),
                                            EmployeeId = x.Field<int>("EmployeeId"),
                                            EmployeeFirstName = Common.GetNotNullValue(x.Field<string>("EmployeeFirstName")),
                                            EmployeeLastName = x.Field<string>("EmployeeLastName"),
                                            FranchiseId = x.Field<int>("FranchiseId"),
                                            FranchiseName = x.Field<string>("FranchiseName"),
                                            CompanyId = x.Field<int>("CompanyId"),
                                            CompanyName = x.Field<string>("CompanyName"),
                                            CompanyLocationId = x.Field<int>("CompanyLocationId"),
                                            LocationName = Common.GetNotNullValue(x.Field<string>("LocationName")),
                                            CompanyDepartmentId = x.Field<int>("CompanyDepartmentId"),
                                            DepartmentName = Common.GetNotNullValue(x.Field<string>("DepartmentName")),
                                            CompanyContactId = x.Field<int>("CompanyContactId"),
                                            ContactName = Common.GetNotNullValue(x.Field<string>("ContactName")),
                                            JobOrderId = x.Field<int>("JobOrderId"),
                                            JobTitle = x.Field<string>("JobTitle"),
                                            Year = x.Field<int>("Year"),
                                            WeekOfYear = x.Field<int>("WeekOfYear"),
                                            SundayDate = x.Field<DateTime>("SundayDate"),
                                            Sunday = x.Field<decimal>("Sunday"),
                                            MondayDate = x.Field<DateTime>("MondayDate"),
                                            Monday = x.Field<decimal>("Monday"),
                                            TuesdayDate = x.Field<DateTime>("TuesdayDate"),
                                            Tuesday = x.Field<decimal>("Tuesday"),
                                            WednesdayDate = x.Field<DateTime>("WednesdayDate"),
                                            Wednesday = x.Field<decimal>("Wednesday"),
                                            ThursdayDate = x.Field<DateTime>("ThursdayDate"),
                                            Thursday = x.Field<decimal>("Thursday"),
                                            FridayDate = x.Field<DateTime>("FridayDate"),
                                            Friday = x.Field<decimal>("Friday"),
                                            SaturdayDate = x.Field<DateTime>("SaturdayDate"),
                                            Saturday = x.Field<decimal>("Saturday"),
                                            SubTotalHours = x.Field<decimal>("SubTotalHours"),
                                            OTHours = x.Field<decimal>("OTHours"),
                                            RegularHours = x.Field<decimal>("RegularHours"),
                                            ApprovedBy = x.Field<string>("ApprovedBy"),
                                        });
            }

            // filter by account role
            var result = _FilterTimeSheetHistoryByAccount(sheets, accountId);

            // filtering
            var filter = Common.GetLinqFilterStatement<EmployeeTimeChartHistory>("TimeSheetHistory", filterBy, filterCondition, filterValue);
            if (filter != null)
                result = result.Where(filter);

            // sorting and filtering bu user input
            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "EmployeeNumber";
            if (String.IsNullOrWhiteSpace(sortOrder))
                sortOrder = "desc";

            string _sortStr = Common.GetSortStatement("TimeSheetHistory", sortBy, sortOrder).Replace(" asc ", "").Replace(" desc ", " descending ");
            result = result.OrderBy(_sortStr);

            if (result != null)
            {
                // paging
                return new PagedListResult<SimpleEmployeeTimeChartHistory>()
                {
                    TotalCount = result.Count(),
                    Items = result.Skip(pageSize * pageIndex).Take(pageSize).AsQueryable().ProjectTo<SimpleEmployeeTimeChartHistory>().ToList()
                };
            }
            else
                return null;
        }


        private IEnumerable<EmployeeTimeChartHistory> _FilterTimeSheetHistoryByAccount(IEnumerable<EmployeeTimeChartHistory> timesheets, int accountId)
        {
            var _accountService = EngineContext.Current.Resolve<IAccountService>();
            var account = _accountService.GetAccountById(accountId);
            var _timeSheetHistoryService = EngineContext.Current.Resolve<IEmployeeTimeChartHistoryService>();
            return _timeSheetHistoryService.FilterTimeSheetHistoryByAccount(timesheets, account);
        }
    }


    public class SimpleEmployeeTimeChartHistory
    {
        public string EmployeeNumber { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int FranchiseId { get; set; }
        public string FranchiseName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CompanyLocationId { get; set; }
        public string LocationName { get; set; }
        public int CompanyDepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyContactId { get; set; }
        public string ContactName { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public DateTime SundayDate { get; set; }
        public decimal Sunday { get; set; }
        public DateTime MondayDate { get; set; }
        public decimal Monday { get; set; }
        public DateTime TuesdayDate { get; set; }
        public decimal Tuesday { get; set; }
        public DateTime WednesdayDate { get; set; }
        public decimal Wednesday { get; set; }
        public DateTime ThursdayDate { get; set; }
        public decimal Thursday { get; set; }
        public DateTime FridayDate { get; set; }
        public decimal Friday { get; set; }
        public DateTime SaturdayDate { get; set; }
        public decimal Saturday { get; set; }
        public Nullable<decimal> SubTotalHours { get; set; }
        public Nullable<decimal> OTHours { get; set; }
        public Nullable<decimal> RegularHours { get; set; }
        public string ApprovedBy { get; set; }
    }
}
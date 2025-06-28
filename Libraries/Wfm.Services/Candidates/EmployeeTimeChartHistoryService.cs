using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Data;
using Wfm.Services.Companies;
using Wfm.Services.Localization;

namespace Wfm.Services.Candidates
{
    public partial class EmployeeTimeChartHistoryService : IEmployeeTimeChartHistoryService
    {
        #region Fields
       
        private readonly IDbContext _dbContext;
        private readonly ILocalizationService _localizationService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        #endregion

        #region Ctor
        public EmployeeTimeChartHistoryService(
                                                IDbContext dbContext, 
                                                ILocalizationService localizationService,
                                                IRecruiterCompanyService recruiterCompanyService)
        {
            _dbContext=dbContext;
            _localizationService = localizationService;
            _recruiterCompanyService = recruiterCompanyService;
        }
        #endregion

        #region Methods
        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByDate(int clientId, DateTime start, DateTime end,string status, string weeklyStatus="0,1")
        {
            SqlParameter[] paras = new SqlParameter[5];
            paras[0] = new SqlParameter("@StartDate", start);
            paras[1] = new SqlParameter("@EndDate", end);
            paras[2] = new SqlParameter("@Status",status);
            paras[3] = new SqlParameter("@WeeklyStatus", weeklyStatus);
            paras[4] = new SqlParameter("@ClientId", clientId);
            var result = _dbContext.SqlQuery<EmployeeTimeChartHistory>("EXEC [dbo].[Timesheet_Weekly] @StartDate, @EndDate, @Status,@WeeklyStatus, @ClientId",paras);
            return result.ToList();
        }

        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByDate(DateTime start, DateTime end, string status, string weeklyStatus = "0,1", int companyId = 0)
        {
            var result = this.GetAllEmployeeTimeSheetHistoryByDate(companyId, start, end, status, weeklyStatus); // When companyId is passed as zero, the stored procedure does NOT filter the data for the company and returns the data for all companies
            return result.ToList();
        }

        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByCandidateId(int CanidateId, DateTime? start = null, DateTime? end = null)
        {
            end = end ?? DateTime.Today;
            start = start ?? end.Value.AddMonths(-12);
            var result = GetAllEmployeeTimeSheetHistoryByDate(start.Value, end.Value, ((Int32)(CandidateWorkTimeStatus.Approved)).ToString());
            
            return result.Where(x => x.EmployeeId == CanidateId).ToList();
        }


        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryForInvoice(DateTime start, DateTime end, int companyId, int vendorId)
        {
            IList<EmployeeTimeChartHistory> result = GetAllEmployeeTimeSheetHistoryByDate(start, end, ((Int32)CandidateWorkTimeStatus.Approved).ToString());
            // Whne an employee works in multiple companies, we can't charge either clients for the OT hours, but we still need to invoice the clients for the entire hours that employee has worked for them (with regular rate)
            // That's why in here, we are setting the OT hours to 0 and move those hours into the regular hours column for the employees, who have worked in multiple companies
            var grouped1 = result.GroupBy(x => new { x.EmployeeId, x.CompanyId }).Select(group => new { key = group.Key, EmployeeId = group.Key.EmployeeId }).ToList();
            var workedInMultipleCompanies = grouped1.GroupBy(x => new { x.EmployeeId }).Select(group => new { key = group.Key, RowCount = group.Count() }).Where(x => x.RowCount > 1).ToList();
            foreach (var emp in workedInMultipleCompanies)
            {
                foreach (var item in result.Where(x => x.EmployeeId == emp.key.EmployeeId))
                {
                    item.RegularHours = item.RegularHours + item.OTHours;
                    item.OTHours = 0;
                    item.Note = _localizationService.GetResource("Admin.Timesheet.CreateInvoice.EmpWorkedInMultiCompanies"); //"Employee worked in multiple companies.";
                }
            }
        
            return result.Where(x => x.CompanyId == companyId && x.FranchiseId == vendorId).ToList();
        }


        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByDateAndAccount(DateTime start, DateTime end, Account account, string status, string weeklyStatus = "0,1")
        {
            var companyId = account.IsClientAccount ? account.CompanyId : 0;
            IList<EmployeeTimeChartHistory> result = GetAllEmployeeTimeSheetHistoryByDate(start, end, status, weeklyStatus, companyId);
           
            return FilterTimeSheetHistoryByAccount(result, account).ToList();
        }


        public IEnumerable<EmployeeTimeChartHistory> FilterTimeSheetHistoryByAccount(IEnumerable<EmployeeTimeChartHistory> table, Account account = null)
        {
            if (account == null)
                return null;
            //for non-client account
            if (!account.IsClient())
            {
                List<int> ids = new List<int>();
                if (account.IsVendor())
                {
                    table = table.Where(x => x.FranchiseId == account.FranchiseId);
                    if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                    {
                        ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                        table = table.Where(x => ids.Contains(x.CompanyId));
                    }
                }
                if (account.IsMSPRecruiter() || account.IsMSPRecruiterSupervisor())
                {
                    ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                    table = table.Where(x => ids.Contains(x.CompanyId));
                }
            }
            // account is a client account
            else
            {
                // query within Franchise
                //query = query.Where(cwt => cwt.FranchiseId == account.FranchiseId);

                // query within company
                table = table.Where(cwt => cwt.CompanyId == account.CompanyId);

                // except Voided and Rejected
                //query = query.Where(cwt => cwt.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided &&
                //   (showRejected || cwt.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Rejected));

                // net work time is greater than 1
                //query = query.Where(cwt => cwt.NetWorkTimeInHours >= 1);


                // Check account role and determine search range
                //----------------------------------------------------
                if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

                // Jobs for Location Manager
                else if (account.IsCompanyLocationManager())
                    table = table.Where(cwt =>
                        cwt.CompanyLocationId > 0 &&
                        cwt.CompanyLocationId == account.CompanyLocationId); // search within locatin

                // Jobs for Department Supervisor
                else if (account.IsCompanyDepartmentSupervisor())
                    table = table.Where(cwt =>
                        cwt.CompanyLocationId > 0 &&
                        cwt.CompanyLocationId == account.CompanyLocationId &&
                        cwt.CompanyDepartmentId > 0 &&
                        cwt.CompanyDepartmentId == account.CompanyDepartmentId &&   // search within department
                        cwt.CompanyContactId == account.Id);
                else if (account.IsCompanyDepartmentManager())
                    table = table.Where(cwt =>
                        cwt.CompanyLocationId > 0 &&
                        cwt.CompanyLocationId == account.CompanyLocationId &&
                        cwt.CompanyDepartmentId > 0 &&
                        cwt.CompanyDepartmentId == account.CompanyDepartmentId);
                else
                    return null; // no role
            }

            table = from cwt in table
                    orderby cwt.WeekOfYear descending // sort by job start date
                    select cwt;


            return table;
        }

        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByIds(int[] ids, DateTime start, DateTime end,string status,string weeklyStatus)
        {
            if (ids == null || ids.Length == 0)
                return new List<EmployeeTimeChartHistory>();

            var table = GetAllEmployeeTimeSheetHistoryByDate(start, end,status,weeklyStatus);

            var query = from b in table
                        where ids.Contains(b.Id)
                        select b;

            var timecharts = query.ToList();

            //sort by passed identifiers
            var sortedTimeCharts = new List<EmployeeTimeChartHistory>();
            foreach (int id in ids)
            {
                var timechart = timecharts.Find(x => x.Id == id);
                if (timechart != null)
                    sortedTimeCharts.Add(timechart);
            }
            return sortedTimeCharts.OrderBy(x=>x.EmployeeId).ToList();

        }


        public IList<EmployeeTimeChartHistory> GetAllEmployeeTimeSheetHistoryByIds(int[] ids, DateTime start, DateTime end, Account account)
        {
            if (ids == null || ids.Length == 0)
                return new List<EmployeeTimeChartHistory>();

            var status = ((int)CandidateWorkTimeStatus.Approved).ToString();        // approved only
            var table = GetAllEmployeeTimeSheetHistoryByDateAndAccount(start, end, account, status);

            var query = from b in table
                        where ids.Contains(b.Id)
                        select b;

            var timecharts = query.ToList();

            //sort by passed identifiers
            var sortedTimeCharts = new List<EmployeeTimeChartHistory>();
            foreach (int id in ids)
            {
                var timechart = timecharts.Find(x => x.Id == id);
                if (timechart != null)
                    sortedTimeCharts.Add(timechart);
            }

            return sortedTimeCharts.OrderBy(x => x.EmployeeId).ToList();
        }

        public IList<CandidateWorkTimeStatusClass> GetAllCandidateWorkTimeStatusFromEnum()
        {
            List<CandidateWorkTimeStatusClass> result = new List<CandidateWorkTimeStatusClass>();
            
            foreach (var item in Enum.GetValues(typeof(CandidateWorkTimeStatus)))
            {
                CandidateWorkTimeStatusClass one = new CandidateWorkTimeStatusClass();
                one.Name = Enum.GetName(typeof(CandidateWorkTimeStatus), item);
                one.Value = (Int32)item;
                if (one.Value != (int)CandidateWorkTimeStatus.Matched && one.Value != (int)CandidateWorkTimeStatus.Voided && one.Value != (int)CandidateWorkTimeStatus.Processed)
                {
                    result.Add(one);
                }
            }
            return result;
        }

        public IList<InvoiceUpdateDetail> GetInvoiceUpdatesFromDailyWorkTime(string ids)
        {
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@ids", ids);
            var result = _dbContext.SqlQuery<InvoiceUpdateDetail>("EXEC [dbo].[InvoiceUpdatesDetails] @ids", paras);
            return result.ToList();
        }
        #endregion
        
    }

}

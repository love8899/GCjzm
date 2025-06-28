using CanadianTaxTable.TaxTables;
using Common.TaxTables;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Employees;
using Wfm.Data;



namespace Wfm.Services.Employees
{
    public class EmployeeTD1Service : IEmployeeTD1Service
    {
        #region Constants

        private const string    EMPLOYEETD1_PATTERN_KEY = "Wfm.EmployeeTD1.";

        #endregion

        #region Fields

        private readonly IRepository<EmployeeTD1> _employeeTD1Repository;
        private readonly ICacheManager _cacheManager;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor

        public EmployeeTD1Service(ICacheManager cacheManager, IRepository<EmployeeTD1> employeeTD1Repository, IDbContext dbContext)
        {
            _cacheManager = cacheManager;
            _employeeTD1Repository = employeeTD1Repository;
            _dbContext = dbContext;
        }

        #endregion

        #region CRUD

        public void InsertEmployeeTD1(EmployeeTD1 employeeTD1)
        {
            if (employeeTD1 == null)
                throw new ArgumentNullException("employeeTD1");

            _employeeTD1Repository.Insert(employeeTD1);

            _cacheManager.RemoveByPattern(EMPLOYEETD1_PATTERN_KEY);
        }

        public void UpdateEmployeeTD1(EmployeeTD1 employeeTD1)
        {
            if (employeeTD1 == null)
                throw new ArgumentNullException("employeeTD1");

            _employeeTD1Repository.Update(employeeTD1);

            _cacheManager.RemoveByPattern(EMPLOYEETD1_PATTERN_KEY);
        }

        public void DeleteEmployeeTD1(EmployeeTD1 employeeTD1)
        {
            if (employeeTD1 == null)
                throw new ArgumentNullException("employeeTD1");
            
            _employeeTD1Repository.Delete(employeeTD1);

            _cacheManager.RemoveByPattern(EMPLOYEETD1_PATTERN_KEY);
        }

        #endregion


        #region Employee TD1

        public EmployeeTD1 GetEmployeeTD1ById(int id)
        {
            return _employeeTD1Repository.GetById(id);
        }


        public EmployeeTD1 GetEmployeeTD1ByEmployeeAndYearAndProvince(int employeeId, int year, string provinceCode)
        {
            return GetAllEmployeeTD1s().Where(x => x.CandidateId == employeeId && x.Year == year && x.Province_Code == provinceCode).FirstOrDefault();
        }

        #endregion


        #region LIST
        
        public IQueryable<EmployeeTD1> GetAllEmployeeTD1s()
        {
            return _employeeTD1Repository.Table;
        }


        public IQueryable<EmployeeTD1> GetAllEmployeeTD1sByEmployeeId(int employeeId, int? year)
        {
            var query = GetAllEmployeeTD1s().Where(x => x.CandidateId == employeeId);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (query.Count() <= 0)
            {
                TaxTable taxCalculator = TaxTableUtilities.GetTaxTableForDate(new DateTime(year.Value, 1, 1));
                Dictionary<string, decimal> basicAmounts = taxCalculator.GetAllBasicAmounts();
                Create_All_Default_TD1_Data(employeeId, year.Value, basicAmounts);
            }

            return query;
        }

        private void Create_All_Default_TD1_Data(int CandidateId, int Year, Dictionary<string, decimal> BasicCreditAmount)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"if not exists (select 1 from Candidate_TD1 where CandidateId=@candidateId and Year = @Year)
                             begin ");
            foreach (var row in BasicCreditAmount)
            {
                sql.AppendLine(String.Format("Insert Into Candidate_TD1 (CandidateId, Year, Province_Code, Basic_Amount ) values(@CandidateId, @Year, '{0}', {1} )",
                                             row.Key, row.Value.ToString()));
            }
            sql.AppendLine(" end ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CandidateId", CandidateId));
            parameters.Add(new SqlParameter("Year", Year));

            _dbContext.ExecuteSqlCommand(sql.ToString(), false, null, parameters.ToArray());
        }
        #endregion
    }
}

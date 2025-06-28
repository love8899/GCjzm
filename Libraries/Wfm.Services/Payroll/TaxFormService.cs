using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wfm.Core.Domain.Payroll;
using Wfm.Data;

namespace Wfm.Services.Payroll
{
    public class TaxFormService:ITaxFormService
    {
        private readonly IDbContext _dbContext;
        public TaxFormService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<TaxForm> GetAllT4sByCandidateIdAndYear(int candidateId, int year)
        {
            if (year >= 2015)
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine(String.Format(@" IF OBJECT_ID(N'dbo.T4_{0}', N'U') IS NOT NULL
                                                SELECT [Id],
                                                        'T4' as FormType,
                                                        @year as Year,
                                                        [ProvinceCode] as Province,
                                                        EmploymentIncome as Income,
                                                        IncomeTax as Tax,
                                                        CreatedOn as IssueDate,
                                                        Case ReportTypeCode
                                                            When  'O' Then 'Original' 
								                            When  'A' Then 'Amended'
													        When  'C' Then 'Cancel'
												        End as SlipType,
                                                        IsSubmitted
                                                FROM T4_{0}
                                                Where (IsSubmitted = 1 or EmailSent = 1) AND CandidateId = @CandidateId ", year));

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CandidateId", candidateId));
                parameters.Add(new SqlParameter("year", year));

                return _dbContext.SqlQuery<TaxForm>(query.ToString(), parameters.ToArray());
            }
            return Enumerable.Empty<TaxForm>();
        }
        public IEnumerable<TaxForm> GetAllRL1sByCandidateIdAndYear(int candidateId, int year)
        {
            if (year >= 2015)
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine(String.Format(@" IF OBJECT_ID(N'dbo.RL1_{0}', N'U') IS NOT NULL
                                                 Select rl.[Id],
                                                        'RL1' as FormType, 
                                                        @year as [Year],
                                                        sp.Abbreviation as Province,
                                                        Box_A as Income,
                                                        Box_E as Tax,
                                                        CreatedOn as IssueDate, 
                                                        Case Code 
                                                            When  'R' Then 'Original' 
														    When  'A' Then 'Amended'
														    When  'D' Then 'Cancel'
														End as SlipType,
                                                        IsSubmitted
                                                from RL1_{0} rl
                                                 inner join StateProvince sp on sp.Id=rl.StateProvinceId
                                                where (IsSubmitted=1 or EmailSent =1) AND CandidateId=@CandidateId", year));

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CandidateId", candidateId));
                parameters.Add(new SqlParameter("year", year));

                return _dbContext.SqlQuery<TaxForm>(query.ToString(), parameters.ToArray());
            }
            return Enumerable.Empty<TaxForm>();
        }

        public IEnumerable<TaxForm> GetAllTaxFormsByCandidateIdAndYear(int candidateId, int year)
        {
            List<TaxForm> t4s = GetAllT4sByCandidateIdAndYear(candidateId, year).ToList();
            List<TaxForm> rl1s = GetAllRL1sByCandidateIdAndYear(candidateId, year).ToList();
            return t4s.Union(rl1s);
        }
    }
}

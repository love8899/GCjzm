using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.JobOrders;
using Wfm.Data;
using Wfm.Services.Companies;


namespace Wfm.Services.JobOrders
{
    public partial class BasicJobOrderInfoService:IBasicJobOrderInfoService
    {
        #region fields
        
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly ICompanyVendorService _companyVendorService;
        #endregion

        #region Constructor

        public BasicJobOrderInfoService(IDbContext dbContext, IWorkContext workContext, IRecruiterCompanyService recruiterCompanyService, ICompanyVendorService companyVendorService)
        {
            _dbContext = dbContext;
            _workContext = workContext;
            _recruiterCompanyService = recruiterCompanyService;
            _companyVendorService = companyVendorService;
        }
        
        #endregion

        #region Method
        
        public IList<BasicJobOrderInfo> GetAllBasicJobOrderInfoByDate(DateTime? startDate, DateTime? endDate)
        {
            List<int> companyIds = new List<int>();
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("startDate", startDate.HasValue?startDate.Value:System.Data.SqlTypes.SqlDateTime.MinValue);
            paras[1] = new SqlParameter("endDate", endDate.HasValue ? endDate.Value : System.Data.SqlTypes.SqlDateTime.MaxValue);
            IList<BasicJobOrderInfo> result = _dbContext.SqlQuery<BasicJobOrderInfo>("Exec [dbo].[BasicJobOrderInfo] @startDate,@endDate", paras).ToList();
            Account account = _workContext.CurrentAccount;
            if (account.IsVendor())
            {
                var companyList = _companyVendorService.GetAllCompaniesByVendorId(account.FranchiseId).Select(x => x.CompanyId);
                result = result.Where(x => x.FranchiseId == account.FranchiseId&&companyList.Contains(x.CompanyId)).ToList();
                if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                {
                    companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                    result = result.Where(x => companyIds.Contains(x.CompanyId)).ToList();
                }
            }
            if (account.IsMSPRecruiter() || account.IsMSPRecruiterSupervisor())
            {
                companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                result = result.Where(x => companyIds.Contains(x.CompanyId)).ToList();
            }
            return result;
        }

        #endregion
    }
}

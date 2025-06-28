using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Companies;
using System.Data.Entity;

namespace Wfm.Services.JobOrders
{
    public partial class DirectHireJobOrderService : IDirectHireJobOrderService
    {
        #region Fields
        private readonly IRepository<JobOrder> _directHireJobOrderRepository;
        private readonly IRepository<Account> _accountRepository; 
        private readonly IRepository<FeeType> _feeTypeRepository;
        private readonly IRepository<CandidateKeySkill> _candidateKeySkillsRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly ICompanyVendorService _companyVendorService;

        #endregion 

        #region Ctor
        public DirectHireJobOrderService(
                    IRepository<JobOrder> directHireJobOrderRepository,
                    IRepository<FeeType> feeTypeRepository,
                    IRepository<Account> accountRepository,
                    IRepository<CandidateKeySkill> candidateKeySkillsRepository,
                     IWorkContext workContext,
                    IRepository<Candidate> candidateRepository,
                    IRecruiterCompanyService recruiterCompanyService, 
                     ICompanyVendorService companyVendorService
            )
        {
            _directHireJobOrderRepository = directHireJobOrderRepository;
            _feeTypeRepository = feeTypeRepository;
            _accountRepository = accountRepository;
            _candidateKeySkillsRepository = candidateKeySkillsRepository;
            _workContext = workContext;
            _candidateRepository = candidateRepository;
            _recruiterCompanyService = recruiterCompanyService;
            _companyVendorService = companyVendorService;
        }
        #endregion

        #region CRUD

        public void InsertDirectHireJobOrder(JobOrder directHireJobOrder)
        {
            if (directHireJobOrder == null)
                throw new ArgumentNullException("directHireJobOrder");

            directHireJobOrder.CreatedOnUtc = DateTime.UtcNow;
            directHireJobOrder.UpdatedOnUtc = DateTime.UtcNow;

            _directHireJobOrderRepository.Insert(directHireJobOrder);
        }
        public void UpdateDirectHireJobOrder(JobOrder directHireJobOrder)
        {
            if (directHireJobOrder == null)
                throw new ArgumentNullException("directHireJobOrder");

            directHireJobOrder.UpdatedOnUtc = DateTime.UtcNow;

            _directHireJobOrderRepository.Update(directHireJobOrder);
        }


        #endregion

        #region DirectHireJobOrder
        public JobOrder GetDirectHireJobOrderById(int id)
        {
            if (id == 0)
                return null;

            return _directHireJobOrderRepository.Table
                    .Include(x => x.Company)
                    .Include(x => x.Shift)
                  .Where(x => x.Id == id).FirstOrDefault();
        }
        public JobOrder GetDirectHireJobOrderByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            return _directHireJobOrderRepository.Table.Where(x => x.JobOrderGuid == guid).FirstOrDefault();
        }

        #endregion

        #region PagedList
        public IQueryable<DirectHireJobOrderList> GetDirectHireJobOrderList(Account account, DateTime startDate, DateTime endDate)
        {
            if (account == null)
                return null;

            var query = _directHireJobOrderRepository.TableNoTracking;


            // add check for joborder type
             query = query.Where(j => j.JobOrderType.IsDirectHire);

            // IsLimitedToFranchises
            if (account.IsLimitedToFranchises)
                query = query.Where(jo => jo.FranchiseId == account.FranchiseId);         

            var result = from jo in query
                         from ac in _accountRepository.TableNoTracking.Where(a=>a.Id==jo.RecruiterId).DefaultIfEmpty()
                         where jo.IsDeleted == false && (endDate >= jo.StartDate && jo.StartDate>=startDate) && (jo.HiringDurationExpiredDate == null || jo.HiringDurationExpiredDate >= startDate)
                         orderby jo.UpdatedOnUtc descending
                    select new DirectHireJobOrderList(){
                      CompanyGuid=jo.Company.CompanyGuid,
                      CompanyId=jo.CompanyId,
                      CompanyJobNumber=jo.CompanyJobNumber,
                      CompanyLocationId = jo.CompanyLocationId,
                      CompanyName=jo.Company.CompanyName,
                      CreatedOnUtc=jo.CreatedOnUtc,
                      JobOrderGuid = jo.JobOrderGuid,
                      FeeType=jo.FeeType.FeeTypeName,
                      FranchiseId=jo.FranchiseId,
                      HiringDurationExpiredDate=jo.HiringDurationExpiredDate,
                      Id=jo.Id,
                      IsPublished=jo.IsPublished,
                      JobOrderCategory=jo.JobOrderCategory.CategoryName,
                      JobTitle=jo.JobTitle,
                      Salary=jo.Salary,
                      SalaryMax=jo.SalaryMax,
                      SalaryMin=jo.SalaryMin,
                      Status=jo.JobOrderStatus.JobOrderStatusName,
                      UpdatedOnUtc=jo.UpdatedOnUtc,
                      RecruiterName=ac.FirstName+" "+ac.LastName,
                      StartDate=jo.StartDate
                    };

            List<int> companyIds = new List<int>();           
            if (account.IsVendor())
            {
                var companyList = _companyVendorService.GetAllCompaniesByVendorId(account.FranchiseId).Select(x => x.CompanyId);
                result = result.Where(x => x.FranchiseId == account.FranchiseId && companyList.Contains(x.CompanyId));
                if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                {
                    companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                    result = result.Where(x => companyIds.Contains(x.CompanyId));
                }
            }
            if (account.IsMSPRecruiter() || account.IsMSPRecruiterSupervisor())
            {
                companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                result = result.Where(x => companyIds.Contains(x.CompanyId));
            }

            return result;

        }

        public IQueryable<JobOrder> GetAllDirectHireJobOrdersAsQueryable(Account account) 
        {
            if (account == null)
                return null;

            var query = _directHireJobOrderRepository.Table;
         
            // add check for joborder type          
            query = query.Where(j => j.JobOrderType.IsDirectHire);

            // IsLimitedToFranchises
            if (account.IsLimitedToFranchises)
                query = query.Where(jo => jo.FranchiseId == account.FranchiseId);

            query = from jo in query
                    where jo.IsDeleted == false
                    orderby jo.UpdatedOnUtc descending
                    select jo;


            return query.AsQueryable();
        }
        #endregion

        public IEnumerable<FeeType> GetAllFeeTypes() 
        {
            return _feeTypeRepository.TableNoTracking;
        }
        public IQueryable<DirectHireCandidatePoolList> GetDirectHireCandidatePoolList() 
        {
            var query = _candidateRepository.TableNoTracking;
         
                query = query.Where(x => x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
                query = query.Where(c => c.IsDeleted == false && c.UseForDirectPlacement);

            Account account = _workContext.CurrentAccount;
            if (account.IsVendor())
                query = query.Where(x => x.FranchiseId == account.FranchiseId);

            var result = from c in query
                         from s in _candidateKeySkillsRepository.TableNoTracking.Where(x => x.CandidateId == c.Id && x.IsDeleted==false).DefaultIfEmpty()
                         orderby c.CreatedOnUtc descending, s.LastUsedDate descending
                         select new DirectHireCandidatePoolList()
                         {
                             CandidateGuid=c.CandidateGuid,
                             CandidateId = c.Id,
                             FirstName = c.FirstName,
                             IsDeleted = s.IsDeleted==null?false:s.IsDeleted,
                             KeySkill = s.KeySkill,
                             LastName = c.LastName,
                             LastUsedDate = s.LastUsedDate,
                             Note = c.Note,
                             YearsOfExperience = s.YearsOfExperience,
                             EmployeeId = c.EmployeeId
                         };

            return result.AsQueryable();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Companies
{
    public partial class RecruiterCompanyService:IRecruiterCompanyService
    {
        #region Field
        private readonly IRepository<RecruiterCompany> _companyRecruitersRepository;
        #endregion

        #region Constructors
        public RecruiterCompanyService(IRepository<RecruiterCompany> companyRecruitersRepository)
        {
            _companyRecruitersRepository = companyRecruitersRepository;
        }
        #endregion

        #region CRUD
        public virtual void InsertCompanyRecruiter(RecruiterCompany companyRecruiter)
        {
            if (companyRecruiter == null)
            {
                throw new ArgumentNullException("companyRecruiter");
            }
            if (IsDuplicateConnection(companyRecruiter.CompanyId, companyRecruiter.AccountId))
                return;
            _companyRecruitersRepository.Insert(companyRecruiter);
        }

        public virtual void UpdateCompanyRecruiter(RecruiterCompany companyRecruiter)
        {
            if (companyRecruiter == null)
            {
                throw new ArgumentNullException("companyRecruiter");
            }
            bool alreadyExistConnection = _companyRecruitersRepository.TableNoTracking.Where(x => x.AccountId == companyRecruiter.AccountId && x.CompanyId == companyRecruiter.CompanyId).Count() >= 1;
            if (alreadyExistConnection)
                _companyRecruitersRepository.Delete(companyRecruiter);
            else
                _companyRecruitersRepository.Update(companyRecruiter);
        }

        public virtual void DeleteCompanyRecruiter(RecruiterCompany companyRecruiter)
        {
            if (companyRecruiter == null)
            {
                throw new ArgumentNullException("companyRecruiter");
            }
            //companyRecruiter.IsActive = false;
            _companyRecruitersRepository.Delete(companyRecruiter);
        }
        #endregion

        #region Method

        public IQueryable<RecruiterCompany> GetAllRecruitersAsQueryable(Account account = null)
        {
            var query = _companyRecruitersRepository.TableNoTracking;

            if (account != null && !account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(x => x.Account.FranchiseId == account.FranchiseId);

            return query;
        }

        
        public IList<RecruiterCompany> GetAllRecruitersByCompanyId(int companyId)
        {
            List<RecruiterCompany> result = _companyRecruitersRepository.Table.Where(x => x.CompanyId == companyId).ToList();
            return result;
        }

        public RecruiterCompany GetRecruiterCompanyById(int id)
        {
            RecruiterCompany result = _companyRecruitersRepository.Table.Where(x => x.Id == id).FirstOrDefault();
            return result;

        }

        public void DeleteRecruiterCompanyById(int id)
        {
            RecruiterCompany recruiter_company = GetRecruiterCompanyById(id);
            if (recruiter_company != null)
            {
                DeleteCompanyRecruiter(recruiter_company);
            }
        }
        public List<int> GetCompanyIdsByRecruiterId(int recruiterId)
        {
            List<int> result = _companyRecruitersRepository.Table.Where(x => x.AccountId == recruiterId).Select(x => x.CompanyId).Distinct().ToList();
            return result;
        }

        public string GetAllRecruitersEmailByCompanyId(int companyId, int franchiseId)
        {
            string[] emails = _companyRecruitersRepository.TableNoTracking.Where(x => x.CompanyId == companyId && x.Account.FranchiseId == franchiseId).Select(x => x.Account.Email).Distinct().ToArray();
            return String.Join(";", emails);
        }

        public IList<RecruiterCompany> GetAllRecruitersByCompanyGuid(Guid? guid)
        {
            List<RecruiterCompany> result = _companyRecruitersRepository.Table.Where(x => x.Company.CompanyGuid == guid).ToList();
            return result;
        }

        public void DeleteCompanyRecruitersByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var recruiters = _companyRecruitersRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (recruiters.Count() > 0)
                _companyRecruitersRepository.Delete(recruiters);

        }

        public IList<SelectListItem> GetAllCompaniesByRecruiter(int id)
        {
            return _companyRecruitersRepository.Table.Where(x => x.AccountId == id).Select(x => new SelectListItem() { Text = x.Company.CompanyName, Value = x.CompanyId.ToString() }).ToList() ;
        }

        public IList<RecruiterCompany> GetAllRecruiterCompaniesByRecruiterId(int id)
        {
            List<RecruiterCompany> result = _companyRecruitersRepository.Table.Where(x => x.AccountId==id).ToList();
            return result;
        }

        public bool IsDuplicateConnection(int companyId, int recruiterId)
        {
            var result = _companyRecruitersRepository.TableNoTracking.Where(x => x.AccountId == recruiterId && x.CompanyId == companyId);
            return result.Count() > 0;
        }
        #endregion
    }
}

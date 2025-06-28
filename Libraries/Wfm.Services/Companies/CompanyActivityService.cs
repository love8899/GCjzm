using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public class CompanyActivityService:ICompanyActivityService
    {
        #region Field
        private readonly IRepository<CompanyActivity> _companyActivityRepository;
        #endregion

        #region CTOR
        public CompanyActivityService(IRepository<CompanyActivity> companyActivityRepository)
	    {
            _companyActivityRepository = companyActivityRepository;
	    }
	    #endregion

        #region CRUD
        public void Create(CompanyActivity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyActivity");

            _companyActivityRepository.Insert(entity);
        }

        public CompanyActivity Retrieve(int id)
        {
            var companyActivities = _companyActivityRepository.Table.Where(x => x.Id == id);
            return companyActivities.FirstOrDefault();
        }

        public void Update(CompanyActivity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyActivity");

            _companyActivityRepository.Update(entity);
        }

        public void Delete(CompanyActivity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyActivity");

            _companyActivityRepository.Delete(entity);
        }
        #endregion
    }
}

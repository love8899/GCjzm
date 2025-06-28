using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public class CompanyStatusService:ICompanyStatusService
    {
        #region Field
        private readonly IRepository<CompanyStatus> _companyStatusRepository;
        #endregion

        #region CTOR
        public CompanyStatusService(IRepository<CompanyStatus> companyStatusRepository)
        {
            _companyStatusRepository = companyStatusRepository;
        }
        #endregion

        #region CRUD
        public void Create(CompanyStatus entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyStatus");

            _companyStatusRepository.Insert(entity);
        }
        public CompanyStatus Retrieve(int id)
        {
            var companyStatus = _companyStatusRepository.GetById(id);
            return companyStatus;
        }
        public void Update(CompanyStatus entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyStatus");

            _companyStatusRepository.Update(entity);
        }
        public void Delete(CompanyStatus entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyStatus");

            _companyStatusRepository.Delete(entity);
        }
        #endregion

        #region Method
        public IList<SelectListItem> GetAllCompanyStatusForDropDownList()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            //status.Add(new SelectListItem() { Text = "None", Value = "0" });
            var companyStatus = _companyStatusRepository.TableNoTracking.Select(x => new SelectListItem() { Text = x.StatusName, Value = x.Id.ToString() });
            status.AddRange(companyStatus);
            return status;
        }

        public IList<CompanyStatus> GetAllCompanyStatus()
        {
            return _companyStatusRepository.Table.ToList();
        }

        public CompanyStatus GetCompanyStatusByName(string name)
        {
            CompanyStatus status = _companyStatusRepository.TableNoTracking.Where(x => x.StatusName == name).FirstOrDefault();
            return status;
        }
        #endregion
    }
}

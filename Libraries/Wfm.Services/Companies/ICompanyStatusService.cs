using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public interface ICompanyStatusService
    {
        #region CRUD
        void Create(CompanyStatus entity);
        CompanyStatus Retrieve(int id);
        void Update(CompanyStatus entity);
        void Delete(CompanyStatus entity);
        #endregion

        #region Method
        IList<SelectListItem> GetAllCompanyStatusForDropDownList();

        IList<CompanyStatus> GetAllCompanyStatus();

        CompanyStatus GetCompanyStatusByName(string name);
        #endregion
    }
}

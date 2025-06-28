using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public interface ICompanyActivityService
    {
        #region CRUD
        void Create(CompanyActivity entity);
        CompanyActivity Retrieve(int id);
        void Update(CompanyActivity entity);
        void Delete(CompanyActivity entity);
        #endregion

        #region Method
        
        #endregion
    }
}

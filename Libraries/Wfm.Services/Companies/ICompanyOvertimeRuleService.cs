using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public interface ICompanyOvertimeRuleService
    {
        #region CRUD
        void Create(CompanyOvertimeRule entity);
        CompanyOvertimeRule Retrieve(int id);
        void Update(CompanyOvertimeRule entity);
        void Delete(CompanyOvertimeRule entity);
        #endregion

        #region Method
        IList<CompanyOvertimeRule> GetAllCompanyOvertimeRuleByCompanyGuid(Guid? guid);

        IQueryable<CompanyOvertimeRule> GetAllCompanyOvertimeRules(bool showInactive = false);

        void DeleteAllCompanyOvertimeRulesByCompanyGuid(Guid? guid);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public interface ICompanyEmailTemplateService
    {
        #region CRUD
        void Create(CompanyEmailTemplate entity);
        CompanyEmailTemplate Retrieve(int id);
        void Update(CompanyEmailTemplate entity);
        void Delete(CompanyEmailTemplate entity);
        #endregion

        #region Method
        IQueryable<CompanyEmailTemplate> GetAllEmailTemplate();
        IList<SelectListItem> GetAllEmailTemplateAsSelectedList();
        IQueryable<CompanyEmailTemplate> GetAllEmailTemplateByCompanyId(int companyId);
        CompanyEmailTemplate GetEmailTemplate(int type, int companyId, int locationId, int departmentId);

        bool DuplicateEmailTemplate(CompanyEmailTemplate entity);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public partial interface ICompanyContactService
    {

        #region CRUD

        void InsertCompanyContact(Account companyContact);

        void UpdateCompanyContact(Account companyContact);

        void DeleteCompanyContact(Account companyContact);

        #endregion

        #region  CompanyContact

        Account GetCompanyContactByEmail(string email);

        Account GetCompanyContactById(int id);

        void DeleteAllCompanyContactsByCompanyGuid(Guid? guid);

        #endregion

        #region List

        IList<Account> GetCompanyContactsByCompanyId(int companyId);

        IList<Account> GetCompanyClientAdminsByCompanyId(int companyId);

        IList<SelectListItem> GetCompanyContactsByCompanyIdAsSelectList(int companyId);

        IList<Account> GetCompanyContactsByCompanyIdAndLocationIdAndDepartmentId(int companyId, int locationId, int departmentId);

        IQueryable<Account> GetAllCompanyContactsAsQueryable(bool showInactive = false, bool showHidden = false);

        IQueryable<Account> GetAllCompanyContactsByAccountAsQueryable(Account account, bool showInactive = false, bool showHidden = false);

        IQueryable<CompanyContact> GetCompanyContactsAsQueryable(bool showInactive = false, bool showHidden = false);

        IQueryable<CompanyContact> GetCompanyClientAdminsAsQueryable(bool showInactive = false, bool showHidden = false);

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public partial interface ICompanyDepartmentService
    {
        #region CRUD

        void Insert(CompanyDepartment companyDepartment);

        void Update(CompanyDepartment companyDepartment);

        void Delete(CompanyDepartment companyDepartment);

        #endregion

        #region CompanyDepartment

        CompanyDepartment GetCompanyDepartmentById(int id);

        void DeleteAllCompanyDepartmentByCompanyGuid(Guid? guid);

        #endregion

        #region LIST

        IQueryable<CompanyDepartment> GetAllCompanyDepartmentsAsQueryable(bool notTracking = true);

        IList<CompanyDepartment> GetAllCompanyDepartmentsByCompanyId(int companyId, bool activeOnly = true);
        IList<CompanyDepartment> GetAllCompanyDepartmentsByCompanyGuid(Guid? companyGuid, bool activeOnly = true);

        IList<SelectListItem> GetAllCompanyDepartmentsByCompanyIdAsSelectList(int companyId, bool activeOnly = true);

        IList<SelectListItem> GetAllCompanyDepartmentsByCompanyGuidAsSelectList(Guid? companyGuid, bool activeOnly = true);

        IList<CompanyDepartment> GetAllCompanyDepartmentsByAccount(Account account, bool showInactive = false, bool showHidden = false);

        IList<CompanyDepartment> GetAllCompanyDepartmentByLocationId(int locationId, bool activeOnly = true);

        IList<CompanyDepartment> GetAllCompanyDepartmentByLocationName(string location, bool activeOnly = true);

        IList<SelectListItem> GetAllCompanyDepartmentsForDropDownList(bool showInactive = false, bool showHidden = false);

        #endregion

    }
}

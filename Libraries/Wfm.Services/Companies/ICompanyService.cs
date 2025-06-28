using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Companies;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using System.Web.Mvc;
using System.Collections;

namespace Wfm.Services.Companies

{
    public partial interface ICompanyService
    {
        #region CRUD

        void InsertCompany(Company company);

        void UpdateCompany(Company company);

        void DeleteCompany(Company company);

        #endregion

        #region Company

        /// <summary>
        /// Gets the company by CompanyId
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        Company GetCompanyById(int Id, bool enableTracking = false);

        Company GetCompanyByCode(string code);

        Company GetCompanyByGuid(Guid? guid);
        Company GetCompanyByIdForScheduleTask(int Id, bool enableTracking = false);

        Company GetAdminCompany();

        void DeleteCompanyByGuid(Guid? guid);
        #endregion

        #region LIST

        IList<SelectListItem> GetCompanyListForCandidate(bool showInactive = false, bool showHidden = false);
        /// <summary>
        /// Gets all companies. For DropDownList
        /// </summary>
        /// <returns></returns>
        IList<SelectListItem> GetAllCompanies();
        IList<SelectListItem> GetAllCompanies(Account account);

        /// <summary>
        /// Gets all companies. For GridView
        /// </summary>
        /// <param name="FilterCols">The filter cols.</param>
        /// <param name="FilterCommand">The filter command.</param>
        /// <param name="SortCommand">The sort command.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        IPagedList<Company> GetAllCompanies(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all companies asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        IQueryable<Company> GetAllCompaniesAsQueryable(Account account, bool showInactive = false, bool showHidden = false);

        IQueryable<Company> Secure_GetAllCompanies(Account account, bool showInactive = false, bool showHidden = false);
        Company Secure_GetCompanyById(Account account, int Id, bool enableTracking);

        #endregion

        #region Company Search
        IList<Company> SearchCompanies(string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showHidden = false);
        #endregion

        #region Statistics

        int GetTotalCompanyByDate(DateTime date);

        #endregion

        #region Job Roles
        IQueryable<CompanyJobRole> GetAllJobRoles(int companyId);
        IEnumerable GetAllJobRolesSelectList(int companyId);
        IQueryable<CompanyJobRole> GetAllJobRolesByAccount(Account account);
        CompanyJobRole GetJobRoleById(int jobRoleId);
        void InsertCompanyJobRole(CompanyJobRole jobRole, int[] requiredSkillId);
        void UpdateCompanyJobRole(CompanyJobRole jobRole, int[] requiredSkillId);
        void DeleteCompanyJobRole(int jobRoleId);
        #endregion

        #region Shfits
        IQueryable<CompanyShift> GetAllShifts(int companyId);
        IEnumerable GetGetAllShiftsSelectList(int companyId);
        IQueryable<CompanyShift> GetAllShiftsByAccount(Account account);
        CompanyShift GetCompanyShiftById(int companyShiftId);
        void InsertNewCompanyShift(CompanyShift shift);
        void UpdateCompanyShift(CompanyShift shift);
        void DeleteCompanyShift(int companyShiftId);
        IEnumerable<CompanyShiftJobRole> GetJobRolesOfShift(int companyShiftId);
        void InsertCompanyShiftJobRoles(int companyShiftId, IEnumerable<CompanyShiftJobRole> roles);
        void UpdateCompanyShiftJobRoles(int companyShiftId, IEnumerable<CompanyShiftJobRole> roles);
        void DeleteCompanyShiftJobRoles(IEnumerable<CompanyShiftJobRole> roles);
        #endregion
    }
}

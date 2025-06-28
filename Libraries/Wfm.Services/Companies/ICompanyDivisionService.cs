using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Accounts;
using System;


namespace Wfm.Services.Companies
{
    public partial interface ICompanyDivisionService
    {
        #region CRUD

        void InsertCompanyLocation(CompanyLocation companyLocation);

        void UpdateCompanyLocation(CompanyLocation companyLocation);

        void DeleteCompanyLocation(CompanyLocation companyLocation);

        #endregion

        #region CompanyLocation

        CompanyLocation GetCompanyLocationById(int id);

        Company GetCompanyByLocationId(int locationId);

        void DeleteAllCompanyLocationsByCompanyGuid(Guid? guid);

        #endregion

        #region LIST

        IList<CompanyLocation> GetAllCompanyLocationsByCompanyId(int? companyId, bool activeOnly = true);
        IList<CompanyLocation> GetAllCompanyLocationsByCompanyGuid(Guid? companyGuid, bool activeOnly = true);

        IList<SelectListItem> GetAllCompanyLocationsByCompanyIdAsSelectList(int? companyId, bool activeOnly = true);
        IList<SelectListItem> GetAllCompanyLocationsByCompanyGuidAsSelectList(Guid? companyGuid, bool activeOnly = true);

        IList<CompanyLocation> SQL_GetAllCompanyLocationsByCompanyId(int? companyId, bool activeOnly = true);

        IList<CompanyLocation> GetAllCompanyLocationsByAccount(Account account, bool showInactive = false, bool showHidden = false);

        IPagedList<CompanyLocation> GetAllCompanyLocations(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false);


        #endregion

  }
}

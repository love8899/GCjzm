using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Accounts;
using System;


namespace Wfm.Services.Companies
{
    public partial interface ICompanyVendorService
    {
        #region CRUD

        void InsertCompanyVendor(CompanyVendor companyVendor);

        void UpdateCompanyVendor(CompanyVendor companyVendor);

        void DeleteCompanyVendor(CompanyVendor companyVendor);

        #endregion


        #region Company Vendor

        CompanyVendor GetCompanyVendorById(int id);

        CompanyVendor GetCompanyVendorByComanyAndVendor(int companyId, int vendorId);


        #endregion


        #region LIST

        IQueryable<CompanyVendor> GetAllCompanyVendors();

        IQueryable<CompanyVendor> GetAllCompanyVendorsByCompanyId(int companyId, bool activeOnly = true);
        IQueryable<CompanyVendor> GetAllCompanyVendorsByCompanyGuid(Guid? guid, bool activeOnly = true);

        IList<SelectListItem> GetAllCompanyVendorsByCompanyIdAsSelectList(int companyId, bool activeOnly = true);

        IQueryable<CompanyVendor> GetAllCompaniesByVendorId(int vendorId);
        #endregion

        #region Method
        void SetDefaultCompanyVendor(int company);

        void DeleteAllCompanyVendorsByCompanyGuid(Guid? guid);
        #endregion
    }
}

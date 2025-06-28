using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Data;
using Wfm.Services.Franchises;
using Wfm.Services.Logging;


namespace Wfm.Services.Companies
{
    public partial class CompanyVendorService : ICompanyVendorService
    {
        #region Fields

        private readonly IRepository<CompanyVendor> _companyVendorRepository;
        private readonly IFranchiseService _franchiseService;
        private readonly ILogger _logger;
        #endregion 


        #region Ctor

        public CompanyVendorService(IRepository<CompanyVendor> companyVendorRepository, IFranchiseService franchiseService, ILogger logger)
        {
            _companyVendorRepository = companyVendorRepository;
            _franchiseService = franchiseService;
            _logger = logger;
        }

        #endregion


        #region CRUD

        public void InsertCompanyVendor(CompanyVendor companyVendor)
        {
            if (companyVendor == null)
                throw new ArgumentNullException("companyVendor");

            _companyVendorRepository.Insert(companyVendor);
        }

        public void UpdateCompanyVendor(CompanyVendor companyVendor)
        {
            if (companyVendor == null)
                throw new ArgumentException("companyVendor");
            
            _companyVendorRepository.Update(companyVendor);
        }

        public void DeleteCompanyVendor(CompanyVendor companyVendor)
        {
            if (companyVendor == null)
                throw new ArgumentException("companyVendor");

            _companyVendorRepository.Delete(companyVendor);
        }

        #endregion


        #region CompanyVendor

        public CompanyVendor GetCompanyVendorById(int id)
        {
            if (id == 0)
                return null;

            var query = _companyVendorRepository.Table;

            query = query.Where(x => x.Id == id);

            return query.FirstOrDefault();
        }


        public CompanyVendor GetCompanyVendorByComanyAndVendor(int companyId, int vendorId)
        {
            if (companyId == 0 || vendorId == 0)
                return null;

            var query = _companyVendorRepository.Table;

            query = query.Where(x => x.CompanyId == companyId && x.VendorId == vendorId);

            return query.FirstOrDefault();
        }

        #endregion


        #region LIST

        public IQueryable<CompanyVendor> GetAllCompanyVendors()
        {
            var query = _companyVendorRepository.Table;

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            return query;
        }


        public IQueryable<CompanyVendor> GetAllCompanyVendorsByCompanyId(int companyId, bool activeOnly = true)
        {
            if (companyId == 0)
                return Enumerable.Empty<CompanyVendor>().AsQueryable();
            
            var query = _companyVendorRepository.Table;

            query = query.Where(x => x.CompanyId == companyId);

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            return query.OrderByDescending(c => c.UpdatedOnUtc);
        }

        public IQueryable<CompanyVendor> GetAllCompanyVendorsByCompanyGuid(Guid? guid, bool activeOnly = true)
        {
            if (guid == null || guid==Guid.Empty)
                return Enumerable.Empty<CompanyVendor>().AsQueryable();

            var query = _companyVendorRepository.Table;

            query = query.Where(x => x.Company.CompanyGuid == guid);

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            return query.OrderByDescending(c => c.UpdatedOnUtc);
        }

        public IList<SelectListItem> GetAllCompanyVendorsByCompanyIdAsSelectList(int companyId, bool activeOnly = true)
        {
            var vendors = GetAllCompanyVendorsByCompanyId(companyId, activeOnly);

            var result = vendors.Select(x => new SelectListItem()
            {
                Text = x.Vendor.FranchiseName,
                Value = x.VendorId.ToString()
            }).ToList();

            return result;
        }

        public IQueryable<CompanyVendor> GetAllCompaniesByVendorId(int vendorId)
        {
            return _companyVendorRepository.TableNoTracking.Where(x => x.VendorId == vendorId && x.IsActive);
        }
        #endregion

        #region Method
        public void SetDefaultCompanyVendor(int companyId)
        {
            CompanyVendor defaultCompanyVendor = new CompanyVendor();
            defaultCompanyVendor.CompanyId = companyId;
            defaultCompanyVendor.VendorId = _franchiseService.GetDefaultMSPId();
            defaultCompanyVendor.IsActive = true;
            try
            {
                _companyVendorRepository.Insert(defaultCompanyVendor);
            }
            catch (Exception ex)
            {
                _logger.Error("SetDefaultCompanyVendor():fail to set default company vendor!", ex);
            }
        }

        public void DeleteAllCompanyVendorsByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var vendors = _companyVendorRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (vendors.Count() > 0)
                _companyVendorRepository.Delete(vendors);
        }
        #endregion
    }
}

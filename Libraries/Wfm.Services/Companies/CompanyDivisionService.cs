using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Data;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using System.Text;
using Wfm.Data;
using System.Data.SqlClient;


namespace Wfm.Services.Companies
{
    public partial class CompanyDivisionService : ICompanyDivisionService
    {

        #region Fields

        private readonly IRepository<CompanyLocation> _locationRepository;
        private readonly IRepository<Account> _contactRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IDbContext _dbContext;

        #endregion 

        #region Ctor

        public CompanyDivisionService(
            IRepository<CompanyLocation> locationRepository,
            IRepository<Account> contactRepository,
            IRepository<Company> companyRepository,
            IDbContext dbContext)
        {
            _locationRepository = locationRepository;
            _contactRepository = contactRepository;
            _companyRepository = companyRepository;
            _dbContext = dbContext;
        }

        #endregion


        #region CRUD

        public void InsertCompanyLocation(CompanyLocation companyLocation)
        {
            if (companyLocation == null) throw new ArgumentNullException("companyLocation");
            _locationRepository.Insert(companyLocation);
        }

        public void UpdateCompanyLocation(CompanyLocation companyLocation)
        {
            if (companyLocation == null) throw new ArgumentException("companyLocation");
            _locationRepository.Update(companyLocation);
        }

        public void DeleteCompanyLocation(CompanyLocation companyLocation)
        {
            if (companyLocation == null) throw new ArgumentException("companyLocation");
            _locationRepository.Delete(companyLocation);
        }

        #endregion

        #region CompanyLocation

        /// <summary>
        /// Gets the company location by location id.
        /// </summary>
        /// <param name="id">The location id.</param>
        /// <returns></returns>
        public CompanyLocation GetCompanyLocationById(int id)
        {
            if (id == 0)
                return null;

            var query = _locationRepository.Table;

            query = from l in query
                    where l.Id == id && l.IsDeleted==false
                    select l;

            return query.FirstOrDefault();
        }

        public Company GetCompanyByLocationId(int locationId)
        {
            if (locationId == 0)
                return null;

            var query =  _companyRepository.Table;

            query = from c in query
                    join loc in _locationRepository.Table on c.Id equals loc.CompanyId
                    where loc.Id == locationId
                    orderby c.CompanyName
                    select c;

            return query.FirstOrDefault();
        }

        public void DeleteAllCompanyLocationsByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var locations = _locationRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (locations.Count() > 0)
                _locationRepository.Delete(locations);

        }
        #endregion

        #region LIST

        /// <summary>
        /// Gets the company location by company id. DropDownList
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        public IList<CompanyLocation> GetAllCompanyLocationsByCompanyId(int? companyId, bool activeOnly = true)
        {
            if (companyId == 0 || companyId == null)
                return null;

            var query = _locationRepository.Table;

            query = from c in query
                    where c.CompanyId == companyId && c.IsDeleted==false
                    select c;

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            return query.ToList();
        }

        public IList<CompanyLocation> GetAllCompanyLocationsByCompanyGuid(Guid? companyGuid, bool activeOnly = true)
        {
            List<CompanyLocation> list = new List<CompanyLocation>();
            if (companyGuid == null || companyGuid == Guid.Empty)
                return list;
            var query = _locationRepository.Table;

            query = from c in query
                    where c.Company.CompanyGuid == companyGuid && c.IsDeleted == false
                    select c;

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            list = query.OrderByDescending(c => c.UpdatedOnUtc).ToList();

            return list;

        }


        public IList<SelectListItem> GetAllCompanyLocationsByCompanyIdAsSelectList(int? companyId, bool activeOnly = true)
        {
            var locations = GetAllCompanyLocationsByCompanyId(companyId, activeOnly);

            var result = new List<SelectListItem>();
            foreach (var l in locations)
            {
                var item = new SelectListItem()
                {
                    Text = l.LocationName,
                    Value = l.Id.ToString()
                };

                result.Add(item);
            }

            return result;
        }

        public IList<SelectListItem> GetAllCompanyLocationsByCompanyGuidAsSelectList(Guid? companyGuid, bool activeOnly = true)
        {
            var locations = GetAllCompanyLocationsByCompanyGuid(companyGuid, activeOnly);

            var result = locations.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.LocationName }).ToList();

            return result;
        }

        public IList<CompanyLocation> SQL_GetAllCompanyLocationsByCompanyId(int? companyId, bool activeOnly = true)
        {
            if (companyId == 0 || companyId == null)
                return null;

            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT   cl.Id 
                                  , cl.CompanyId 
                                  , cl.LocationName 
                                  , cl.PrimaryPhone 
                                  , cl.SecondaryPhone 
                                  , cl.FaxNumber 
                                  , cl.UnitNumber 
                                  , cl.AddressLine1 
                                  , cl.AddressLine2 
                                  , cl.AddressLine3 
                                  , city.CityName as City
                                  , StateProvince.Abbreviation as StateProvince
                                  , country.CountryName as Country
                                  , cl.PostalCode 
                                  , cl.IsActive 
                                  , cl.IsDeleted 
                                  , cl.EnteredBy 
                                  , cl.DisplayOrder 
                                  , cl.CreatedOnUtc 
                                  , cl.UpdatedOnUtc 
                                  , cl.CountryId 
                                  , cl.StateProvinceId 
                                  , cl.CityId 
                                  , cl.LastPunchClockFileUploadDateTimeUtc 
                                  , cl.LastWorkTimeCalculationDateTimeUtc 
                              FROM CompanyLocation cl
                               left outer join Country on cl.CountryId = country.Id 
                               left outer join StateProvince on cl.StateProvinceId = StateProvince.Id
                               left outer join City on cl.CityId = city.Id 
                              Where cl.IsDeleted = 0  and cl.CompanyId = @Id ");


            if (activeOnly)
                query = query.Append(" and cl.IsActive = 1 "); 

            query.Append(" Order by cl.UpdatedOnUtc desc");

            var data = _dbContext.SqlQuery<CompanyLocation>(query.ToString(), new SqlParameter("Id", companyId));
            List<CompanyLocation> result = data.ToList<CompanyLocation>();

            return result;
        }
        public IList<CompanyLocation> GetAllCompanyLocationsByAccount(Account account, bool showInactive = false, bool showHidden = false)
        {
            var query = _locationRepository.Table;

            // query within company
            query = query.Where(cl => cl.CompanyId == account.CompanyId);

            // active
            if (!showInactive)
                query = query.Where(cl => cl.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(cl => cl.IsDeleted == false);


            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}
                
            // locations for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(cl => 
                    cl.Id == account.CompanyLocationId); // search within locatin
            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(cl =>
                    cl.Id == account.CompanyLocationId);
            else if(account.IsCompanyDepartmentManager())
                query = query.Where(cl =>
                    cl.Id == account.CompanyLocationId);
            else
                return null; // No role


            query = from b in query
                    orderby b.UpdatedOnUtc descending
                    select b;


            return query.ToList();
        }

        /// <summary>
        /// Gets all company locations. GridView
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IPagedList<CompanyLocation> GetAllCompanyLocations(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _locationRepository.Table;

            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            IPagedList<CompanyLocation> companyLocations = new PagedList<CompanyLocation>(query, pageIndex, PageSize);
            return companyLocations;
        }

        #endregion
  
    }
}

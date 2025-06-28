using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Caching;
using System.Web.Mvc;
using Wfm.Core.Domain.Companies;
using System.Data.Entity;
using System.Drawing;
using System.IO;

namespace Wfm.Services.Franchises
{
    public partial class FranchiseService : IFranchiseService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string FRANCHISES_ALL_KEY = "Wfm.franchise.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string FRANCHISES_PATTERN_KEY = "Wfm.franchise.";

        #endregion

        #region Fields

        private readonly IRepository<Franchise> _franchiseRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyVendor> _companyVendorRepository;

        #endregion

        #region Ctor

        public FranchiseService(ICacheManager cacheManager,
            IRepository<Franchise> franchiseRepository,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<Company> companyRepository,
            IRepository<CompanyVendor> companyVendorRepository
            )
        {
            _cacheManager = cacheManager;
            _franchiseRepository = franchiseRepository;
            _jobOrderRepository = jobOrderRepository;
            _companyRepository = companyRepository;
            _companyVendorRepository = companyVendorRepository;
        }

        #endregion

        #region CRUD

        public void InsertFranchise(Franchise franchise)
        {
            if (franchise == null)
                throw new ArgumentNullException("franchise");

            _franchiseRepository.Insert(franchise);

            //cache
            _cacheManager.RemoveByPattern(FRANCHISES_PATTERN_KEY);
        }

        public void UpdateFranchise(Franchise franchise)
        {
            if (franchise == null)
                throw new ArgumentNullException("franchise");

            _franchiseRepository.Update(franchise);

            //cache
            _cacheManager.RemoveByPattern(FRANCHISES_PATTERN_KEY);
        }

        public void DeleteFranchise(Franchise franchise)
        {
            if (franchise == null)
                throw new ArgumentNullException("franchise");

            franchise.IsDeleted = true;
            _franchiseRepository.Update(franchise);

            //cache
            _cacheManager.RemoveByPattern(FRANCHISES_PATTERN_KEY);
        }

        #endregion

        #region Franchise

        public Franchise GetFranchiseById(int id)
        {
            if (id == 0)
                return null;

            return _franchiseRepository.GetById(id);
        }

        public Franchise GetFranchiseByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            return _franchiseRepository.Table.Where(x => x.FranchiseGuid == guid).FirstOrDefault();
        }

        public bool IsMSP(int id)
        {
            Franchise franchise = GetFranchiseById(id);
            if (franchise != null)
            {
                return franchise.IsDefaultManagedServiceProvider;
            }
            else
                return false;
        }

        public int GetDefaultMSPId()
        {
            var franchise = _franchiseRepository.Table.Where(x => x.IsDefaultManagedServiceProvider).FirstOrDefault();
            if (franchise == null)
            {
                var company = _companyRepository.TableNoTracking.Where(x => x.IsAdminCompany).FirstOrDefault();
                if (company == null)
                    return 0;
                else
                    return company.Id;
            }
            return franchise.Id;
        }

        public string GetDefaultMSPName()
        {
            var franchise = _franchiseRepository.TableNoTracking.Where(x => x.IsDefaultManagedServiceProvider).FirstOrDefault();
            if (franchise == null)
            {
                var company = _companyRepository.TableNoTracking.Where(x => x.IsAdminCompany).FirstOrDefault();
                if (company == null)
                    return null;
                else
                    return company.CompanyName;
            }
            return franchise.ShortName;
        }

        public Franchise GetPublicFranchise()
        {
            var result = _franchiseRepository.Table.Where(x => x.IsLinkToPublicSite).FirstOrDefault();
            if (result == null)
                result = _franchiseRepository.Table.Where(x => x.IsDefaultManagedServiceProvider).FirstOrDefault();
            if (result == null)
                result = _franchiseRepository.Table.FirstOrDefault();
            return result;
        }
        #endregion

        #region LIST

        /// <summary>
        /// Gets all franchises.
        /// </summary>
        /// <param name="showInactive">if set to <c>true</c> [show inactive].</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IList<Franchise> GetAllFranchises(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(FRANCHISES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _franchiseRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(f => f.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(f => f.IsDeleted == false);

                query = query.OrderBy(f => f.DisplayOrder).ThenBy(f => f.FranchiseName);

                return query.ToList();
            });

        }

        /// <summary>
        /// Gets all franchises as queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showInactive">if set to <c>true</c> [show inactive].</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IQueryable<Franchise> GetAllFranchisesAsQueryable(Account account, bool showInactive = false, bool showHidden = false, bool defaultSortOrder = true)
        {
            var query = _franchiseRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(f => f.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(f => f.IsDeleted == false);

            if (account != null && account.IsLimitedToFranchises)
            {
                if (account.IsClientAccount)
                {
                    var Ids = _companyVendorRepository.TableNoTracking.Where(x => x.CompanyId == account.CompanyId).Select(x => x.VendorId).Distinct();
                    query = query.Where(f => Ids.Contains(f.Id));
                }
                else
                    query = query.Where(f => f.Id == account.FranchiseId);
            }

            if (defaultSortOrder)
                query = query.OrderBy(f => f.DisplayOrder).ThenByDescending(f => f.UpdatedOnUtc);

            return query.AsQueryable();
        }

        /// <summary>
        /// Gets all franchises.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showInactive">if set to <c>true</c> [show inactive].</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IList<Franchise> GetAllFranchises(Account account = null, bool showInactive = false, bool showHidden = false)
        {
            if (account == null)
                return null;

            return GetAllFranchisesAsQueryable(account, showInactive, showHidden).ToList();
        }


        /// <summary>
        /// Gets all vendors as queryable (MSP is excluded from the list).
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showInactive">if set to <c>true</c> [show inactive].</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IQueryable<Franchise> GetAllVendorsAsQueryable(Account account, bool showInactive = false, bool showHidden = false)
        {
            if (account == null)
                return null;

            var query = this.GetAllFranchisesAsQueryable(account, showInactive, showHidden).AsNoTracking();

            // exclude MSP
            query = query.Where(x => !x.IsDefaultManagedServiceProvider);

            return query.AsQueryable();
        }

        public IList<System.Web.Mvc.SelectListItem> GetAllFranchisesAsSelectList(Account account, bool showInactive = false, bool showHidden = false, bool idVal = true)
        {
            if (account == null)
                return null;

            var query = this.GetAllFranchisesAsQueryable(account, showInactive, showHidden, false)
                            .OrderBy(f => f.FranchiseName)
                            .AsNoTracking();

            var result = query.Select(x => new SelectListItem
            {
                Value = idVal ? x.Id.ToString() : x.FranchiseName,
                Text = x.FranchiseName
            }).ToList();

            return result;
        }

        /// <summary>
        /// Gets all vendors as select list (MSP is excluded from the list).
        /// </summary>
        /// <param name="account"></param>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<System.Web.Mvc.SelectListItem> GetAllVendorsAsSelectList(Account account, bool showInactive = false, bool showHidden = false)
        {
            if (account == null)
                return null;

            var query = this.GetAllFranchisesAsQueryable(account, showInactive, showHidden, false)
                            .OrderBy(f => f.FranchiseName)
                            .AsNoTracking();

            // exclude MSP
            query = query.Where(x => !x.IsDefaultManagedServiceProvider);

            var result = query.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FranchiseName }).ToList();

            return result;
        }

        public byte[] GetPublicFranchiseLogo()
        {
            var franchise = GetPublicFranchise();
            if (franchise == null)
                return null;
            else
                return franchise.FranchiseLogo;
        }
        #endregion

        #region Franchise Logo
        public Image GetFranchiseLogo(int franchiseId)
        {
            var franchise = GetFranchiseById(franchiseId);
            if (franchise != null && franchise.FranchiseLogo != null)
            {
                using (MemoryStream stream = new MemoryStream(franchise.FranchiseLogo))
                {
                    Image logo = Image.FromStream(stream);
                    return logo;
                }
            }
            return null;
        }
        #endregion

    }
}

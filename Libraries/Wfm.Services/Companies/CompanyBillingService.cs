using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Core;

namespace Wfm.Services.Companies
{
    public partial class CompanyBillingService : ICompanyBillingService
    {
        #region Fields

        private readonly IRepository<CompanyBillingRate> _companyBillingRepository;

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyBillingService"/> class.
        /// </summary>
        /// <param name="CompanyBillingRepository">The CompanyBilling repository.</param>
        public CompanyBillingService(
            IRepository<CompanyBillingRate> CompanyBillingRepository
            )
        {
            _companyBillingRepository = CompanyBillingRepository;
        }

        #endregion

        #region CRUD
        /// <summary>
        /// Inserts the CompanyBilling.
        /// </summary>
        /// <param name="companyBillingRate">The CompanyBilling.</param>
        /// <exception cref="System.ArgumentNullException">CompanyBilling</exception>
        public virtual void Insert(CompanyBillingRate companyBillingRate)
        {
            if (companyBillingRate == null)
                throw new ArgumentNullException("companyBillingRate");

            companyBillingRate.CreatedOnUtc = DateTime.UtcNow;
            companyBillingRate.UpdatedOnUtc = companyBillingRate.CreatedOnUtc;

            _companyBillingRepository.Insert(companyBillingRate);
        }


        /// <summary>
        /// Updates the CompanyBilling.
        /// </summary>
        /// <param name="companyBillingRate">The CompanyBilling.</param>
        /// <exception cref="System.ArgumentNullException">CompanyBilling</exception>
        public virtual void Update(CompanyBillingRate companyBillingRate)
        {
            if (companyBillingRate == null)
                throw new ArgumentNullException("companyBillingRate");

            companyBillingRate.UpdatedOnUtc = DateTime.UtcNow;

            _companyBillingRepository.Update(companyBillingRate);
        }


        /// <summary>
        /// Deletes the CompanyBilling.
        /// </summary>
        /// <param name="companyBillingRate">The CompanyBilling.</param>
        /// <exception cref="System.ArgumentNullException">CompanyBilling</exception>
        public virtual void Delete(CompanyBillingRate companyBillingRate)
        {
            if (companyBillingRate == null)
                throw new ArgumentNullException("companyBillingRate");

            companyBillingRate.IsActive = false;
            companyBillingRate.IsDeleted = true;
            companyBillingRate.UpdatedOnUtc = DateTime.UtcNow;

            _companyBillingRepository.Update(companyBillingRate);
        }

        #endregion


        #region CompanyBillingRate

        /// <summary>
        /// Gets a CompanyBilling
        /// </summary>
        /// <param name="CompanyBillingRateId"></param>
        /// <returns>CompanyBilling</returns>
        public CompanyBillingRate GetCompanyBillingRateById(int Id)
        {
            if (Id == 0)
                return null;

            return _companyBillingRepository.GetById(Id);
        }

        #endregion


        #region LIST

        public IList<CompanyBillingRate> GetAllCompanyBillingRatesByIds(string selectedIds)
        {
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                IList<CompanyBillingRate> result = _companyBillingRepository.Table.Where(x => ids.Contains(x.Id)).ToList();
                return result;
            }
            return null;
            
        }


        /// <summary>
        /// Gets all companies.
        /// </summary>
        /// <returns></returns>
        public IList<CompanyBillingRate> GetAllCompanyBillingRates()
        {
            var query = _companyBillingRepository.Table;

            query = query.Where(c => c.IsActive == true);
            query = query.Where(c => c.IsDeleted == false);

            query = from c in query
                    orderby c.DisplayOrder, c.UpdatedOnUtc descending
                    select c;

            return query.ToList();
        }

        public IList<CompanyBillingRate> GetAllCompanyBillingRatesByCompanyId(int id)
        {
            var query = _companyBillingRepository.Table;

            query = query.Where(c => c.IsActive == true);
            query = query.Where(c => c.IsDeleted == false);
            query = query.Where(c => c.CompanyId == id);

            query = from c in query
                    orderby c.DisplayOrder, c.UpdatedOnUtc descending
                    select c;

            return query.ToList();
        }

        public IList<CompanyBillingRate> GetAllCompanyBillingRatesByCompanyIdAndRefDate(int id, DateTime? refDate)
        {
            var query = _companyBillingRepository.Table;

            refDate = refDate ?? DateTime.Today; 

            query = query.Where(c => c.IsActive == true);
            query = query.Where(c => c.IsDeleted == false);
            query = query.Where(c => c.CompanyId == id);          
            query = query.Where(x => x.EffectiveDate <= refDate);
            query = query.Where(x => x.DeactivatedDate == null || x.DeactivatedDate >= refDate);

            query = from c in query
                    orderby c.DisplayOrder, c.UpdatedOnUtc descending
                    select c;

            return query.ToList();
        }

        public CompanyBillingRate GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(int companyId, int locationId, string rateCode, DateTime? date)
        {
            if (date == null)
            {
                date = DateTime.Today;
            }
            var query = _companyBillingRepository.Table;
            query = query.Where(x => x.IsActive == true &&
                                     x.IsDeleted == false &&
                                     x.CompanyId == companyId &&
                                     x.CompanyLocationId == locationId &&
                                     x.RateCode == rateCode &&
                                     x.EffectiveDate <= date);
            query = query.Where(x => x.DeactivatedDate == null ? true : x.DeactivatedDate >= date);

            var result =  query.FirstOrDefault();
            return result;
        }


        public IQueryable<CompanyBillingRate> GetCompanyBillingRatesByCompanyIdAndRateCodeAndDate(int companyId, int positionId, string shiftCode, DateTime? refDate = null)
        {
            var query = _companyBillingRepository.TableNoTracking;

            query = query.Where(x => x.IsActive == true && x.IsDeleted == false); 
            query = query.Where(x => x.CompanyId == companyId  && x.PositionId == positionId && x.ShiftCode == shiftCode);
            query = query.Where(x => x.EffectiveDate <= refDate && (!x.DeactivatedDate.HasValue || x.DeactivatedDate >= refDate));

            return query;
        }


        public IQueryable<CompanyBillingRate> GetCompanyBillingRatesByCompanyIdAndLocationIdAndRateCodeAndDate(int companyId, int locationId, int positionId, string shiftCode, DateTime? refDate = null)
        {
            var query = _companyBillingRepository.TableNoTracking;

            query = query.Where(x => x.IsActive == true && x.IsDeleted == false);
            query = query.Where(x => x.CompanyId == companyId && x.CompanyLocationId == locationId && x.PositionId == positionId && x.ShiftCode == shiftCode);
            query = query.Where(x => x.EffectiveDate <= refDate && (!x.DeactivatedDate.HasValue || x.DeactivatedDate >= refDate));

            return query;
        }

        /// <summary>
        /// Gets all companies.
        /// </summary>
        /// <param name="FilterCols">The filter cols.</param>
        /// <param name="FiltersCommand">The filters command.</param>
        /// <param name="SortCommand">The sort command.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        // public IPagedList<CompanyBilling> GetAllCompanyBilling(List<string> FilterCols = null, List<string> FiltersCommand = null, string SortCommand = "", int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false)
        public IPagedList<CompanyBillingRate> GetAllCompanyBillingRates(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _companyBillingRepository.Table; //no where filter. get all records.

            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = query.OrderBy(b => b.CompanyId);

            int total = _companyBillingRepository.Table.Count(); // counting total records

            IPagedList<CompanyBillingRate> companies = new PagedList<CompanyBillingRate>(query, pageIndex, PageSize, total);
            return companies;
        }


        public IQueryable<CompanyBillingRate> GetAllCompanyBillingRatesAsQueryable(bool showHidden = false)
        {
            var query = _companyBillingRepository.Table;

            if (!showHidden)
                query = query.Where(b => !b.IsDeleted);


            query = from b in query
                    orderby b.CompanyId,b.RateCode, b.EffectiveDate descending
                    select b;

            return query.AsQueryable();
        }


        public IQueryable<CompanyBillingRate> GetBillingRatesUpdated(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date.ToUniversalTime();
            endDate = endDate.Date.AddDays(1).ToUniversalTime();    // next day

            var query = this.GetAllCompanyBillingRatesAsQueryable();
            query = query.Where(x => (x.CreatedOnUtc >= startDate && x.CreatedOnUtc <= endDate) ||
                                     (x.UpdatedOnUtc >= startDate && x.UpdatedOnUtc <= endDate));

            return query;
        }

        #endregion


        #region Delete billing rate
        public void DeleteCompanyBillingRatesByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var billingRates = _companyBillingRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (billingRates.Count() > 0)
                _companyBillingRepository.Delete(billingRates);
        }
        #endregion
    }
}

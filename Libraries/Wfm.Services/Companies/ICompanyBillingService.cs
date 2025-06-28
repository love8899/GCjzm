using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Companies;
using Wfm.Core;
using System;

namespace Wfm.Services.Companies

{
    public partial interface ICompanyBillingService
    {
        #region CRUD

        void Insert(CompanyBillingRate companyBillingRate);
        void Update(CompanyBillingRate companyBillingRate);
        void Delete(CompanyBillingRate companyBillingRate);

        #endregion

        #region CompanyBillingRate

        CompanyBillingRate GetCompanyBillingRateById(int id);

        #endregion

        #region LIST

        IList<CompanyBillingRate> GetAllCompanyBillingRatesByIds(string selectedIds);
        IList<CompanyBillingRate> GetAllCompanyBillingRates();
        IList<CompanyBillingRate> GetAllCompanyBillingRatesByCompanyId(int id);
        IList<CompanyBillingRate> GetAllCompanyBillingRatesByCompanyIdAndRefDate(int id, DateTime? refDate);

        IPagedList<CompanyBillingRate> GetAllCompanyBillingRates(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false);

        IQueryable<CompanyBillingRate> GetAllCompanyBillingRatesAsQueryable(bool showHidden = false);

        IQueryable<CompanyBillingRate> GetBillingRatesUpdated(DateTime startDate, DateTime endDate);

        CompanyBillingRate GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(int companyId, int locationId, string rateCode, DateTime? date);
       

        IQueryable<CompanyBillingRate> GetCompanyBillingRatesByCompanyIdAndRateCodeAndDate(int companyId, int positionId, string shiftCode, DateTime? refDate = null);

        IQueryable<CompanyBillingRate> GetCompanyBillingRatesByCompanyIdAndLocationIdAndRateCodeAndDate(int companyId, int locationId, int positionId, string shiftCode, DateTime? refDate = null);

        #endregion

        #region Delete billing rate 
        void DeleteCompanyBillingRatesByCompanyGuid(Guid? guid);
        #endregion
    }
}

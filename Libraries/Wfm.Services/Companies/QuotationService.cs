using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public partial class QuotationService : IQuotationService
    {
        #region Fields

        private readonly IRepository<Quotation> _quotationRepository;

        #endregion


        #region Ctor

        public QuotationService(IRepository<Quotation> quotationRepository)
        {
            _quotationRepository = quotationRepository;
        }

        #endregion


        public void InsertQuotation(Quotation quotation)
        {
            if (quotation == null)
                throw new ArgumentNullException("quotation");

            _quotationRepository.Insert(quotation);
        }


        public void DeleteQuotation(Quotation quotation)
        {
            if (quotation == null)
                throw new ArgumentNullException("quotation");

            _quotationRepository.Delete(quotation);
        }


        public Quotation GetQuotationById(int id)
        {
            if (id == 0)
                return null;

            return _quotationRepository.GetById(id);
        }


        public IQueryable<Quotation> GetAllQuotationsByBillingRateId(int billingRateId)
        {
            var query = _quotationRepository.TableNoTracking;

            return query.Where(x => x.CompanyBillingRateId == billingRateId);
        }

    }
}

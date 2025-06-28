using System.Linq;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public partial interface IQuotationService
    {
        void InsertQuotation(Quotation quotation);

        void DeleteQuotation(Quotation quotation);

        Quotation GetQuotationById(int id);

        IQueryable<Quotation> GetAllQuotationsByBillingRateId(int billingRateId);
    }
}

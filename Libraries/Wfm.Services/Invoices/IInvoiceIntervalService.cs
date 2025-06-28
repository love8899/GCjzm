using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Invoices;


namespace Wfm.Services.Invoices
{
    public partial interface IInvoiceIntervalService
    {
        void InsertInvoiceInterval(InvoiceInterval salutation);

        void UpdateInvoiceInterval(InvoiceInterval salutation);

        void DeleteInvoiceInterval(InvoiceInterval salutation);

        InvoiceInterval GetInvoiceIntervalById(int id);

        int GetInvoiceIntervalIdByCode(string code);

        IQueryable<InvoiceInterval> GetAllInvoiceIntervals(bool tracking = false);
    }
}

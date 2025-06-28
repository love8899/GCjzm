using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Invoices;
using Wfm.Core.Data;
using Wfm.Core.Caching;
using Wfm.Services.Events;


namespace Wfm.Services.Invoices
{
    public partial class InvoiceIntervalService : IInvoiceIntervalService
    {
        #region Fields

        private readonly IRepository<InvoiceInterval> _invoiceIntervalRepository;

        #endregion


        #region Ctor

        public InvoiceIntervalService(IRepository<InvoiceInterval> invoiceIntervalRepository)
        {
            _invoiceIntervalRepository = invoiceIntervalRepository;
        }

        #endregion


        #region CRUD

        public void InsertInvoiceInterval(InvoiceInterval invoiceInterval)
        {
            if (invoiceInterval == null)
                throw new ArgumentNullException("invoiceInterval");

            _invoiceIntervalRepository.Insert(invoiceInterval);
        }

        public void UpdateInvoiceInterval(InvoiceInterval invoiceInterval)
        {
            if (invoiceInterval == null)
                throw new ArgumentNullException("invoiceInterval");

            _invoiceIntervalRepository.Update(invoiceInterval);
        }


        public void DeleteInvoiceInterval(InvoiceInterval invoiceInterval)
        {
            if (invoiceInterval == null)
                throw new ArgumentNullException("invoiceInterval");

            _invoiceIntervalRepository.Update(invoiceInterval);
        }

        #endregion


        #region InvoiceInterval

        public InvoiceInterval GetInvoiceIntervalById(int id)
        {
            if (id == 0)
                return null;

            return _invoiceIntervalRepository.GetById(id);
        }


        public int GetInvoiceIntervalIdByCode(string code)
        {
            if (String.IsNullOrEmpty(code))
                return 0;

            var invoiceInterval = _invoiceIntervalRepository.Table.Where(x => x.Code.ToLower() == code.ToLower()).FirstOrDefault();

            return invoiceInterval != null ? invoiceInterval.Id : 0;
        }

        #endregion


        #region LIST

        public IQueryable<InvoiceInterval> GetAllInvoiceIntervals(bool tracking = false)
        {
            return tracking ? _invoiceIntervalRepository.Table : _invoiceIntervalRepository.TableNoTracking;
        }

        #endregion
    }
}

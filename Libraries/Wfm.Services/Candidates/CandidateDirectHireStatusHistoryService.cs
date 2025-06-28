using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.Candidates
{
    public partial class CandidateDirectHireStatusHistoryService :ICandidateDirectHireStatusHistoryService
    {

        #region Fields

        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<CandidateDirectHireStatusHistory> _candidateDirectHireStatusHistoriesRepository;

        #endregion

        #region Ctors
        public CandidateDirectHireStatusHistoryService(
            IRepository<JobOrder> jobOrderRepository,
            IRepository<CandidateDirectHireStatusHistory> candidateDirectHireStatusHistoriesRepository
            )
        {
            _jobOrderRepository = jobOrderRepository;
            _candidateDirectHireStatusHistoriesRepository = candidateDirectHireStatusHistoriesRepository;
        }
        #endregion


        #region CRUD

        public void InsertCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistory candidateDirectHireStatusHistory)
        {
            if (candidateDirectHireStatusHistory == null) 
                throw new ArgumentNullException("candidateDirectHireStatusHistory");

            _candidateDirectHireStatusHistoriesRepository.Insert(candidateDirectHireStatusHistory);
        }

        public void UpdateCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistory candidateDirectHireStatusHistory)
        {
            if (candidateDirectHireStatusHistory == null)
                throw new ArgumentNullException("candidateDirectHireStatusHistory");

            candidateDirectHireStatusHistory.UpdatedOnUtc = DateTime.UtcNow;
            _candidateDirectHireStatusHistoriesRepository.Update(candidateDirectHireStatusHistory);
        }
        #endregion

        #region CandidateDirectHireStatusHistory
        public CandidateDirectHireStatusHistory GetCandidateDirectHireStatusHistoryById(int id)
        {
            if (id == 0)
                return null;

            return _candidateDirectHireStatusHistoriesRepository.GetById(id);
        }
        #endregion


        #region LIST

        public IQueryable<CandidateDirectHireStatusHistory> GetCandidateDirectHireStatusHistoryByCandidateAndJobOrderId(int candidateId, int jobOrderId)
        {
            var query = _candidateDirectHireStatusHistoriesRepository.Table;

            query = from c in query
                    where c.CandidateId == candidateId && c.JobOrderId==jobOrderId
                    orderby c.CreatedOnUtc descending
                    select c;

            return query.AsQueryable();
        }

        public IQueryable<CandidateDirectHireStatusHistory> GetAllCandidateDirectHireStatusHistoriesAsQueryable()
        {
            var query = _candidateDirectHireStatusHistoriesRepository.Table;

            query = from c in query
                    orderby c.CreatedOnUtc descending
                    select c;

            return query.AsQueryable();
        }

        #endregion


        #region Invoice

        public decimal GetDirectHireJobOrderFeeAmount(JobOrder jobOrder, decimal? salary)
        {
            var result = 0m;

            if (jobOrder.FeeAmount.HasValue && jobOrder.FeeAmount > 0)
                result = jobOrder.FeeAmount.Value;
            else if (jobOrder.FeePercent.HasValue && salary.HasValue)
                    result = salary.Value * jobOrder.FeePercent.Value / 100m;

            return result;
        }


        public void SetDirectHireInvoiceDate(IList<CandidateDirectHireStatusHistory> histories, DateTime invoiceDate)
        {
            foreach(var h in histories)
            {
                h.InvoiceDate = invoiceDate;

                this.UpdateCandidateDirectHireStatusHistory(h);
            }
        }

        #endregion

    }
}

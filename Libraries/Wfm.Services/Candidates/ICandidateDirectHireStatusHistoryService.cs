using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateDirectHireStatusHistoryService
    {
        #region CRUD

        void InsertCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistory candidateDirectHireStatusHistory);
        void UpdateCandidateDirectHireStatusHistory(CandidateDirectHireStatusHistory candidateDirectHireStatusHistory);
        #endregion

        #region CandidateJobOrderStatusHistory

        CandidateDirectHireStatusHistory GetCandidateDirectHireStatusHistoryById(int id);
     
        #endregion

        #region LIST 

        IQueryable<CandidateDirectHireStatusHistory> GetCandidateDirectHireStatusHistoryByCandidateAndJobOrderId(int candidateId, int jobOrderId);
        IQueryable<CandidateDirectHireStatusHistory> GetAllCandidateDirectHireStatusHistoriesAsQueryable();

        #endregion


        #region Invoice

        decimal GetDirectHireJobOrderFeeAmount(JobOrder jobOrder, decimal? salary);

        void SetDirectHireInvoiceDate(IList<CandidateDirectHireStatusHistory> histories, DateTime invoiceDate);
        
        #endregion

    }
}

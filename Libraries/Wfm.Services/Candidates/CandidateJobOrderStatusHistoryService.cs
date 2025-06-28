using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.Candidates
{
    public partial class CandidateJobOrderStatusHistoryService :ICandidateJobOrderStatusHistoryService
    {

        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<CandidateJobOrderStatusHistory> _candidateJobOrderStatusHistoriesRepository;

        #endregion

        #region Ctors
        public CandidateJobOrderStatusHistoryService(IWorkContext workContext,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<CandidateJobOrderStatusHistory> candidateJobOrderStatusHistories
            )
        {
            _workContext = workContext;
            _jobOrderRepository = jobOrderRepository;
            _candidateJobOrderStatusHistoriesRepository = candidateJobOrderStatusHistories;
        }
        #endregion


        #region CRUD

        #endregion

        #region CandidateJobOrderStatusHistory

        /// <summary>
        /// Gets CandidateJobOrderStatusHistory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>CandidateJobOrderStatusHistory</returns>
        public CandidateJobOrderStatusHistory GetCandidateJobOrderStatusHistoryById(int id)
        {
            if (id == 0)
                return null;

            return _candidateJobOrderStatusHistoriesRepository.GetById(id);
        }


        public CandidateJobOrderStatusHistory GetLastCandidateJobOrderStatusHistoryByCandidateIdAndJobOrderId(int candidateId, int jobOrderId)
        {
            var query = _candidateJobOrderStatusHistoriesRepository.Table;

            query = from c in query
                    join jo in _jobOrderRepository.Table on c.JobOrderId equals jo.Id
                    where c.CandidateId == candidateId && c.JobOrderId == jobOrderId && jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active
                    orderby c.CreatedOnUtc descending // get latest status history
                    select c;

            return query.FirstOrDefault();
        }


        public CandidateJobOrderStatusHistory GetLastCandidateJobOrderStatusHistoryByCandidateIdAndJobOrderIdAndJobStartDate(int candidateId, int jobOrderId, DateTime jobStartDate)
        {
            var query = _candidateJobOrderStatusHistoriesRepository.Table;

            query = from c in query
                    join jo in _jobOrderRepository.Table on c.JobOrderId equals jo.Id
                    where jo.JobOrderStatusId == (int)JobOrderStatusEnum.Active &&
                          c.CandidateId == candidateId &&
                          c.JobOrderId == jobOrderId &&
                          (!jo.EndDate.HasValue || 
                            (jobStartDate.Date < jo.EndDate || (jobStartDate.Date == jo.EndDate && jobStartDate.Hour <= jo.EndTime.Hour)))
                          
                    orderby c.CreatedOnUtc descending // get latest status history
                    select c;

            return query.FirstOrDefault();
        }

        #endregion


        #region LIST

        public IList<CandidateJobOrderStatusHistory> GetCandidateJobOrderStatusHistoryByCandidateIdAndJobOrderId(int candidateId, int jobOrderId)
        {
            var query = _candidateJobOrderStatusHistoriesRepository.Table;

            query = from c in query
                    where c.CandidateId == candidateId && c.JobOrderId == jobOrderId
                    orderby c.CreatedOnUtc
                    select c;

            return query.ToList();
        }


        public IList<CandidateJobOrderStatusHistory> GetCandidateJobOrderStatusHistoryByJobOrderId(int jobOrderId)
        {
            var query = _candidateJobOrderStatusHistoriesRepository.Table;
            query = from c in query
                    where c.JobOrderId == jobOrderId
                    orderby c.CreatedOnUtc descending
                    select c;

            return query.ToList();
        }

        public IList<CandidateJobOrderStatusHistory> GetLastCandidateJobOrderStatusByJobOrderId(int jobOrderId)
        {
            var query = _candidateJobOrderStatusHistoriesRepository.Table;

            //query = from c in query
            //        where c.JobOrderId == jobOrderId
            //        orderby c.CreatedOnUtc descending
            //        group c by c.CandidateId into grp
            //        let LastStatusPerCandidate = grp.Max ( g=>g.CreatedOnUtc )
            //        from g in grp
            //        where g.CreatedOnUtc == LastStatusPerCandidate
            //        select g;

            query = from c in query
                    where c.JobOrderId == jobOrderId
                    group c by c.CandidateId into grp
                    select grp.OrderByDescending(g => g.CreatedOnUtc).FirstOrDefault();

            return query.ToList();
        }



        public IQueryable<CandidateJobOrderStatusHistory> GetCandidateJobOrderStatusHistoryByCandidateIdAsQueryable(int candidateId,bool includeDirectHire=false)
        {
            var query = _candidateJobOrderStatusHistoriesRepository.TableNoTracking;

            query = query.Where(x => x.CandidateId == candidateId).Distinct()
                         .OrderBy(x => x.StartDate.Value).ThenBy(x => x.EndDate ?? DateTime.MinValue);
                         
            if (!includeDirectHire)
                query = query.Where(c => !c.JobOrder.JobOrderType.IsDirectHire);

            return query.AsQueryable();
        }

        #endregion

    }
}

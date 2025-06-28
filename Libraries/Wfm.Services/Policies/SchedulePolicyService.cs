using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Policies;


namespace Wfm.Services.Policies
{
    public partial class SchedulePolicyService : ISchedulePolicyService
    {
        #region Fields

        private readonly IRepository<SchedulePolicy> _schedulePolicyRepository;
        private readonly IRepository<JobOrder> _jobOrders;

        #endregion

        #region Ctor

        public SchedulePolicyService(                             
            IRepository<SchedulePolicy> schedulePolicyRepository,
            IRepository<JobOrder> jobOrders
            )
        {
            _schedulePolicyRepository = schedulePolicyRepository;
            _jobOrders = jobOrders;
        }

        #endregion 


        #region CRUD

        public void Insert(SchedulePolicy schedulePolicy)
        {
            if (schedulePolicy == null)
                throw new ArgumentNullException("schedulePolicy");

            _schedulePolicyRepository.Insert(schedulePolicy);
        }

        public void Update(SchedulePolicy schedulePolicy)
        {
            if (schedulePolicy == null)
                throw new ArgumentNullException("schedulePolicy");

            _schedulePolicyRepository.Update(schedulePolicy);
        }

        #endregion


        #region SchedulePolicy

        public SchedulePolicy GetSchedulePolicyById(int id)
        {
            if (id == 0)
                return null;

            return _schedulePolicyRepository.GetById(id);
        }

        public SchedulePolicy GetSchedulePolicieByJobOrderId(int id)
        {
            if (id == 0)
                return null;

            var query = _schedulePolicyRepository.Table;
            var jobOrder = _jobOrders.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();

            return jobOrder != null ? query.Where(x => x.Id == jobOrder.SchedulePolicyId).FirstOrDefault() : null;
        }

        public IList<SchedulePolicy> GetSchedulePoliciesByCompanyId(int id)
        {
            if (id == 0)
                return null;

            var query = _schedulePolicyRepository.Table;

            query = from i in query
                    where i.CompanyId == id
                    select i;

            return query.ToList();
        }

        public IQueryable<SchedulePolicy> GetAllSchedulePoliciesAsQueryable(bool showHidden = false)
        {
            var query = _schedulePolicyRepository.Table;

            if (!showHidden)
                query = query.Where(p => !p.IsDeleted);

            query = from s in query
                    orderby s.UpdatedOnUtc
                    select s;

            return query.AsQueryable();
        }

        #endregion

    }
}

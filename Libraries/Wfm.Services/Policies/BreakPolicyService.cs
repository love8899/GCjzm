using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial class BreakPolicyService : IBreakPolicyService
    {
        #region Fields

        private readonly IRepository<BreakPolicy> _breakPolicyRepository;

        #endregion

        #region Ctor

        public BreakPolicyService(                             
            IRepository<BreakPolicy> breakPolicyRepository
            )
        {
            _breakPolicyRepository = breakPolicyRepository;
        }

        #endregion 


        #region CRUD

        public void Insert(BreakPolicy breakPolicy)
        {
            if (breakPolicy == null)
                throw new ArgumentNullException("breakPolicy");

            //insert
            _breakPolicyRepository.Insert(breakPolicy);
        }

        public void Update(BreakPolicy breakPolicy)
        {
            if (breakPolicy == null)
                throw new ArgumentNullException("breakPolicy");

            _breakPolicyRepository.Update(breakPolicy);
        }

        #endregion


        #region BreakPolicy

        public BreakPolicy GetBreakPolicyById(int id)
        {
            if (id == 0)
                return null;

            return _breakPolicyRepository.GetById(id);
        }

        public IList<BreakPolicy> GetBreakPoliciesByCompanyId(int id)
        {
            if (id == 0)
                return null;

            var query = _breakPolicyRepository.Table;

            query = from i in query
                    where i.CompanyId == id && i.IsActive==true && i.IsDeleted == false
                    select i;

            return query.ToList();
        }

        public IQueryable<BreakPolicy> GetAllBreakPoliciesAsQueryable(bool showHidden = false)
        {
            var query = _breakPolicyRepository.Table;

            query = from s in query
                    where (showHidden || !s.IsDeleted)
                    orderby s.UpdatedOnUtc
                    select s;

            if (!showHidden)
                query = query.Where(p => !p.IsDeleted);

            return query.AsQueryable();
        }

        #endregion

    }
}

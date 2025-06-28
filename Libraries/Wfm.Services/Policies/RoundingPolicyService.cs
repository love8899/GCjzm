using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial class RoundingPolicyService : IRoundingPolicyService
    {
        #region Fields

        private readonly IRepository<RoundingPolicy> _roundingPolicyRepository;

        #endregion

        #region Ctor

        public RoundingPolicyService(                             
            IRepository<RoundingPolicy> roundingPolicyRepository
            )
        {
            _roundingPolicyRepository = roundingPolicyRepository;
        }

        #endregion 


        #region CRUD
        
        public void Insert(RoundingPolicy roundingPolicy)
        {
            if (roundingPolicy == null)
                throw new ArgumentNullException("roundingPolicy");

            _roundingPolicyRepository.Insert(roundingPolicy);
        }

        public void Update(RoundingPolicy roundingPolicy)
        {
            if (roundingPolicy == null)
                throw new ArgumentNullException("roundingPolicy");

            _roundingPolicyRepository.Update(roundingPolicy);
        }

        #endregion


        #region RoundingPolicy

        public RoundingPolicy GetRoundingPolicyById(int id)
        {
            if (id == 0)
                return null;

            return _roundingPolicyRepository.GetById(id);
        }

        public IList<RoundingPolicy> GetRoundingPoliciesByCompanyId(int id)
        {
            if (id == 0)
                return null;

            var query = _roundingPolicyRepository.Table;

            query = from i in query
                    where i.CompanyId == id && i.IsActive==true && i.IsDeleted==false
                    select i;

            return query.ToList();
        }

        public IQueryable<RoundingPolicy> GetAllRoundingPoliciesAsQueryable(bool showHidden = false)
        {
            var query = _roundingPolicyRepository.Table;

            if (!showHidden)
                query = query.Where(p => !p.IsDeleted);

            query = from p in query
                    orderby p.UpdatedOnUtc
                    select p;

            return query.AsQueryable();
        }

        #endregion

    }
}

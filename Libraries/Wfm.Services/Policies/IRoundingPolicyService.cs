using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial interface IRoundingPolicyService
    {
        void Insert(RoundingPolicy roundingPolicy);

        void Update(RoundingPolicy roundingPolicy);


        RoundingPolicy GetRoundingPolicyById(int id);


        IList<RoundingPolicy> GetRoundingPoliciesByCompanyId(int id);

        IQueryable<RoundingPolicy> GetAllRoundingPoliciesAsQueryable(bool showHidden = false);
    }
}

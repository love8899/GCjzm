using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial interface IBreakPolicyService
    {
        void Insert(BreakPolicy breakPolicy);

        void Update(BreakPolicy breakPolicy);


        BreakPolicy GetBreakPolicyById(int id);


        IList<BreakPolicy> GetBreakPoliciesByCompanyId(int id);

        IQueryable<BreakPolicy> GetAllBreakPoliciesAsQueryable(bool showHidden = false);
    }
}

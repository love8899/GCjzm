using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial interface IMealPolicyService
    {
        void Insert(MealPolicy mealPolicy);

        void Update(MealPolicy mealPolicy);

        MealPolicy GetMealPolicyById(int id);

        IList<MealPolicy> GetMealPoliciesByCompanyId(int id);

        IQueryable<MealPolicy> GetAllMealPoliciesAsQueryable(bool showHidden = false);

    }
}

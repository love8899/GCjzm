using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial class MealPolicyService : IMealPolicyService
    {
        #region Fields

        private readonly IRepository<MealPolicy> _mealPolicyRepository;

        #endregion

        #region Ctor

        public MealPolicyService(                             
            IRepository<MealPolicy> mealPolicyRepository
            )
        {
            _mealPolicyRepository = mealPolicyRepository;
        }

        #endregion 


        #region CRUD

        public void Insert(MealPolicy mealPolicy)
        {
            if (mealPolicy == null)
                throw new ArgumentNullException("mealPolicy");

            _mealPolicyRepository.Insert(mealPolicy);
        }

        public void Update(MealPolicy mealPolicy)
        {
            if (mealPolicy == null)
                throw new ArgumentNullException("mealPolicy");

            _mealPolicyRepository.Update(mealPolicy);
        }

        #endregion


        #region MealPolicy

        public MealPolicy GetMealPolicyById(int id)
        {
            if (id == 0)
                return null;

            return _mealPolicyRepository.GetById(id);
        }

        public IList<MealPolicy> GetMealPoliciesByCompanyId(int id)
        {
            if (id == 0)
                return null;

            var query = _mealPolicyRepository.Table;

            query = from i in query
                    where i.CompanyId == id && i.IsActive==true &&  i.IsDeleted==false
                    select i;

            return query.ToList();
        }

        public IQueryable<MealPolicy> GetAllMealPoliciesAsQueryable(bool showHidden = false)
        {
            var query = _mealPolicyRepository.Table;

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

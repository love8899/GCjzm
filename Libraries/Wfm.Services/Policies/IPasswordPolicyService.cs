using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public interface IPasswordPolicyService
    {


        #region Ctor
        
        #endregion

        #region CRUD
        void Insert(PasswordPolicy entity);

        PasswordPolicy Retrieve(Guid? guid);

        void Update(PasswordPolicy entity);

        void Delete(PasswordPolicy entity);

        #endregion

        #region Method
        IQueryable<PasswordPolicy> GetAllPasswordPolicies();

        IList<SelectListItem> GetAllPasswordPoliciesAsSelectItem();
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Policies;
using Wfm.Services.Logging;

namespace Wfm.Services.Policies
{
    public class PasswordPolicyService:IPasswordPolicyService
    {
        #region Field
        private readonly IRepository<PasswordPolicy> _passwordPolicyRepository;
        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public PasswordPolicyService(IRepository<PasswordPolicy> passwordPolicyRepository, ILogger logger)
        {
            _passwordPolicyRepository = passwordPolicyRepository;
            _logger = logger;
        }
        #endregion

        #region CRUD
        public void Insert(PasswordPolicy entity)
        {
            if (entity == null)
                return;
            _passwordPolicyRepository.Insert(entity);
        }

        public PasswordPolicy Retrieve(Guid? guid)
        {
            if (guid == null)
                return null;
            var result = _passwordPolicyRepository.Table.Where(x => x.PasswordPolicyGuid == guid).FirstOrDefault();
            return result;
        }

        public void Update(PasswordPolicy entity)
        {
            if (entity == null)
                return;
            _passwordPolicyRepository.Update(entity);
        }

        public void Delete(PasswordPolicy entity)
        {
            if (entity == null)
                return;
            _passwordPolicyRepository.Delete(entity);
        }
        #endregion


        #region Method
        public IQueryable<PasswordPolicy> GetAllPasswordPolicies()
        {
            return _passwordPolicyRepository.Table;
        }

        public IList<SelectListItem> GetAllPasswordPoliciesAsSelectItem()
        {
            var result = _passwordPolicyRepository.TableNoTracking.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Code }).ToList();
            result.Add(new SelectListItem() { Value = "0", Text = "None" });
            
            return result.OrderBy(x=>x.Value).ToList();
        }
        #endregion
    }
}

using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Common
{
    public class AccountRoleService : IAccountRoleService
    {
        #region Fields

        private readonly IRepository<AccountRole> _accountRoleRepository;
        private readonly IWorkContext _workContext;
     
        #endregion

        #region Ctor

        public AccountRoleService(
            IRepository<AccountRole> accountRoleRepository,
            IWorkContext workContext
            )
        {
            this._accountRoleRepository = accountRoleRepository;
            this._workContext = workContext;
        }

        #endregion

        public IQueryable<AccountRole> GetAllAccountRolesForDropDownList(bool isClient)
        {
            var query = _accountRoleRepository.TableNoTracking;

            if (!isClient)
            {
                // exclude Client Admin
                query = query.Where(x => x.IsClientRole == false &&
                                    x.SystemName != Wfm.Core.Domain.Accounts.AccountRoleSystemNames.ClientAdministrators);

                if (_workContext.CurrentAccount.IsLimitedToFranchises) { query = query.Where(x => x.IsVendorRole); }
            }
            else
            {
                query = query.Where(x => x.IsClientRole == true ||
                                    x.SystemName == Wfm.Core.Domain.Accounts.AccountRoleSystemNames.ClientAdministrators);
            }

            query = query.OrderBy(x => x.AccountRoleName);

            return query;
        }
    }
}

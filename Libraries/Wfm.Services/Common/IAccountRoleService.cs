using System.Linq;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Common
{
    public interface IAccountRoleService
    {
        IQueryable<AccountRole> GetAllAccountRolesForDropDownList(bool isClient);
    }
}

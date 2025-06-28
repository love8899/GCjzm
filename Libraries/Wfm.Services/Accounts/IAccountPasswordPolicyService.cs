using System.Linq;
using System.Text;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Accounts
{
    public interface IAccountPasswordPolicyService
    {
        #region CRUD
        void Create(AccountPasswordPolicy entity);
        AccountPasswordPolicy Retrieve(string type);
        void Update(AccountPasswordPolicy entity);
        void Delete(AccountPasswordPolicy entity);
        #endregion

        #region Method
        IQueryable<AccountPasswordPolicy> GetAllAccountPasswordPolicy();
        bool ApplyPasswordPolicy(int accountId, string accountType, string newPassword, string currentPassword, PasswordFormat currentPasswordFormat, string currentPasswordSalt, out StringBuilder errors);
        #endregion
    }
}

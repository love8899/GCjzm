using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IBankService
    {
        #region CRUD

        void InsertBank(Bank bank);

        void UpdateBank(Bank bank);

        void DeleteBank(Bank bank);

        #endregion

        Bank GetBankById(int id);

        IList<Bank> GetAllBanks(bool showInactive = false, bool showHidden = false);

    }
}

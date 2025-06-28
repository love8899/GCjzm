using Wfm.Core;
using Wfm.Services.Accounts;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using System;
using Wfm.Client.Extensions;
using System.Web.Mvc;

namespace Wfm.Client.Models.Accounts
{
    public class Accounts_BL
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        
        #endregion


        public Accounts_BL(
            IAccountService accountService,
            IWorkContext workContext
            )
        {
            _accountService = accountService;
            _workContext = workContext;
        }


        #region Method

        public AccountDelegateModel GetAccountDelegateModelForPopUp(int id)
        {
            var allInstances = _accountService.GetDelegates(_workContext.CurrentAccount.Id);
            var instance = allInstances.FirstOrDefault(x => x.Id == id);
            if (instance == null)
            {
                instance = new AccountDelegate()
                {
                    AccountId = _workContext.CurrentAccount.Id,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(8),
                };
            }
            var model = instance.ToModel();
            model.AvaliableAccounts = GetAvailableAccounts();

            return model;
        }


        public IQueryable<SelectListItem> GetAvailableAccounts(bool longName = true)
        {
            return _accountService.GetAllAccountsAsQueryable(_workContext.CurrentAccount, false, true, false)
                .Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId && x.Id != _workContext.CurrentAccount.Id)
                .Select(x => new SelectListItem()
                {
                    Text = x.FirstName + " " + x.LastName + (!longName ? "" : " (" + x.Username + ")"),
                    Value = x.Id.ToString()
                });
        }


        public bool AnyDelegateConflict(AccountDelegate entity)
        {
            var result = true;

            // existing delagates for the account and delegat account
            var delegates = _accountService.GetDelegates(_workContext.CurrentAccount.Id)
                .Where(x => x.DelegateAccountId == entity.DelegateAccountId)
                .Where(x => x.Id != entity.Id);

            // date range overlap
            result = delegates.Any(x => x.StartDate <= entity.EndDate && x.EndDate >= entity.StartDate);

            return result;
        }

        #endregion
    }
}

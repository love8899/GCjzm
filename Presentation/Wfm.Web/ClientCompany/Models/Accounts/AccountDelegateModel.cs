using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Client.Validators.Accounts;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Accounts
{
    [Validator(typeof(AccountDelegateValidator))]
    public class AccountDelegateModel : BaseWfmEntityModel
    {
        public int AccountId { get; set; }
        public int DelegateAccountId { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.Account.Delegate.Account")]
        public string DelegateAccountName { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime StartDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime EndDate { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Remark { get; set; }
        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive
        {
            get
            {
                return StartDate <= DateTime.Today && EndDate >= DateTime.Today;
            }
        }
        public IEnumerable<SelectListItem> AvaliableAccounts { get; set; }
    }
}

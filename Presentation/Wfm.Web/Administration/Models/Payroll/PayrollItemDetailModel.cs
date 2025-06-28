using FluentValidation.Attributes;
using Wfm.Admin.Validators.Payroll;
using Wfm.Web.Framework;

namespace Wfm.Admin.Models.Payroll
{
    [Validator(typeof(PayrollItemValidator))]
    public class PayrollItemDetailModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        [WfmResourceDisplayName("Common.Name")]
        public string Description { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsPensionable { get; set; }
        public bool IsInsurable { get; set; }
        public int TypeId { get; set; }
        public int SubTypeId { get; set; }
        public string State_Code { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AccrueVacation { get; set; }
        public int? PayOutItemId { get; set; }
        public int? BalanceItemId { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public decimal? Rate { get; set; }
        public bool HasYtdMaximum { get; set; }
        public decimal YTD_Maximum { get; set; }
        public bool PrintOnPayStub { get; set; }
        public int FranchiseId { get; set; }

        #region Enable Property
        public bool EnableRate { get; set; }
        public bool EnableCreditAccount { get; set; }
        public bool EnableDebitAccount { get; set; }
        public bool EnableHasYtdMaximum { get; set; }
        public bool EnableYTD_Maximum { get; set; }
        public bool EnablePrintOnPayStub { get; set; }
        public bool EnableOptions { get; set; }
        #endregion


        #region Type and SubType
        public string Type { get; set; }
        public string SubType { get; set; }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Core.Domain.Payroll
{
    public partial class Payroll_Item
    {
        public Payroll_Item()
        {
           // this.Candidate_Payment_History_Detail = new List<Candidate_Payment_History_Detail>();
            this.OvertimeRuleSettings = new List<OvertimeRuleSetting>();
            //this.Payroll_InProgress_Detail = new List<Payroll_InProgress_Detail>();
        }

        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int TypeID { get; set; }
        public int SubTypeId { get; set; }
        public string State_Code { get; set; }
        public bool PrintOnPayStub { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsPensionable { get; set; }
        public bool IsInsurable { get; set; }

        public int FranchiseId { get; set; }
        public string CreditAccount { get; set; }
        public string DebitAccount { get; set; }
        public Nullable<int> PayOutItemId { get; set; }
        public Nullable<int> BalanceItemId { get; set; }
        public Nullable<decimal> Rate { get; set; }
        //public virtual ICollection<Candidate_Payment_History_Detail> Candidate_Payment_History_Detail { get; set; }
        public virtual ICollection<OvertimeRuleSetting> OvertimeRuleSettings { get; set; }
        //public virtual ICollection<Payroll_InProgress_Detail> Payroll_InProgress_Detail { get; set; }
        public virtual Payroll_Item_SubType Payroll_Item_SubType { get; set; }
        public virtual Payroll_Item_Type Payroll_Item_Type { get; set; }
        public bool AccrueVacation { get; set; }
        public bool HasYtdMaximum { get; set; }
        public decimal YTD_Maximum { get; set; }

        public virtual ICollection<TaxFormBox> TaxForms { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise Vendor { get; set; }
    }
}

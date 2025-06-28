using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Core.Domain.Candidates
{
    public partial class Candidate_Payment_History
    {
        public Candidate_Payment_History()
        {
            this.Candidate_Payment_History_Detail = new List<Candidate_Payment_History_Detail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CandidatePaymentHistoryGuid { get; set; }

        public int Id { get; set; }
        public Nullable<int> CandidateId { get; set; }
        public string Year { get; set; }
        public Nullable<System.DateTime> Payment_Date { get; set; }
        public string Cheque_Number { get; set; }
        public string Direct_Deposit_Number { get; set; }
        public Nullable<int> CompanyId { get; set; }
       // public Nullable<int> EmployeeType { get; set; }
        public Nullable<int> PayrollBatchId { get; set; }
        public string ProvinceCode { get; set; }
        //public byte[] Paystub { get; set; }
        public Nullable<int> CheckStatusId { get; set; }
        public Nullable<bool> IsEmailed { get; set; }
        public Nullable<bool> IsPrinted { get; set; }
        public virtual Check_Status Check_Status { get; set; }

        public byte[] Paystub { get; set; }

        public string Note { get; set; }

        public virtual ICollection<Candidate_Payment_History_Detail> Candidate_Payment_History_Detail { get; set; }
    }
}

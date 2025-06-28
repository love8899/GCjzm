using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Core.Domain.Payroll
{
    public partial class Check_Status
    {
        public Check_Status()
        {
            this.Candidate_Payment_History = new List<Candidate_Payment_History>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Candidate_Payment_History> Candidate_Payment_History { get; set; }
    }
}

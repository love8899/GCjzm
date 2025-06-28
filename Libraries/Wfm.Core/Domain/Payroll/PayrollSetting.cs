using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Payroll
{
    public class PayrollSetting
    {
        public int FranchiseId { get; set; }
        public Guid FranchiseGuid { get; set; }
        public string Client_Number { get; set; }
        public string Transmission_Header { get; set; }
        public string BusinessNumber { get; set; }
        public decimal? EIRate { get; set; }
        public string NEQ { get; set; }
        public string QuebecIdentificationNumber { get; set; }
        public string RL1TransmitterNumber { get; set; }
        public string DDFileLayout { get; set; }
        public int? DDFileLayoutId { get; set; }
        public string RL1SequentialNumber { get; set; }
        public string RL1XMLSequentialNumber { get; set; }
        public string InstitutionNumber { get; set; }
        public string TransitNumber { get; set; }
        public string AccountNumber { get; set; }
    }
}

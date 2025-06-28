using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.WSIB
{
    public class CandidateWSIBCommonRate:BaseEntity
    {
        public int CandidateId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Ratio { get; set; }
        public int EnteredBy { get; set; }
        public string Code { get; set; }
        public int ProvinceId { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual StateProvince StateProvince { get; set; }
    }
}

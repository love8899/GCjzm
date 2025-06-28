using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Core.Domain.Tests
{
    public class CandidateTestLink : BaseEntity
    {
        public int CandidateId { get; set; }
        public int TestCategoryId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CandidateTestLinkGuid { get; set; }
        public DateTime ValidBefore { get; set; }
        public bool IsUsed { get; set; }
        public int EnteredBy { get; set; }
        public virtual Candidate Candidate { get; set; }
        public virtual TestCategory TestCategory { get; set; }
    }
}

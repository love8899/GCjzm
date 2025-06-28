using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Common
{
    public class DNRReason
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}

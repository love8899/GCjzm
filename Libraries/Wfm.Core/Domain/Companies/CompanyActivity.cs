using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyActivity:BaseEntity
    {
        public int CompanyId { get; set; }
        public int ActivityTypeId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Note { get; set; }
        public virtual Company Company { get; set; }
        public virtual ActivityType ActivityType { get; set; }
    }
}

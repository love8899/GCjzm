using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.JobOrders
{
    public class JobOrderOpening : BaseEntity
    {
        public int JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OpeningNumber { get; set; }

        public string Note { get; set; }
        public int EnteredBy { get; set; }

        public virtual JobOrder JobOrder { get; set; }
    }
}

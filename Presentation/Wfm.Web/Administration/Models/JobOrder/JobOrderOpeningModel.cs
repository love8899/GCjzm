using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Admin.Models.JobOrder
{
    public class JobOrderOpeningModel
    {
        public DateTime StartDate { get; set; }
        public int OpeningAvailable { get; set; }
        public DateTime ChangedDateTime { get; set; }
        public string ChangedBy { get; set; }
        public string Note { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

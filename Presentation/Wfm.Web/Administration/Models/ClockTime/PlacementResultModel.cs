using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wfm.Admin.Models.ClockTime
{
    public class PlacementResultModel
    {
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public DateTime Date { get; set; }
        public int DisplayOrder { get; set; }
        public bool Guessed { get; set; }
        public int? CandidateMatchedWorkTimeId { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Wfm.Admin.Models.ClockTime
{
    public class CandidateCurrentStatusModel
    {
        public List<CandidateJobOrderSimpleModel> JobOrders { get; set; }
        public List<CandidateClockTimeModel> PunchRecords { get; set; }
        public bool Onboarded { get; set; }
    }

    public class CandidateJobOrderSimpleModel
    {
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
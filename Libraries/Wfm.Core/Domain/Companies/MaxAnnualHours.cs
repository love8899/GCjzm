using System;


namespace Wfm.Core.Domain.Companies
{
    public partial class MaxAnnualHours
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MaxHours { get; set; }
        public int Threshold { get; set; }
    }
}

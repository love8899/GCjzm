using System;

namespace Wfm.Admin.Models.Candidate
{
    public partial class PoolProfile
    {
        public int[] CompanyIds { get; set; }

        public int[] ShiftIds { get; set; }

        public int[] PositionIds { get; set; }

        public DateTime RefDate { get; set; }
    }

}

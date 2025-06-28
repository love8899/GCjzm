using System;

namespace Wfm.Core.Domain.JobOrders
{
    public class AttendancePreview
    {
        public Guid JobOrderGuid { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string JobShift { get; set; }
        public DateTime JobStartDate { get; set; }
        public DateTime? JobEndDate { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CompanyLocationId { get; set; }
        public string LocationName { get; set; }
        public int CompanyDepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyContactId { get; set; }
        public string ContactName { get; set; }

        public int SundayPlaced { get; set; }
        public int SundayPunched { get; set; }
        public int SundayValid { get; set; }
        public int SundayMissing { get; set; }
        public bool SundaySwitch { get; set; }

        public int MondayPlaced { get; set; }
        public int MondayPunched { get; set; }
        public int MondayValid { get; set; }
        public int MondayMissing { get; set; }
        public bool MondaySwitch { get; set; }

        public int TuesdayPlaced { get; set; }
        public int TuesdayPunched { get; set; }
        public int TuesdayValid { get; set; }
        public int TuesdayMissing { get; set; }
        public bool TuesdaySwitch { get; set; }

        public int WednesdayPlaced { get; set; }
        public int WednesdayPunched { get; set; }
        public int WednesdayValid { get; set; }
        public int WednesdayMissing { get; set; }
        public bool WednesdaySwitch { get; set; }

        public int ThursdayPlaced { get; set; }
        public int ThursdayPunched { get; set; }
        public int ThursdayValid { get; set; }
        public int ThursdayMissing { get; set; }
        public bool ThursdaySwitch { get; set; }

        public int FridayPlaced { get; set; }
        public int FridayPunched { get; set; }
        public int FridayValid { get; set; }
        public int FridayMissing { get; set; }
        public bool FridaySwitch { get; set; }

        public int SaturdayPlaced { get; set; }
        public int SaturdayPunched { get; set; }
        public int SaturdayValid { get; set; }
        public int SaturdayMissing { get; set; }
        public bool SaturdaySwitch { get; set; }

        public DateTime WeekStartDate { get; set; }

        public int FranchiseId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Scheduling;
using Wfm.Services.Scheduling;

namespace Wfm.Shared.Models.Scheduling
{
    public class ShiftViewDayTuple : IShiftViewDay
    {
        public IEnumerable<IEmployeeScheduleViewModel> EmployeeSchedules
        {
            get; set;
        }

        public ShiftScheduleDaily Shift
        {
            get; set;
        }
    }
}

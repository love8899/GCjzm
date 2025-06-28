using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface IAttendanceListService
    {
        IList<AttendanceList> GetAllAttendanceListByIdsAndDate(string ids, DateTime startDate, DateTime endDate);
    }
}

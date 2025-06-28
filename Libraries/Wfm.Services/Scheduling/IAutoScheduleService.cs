using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Services.Scheduling
{
    public interface IAutoScheduleService
    {
        bool AutoFillupVacancy(int schedulePeriodId, int companyShiftId, bool ignoreWarning, bool priorityToRecentEmployee, out IEnumerable<IScheduleDetailErrorModel> warnings);
        void ResetEmployeeSchedule(int schedulePeriodId, int companyShiftId);
        void PublishEmployeeSchedule(int schedulePeriodId, int companyShiftId);
        IEnumerable<IScheduleDetailErrorModel> MoveEmployeeSchedule(int schedulePeriodId, int fromCompanyShiftId, int fromCompanyRoleId, int employeeId, int toCompanyShiftid, int toCompanyRoleId);
        ScheduleJobOrder GetScheduleJobOrderByJobOrderId(int jobOrderId);
        void UpdateScheduleJobOrder(int scheduleJobOrderId, int? supervisorAccountId, DateTime startDate, DateTime? endDate, string jobTitle);
        IEnumerable GetEmployeeResourceList();
    }
}

using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Policies;

namespace Wfm.Services.Policies
{
    public partial interface ISchedulePolicyService
    {
        void Insert(SchedulePolicy schedulePolicy);

        void Update(SchedulePolicy schedulePolicy);


        SchedulePolicy GetSchedulePolicyById(int id);

        SchedulePolicy GetSchedulePolicieByJobOrderId(int id);

        IList<SchedulePolicy> GetSchedulePoliciesByCompanyId(int id);

        IQueryable<SchedulePolicy> GetAllSchedulePoliciesAsQueryable(bool showHidden = false);

    }
}

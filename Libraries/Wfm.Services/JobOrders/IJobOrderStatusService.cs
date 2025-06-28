using System.Collections.Generic;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.JobOrders
{
    public partial interface IJobOrderStatusService
    {
        #region CRUD

        void InsertJobOrderStatus(JobOrderStatus jobOrderStatus);

        void UpdateJobOrderStatus(JobOrderStatus jobOrderStatus);

        void DeleteJobOrderStatus(JobOrderStatus jobOrderStatus);

        #endregion
        
        JobOrderStatus GetJobOrderStatusById(int id);

        int? GetJobOrderStatusIdByName(string name);

        IList<JobOrderStatus> GetAllJobOrderStatus(bool showInactive = false, bool showHidden = false);

    }
}

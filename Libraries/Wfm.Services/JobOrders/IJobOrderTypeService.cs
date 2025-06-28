using System.Collections.Generic;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.JobOrders
{
    public partial interface IJobOrderTypeService
    {
        #region CRUD

        void InsertJobOrderType(JobOrderType jobOrderType);

        void UpdateJobOrderType(JobOrderType jobOrderType);

        void DeleteJobOrderType(JobOrderType jobOrderType);

        #endregion

        JobOrderType GetJobOrderTypeById(int id);

        IList<JobOrderType> GetAllJobOrderTypes(bool showInactive = false, bool showHidden = false);

        JobOrderType GetJobOrderTypeByTypeName(string name);

    }
}

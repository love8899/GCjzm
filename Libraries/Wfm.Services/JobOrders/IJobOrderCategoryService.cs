using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.JobOrders
{
    public partial interface IJobOrderCategoryService
    {

        #region CRUD

        void InsertJobOrderCategory(JobOrderCategory jobOrderCategory);

        void UpdateJobOrderCategory(JobOrderCategory jobOrderCategory);

        void DeleteJobOrderCategory(JobOrderCategory jobOrderCategory);

        #endregion

        #region Methods

        JobOrderCategory GetJobOrderCategoryById(int id);

        IList<JobOrderCategory> GetAllJobOrderCategories(bool showInactive = false, bool showHidden = false);

        IQueryable<JobOrderCategory> GetAllJobOrderCategoriesAsQueryable(bool showInactive = false, bool showHidden = false);

        #endregion

    }
}

using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Tests;


namespace Wfm.Services.JobOrders
{
    public partial interface IJobOrderTestCategoryService
    {
        #region CRUD

        void InsertJobOrderTestCategory(JobOrderTestCategory jobOrderTestCategory);

        void UpdateJobOrderTestCategory(JobOrderTestCategory jobOrderTestCategory);

        void DeleteJobOrderTestCategory(JobOrderTestCategory jobOrderTestCategory);

        #endregion


        #region Methods

        JobOrderTestCategory GetJobOrderTestCategoryById(int id);

        IList<JobOrderTestCategory> GetJobOrderTestCategoryByJobOrderId(int jobOrderId);

        IQueryable<TestCategory> GetTestCategoriesByCompany(int companyId);

        #endregion

    }
}

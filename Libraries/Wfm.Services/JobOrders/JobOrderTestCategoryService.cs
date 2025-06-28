using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Data;


namespace Wfm.Services.JobOrders
{
    public partial class JobOrderTestCategoryService : IJobOrderTestCategoryService
    {

        #region Fields

        private readonly IRepository<JobOrderTestCategory> _jobOrderTestCategoryRepository;

        #endregion

        #region Ctor

        public JobOrderTestCategoryService(
            IRepository<JobOrderTestCategory> jobOrderTestCategoryRepository
            )
        {
            _jobOrderTestCategoryRepository = jobOrderTestCategoryRepository;
        }

        #endregion


        #region CRUD

        public void InsertJobOrderTestCategory(JobOrderTestCategory jobOrderTestCategory)
        {
            if (jobOrderTestCategory == null) throw new ArgumentNullException("jobOrderTestCategory");

            _jobOrderTestCategoryRepository.Insert(jobOrderTestCategory);
        }

        public void UpdateJobOrderTestCategory(JobOrderTestCategory jobOrderTestCategory)
        {
            if (jobOrderTestCategory == null) throw new ArgumentNullException("jobOrderTestCategory");

            _jobOrderTestCategoryRepository.Update(jobOrderTestCategory);
        }

        public void DeleteJobOrderTestCategory(JobOrderTestCategory jobOrderTestCategory)
        {
            //throw new NotImplementedException();
            if (jobOrderTestCategory == null) throw new ArgumentNullException("jobOrderTestCategory");

            _jobOrderTestCategoryRepository.Delete(jobOrderTestCategory);
        }

        #endregion


        #region Methods

        public JobOrderTestCategory GetJobOrderTestCategoryById(int id)
        {
            if(id == 0)
                return null;
            
            return _jobOrderTestCategoryRepository.GetById(id);
        }

        public IList<JobOrderTestCategory> GetJobOrderTestCategoryByJobOrderId(int jobOrderId)
        {
            var query = _jobOrderTestCategoryRepository.Table;

            query = from jtc in query
                    where jtc.JobOrderId == jobOrderId
                    orderby jtc.UpdatedOnUtc
                    select jtc;

            return query.ToList();
        }


        public IQueryable<TestCategory> GetTestCategoriesByCompany(int companyId)
        {
            return _jobOrderTestCategoryRepository.TableNoTracking.Where(x => x.JobOrder.CompanyId == companyId).Select(x => x.TestCategory).Distinct();
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Services.JobOrders
{
    public partial interface IDirectHireJobOrderService
    {
        #region CRUD

        void InsertDirectHireJobOrder(JobOrder directHireJobOrder);
        void UpdateDirectHireJobOrder(JobOrder directHireJobOrder); 

        #endregion
         
        #region JobOrder 

        JobOrder GetDirectHireJobOrderById(int id);

        JobOrder GetDirectHireJobOrderByGuid(Guid? guid);     

        #endregion 

        #region PagedList
    
        // admin helper methods
        //----------------------------------
        IQueryable<DirectHireJobOrderList> GetDirectHireJobOrderList(Account account, DateTime startDate, DateTime endDate);

        IQueryable<JobOrder> GetAllDirectHireJobOrdersAsQueryable(Account account); 

        #endregion

        IEnumerable<FeeType> GetAllFeeTypes();

        IQueryable<DirectHireCandidatePoolList> GetDirectHireCandidatePoolList();
    }
}


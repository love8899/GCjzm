using System.Collections.Generic;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.JobOrder
{
    public partial class JobOrderListModel : BaseWfmModel
    {
        public JobOrderListModel()
        {
            PagingFilterContext = new JobOrderPagingFilteringModel();

        }

        public JobOrderPagingFilteringModel PagingFilterContext { get; set; }
        public IList<JobOrderModel> JobOrders { get; set; }
        
    }
}
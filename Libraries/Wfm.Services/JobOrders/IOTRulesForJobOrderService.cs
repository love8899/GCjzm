using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.JobOrders
{
    public partial interface IOTRulesForJobOrderService
    {
        #region Method
        IList<OTRulesForJobOrder> GetOTRulesForJobOrder(int jobOrderId);
        #endregion
    }
}

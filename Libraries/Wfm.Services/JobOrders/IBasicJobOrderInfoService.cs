using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Services.JobOrders
{
    public partial interface IBasicJobOrderInfoService
    {
        IList<BasicJobOrderInfo> GetAllBasicJobOrderInfoByDate(DateTime? startDate, DateTime? endDate);
    }
}

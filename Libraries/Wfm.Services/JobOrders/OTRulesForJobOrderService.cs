using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Data;

namespace Wfm.Services.JobOrders
{
    public partial class OTRulesForJobOrderService:IOTRulesForJobOrderService
    {
        #region Field
        private readonly IDbContext _dbContext; 
        #endregion

        #region Ctor
        public OTRulesForJobOrderService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        #region Method

        #endregion
        public IList<OTRulesForJobOrder> GetOTRulesForJobOrder(int jobOrderId)
        {
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("JobOrderId", jobOrderId);
            List<OTRulesForJobOrder> result = _dbContext.SqlQuery<OTRulesForJobOrder>(@"declare @ruleLevel varchar(50)
                                                                                        Exec @ruleLevel = dbo.Get_OT_Rule_Level @JobOrderId;
                                                                                        print @ruleLevel
                                                                                        select * 
                                                                                        from dbo.Get_OT_Rules_For_JobOrder(@JobOrderId,@ruleLevel)", paras).ToList();
            return result;
        }
    }
}

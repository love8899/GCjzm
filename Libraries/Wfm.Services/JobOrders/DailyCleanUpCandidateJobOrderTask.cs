using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;
using Wfm.Data;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;

namespace Wfm.Services.JobOrders
{
    public class DailyCleanUpCandidateJobOrderTask : IScheduledTask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        #endregion

        #region Ctor
        public DailyCleanUpCandidateJobOrderTask(ILogger logger, IDbContext dbContext, IWorkflowMessageService workflowMessageService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _workflowMessageService = workflowMessageService;
        }
        #endregion

        #region Method
        public virtual void Execute()
        {
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("today",DateTime.Today);
            var result = _dbContext.SqlQuery<DailyCleanUpCandidateJobOrderResult>("Exec DailyCleanUpCandidateJobOrder @today", paras).ToList();
            if (result.Count() > 0)
            {
                try
                {
                    //send email to supervisor and recruiter
                    foreach (var item in result)
                    {
                        int sendResult = _workflowMessageService.SendCandidateOutOfJobOrderMessage(item.JobOrderId, item.CandidateId, item.TwoDaysBeforeLastWorkingDate.Value);
                        if (sendResult == 0)
                            _logger.Error("SendCandidateOutOfJobOrderMessage():No template or no receiver");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("SendCandidateOutOfJobOrderMessage():", ex);
                }
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
        #endregion
    }
}

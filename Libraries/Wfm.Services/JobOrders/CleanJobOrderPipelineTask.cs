using System.Text;
using System.Threading.Tasks;
using Wfm.Data;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;

namespace Wfm.Services.JobOrders
{
    public partial class CleanJobOrderPipelineTask:IScheduledTask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor
        public CleanJobOrderPipelineTask(ILogger logger, IDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        #endregion

        #region Method
        public virtual void Execute()
        {
            
            //IList<JobOrder> data = _jobOrderService.GetAllDeletedOrClosedOrEndedJobOrder(today);
            StringBuilder query = new StringBuilder();
            query.AppendLine(@"
                                declare @NoStausId int= (select Id from CandidateJobOrderStatus where StatusName='No status')
                                Update CandidateJobOrder
                                Set StartDate = (select StartDate from JobOrder where Id = JobOrderId)
                                where Id in 
                                (
	                                select cjo.Id from CandidateJobOrder cjo
	                                inner join JobOrder jo on jo.Id = cjo.JobOrderId
	                                where (jo.EndDate is null and cjo.EndDate is null and cjo.StartDate<jo.StartDate) or 
			                                (jo.EndDate is null and cjo.EndDate is not null and cjo.StartDate<jo.StartDate and cjo.EndDate>jo.StartDate) or 
			                                (jo.EndDate is not null and cjo.EndDate is not null and cjo.StartDate<jo.StartDate and cjo.EndDate>=jo.StartDate and jo.EndDate>cjo.EndDate)
                                )
                                

                                Update CandidateJobOrder 
                                Set StartDate=(select StartDate from JobOrder where Id = JobOrderId),
	                                EndDate = (select EndDate from JobOrder where Id = JobOrderId)
                                where Id in
                                (
	                                select cjo.Id from CandidateJobOrder cjo
	                                inner join JobOrder jo on jo.Id = cjo.JobOrderId
	                                where jo.EndDate is not null and cjo.EndDate is null and cjo.StartDate<jo.StartDate 
                                )


                                Update CandidateJobOrder 
                                Set	EndDate = (select EndDate from JobOrder where Id = JobOrderId)
                                where Id in
                                (
	                                select cjo.Id from CandidateJobOrder cjo
	                                inner join JobOrder jo on jo.Id = cjo.JobOrderId
	                                where (jo.EndDate is not null and cjo.EndDate is null and cjo.StartDate>=jo.StartDate and cjo.StartDate<=jo.EndDate) or 
		                                (jo.EndDate is not null and cjo.EndDate is not null and cjo.StartDate>=jo.StartDate and cjo.StartDate<=jo.EndDate and cjo.EndDate>jo.EndDate)
                                )
                        

                                Delete from CandidateJobOrder
                                where Id in 
                                (
	                                select cjo.Id from CandidateJobOrder cjo
	                                inner join JobOrder jo on jo.Id = cjo.JobOrderId
	                                where (jo.EndDate is null and cjo.EndDate is not null and cjo.EndDate<jo.StartDate) or 
		                                (jo.EndDate is not null and cjo.EndDate is null and cjo.StartDate>jo.EndDate) or 
		                                (jo.EndDate is not null and cjo.EndDate is not null and cjo.EndDate<jo.StartDate) or 
		                                (jo.EndDate is not null and cjo.EndDate is not null and cjo.StartDate>jo.EndDate)
                                )

                                
                                
                                Update CandidateJobOrder
                                Set CandidateJobOrderStatusId = @NoStausId
                                where Id in 
                                (
	                                select cjo.Id from CandidateJobOrder cjo 
	                                inner join JobOrder jo on jo.Id = cjo.JobOrderId
	                                inner join JobOrderStatus jos on jos.Id = jo.JobOrderStatusId and jos.JobOrderStatusName='Canceled'
	                                inner join CandidateJobOrderStatus cjs on cjs.Id = cjo.CandidateJobOrderStatusId and cjs.StatusName='Placed'
                                )
                            ");
            _dbContext.ExecuteSqlCommand(query.ToString());
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
        #endregion
    }
}

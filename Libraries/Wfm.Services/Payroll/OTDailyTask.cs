using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wfm.Data;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;

namespace Wfm.Services.Payroll
{
    public class OTDailyTask:IScheduledTask
    {
        
        #region Fields
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor

        public OTDailyTask(ILogger logger, IDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        #endregion

        public virtual void Execute()
        {
            try
            {
                DateTime today = DateTime.Today;
                int delta = DayOfWeek.Sunday - today.DayOfWeek;
                DateTime start_date = today.AddDays(delta);
                DateTime end_date = start_date.AddDays(6);
                DateTime previous_week_start_date = start_date.AddDays(-7);
                DateTime previous_week_end_date = end_date.AddDays(-7);

                SqlParameter[] paras = new SqlParameter[1];
                paras[0] = new SqlParameter("StartDate", start_date);
                
                SqlParameter[] paras2 = new SqlParameter[1];
                paras2[0] = new SqlParameter("StartDate", previous_week_start_date);
                
                _dbContext.ExecuteSqlCommand("EXEC [dbo].[Calculate_Overtime_For_Submitted_Hours] @StartDate", false, null, paras);
                _dbContext.ExecuteSqlCommand("EXEC [dbo].[Calculate_Overtime_For_Approved_Hours] @StartDate", false, null, paras2);

                // populate the alerts for employees, who are approaching the OT threshold
                SqlParameter[] paras3 = new SqlParameter[2];
                paras3[0] = new SqlParameter("StartDate", start_date);
                paras3[1] = new SqlParameter("EndDate", end_date);
                _dbContext.ExecuteSqlCommand("EXEC [dbo].[Create_Overtime_Alerts] @StartDate, @EndDate", false, null, paras3);

            }
            catch (Exception ex)
            {
                _logger.Error("OTDailyTask: Execute():", ex);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}

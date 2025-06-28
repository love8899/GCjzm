using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Data;

namespace Wfm.Services.Candidates
{
    public partial class AttendanceListService:IAttendanceListService
    {
        #region Fields
        private readonly IRepository<AttendanceList> _attendanceListRepository;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor
        public AttendanceListService(IRepository<AttendanceList> attendanceListRepository, IDbContext dbContext)
        {
            _attendanceListRepository = attendanceListRepository;
            _dbContext=dbContext;
        }
        #endregion
        #region Method
        public IList<AttendanceList> GetAllAttendanceListByIdsAndDate(string ids, DateTime startDate, DateTime endDate)
        {
            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("startDate", startDate);
            paras[1] = new SqlParameter("endDate", endDate);
            paras[2] = new SqlParameter("jobOrders", ids);
            return _dbContext.SqlQuery<AttendanceList>("Exec AttendanceListForJobOrders @startDate, @endDate, @jobOrders", paras).ToList();
        }
        #endregion
    }
}

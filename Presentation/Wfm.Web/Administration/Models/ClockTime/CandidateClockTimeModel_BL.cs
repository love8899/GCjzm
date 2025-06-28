using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using Wfm.Services.ClockTime;



namespace Wfm.Admin.Models.ClockTime
{
    public class CandidateClockTimeModel_BL
    {
        public DataSourceResult GetAllCandidateClockTime(DataSourceRequest request, IClockTimeService _clockTimeService, string startDate, string endDate, string status, int? candidateId)
        {
            var candidateClockTimes = _clockTimeService.GetAllCandidateClockTimesAsQueryable();
            DateTime fromDate, toDate;

            if (!String.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out fromDate))
                candidateClockTimes = candidateClockTimes.Where(x => x.ClockInOut >= fromDate);

            if (!String.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out toDate))
            {
                toDate = toDate.AddDays(1);
                candidateClockTimes = candidateClockTimes.Where(x => x.ClockInOut < toDate);
            }

            string[] ids = String.IsNullOrEmpty(status) ? null : status.Split(',');
            if (ids != null && ids.Count() > 0)
                candidateClockTimes = candidateClockTimes.Where(x => ids.Contains(x.CandidateClockTimeStatusId.ToString()));
            var result = candidateClockTimes.ProjectTo<CandidateClockTimeModel>();

            if (candidateId.HasValue)
                result = result.Where(x => x.CandidateId == candidateId);
            result = result.OrderBy(x => x.ClockInOut).ThenBy(x => x.CandidateId);
            return result.ToDataSourceResult(request);
        }
    }
}

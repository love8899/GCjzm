using AutoMapper.QueryableExtensions;
using System;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.ClockTime;
using Wfm.Services.Candidates;


namespace Wfm.Client.Models.ClockTime
{
    public class CandidateClockTime_BL
    {
        public IQueryable<CandidateClockTimeModel> GetAllCandidateClockTime(Account account, IClockTimeService _clockTimeService, DateTime startDate, DateTime endDate)
        {
            var candidateClockTimes = _clockTimeService.GetAllCandidateClockTimesAsQueryable();

            if (startDate != null)
                candidateClockTimes = candidateClockTimes.Where(x => x.ClockInOut >= startDate);
            if (endDate != null)
            {
                endDate = endDate.AddDays(1);
                candidateClockTimes = candidateClockTimes.Where(x => x.ClockInOut < endDate);
            }

            // filetering
            //candidateClockTimes = candidateClockTimes.Where(x => x.CompanyId == account.CompanyId);
            if (!account.IsCompanyAdministrator() && !account.IsCompanyHrManager())
            {
                candidateClockTimes = candidateClockTimes.Where(x => x.CompanyLocationId == account.CompanyLocationId);

                // TODO: filtered by candidates placed in date range for the department
                if (account.IsCompanyDepartmentSupervisor())
                    ;

                else if (account.IsCompanyDepartmentManager())
                    ;
            }

            // sorting
            candidateClockTimes = candidateClockTimes.OrderBy(x => x.ClockInOut).ThenBy(x => x.CandidateId);

            return candidateClockTimes.ProjectTo<CandidateClockTimeModel>();
        }
    }
}

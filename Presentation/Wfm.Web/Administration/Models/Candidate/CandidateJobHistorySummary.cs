using System;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Candidates;
using Wfm.Services.JobOrders;
using Wfm.Services.TimeSheet;


namespace Wfm.Admin.Models.Candidate
{

    public class CandidateJobHistorySummary
    {
        public IQueryable<CandidateJobHistoryModel> GetCandidateJobHistorySummary(int candidateId,
                                                                                  DateTime startDate,
                                                                                  IWorkContext _workContext,
                                                                                  IWorkTimeService _workTimeService,
                                                                                  IJobOrderService _jobOrderService,
                                                                                  ICandidateJobOrderService _candidateJobOrderService)
        {
            int startYear = CommonHelper.GetYearAndWeekNumber(startDate, out int startWeek);

            var placements = _candidateJobOrderService.GetCandidateJobOrderHistoryByCandidateIdAsQueryable(candidateId)
                             .GroupBy(grp => grp.JobOrderId)
                             .Select(x => new
                             {
                                 JobOrderId = x.Key,
                                 JobOrderGuid = x.FirstOrDefault().JobOrderGuid,
                                 RatingValue = x.OrderBy(y => y.EndDate.HasValue).ThenByDescending(y => y.EndDate).FirstOrDefault().RatingValue,
                                 RatingCommment = x.OrderBy(y => y.EndDate.HasValue).ThenByDescending(y => y.EndDate).FirstOrDefault().RatingComment,
                                 RateBy = x.OrderBy(y => y.EndDate.HasValue).ThenByDescending(y => y.EndDate).FirstOrDefault().RatedBy,
                                 JobOrderStartDate = x.FirstOrDefault().JobOrderStartDate,
                                 JobOrderEndDate = x.FirstOrDefault().JobOrderEndDate
                             });

            var workTimes = _workTimeService.GetAllCandidateWorkTimeAsQueryable(false)
                            .Where(x => x.CandidateId == candidateId && 
                                        (x.Year > startYear || (x.Year == startYear && x.WeekOfYear >= startWeek)) &&
                                        x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved);

            var summary = workTimes.GroupBy(t => t.JobOrderId)
                          .Select(x => new
                           {
                               JobOrderId = x.FirstOrDefault().JobOrderId,
                               CompanyName = x.FirstOrDefault().JobOrder.Company.CompanyName,
                               JobTitle = x.FirstOrDefault().JobOrder.JobTitle,
                               TotalHours = x.Sum(cwt => cwt.NetWorkTimeInHours),
                           });

            var result = from s in summary
                         join c in placements on s.JobOrderId equals c.JobOrderId
                         select new CandidateJobHistoryModel
                         {
                             CandidateId = candidateId,
                             JobOrderId = s.JobOrderId,
                             JobOrderGuid = c.JobOrderGuid,
                             CompanyName = s.CompanyName,
                             JobTitle = s.JobTitle,
                             JobOrderStartDate = c.JobOrderStartDate,
                             JobOrderEndDate = c.JobOrderEndDate,
                             RatingValue = c.RatingValue,
                             RatingComment = c.RatingCommment,
                             RatedBy = c.RateBy,
                             TotalHours = s.TotalHours,
                         };

            return result.OrderBy(x => x.JobOrderEndDate.HasValue).ThenByDescending(x => x.JobOrderEndDate);
        }

        public IQueryable<JobHistoryModel> GetCandidateJobHistory(int candidateId, 
                                                                  int jobOrderId,
                                                                  DateTime startDate,
                                                                  IWorkContext _workContext,
                                                                  IWorkTimeService _workTimeService,
                                                                  IRepository<CandidateWorkOverTime> _overtimeRepository,
                                                                  IJobOrderService _jobOrderService,
                                                                  ICandidateJobOrderService _candidateJobOrderService)
        {
            int startYear = CommonHelper.GetYearAndWeekNumber(startDate, out int startWeek);

            var workTimes = _workTimeService.GetAllCandidateWorkTimeAsQueryable()
                            .Where(x => x.CandidateId == candidateId && 
                                        x.JobOrderId == jobOrderId &&
                                        (x.Year > startYear || (x.Year == startYear && x.WeekOfYear >= startWeek)) &&
                                        x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved)
                            .AsEnumerable()
                            .GroupBy(t => new { Year = t.Year, WeekOfYear = t.WeekOfYear } )
                            .Select(x => new
                            {
                                CompanyName = x.FirstOrDefault().JobOrder.Company.CompanyName,
                                JobTitle = x.FirstOrDefault().JobOrder.JobTitle,
                                Year = x.Key.Year,
                                WeekOfyear = x.Key.WeekOfYear,
                                Sunday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Sunday).Sum(c => c.NetWorkTimeInHours),
                                Monday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Monday).Sum(c => c.NetWorkTimeInHours),
                                Tuesday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Tuesday).Sum(c => c.NetWorkTimeInHours),
                                Wednesday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Wednesday).Sum(c => c.NetWorkTimeInHours),
                                Thursday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Thursday).Sum(c => c.NetWorkTimeInHours),
                                Friday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Friday).Sum(c => c.NetWorkTimeInHours),
                                Saturday = x.Where(c => c.JobStartDateTime.DayOfWeek == DayOfWeek.Saturday).Sum(c => c.NetWorkTimeInHours),
                                TotalHours = x.Sum(c => c.NetWorkTimeInHours)
                            });

            var overtimes = _overtimeRepository.TableNoTracking
                            .Where(x => x.CandidateId == candidateId && x.JobOrderId == jobOrderId)
                            .GroupBy(g => new { Year = g.Year, WeekOfYear = g.WeekOfYear })
                            .Select(x => new
                            {
                                Year = x.Key.Year,
                                WeekOfYear = x.Key.WeekOfYear,
                                OTHours = x.Sum(o => o.OvertimeHours)
                            });

            var result = from w in workTimes
                         from o in overtimes.Where(o => o.Year == w.Year && o.WeekOfYear == w.WeekOfyear).DefaultIfEmpty()
                         select new JobHistoryModel
                         {
                             CandidateId = candidateId,
                             JobOrderId = jobOrderId,
                             CompanyName = w.CompanyName,
                             JobTitle = w.JobTitle,
                             Year = w.Year,
                             WeekOfYear = w.WeekOfyear,
                             Sunday = w.Sunday,
                             Monday = w.Monday,
                             Tuesday = w.Tuesday,
                             Wednesday = w.Wednesday,
                             Thursday = w.Thursday,
                             Friday = w.Friday,
                             Saturday = w.Saturday,
                             SubTotalHours = w.TotalHours,
                             OTHours = o == null ? 0 : o.OTHours
                         };

            return result.OrderByDescending(x => x.Year).ThenByDescending(x => x.WeekOfYear).AsQueryable();
        }

    }

}

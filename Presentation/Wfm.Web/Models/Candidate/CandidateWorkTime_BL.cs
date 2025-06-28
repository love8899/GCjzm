using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Candidates;
using Wfm.Services.JobOrders;
using Wfm.Services.TimeSheet;
using Wfm.Web.Models.TimeSheet;


namespace Wfm.Web.Models.Candidate
{
    public class CandidateWorkTime_BL
    {
        #region Fields

        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IWorkTimeService _workTimeService;
        
        #endregion


        #region Ctor

        public CandidateWorkTime_BL(
            ICandidateJobOrderService candidateJobOrderService,
            IJobOrderService jobOrderService,
            IWorkTimeService workTimeService)
        {
            _candidateJobOrderService = candidateJobOrderService;
            _jobOrderService = jobOrderService;
            _workTimeService = workTimeService;
        } 

        #endregion


        public IList<CandidateWorkTime> CandidateWorkTimeByWeek(int candidateId, DateTime refDate)
        {
            var startDate = refDate.Date.GetDayOfThisWeek(DayOfWeek.Sunday);
            var endDate = refDate.Date.GetDayOfThisWeek(DayOfWeek.Saturday).AddDays(1);
            var dates = Enumerable.Range(0, 7).Select(offset => startDate.AddDays(offset)).Where(x => x <= DateTime.Today);

            var times = _workTimeService.GetWorkTimeByCandidateIdAsQueryable(candidateId)
                        .Where(x => x.JobStartDateTime >= startDate && x.JobStartDateTime < endDate)
                        .Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided)
                        .ToList();
            var workTimeByDay = times.Select(x => new DailyJobOrder()
                                {
                                    WorkDate = x.JobStartDateTime.Date,
                                    JobOrder = x.JobOrder,
                                });

            // TODO: exlcude non work dates and holidays (possibly unneccessary ???)
            var placement = _candidateJobOrderService.GetCandidateJobOrderByCandidateIdAsQueryable(candidateId)
                            .Where(x => x.StartDate <= endDate && (!x.EndDate.HasValue || x.EndDate > startDate))
                            .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed);
            var placementByDay = from d in dates
                                 from p in placement.Where(p => d >= p.StartDate && (!p.EndDate.HasValue || d <= p.EndDate))
                                 select new DailyJobOrder()
                                 {
                                     WorkDate = d,
                                     JobOrder = p.JobOrder
                                 };

            placementByDay = placementByDay.Where(p => !workTimeByDay.Any(w => p.WorkDate == w.WorkDate && p.JobOrder == p.JobOrder));
            var emptyTimes = placementByDay.Select(x => new CandidateWorkTime()
            {
                CandidateId = candidateId,
                JobOrderId = x.JobOrder.Id,
                JobOrder = x.JobOrder,
                CompanyId = x.JobOrder.CompanyId,
                CompanyLocationId = x.JobOrder.CompanyLocationId,
                CompanyDepartmentId = x.JobOrder.CompanyDepartmentId,
                CompanyContactId = x.JobOrder.CompanyContactId,
                JobStartDateTime = x.WorkDate + x.JobOrder.StartTime.TimeOfDay,
                JobEndDateTime = x.WorkDate.AddDays(x.JobOrder.StartTime.TimeOfDay > x.JobOrder.EndTime.TimeOfDay ? 1 : 0) + x.JobOrder.EndTime.TimeOfDay
            });

            return times.Union(emptyTimes).OrderBy(x => x.JobStartDateTime).ToList();
        }


        public class DailyJobOrder
        {
            public DateTime WorkDate { get; set; }
            public Wfm.Core.Domain.JobOrders.JobOrder JobOrder { get; set; }
        }


        public void SaveWorkTime(IEnumerable<CandidateWorkTimeModel> models)
        {
            foreach (var model in models)
            {
                var entity = new CandidateWorkTime();
                if (model.Id > 0)
                {
                    entity = _workTimeService.GetWorkTimeById(model.Id);

                    // update job start/end times, as job order may change
                    entity.JobStartDateTime = entity.JobStartDateTime.Date + entity.JobOrder.StartTime.TimeOfDay;
                    var crossDay = entity.JobOrder.EndTime.TimeOfDay < entity.JobOrder.StartTime.TimeOfDay;
                    entity.JobEndDateTime = entity.JobStartDateTime.Date.AddDays(crossDay ? 1 : 0) + entity.JobOrder.EndTime.TimeOfDay;
                    var jobDuration = entity.JobEndDateTime - entity.JobStartDateTime;
                    entity.JobOrderDurationInMinutes = (decimal)(jobDuration.TotalMinutes);
                    entity.JobOrderDurationInHours = (decimal)(jobDuration.TotalHours);
                    // TODO: other changes from job order (like loc. or dept.)

                    entity.ClockIn = model.ClockIn;
                    entity.ClockOut = model.ClockOut;
                    entity.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.PendingApproval;
                }
                else
                {
                    // skip, if duplicate
                    entity = _workTimeService.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(model.CandidateId, model.JobOrderId, model.StartDate);
                    if (entity != null)
                        continue;

                    var jobOrder = _jobOrderService.GetJobOrderById(model.JobOrderId);
                    entity = _workTimeService.PrepareCandidateWorkTimeByJobOrderAndDate(null, jobOrder, model.StartDate);
                    entity.CandidateId = model.CandidateId;
                    entity.ClockIn = model.ClockIn;
                    entity.ClockOut = model.ClockOut;
                    entity.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.PendingApproval;
                    entity.Source = WorkTimeSource.Manual;
                }

                _workTimeService.CalculateAndSaveWorkTime(entity);
                model.NetWorkTimeInHours = entity.NetWorkTimeInHours;

                if (model.Id == 0)
                {
                    model.Id = entity.Id;
                    model.CandidateWorkTimeStatusId = entity.CandidateWorkTimeStatusId;
                }
            }
        }
    }
}
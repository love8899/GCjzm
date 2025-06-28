using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;
using Wfm.Admin.Extensions;

namespace Wfm.Admin.Models.ClockTime
{
    public class CandidateCurrentStatusModel_BL
    {
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IClockTimeService _clockTimeService;
        private readonly ICandidateService _candidateService;

        public CandidateCurrentStatusModel_BL(ICandidateJobOrderService candidateJobOrderService, IClockTimeService clockTimeService, ICandidateService candidateService)
        {
            _candidateJobOrderService = candidateJobOrderService;
            _clockTimeService = clockTimeService;
            _candidateService = candidateService;
        }
        public CandidateCurrentStatusModel GetCandidateCurrentStatus(int clockTimeId)
        {
            CandidateCurrentStatusModel result = new CandidateCurrentStatusModel();
            var clockTime = _clockTimeService.GetClockTimeById(clockTimeId);

            var today = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(clockTime.CompanyId.Value, clockTime.ClockInOut.Date)
                .Where(x => x.CandidateId == clockTime.CandidateId)
                .Select(x => new CandidateJobOrderSimpleModel() 
                            {
                                JobOrderId = x.JobOrderId, JobTitle = x.JobOrder.JobTitle, 
                                Location = x.JobOrder.Company.CompanyLocations.Where(y => y.Id == x.JobOrder.CompanyLocationId).FirstOrDefault().LocationName,
                                StartTime = x.JobOrder.StartTime,
                                EndTime = x.JobOrder.EndTime,
                                Date = clockTime.ClockInOut.Date
                            });
            var yesDate = clockTime.ClockInOut.Date.AddDays(-1);

            var yesterday = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(clockTime.CompanyId.Value, clockTime.ClockInOut.Date.AddDays(-1))
                            .Where(x => x.CandidateId == clockTime.CandidateId)
                             .Select(x => new CandidateJobOrderSimpleModel()
                             {
                                 JobOrderId = x.JobOrderId,
                                 JobTitle = x.JobOrder.JobTitle,
                                 Location = x.JobOrder.Company.CompanyLocations.Where(y => y.Id == x.JobOrder.CompanyLocationId).FirstOrDefault().LocationName,
                                 StartTime = x.JobOrder.StartTime,
                                 EndTime = x.JobOrder.EndTime,
                                 Date = yesDate
                             });

            result.JobOrders = new List<CandidateJobOrderSimpleModel>();
            result.JobOrders.AddRange(today);
            result.JobOrders.AddRange(yesterday);

            DateTime start = clockTime.ClockInOut.AddHours(-12);
            DateTime end = clockTime.ClockInOut.AddHours(12);
            result.PunchRecords = new List<CandidateClockTimeModel>();
            _clockTimeService.GetAllClockTimesByCandidateIdAndLocationIdAndDateTimeRange(
                                    clockTime.CandidateId.Value, clockTime.CompanyLocationId.Value, start,end )
                                 .ForEach(x => { result.PunchRecords.Add(x.ToModel()); });

            result.Onboarded = _candidateService.IsCanidateOnboarded(clockTime.Candidate);
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.TimeSheet;
using Wfm.Core;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;
using Wfm.Services.Companies;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.TimeSheet;

namespace Wfm.Admin.Models.ClockTime
{
    public class PlacementResultModel_BL
    {
        private readonly IClockTimeService _clockTimeService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly CandidateWorkTimeModel_BL _bl;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        public PlacementResultModel_BL(IClockTimeService clockTimeService, ICandidateJobOrderService candidateJobOrderService,
            IWorkContext workContext,ILogger logger,ILocalizationService localizationService,
            IWorkTimeService workTimeService, IRecruiterCompanyService recruiterCompanyService)
        {
            _clockTimeService = clockTimeService;
            _candidateJobOrderService = candidateJobOrderService;
            _workContext = workContext;
            _logger = logger;
            _localizationService = localizationService;
            _workTimeService = workTimeService;
            _recruiterCompanyService = recruiterCompanyService;
            _bl = new CandidateWorkTimeModel_BL(_workContext,logger,_localizationService,_candidateJobOrderService,_clockTimeService,_workTimeService,_recruiterCompanyService);
        }
        public List<PlacementResultModel> GetAllPlacementResultModels(int candidateClockTimeId)
        {
            List<PlacementResultModel> result = new List<PlacementResultModel>();
            var clockTime = _clockTimeService.GetClockTimeById(candidateClockTimeId);
            var today = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(clockTime.CompanyId.Value, clockTime.ClockInOut.Date)
                            .Where(x => x.CandidateId == clockTime.CandidateId)
                            .Select(x => new PlacementResultModel() { Date = clockTime.ClockInOut.Date, DisplayOrder = 0, JobOrderId = x.JobOrderId, JobTitle = x.JobOrder.JobTitle, Guessed = false });
           var yesDate = clockTime.ClockInOut.Date.AddDays(-1);
            var yesterday = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(clockTime.CompanyId.Value, clockTime.ClockInOut.Date.AddDays(-1))
                            .Where(x => x.CandidateId == clockTime.CandidateId)
                            .Select(x => new PlacementResultModel() { Date = yesDate , DisplayOrder = 1, JobOrderId = x.JobOrderId, JobTitle = x.JobOrder.JobTitle,Guessed=false });
            var matchedWorkTimes = _bl.GetMatchedCandidateWorkTime();

            matchedWorkTimes = matchedWorkTimes.Where(x => x.Candidate.CandidateGuid == clockTime.Candidate.CandidateGuid);

            var minClockInOut = clockTime.ClockInOut.AddMinutes(-5);
            var maxClockInOut = clockTime.ClockInOut.AddMinutes(+5);
            matchedWorkTimes = matchedWorkTimes.Where(x => (x.ClockIn >= minClockInOut && x.ClockIn <= maxClockInOut) ||
                                                            (x.ClockOut >= minClockInOut && x.ClockOut <= maxClockInOut));

            var matchedModels = matchedWorkTimes
                                .Select(x => new PlacementResultModel() { Date = x.JobStartDateTime, DisplayOrder = 2, JobOrderId = x.JobOrderId, JobTitle = x.JobOrder.JobTitle, Guessed = true, CandidateMatchedWorkTimeId=x.Id });
            result.AddRange(today);
            result.AddRange(yesterday);
            result.AddRange(matchedModels);
            return result;
            
            
        }
    }
}
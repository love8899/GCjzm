using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Text;
using Wfm.Services.TimeSheet;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.ClockTime;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;
using Wfm.Services.Companies;
using Wfm.Services.Localization;
using Wfm.Services.Logging;


namespace Wfm.Admin.Models.TimeSheet
{
    public class CandidateWorkTimeModel_BL
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IClockTimeService _clockTimeService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;

        #endregion

        #region Ctor

        public CandidateWorkTimeModel_BL(IWorkContext workContext,
                                         ILogger logger,
                                         ILocalizationService localizationService,
                                         ICandidateJobOrderService candidateJobOrderService,
                                         IClockTimeService clockTimeService,
                                         IWorkTimeService workTimeService, 
                                         IRecruiterCompanyService recruiterCompanyService)
        {
            _workContext = workContext;
            _logger = logger;
            _localizationService = localizationService;
            _candidateJobOrderService = candidateJobOrderService;
            _clockTimeService = clockTimeService;
            _workTimeService = workTimeService;
            _recruiterCompanyService = recruiterCompanyService;
        }

        #endregion


        public IQueryable<CandidateWorkTime> GetAllCandidateWorkTime(DataSourceRequest request, bool includeMatched = false, int candidateId = 0, DateTime? jobStartDate = null,DateTime? jobEndDate = null)
        {
            var totalCandidateWorkTimes = _workTimeService.GetAllCandidateWorkTimeAsQueryable();
            if (!includeMatched)
                totalCandidateWorkTimes = totalCandidateWorkTimes.Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Matched);
            if (candidateId > 0)
                totalCandidateWorkTimes = totalCandidateWorkTimes.Where(x => x.CandidateId == candidateId);
            if (jobStartDate.HasValue)
                totalCandidateWorkTimes = totalCandidateWorkTimes.Where(x => x.JobStartDateTime >= jobStartDate);
            if (jobEndDate.HasValue)
            {
                jobEndDate = jobEndDate.Value.Date.AddDays(1).AddMinutes(-1);
                totalCandidateWorkTimes = totalCandidateWorkTimes.Where(x => x.JobStartDateTime <= jobEndDate);
            }
                return totalCandidateWorkTimes;
        }


        public IQueryable<CandidateWorkTime> GetMatchedCandidateWorkTime()
        {
            var allWorkTimes = _workTimeService.GetAllCandidateWorkTimeAsQueryable()
                               //.Where(x => x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Voided || x.CandidateWorkTimeStatusId != (int)CandidateWorkTimeStatus.Rejected);
                               .Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched || x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval);

            var matchedByClockIn = allWorkTimes.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched)
                                   .GroupBy(x => new { x.CandidateId, x.ClockIn })
                                   .Select(grp => new { CandidateId = grp.Key.CandidateId, ClockInOut = grp.Key.ClockIn });
            var result1 = from a in allWorkTimes
                          join m in matchedByClockIn on new { x = a.CandidateId, y = a.ClockIn } equals new { x = m.CandidateId, y = m.ClockInOut }
                          select a;
            var result2 = from a in allWorkTimes
                          join m in matchedByClockIn on new { x = a.CandidateId, y = a.ClockOut } equals new { x = m.CandidateId, y = m.ClockInOut }
                          select a;

            var matchedByClockOut = allWorkTimes.Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Matched)
                                    .GroupBy(x => new { x.CandidateId, x.ClockOut })
                                    .Select(grp => new { CandidateId = grp.Key.CandidateId, ClockInOut = grp.Key.ClockOut });
            var result3 = from a in allWorkTimes
                          join m in matchedByClockOut on new { x = a.CandidateId, y = a.ClockOut } equals new { x = m.CandidateId, y = m.ClockInOut }
                          select a;
            var result4 = from a in allWorkTimes
                          join m in matchedByClockOut on new { x = a.CandidateId, y = a.ClockIn } equals new { x = m.CandidateId, y = m.ClockInOut }
                          select a;

            return result1.Union(result2).Union(result3).Union(result4);
        }


        public void ConfirmWorkTime(int id)
        {
            var cwt = _workTimeService.GetWorkTimeById(id);
            _workTimeService.ChangeCandidateWorkTimeStatus(cwt, CandidateWorkTimeStatus.PendingApproval, _workContext.CurrentAccount);

            // remove other matched work times
            _workTimeService.RemoveOtherMatchedWorkTimes(cwt);

            // update clocktime status
            _workTimeService.SetClockTimeStatusByWorkTime(cwt);

            // update job order placement
            UpdatePlacementForWorkTime(cwt);
        }


        public void UpdatePlacementForWorkTime(CandidateWorkTime cwt)
        {
            var startDate = cwt.JobStartDateTime.Date;
            var cjo = _candidateJobOrderService.GetCandidateJobOrderByCandidateIdAsQueryable(cwt.CandidateId)
                      .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                                  x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate > startDate))
                      .FirstOrDefault();
      
            if (cjo == null || cjo.JobOrderId != cwt.JobOrderId)
            {
                if (cjo != null)
                    _candidateJobOrderService.SetCandidateJobOrderToNoStatus(cjo.CandidateId, cjo.JobOrderId, cjo.CandidateJobOrderStatusId, startDate, startDate);

                int enteredBy = 0;
                if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                    enteredBy = _workContext.CurrentAccount.Id;

                _candidateJobOrderService.InsertOrUpdateCandidateJobOrder(cwt.JobOrderId, cwt.CandidateId, startDate, (int)CandidateJobOrderStatusEnum.Placed, startDate, enteredBy);
            }
        }


        public CandidateClockTimeModel GetDefaultPunchForWorkTime(int workTimeId, DateTime clockInOut)
        {
            var cwt = _workTimeService.GetWorkTimeById(workTimeId);
            return new CandidateClockTimeModel()
            {
                CompanyId = cwt.CompanyId,
                CompanyName = cwt.JobOrder.Company.CompanyName,
                CompanyLocationId = cwt.CompanyLocationId,
                ClockDeviceUid = cwt.ClockDeviceUid,
                SmartCardUid = cwt.SmartCardUid,
                CandidateId = cwt.CandidateId,
                CandidateFirstName = cwt.Candidate.FirstName,
                CandidateLastName = cwt.Candidate.LastName,
                ClockInOut = clockInOut,
            };
        }


        public string SavePunchForWorkTime(CandidateClockTimeModel model, int workTimeId, string inOut)
        {
            StringBuilder errorMessage = new StringBuilder();

            try
            {
                var entity = model.ToEntity();
                entity.Source = ClockTimeSource.Manual;
                entity.CandidateClockTimeStatusId = (int)CandidateClockTimeStatus.NoStatus;
                entity.CreatedOnUtc = entity.UpdatedOnUtc = DateTime.UtcNow;
                entity.EnteredBy = entity.UpdatedBy = _workContext.CurrentAccount.Id;
                _clockTimeService.Insert(entity);

                var cwt = _workTimeService.GetWorkTimeById(workTimeId);
                if (cwt != null)
                {
                    if (inOut == "in")
                        cwt.ClockIn = entity.ClockInOut;
                    else if (inOut == "out")
                        cwt.ClockOut = entity.ClockInOut;
                    _workTimeService.CalculateAndSaveWorkTime(cwt);

                    // cleanup other matched work times
                    _workTimeService.RemoveOtherMatchedWorkTimes(cwt);

                    // update clocktimes' status
                    _workTimeService.SetClockTimeStatusByWorkTime(cwt);
                }
            }
            catch (WfmException ex)
            {
                errorMessage.AppendLine(ex.Message);
            }
            catch (Exception exc)
            {
                errorMessage.AppendLine(_localizationService.GetResource("Common.UnexpectedError"));
                _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
            }

            return errorMessage.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Admin.Extensions;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.ClockTime;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;


namespace Wfm.Admin.Models.ClockTime
{
    public class CandidateSmartCardMatch_BL
    {
        #region Fields

        private readonly ICandidateService _candidateService;
        private readonly ISmartCardService _smartCardService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IClockTimeService _clockTimeService;
        
        #endregion


        #region Ctor

        public CandidateSmartCardMatch_BL(ICandidateService candidateService, 
                                          ISmartCardService smartCardService, 
                                          ICandidateJobOrderService candidateJobOrderService,
                                          IClockTimeService clockTimeService)
        {
            _candidateService = candidateService;
            _smartCardService = smartCardService;
            _candidateJobOrderService = candidateJobOrderService;
            _clockTimeService = clockTimeService;
        }
        
        #endregion


        public IList<CandidateSmartCardMatchModel> GetAllCandidatesMatched(int companyLocationId, string smartCardUid, DateTime clockInOut)
        {
            IList<CandidateSmartCard> smartCards = new List<CandidateSmartCard>();
            var result = new List<CandidateSmartCardMatchModel>();
            var refDate = clockInOut.Date;
            smartCardUid = smartCardUid.Trim();

            // all cards of same candidate
            var candidate = _smartCardService.GetCandidateBySmartCardUid(smartCardUid, activeOnly: false);
            if (candidate != null)
            {
                smartCards = _smartCardService.GetAllSmartCardsByCandidateId(candidate.Id, showInactive: true)
                             .Where(x => x.IsActive || x.SmartCardUid == smartCardUid).ToList();
                result.AddRange(smartCards.Select(x => x.ToMatchModel(smartCardUid)));
            }
            // other candidates
            else
            {
                // TODO: limit job orders by start/end time against clockInOut?
                var placed = _candidateJobOrderService.GetAllCandidateJobOrdersByDateRangeAsQueryable(refDate, refDate, (int)CandidateJobOrderStatusEnum.Placed)
                             .Where(x => x.JobOrder.CompanyLocationId == companyLocationId);
                var placedCandidateIds = placed.Select(x => x.CandidateId);
                
                // partial
                if (smartCardUid.Length < 10)
                {
                    // candidates with cards
                    smartCards = _smartCardService.GetSmartCardsByPartialSmartCardUid(smartCardUid)
                                 .Where(x => placedCandidateIds.Contains(x.CandidateId)).ToList();
                    result.AddRange(smartCards.Join(placed, c => c.CandidateId, p => p.CandidateId, (c, p) => new CandidateSmartCardMatchModel()
                    {
                        SmartCardUid = c.SmartCardUid,
                        IsActive = c.IsActive,
                        CandidateId = c.CandidateId,
                        CandidateFirstName = c.Candidate.FirstName,
                        CandidateLastName = c.Candidate.LastName,
                        EmployeeId = c.Candidate.EmployeeId,
                        FranchiseId = c.Candidate.FranchiseId,
                        JobOrderId =  p.JobOrderId,
                        JobTitle = p.JobOrder.JobTitle,
                        StartTime = p.JobOrder.StartTime,
                        EndTime = p.JobOrder.EndTime,
                        CandidateSmartCardMatchStatusId = (int)CandidateSmartCardMatchStatus.Partial
                    }));
                }
                // unknown
                else
                {
                    // candidates without cards
                    var ListOfCandiatesWithCards = _smartCardService.GetAllSmartCardsAsQueryable(showInactive: false).Select(x => x.CandidateId);
                    result.AddRange(placed.Where(x => !ListOfCandiatesWithCards.Contains(x.CandidateId)).Select(x => new CandidateSmartCardMatchModel()
                    {
                        CandidateId = x.CandidateId,
                        CandidateFirstName = x.Candidate.FirstName,
                        CandidateLastName = x.Candidate.LastName,
                        EmployeeId = x.Candidate.EmployeeId,
                        FranchiseId = x.Candidate.FranchiseId,
                        JobOrderId = x.JobOrderId,
                        JobTitle = x.JobOrder.JobTitle,
                        StartTime = x.JobOrder.StartTime,
                        EndTime = x.JobOrder.EndTime,
                        CandidateSmartCardMatchStatusId = (int)CandidateSmartCardMatchStatus.Unknown
                    }));
                }
            }

            return result;
        }


        public void ConfirmCandidateForClockTimes(string smartCardUid, int candidateId, string selectedUid, int accountId)
        {
            var candidate = _candidateService.GetCandidateById(candidateId);
            if (candidate != null)
            {
                var finalCardUid = selectedUid;
                
                // insert smart card
                if (String.IsNullOrWhiteSpace(selectedUid))
                {
                    finalCardUid = smartCardUid;
                    var now = DateTime.Now;
                    _smartCardService.Insert(new CandidateSmartCard()
                    {
                        CandidateId = candidateId,
                        SmartCardUid = finalCardUid,
                        IsActive = true,
                        ActivatedDate = now.Date,
                        EnteredBy = accountId,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now
                    });
                }
                
                var clockTimes = _clockTimeService.GetAllCandidateClockTimesAsQueryable()
                                 .Where(x => x.SmartCardUid == smartCardUid && !x.CandidateId.HasValue).ToList();
                clockTimes.ForEach(x =>
                {
                    x.SmartCardUid = finalCardUid;
                    x.CandidateId = candidateId;
                    x.CandidateFirstName = candidate.FirstName;
                    x.CandidateLastName = candidate.LastName;
                    x.UpdatedOnUtc = DateTime.UtcNow;
                    _clockTimeService.Update(x);
                });
            }
        }
        
    }
}
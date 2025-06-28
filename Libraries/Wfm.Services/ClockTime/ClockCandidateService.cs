using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using RecogSys.RdrAccess;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Data;
using Wfm.Data;


namespace Wfm.Services.ClockTime
{
    public class ClockCandidateService : IClockCandidateService
    {
        #region Fields

        private readonly IRepository<ClockCandidate> _clockCandidateRepository;
        private readonly ISmartCardService _smartCardService;
        private readonly IHandTemplateService _handTemplateService;
        private readonly IDbContext _dbContext;

        #endregion


        #region Ctor

        public ClockCandidateService(IRepository<ClockCandidate> clockCandidateRepository,
            ISmartCardService smartCardService,
            IHandTemplateService handTemplateService,
            IDbContext dbContext)
        {
            _clockCandidateRepository = clockCandidateRepository;
            _smartCardService = smartCardService;
            _handTemplateService = handTemplateService;
            _dbContext = dbContext;
        }

        #endregion


        #region CRUD

        public void Insert(ClockCandidate clockCandidate)
        {
            if (clockCandidate == null)
                throw new ArgumentNullException("clockCandidate");

            clockCandidate.AddedOnUtc = DateTime.UtcNow;

            _clockCandidateRepository.Insert(clockCandidate);
        }


        public void Update(ClockCandidate clockCandidate)
        {
            if (clockCandidate == null)
                throw new ArgumentNullException("clockCandidate");

            clockCandidate.AddedOnUtc = DateTime.UtcNow;

            _clockCandidateRepository.Update(clockCandidate);
        }


        public virtual void Delete(ClockCandidate clockCandidate)
        {
            if (clockCandidate == null)
                throw new ArgumentNullException("clockCandidate");

            _clockCandidateRepository.Delete(clockCandidate);
        }


        public virtual void Delete(int clockDeviceId, int candidateId)
        {
            var existing = GetClockCandidate(clockDeviceId, candidateId);
            if (existing != null)
                Delete(existing);
        }


        public void InsertOrUpdate(int clockDeviceId, int candidateId)
        {
            var existing = GetClockCandidate(clockDeviceId, candidateId);
            if (existing != null)
                Update(existing);   // update nothing but AddedOnUtc
            else
                Insert(new ClockCandidate()
                {
                    ClockDeviceId = clockDeviceId,
                    CandidateId = candidateId,
                });
        }


        public void RemoveClockCandidates(int clockDeviceId, IEnumerable<int> candidateIds = null)
        {
            var cmd = String.Format("delete ClockCandidate where ClockDeviceId = {0}", clockDeviceId);
            if (candidateIds != null && candidateIds.Any())
                cmd += String.Format(" and CandidateId in ({0})", String.Join(",", candidateIds));

            _dbContext.ExecuteSqlCommand(cmd);
        }

        #endregion


        #region ClockCandidate

        public ClockCandidate GetClockCandidateById(int id)
        {
            return _clockCandidateRepository.GetById(id);
        }


        public ClockCandidate GetClockCandidate(int clockDeviceId, int candidateId)
        {
            return this.GetAllClockCandidatesByClock(clockDeviceId).FirstOrDefault(x => x.CandidateId == candidateId);
        }

        #endregion


        #region List

        public IQueryable<ClockCandidate> GetAllClockCandidatesAsQueryable()
        {
            return _clockCandidateRepository.Table;
        }


        public IQueryable<ClockCandidate> GetAllClockCandidatesByClock(int clockDeviceId)
        {
            return this.GetAllClockCandidatesAsQueryable().AsNoTracking().Where(x => x.ClockDeviceId == clockDeviceId);
        }

        #endregion


        #region Add / Remove

        public bool AddOrRemoveCandidate(CompanyClockDevice clockDevice, HandReader hr, string action, int candidateId)
        {
            var result = false;

            var rsp = new RSI_STATUS();
            if (clockDevice.ManualID)
            {
                if (action == "add")
                {
                    var template = _handTemplateService.GetActiveHandTemplateByCandidateId(candidateId);
                    if (template != null && hr.PutUserRecord(template.ToRsiUserRecord(), rsp))
                    {
                        this.InsertOrUpdate(clockDevice.Id, candidateId);
                        result = true;
                    }
                }
                else if (action == "remove" && hr.RemoveUser(candidateId.ToString(), rsp))
                {
                    this.Delete(clockDevice.Id, candidateId);
                    result = true;
                }
            }
            else
            {
                var smartCards = _smartCardService.GetAllSmartCardsByCandidateId(candidateId, showInactive: true);
                if (action == "add")
                {
                    var smartCard = smartCards.FirstOrDefault(x => x.IsActive);
                    var template = _handTemplateService.GetActiveHandTemplateByCandidateId(candidateId);
                    if (template != null && !String.IsNullOrWhiteSpace(smartCard?.SmartCardUid))
                    {
                        var idStr = _smartCardService.GetIdString(smartCard, clockDevice.IDLength);
                        if (hr.PutUserRecord(template.ToRsiUserRecord(idStr), rsp))
                        {
                            this.InsertOrUpdate(clockDevice.Id, candidateId);
                            result = true;
                        }
                    }
                }
                else if (action == "remove")
                {
                    var removed = true;
                    foreach (var smartCard in smartCards)   // all cards including inactive
                    {
                        if (!String.IsNullOrWhiteSpace(smartCard.SmartCardUid))
                        {
                            var idStr = _smartCardService.GetIdString(smartCard, clockDevice.IDLength);
                            removed &= hr.RemoveUser(idStr, rsp);
                        }
                    }
                    if (removed)
                    {
                        this.Delete(clockDevice.Id, candidateId);
                        result = true;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}

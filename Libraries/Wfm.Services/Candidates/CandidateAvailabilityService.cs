using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Common;


namespace Wfm.Services.Candidates
{
    public class CandidateAvailabilityService : ICandidateAvailabilityService
    {
        #region Fields

        private readonly IRepository<CandidateAvailability> _candidateAvailabilityRepository;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly IShiftService _shiftService;
        private readonly ICandidateService _candidateService;

        #endregion


        #region Ctor

        public CandidateAvailabilityService(IRepository<CandidateAvailability> candidateAvailabilityRepository,
            IRepository<Candidate> candidateRepository,
            IShiftService shiftService,
            ICandidateService candidateService)
        {
            _candidateAvailabilityRepository = candidateAvailabilityRepository;
            _candidateRepository = candidateRepository;
            _shiftService = shiftService;
            _candidateService = candidateService;
        }

        #endregion


        #region CRUD

        public void Insert(CandidateAvailability candidateAvailability)
        {
            if (candidateAvailability == null)
                throw new ArgumentNullException("candidateAvailability");

            candidateAvailability.CreatedOnUtc = candidateAvailability.UpdatedOnUtc = DateTime.UtcNow;

            _candidateAvailabilityRepository.Insert(candidateAvailability);
        }


        public void Update(CandidateAvailability candidateAvailability)
        {
            if (candidateAvailability == null)
                throw new ArgumentNullException("candidateAvailability");

            candidateAvailability.UpdatedOnUtc = DateTime.UtcNow;

            _candidateAvailabilityRepository.Update(candidateAvailability);
        }


        public virtual void Delete(CandidateAvailability candidateAvailability)
        {
            if (candidateAvailability == null)
                throw new ArgumentNullException("candidateAvailability");

            _candidateAvailabilityRepository.Delete(candidateAvailability);
        }

        #endregion


        public CandidateAvailability GetCandidateAvailabilityById(int id)
        {
            return _candidateAvailabilityRepository.GetById(id);
        }


        public IQueryable<CandidateAvailability> GetAllCandidateAvailability(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _candidateAvailabilityRepository.Table;

            if (startDate.HasValue)
                query = query.Where(x => x.Date >= startDate);

            if (endDate.HasValue)
                query = query.Where(x => x.Date <= endDate);

            return query;
        }


        public IQueryable<CandidateAvailability> GetCandidateAvailabilityByCandidate(int candidateId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return GetAllCandidateAvailability(startDate, endDate).Where(x => x.CandidateId == candidateId);
        }


        public IQueryable<CandidateAvailability> GetCandidateAvailabilityByShift(int shiftId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return GetAllCandidateAvailability(startDate, endDate).Where(x => x.ShiftId == shiftId);
        }


        public IQueryable<CandidateAvailability> GetExpectedCandidateAvailabilityByCandidate(int candidateId, DateTime startDate, DateTime endDate, bool byShift = true, bool excludePast = true)
        {
            var today = DateTime.Today;
            var range = (endDate - startDate).Days + 1;
            var dates = Enumerable.Range(0, range).Select(x => startDate.AddDays(x));
            var result = Enumerable.Empty<CandidateAvailability>();

            var existing = GetCandidateAvailabilityByCandidate(candidateId, startDate, endDate);
            var existingDates = existing.Select(x => x.Date);

            // exclude pass dates without existing availability
            if (excludePast)
                dates = dates.Where(x => x > today || existingDates.Contains(x));

            if (!byShift)
            {
                var expectedDates = dates.Where(x => !existingDates.Contains(x));
                result = expectedDates.Select(x => new CandidateAvailability()
                {
                    CandidateId = candidateId,
                    Date = x,
                });
            }
            else
            {
                var candidate = _candidateService.GetCandidateById(candidateId);
                var shifts = _shiftService.GetAllShiftsEnableInSchedule();
                var allDatesAndShifts = dates.SelectMany(d => shifts, (d, s) => new { Date = d, Shift = s });
                result = from a in allDatesAndShifts
                         join e in existing on new { x = a.Date, y = a.Shift.Id } equals new { x = e.Date, y = e.ShiftId } into joined
                         from j in joined.DefaultIfEmpty() where j == null
                         select new CandidateAvailability()
                         {
                             CandidateId = candidateId,
                             Candidate = candidate,
                             Date = a.Date,
                             ShiftId = a.Shift.Id,
                             Shift = a.Shift,
                         };
            }

            return result.AsQueryable();
        }


        public IEnumerable<CandidateAvailability> GetAllCandidateAvailabilityByCandidate(int candidateId, DateTime startDate, DateTime endDate, bool byShift = true, bool excludePast = true)
        {
            var existing = GetCandidateAvailabilityByCandidate(candidateId, startDate, endDate).AsEnumerable();
            var expected = GetExpectedCandidateAvailabilityByCandidate(candidateId, startDate, endDate, byShift, excludePast).AsEnumerable();

            return existing.Concat(expected);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Data;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Data;


namespace Wfm.Services.ClockTime
{
    public class HandTemplateService : IHandTemplateService
    {
        #region Fields

        IRepository<HandTemplate> _handTemplateRepository;
        IRepository<Candidate> _candidateRepository;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;

        #endregion


        #region Ctor

        public HandTemplateService(IRepository<HandTemplate> handTemplateRepository,
            IRepository<Candidate> employeeRepository,
            IWorkContext workContext,
            IDbContext dbContext)
        {
            _handTemplateRepository = handTemplateRepository;
            _candidateRepository = employeeRepository;
            _workContext = workContext;
            _dbContext = dbContext;
        }

        #endregion


        #region CRUD

        public void Insert(HandTemplate handTemplate)
        {
            if (handTemplate == null)
                throw new ArgumentNullException("handTemplate");

            handTemplate.CreatedOnUtc = DateTime.UtcNow;
            handTemplate.UpdatedOnUtc = DateTime.UtcNow;

            _handTemplateRepository.Insert(handTemplate);
        }


        public void Update(HandTemplate handTemplate)
        {
            if (handTemplate == null)
                throw new ArgumentNullException("handTemplate");

            handTemplate.UpdatedOnUtc = DateTime.UtcNow;

            _handTemplateRepository.Update(handTemplate);
        }


        public virtual void Delete(HandTemplate handTemplate)
        {
            if (handTemplate == null)
                throw new ArgumentNullException("handTemplate");

            _handTemplateRepository.Delete(handTemplate);
        }


        public void InsertOrUpdate(HandTemplate handTemplate)
        {
            if (handTemplate == null)
                throw new ArgumentNullException("handTemplate");

            var existing = GetAllHandTemplatesByCandidateId(handTemplate.CandidateId).FirstOrDefault();
            if (existing != null)
            {
                existing.TemplateVector = handTemplate.TemplateVector;
                existing.AuthorityLevel = handTemplate.AuthorityLevel;
                existing.RejectThreshold = handTemplate.RejectThreshold;
                existing.TimeZone = handTemplate.TimeZone;
                existing.IsActive = handTemplate.IsActive;
                existing.IsDeleted = handTemplate.IsDeleted;
                existing.Note = handTemplate.Note;

                Update(existing);
            }
            else
                Insert(handTemplate);
        }

        #endregion


        #region HandTemplate

        public HandTemplate GetHandTemplateById(int id)
        {
            if (id <= 0)
                return null;

            return _handTemplateRepository.GetById(id);
        }


        public HandTemplate GetHandTemplateByGuid(Guid guid)
        {
            if (guid == null)
                return null;

            return _handTemplateRepository.Table.FirstOrDefault(x => x.HandTemplateGuid == guid);
        }


        public HandTemplate GetActiveHandTemplateByCandidateId(int candidateId)
        {
            if (candidateId <= 0)
                return null;

            return _handTemplateRepository.Table.FirstOrDefault(x => x.CandidateId == candidateId && x.IsActive);
        }

        #endregion


        #region List

        public IList<HandTemplate> GetAllHandTemplatesByCandidateId(int candidateId, bool showInactive = false)
        {
            if (candidateId == 0)
                return null;

            var query = this.GetAllHandTemplatesAsQueryable(false, showInactive).AsNoTracking();

            query = from s in query
                    where s.CandidateId == candidateId
                    select s;

            return query.ToList();
        }


        public IQueryable<HandTemplate> GetAllHandTemplatesAsQueryable(bool showHidden = false, bool showInactive = true)
        {
            var query = _handTemplateRepository.Table.Where(x => (showHidden || !x.IsDeleted) && (showInactive || x.IsActive));

            return query.OrderByDescending(x => x.UpdatedOnUtc).ThenByDescending(x => x.CandidateId);
        }

        #endregion

    }
}

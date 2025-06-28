using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Core.Domain.Tests;

namespace Wfm.Services.Test
{
    public class CandidateTestLinkService:ICandidateTestLinkService
    {
        #region Filed
        private readonly IRepository<CandidateTestLink> _candidateTestLinkRepository;
        #endregion

        #region CTOR
        public CandidateTestLinkService(IRepository<CandidateTestLink> candidateTestLinkRepository)
        {
            _candidateTestLinkRepository = candidateTestLinkRepository;
        }
        #endregion

        #region CRUD        
        public void Create(CandidateTestLink entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _candidateTestLinkRepository.Insert(entity);
        }

        public CandidateTestLink Retrieve(int id)
        {
            var entity = _candidateTestLinkRepository.GetById(id);
            return entity;
        }

        public CandidateTestLink Retrieve(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return null;
            var entity = _candidateTestLinkRepository.Table.Where(x => x.CandidateTestLinkGuid == guid).FirstOrDefault();
            return entity;
        }

        public void Update(CandidateTestLink entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _candidateTestLinkRepository.Update(entity);
        }

        public void Delete(CandidateTestLink entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            _candidateTestLinkRepository.Delete(entity);
        }
        #endregion
    }
}

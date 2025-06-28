using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial class CandidateBlacklistService : ICandidateBlacklistService
    {

        #region Fields

        private readonly IRepository<CandidateBlacklist> _candidateBlacklistRepository;
       
        #endregion

        #region Ctor

        public CandidateBlacklistService(
            IRepository<CandidateBlacklist> candidateBlacklistRepository
            )
        {
            _candidateBlacklistRepository = candidateBlacklistRepository;
        }

        #endregion

        #region CRUD

        public void InsertCandidateBlacklist(CandidateBlacklist candidateBlacklist)
        {
            if (candidateBlacklist == null)
                throw new ArgumentNullException("candidateBlacklist");

            _candidateBlacklistRepository.Insert(candidateBlacklist);

        }

        public void UpdateCandidateBlacklist(CandidateBlacklist candidateBlacklist)
        {
            if (candidateBlacklist == null)
                throw new ArgumentNullException("candidateBlacklist");

            _candidateBlacklistRepository.Update(candidateBlacklist);
        }

        public void DeleteCandidateBlacklist(CandidateBlacklist candidateBlacklist)
        {
            if (candidateBlacklist == null)
                throw new ArgumentNullException("candidateBlacklist");

            _candidateBlacklistRepository.Delete(candidateBlacklist);
        }

        #endregion


        #region CandidateBlacklist

        public CandidateBlacklist GetCandidateBlacklistById(int id)
        {
            if (id == 0)
                return null;

            return _candidateBlacklistRepository.GetById(id);
        }


        public CandidateBlacklist GetCandidateBlacklistByCandidateIdAndCompanyId(int candidateId, int? companyId)
        {
            if (candidateId == 0)
                return null;

            var query = _candidateBlacklistRepository.Table;

            query = query.Where(x => x.CandidateId == candidateId && x.ClientId == companyId);

            return query.FirstOrDefault();
        }


        public CandidateBlacklist GetContainingCandidateBlacklist(int candidateId, DateTime startDate, int? companyId)
        {
            if (candidateId == 0 || startDate == null)
                return null;

            var query = GetAllCandidateBlacklistsByCompanyIdAndDate(companyId, startDate);

            query = query.Where(x => x.CandidateId == candidateId);

            return query.FirstOrDefault();
        }


        public IList<CandidateBlacklist> GetContainedCandidateBlacklists(int candidateId, DateTime startDate, int? companyId)
        {
            if (candidateId == 0 || startDate == null)
                return null;

            var query = GetAllCandidateBlacklistsByCandidateId(candidateId);

            query = query.Where(x => x.EffectiveDate >= startDate &&
                                     (x.ClientId == companyId || (x.ClientId.HasValue && !companyId.HasValue)));

            return query.ToList();
        }

        #endregion


        #region LIST

        public IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsAsQueryable(Account account)
        {
            var query = _candidateBlacklistRepository.Table;

            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(x => x.Candidate.FranchiseId == account.FranchiseId);

            return query;
        }


        public IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsByCandidateId(int candidateId)
        {
            if (candidateId == 0)
                return null;

            var query = _candidateBlacklistRepository.Table;

            query = query.Where(x => x.CandidateId == candidateId);

            return query;
        }


        public IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsByCompanyIdAndDate(int? companyId, DateTime refDate)
        {
            if (refDate == null)
                return Enumerable.Empty<CandidateBlacklist>().AsQueryable();

            var query = _candidateBlacklistRepository.TableNoTracking;

            query = GetAllCandidateBlacklistsByDate(refDate)
                    .Where(x => x.ClientId == companyId || (!x.ClientId.HasValue && companyId.HasValue));

            return query;
        }


        public IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsByDate(DateTime refDate)
        {
            if (refDate == null)
                return null;

            var query = _candidateBlacklistRepository.TableNoTracking;

            query = query.Where(x => x.EffectiveDate <= refDate);

            return query;
        }

        #endregion

    }
}

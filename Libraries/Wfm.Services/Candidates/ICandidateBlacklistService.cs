using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Services.Candidates
{
    /// <summary>
    /// Candidate Service Interface
    /// </summary>
    public partial interface ICandidateBlacklistService
    {

        #region CRUD

        void InsertCandidateBlacklist(CandidateBlacklist candidateBlacklist);

        void UpdateCandidateBlacklist(CandidateBlacklist candidateBlacklist);

        void DeleteCandidateBlacklist(CandidateBlacklist candidateBlacklist);

        #endregion

        #region CandidateBlacklist

        CandidateBlacklist GetCandidateBlacklistById(int id);

        CandidateBlacklist GetCandidateBlacklistByCandidateIdAndCompanyId(int candidateId, int? companyId);

        CandidateBlacklist GetContainingCandidateBlacklist(int candidateId, DateTime startDate, int? companyId);

        IList<CandidateBlacklist> GetContainedCandidateBlacklists(int candidateId, DateTime startDate, int? companyId);

        #endregion

        #region LIST

        IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsAsQueryable(Account account);

        IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsByCandidateId(int candidateId);

        IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsByCompanyIdAndDate(int? companyId, DateTime refDate);

        IQueryable<CandidateBlacklist> GetAllCandidateBlacklistsByDate(DateTime refDate);

        #endregion

    }
}

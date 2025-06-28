using System;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateKeySkillService
    {

        /// <summary>
        /// Inserts the candidate key skill.
        /// </summary>
        /// <param name="candidateKeySkill">The candidate key skill.</param>
        void InsertCandidateKeySkill(CandidateKeySkill candidateKeySkill);

        /// <summary>
        /// Deletes the candidate key skill.
        /// </summary>
        /// <param name="candidateKeySkill">The candidate key skill.</param>
        void DeleteCandidateKeySkill(CandidateKeySkill candidateKeySkill);

        /// <summary>
        /// Updates the candidate key skill.
        /// </summary>
        /// <param name="candidateKeySkill">The candidate key skill.</param>
        void UpdateCandidateKeySkill(CandidateKeySkill candidateKeySkill);

        /// <summary>
        /// Gets the candidate key skills by CandidateId.
        /// </summary>
        /// <param name="CandidateId">The candidate id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<CandidateKeySkill> GetCandidateKeySkillsByCandidateId(int CandidateId, int pageIndex = 0, int PageSize=int.MaxValue);

        /// <summary>
        /// Gets the candidate key skill by Id.
        /// </summary>
        /// <param name="CandidateSkillId">The CandidateSkillId.</param>
        /// <returns></returns>
        CandidateKeySkill GetCandidateKeySkillById(int CandidateSkillId);

        CandidateKeySkill GetCandidateKeySkillByGuid(Guid? guid);


        IQueryable<CandidateKeySkill> GetAllCandidateKeySkillsAsQueryable(bool showHidden = false);
        IQueryable<CandidateKeySkillExtended> GetAllCandidateKeySkillsAsQueryable();

    }
}

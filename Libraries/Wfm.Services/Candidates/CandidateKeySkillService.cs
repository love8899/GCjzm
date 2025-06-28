using System;
using System.Linq;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Data;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Candidates
{
    public partial class CandidateKeySkillService : ICandidateKeySkillService
    {
        #region Fields

        private readonly IRepository<CandidateKeySkill> _candidateKeySkillsRepository;
        private readonly IWorkContext _workContext;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Cotr

        public CandidateKeySkillService(IRepository<CandidateKeySkill> candidateKeySkillsRepository, IRepository<Candidate> candidateRepository, IWorkContext workContext, CommonSettings commonSettings)
        {
            _candidateKeySkillsRepository = candidateKeySkillsRepository;
            _workContext = workContext;
            _commonSettings = commonSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the Candidate Skill Id.
        /// </summary>
        /// <param name="CandidateSkillId">The CandidateSkillId.</param>
        /// <returns>CandidateKeySkill</returns>
        public CandidateKeySkill GetCandidateKeySkillById(int CandidateSkillId)
        {
            var query = _candidateKeySkillsRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            CandidateKeySkill _candidateKeySkill = (from d in query
                                                    where d.Id == CandidateSkillId
                                                    select d).FirstOrDefault();

            if (_candidateKeySkill != null) return _candidateKeySkill;
            else return null;
        }

        public CandidateKeySkill GetCandidateKeySkillByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            var result = _candidateKeySkillsRepository.Table.Where(x => x.CandidateKeySkillGuid == guid).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Inserts the candidate keyskill.
        /// </summary>
        /// <param name="candidateKeySkill">The candidate key skill.</param>
        /// <exception cref="System.ArgumentNullException">CandidateKeySkill</exception>
        public void InsertCandidateKeySkill(CandidateKeySkill candidateKeySkill)
        {
            if (candidateKeySkill == null) throw new ArgumentNullException("CandidateKeySkill");

            candidateKeySkill.CreatedOnUtc = DateTime.UtcNow;
            candidateKeySkill.UpdatedOnUtc = DateTime.UtcNow;

            _candidateKeySkillsRepository.Insert(candidateKeySkill);
        }

        /// <summary>
        /// Deletes the candidate keySkill.
        /// </summary>
        /// <param name="candidateKeySkill">The candidate key skill.</param>
        /// <exception cref="System.ArgumentNullException">candidateKeySkill</exception>
        public void DeleteCandidateKeySkill(CandidateKeySkill candidateKeySkill)
        {
            if (candidateKeySkill == null) throw new ArgumentNullException("candidateKeySkill");
            candidateKeySkill.UpdatedOnUtc = DateTime.UtcNow;
            _candidateKeySkillsRepository.Delete(candidateKeySkill);
        }

        /// <summary>
        /// Updates the candidate keySkill.
        /// </summary>
        /// <param name="candidateKeySkill">The candidate key skill.</param>
        /// <exception cref="System.ArgumentNullException">candidateKeySkill</exception>
        public void UpdateCandidateKeySkill(CandidateKeySkill candidateKeySkill)
        {
            if (candidateKeySkill == null) throw new ArgumentNullException("candidateKeySkill");
            candidateKeySkill.UpdatedOnUtc = DateTime.UtcNow;
            _candidateKeySkillsRepository.Update(candidateKeySkill);
        }

        /// <summary>
        /// Gets the candidate key skills by CandidateId.
        /// </summary>
        /// <param name="CandidateId">The candidate id.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <returns></returns>
        public IPagedList<CandidateKeySkill> GetCandidateKeySkillsByCandidateId(int CandidateId, int pageIndex = 0, int PageSize = int.MaxValue)
        {
            var query = _candidateKeySkillsRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(b => b.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = query.Where(b=>b.CandidateId == CandidateId);

            query = query.OrderByDescending(b => b.LastUsedDate).ThenByDescending(b => b.UpdatedOnUtc);

            var candidateKeySkills = new PagedList<CandidateKeySkill>(query, pageIndex, PageSize);

            return candidateKeySkills;
        }


        public IQueryable<CandidateKeySkill> GetAllCandidateKeySkillsAsQueryable(bool showHidden = false)
        {
            var query = _candidateKeySkillsRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            Account account = _workContext.CurrentAccount;
            if (account.IsVendor())
                query = query.Where(x => x.Candidate.FranchiseId == account.FranchiseId);

            query = from a in query
                    orderby a.CreatedOnUtc descending, a.LastUsedDate descending
                    select a;

            return query.AsQueryable();
        }

        public IQueryable<CandidateKeySkillExtended> GetAllCandidateKeySkillsAsQueryable()
        {
            IQueryable<CandidateKeySkill> query = this.GetAllCandidateKeySkillsAsQueryable(false);

            var result = from a in query
                         select new CandidateKeySkillExtended()
                         {
                             CandidateId = a.CandidateId,
                             EmployeeId = a.Candidate.EmployeeId,
                             CandidateGuid = a.Candidate.CandidateGuid,
                             CandidateKeySkillGuid = a.CandidateKeySkillGuid,
                             KeySkill = a.KeySkill,
                             LastUsedDate = a.LastUsedDate, 
                             Note = a.Note, 
                             YearsOfExperience = a.YearsOfExperience,
                             CreatedOnUtc = a.CreatedOnUtc, 
                             UpdatedOnUtc = a.UpdatedOnUtc, 
                             Id= a.Id,
                             CandidateName = a.Candidate.FirstName + " "+ a.Candidate.LastName
                         };

            return result;
        }
        #endregion

 
    }
}

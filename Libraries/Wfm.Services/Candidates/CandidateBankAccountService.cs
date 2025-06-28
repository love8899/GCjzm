using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public class CandidateBankAccountService : ICandidateBankAccountService
    {
        #region Fields

        IRepository<CandidateBankAccount> _candidateBankAccountRepository;
        IRepository<Candidate> _candidateRepository;

        #endregion

        #region Ctor

        public CandidateBankAccountService(IRepository<CandidateBankAccount> candidateBankAccountRepository,
            IRepository<Candidate> candidateRepository)
        {
            _candidateBankAccountRepository = candidateBankAccountRepository;
            _candidateRepository = candidateRepository;
        }

        #endregion


        #region CRUD

        public void Insert(CandidateBankAccount candidateBankAccount)
        {
            if (candidateBankAccount == null)
                throw new ArgumentNullException("candidateBankAccount");

            _candidateBankAccountRepository.Insert(candidateBankAccount);
        }

        public void Update(CandidateBankAccount candidateBankAccount)
        {
            if (candidateBankAccount == null)
                throw new ArgumentNullException("candidateBankAccount");

            _candidateBankAccountRepository.Update(candidateBankAccount);
        }

        public virtual void Delete(CandidateBankAccount candidateBankAccount)
        {
            if (candidateBankAccount == null)
                throw new ArgumentNullException("candidateBankAccount");

            _candidateBankAccountRepository.Delete(candidateBankAccount);
        }

        #endregion


        #region CandidateBankAccount

        public CandidateBankAccount GetCandidateBankAccountById(int id)
        {
            if (id <= 0)
                return null;

            var query = _candidateBankAccountRepository.Table;

            query = from c in _candidateBankAccountRepository.Table
                    where c.Id == id
                    select c;

            return query.FirstOrDefault();
        }

        public bool IsDuplicate(string candidateBankAccountNumber)
        {
            if (string.IsNullOrWhiteSpace(candidateBankAccountNumber))
                return true;

            var query = _candidateBankAccountRepository.Table;

            // check duplication
            query = from c in query
                    where c.AccountNumber == candidateBankAccountNumber
                    select c;

            // TO DO: other validation
            return query.Count() > 0;
        }

        public CandidateBankAccount GetCandidateBankAccountByBankAccountNumber(string candidateBankAccountNumber)
        {
            if (string.IsNullOrWhiteSpace(candidateBankAccountNumber))
                return null;

            var query = _candidateBankAccountRepository.Table;

            query = from c in _candidateBankAccountRepository.Table
                    where c.AccountNumber == candidateBankAccountNumber
                    select c;

            return query.FirstOrDefault();
        }

        public CandidateBankAccount GetActiveCandidateBankAccountByCandidateId(int candidateId)
        {
            if (candidateId <= 0)
                return null;

            var query = _candidateBankAccountRepository.Table;

            query = from c in _candidateBankAccountRepository.Table
                    where c.CandidateId == candidateId && c.IsActive == true
                    orderby c.UpdatedOnUtc descending
                    select c;

            return query.FirstOrDefault();
        }



        public Candidate GetCandidateByBankAccountNumber(string candidateBankAccountNumber)
        {
            if (string.IsNullOrWhiteSpace(candidateBankAccountNumber))
                return null;

            var query = _candidateRepository.Table;

            query = from c in query
                    join a in _candidateBankAccountRepository.Table on c.Id equals a.CandidateId 
                    where a.AccountNumber == candidateBankAccountNumber && a.IsActive == true
                    select c;


            return query.FirstOrDefault();
        }

         #endregion

        #region Paged List

        public IQueryable<CandidateBankAccount> GetAllCandidateBankAccountsByCandidateId(int candidateId)
        {
            if (candidateId == 0)
                return Enumerable.Empty<CandidateBankAccount>().AsQueryable();

            var query = _candidateBankAccountRepository.Table;

            query = from s in query
                    where s.CandidateId == candidateId //&& s.IsActive == true
                    orderby s.CreatedOnUtc descending
                    select s;

            return query;
        }

        public IQueryable<CandidateBankAccount> GetAllCandidateBankAccountsAsQueryable(bool showHidden = false)
        {
            var query = _candidateBankAccountRepository.Table;

            if (!showHidden)
                query = query.Where(c => !c.IsDeleted);

            query = from c in query
                    orderby c.CandidateId descending, c.UpdatedOnUtc descending
                    select c;

            return query.AsQueryable();
        }

        #endregion

    }
}

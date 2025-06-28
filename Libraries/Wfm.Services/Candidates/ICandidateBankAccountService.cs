using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateBankAccountService
    {

        #region CRUD

        void Insert(CandidateBankAccount candidateBankAccount);

        void Update(CandidateBankAccount candidateBankAccount);

        void Delete(CandidateBankAccount candidateBankAccount);

        #endregion


        #region CandidateBankAccount

        CandidateBankAccount GetCandidateBankAccountById(int id);

        bool IsDuplicate(string candidateBankAccountNumber);

        CandidateBankAccount GetCandidateBankAccountByBankAccountNumber(string candidateBankAccountNumber);

        CandidateBankAccount GetActiveCandidateBankAccountByCandidateId(int candidateId);

        Candidate GetCandidateByBankAccountNumber(string candidateBankAccountNumber);

        #endregion

        #region Paged List

        IQueryable<CandidateBankAccount> GetAllCandidateBankAccountsByCandidateId(int CandidateId);

        IQueryable<CandidateBankAccount> GetAllCandidateBankAccountsAsQueryable(bool showHidden = false);

        #endregion

    }
}

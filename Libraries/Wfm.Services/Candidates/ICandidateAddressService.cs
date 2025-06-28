using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    /// <summary>
    /// Candidate Service Interface
    /// </summary>
    public partial interface ICandidateAddressService
    {

        #region CRUD

        /// <summary>
        /// Inserts the candidate address.
        /// </summary>
        /// <param name="candidateAddress">The candidate address.</param>
        void InsertCandidateAddress(CandidateAddress candidateAddress);

        /// <summary>
        /// Updates the candidate address.
        /// </summary>
        /// <param name="candidateAddress">The candidate address.</param>
        void UpdateCandidateAddress(CandidateAddress candidateAddress);

        /// <summary>
        /// Deletes the candidate address.
        /// </summary>
        /// <param name="candidateAddress">The candidate address.</param>
        void DeleteCandidateAddress(CandidateAddress candidateAddress);

        #endregion

        #region Candidates

        CandidateAddress GetCandidateAddressById(int id);

        CandidateAddress GetCandidateAddressByCandidateIdAndType(int candidateId, int typeId);

        CandidateAddress GetCandidateHomeAddressByCandidateId(int candidateId);

        CandidateAddress GetCandidateMailingAddressByCandidateId(int candidateId);

        #endregion

        #region LIST

        IList<CandidateAddress> GetAllCandidateAddressesByCandidateId(int candidateId, bool showInactive = false);

        IQueryable<CandidateAddress> GetAllCandidateAddressesAsQueryable();

        IQueryable<CandidateAddressWithName> GetAllCandidateAddressesForListAsQueryable();
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.ClockTime;

namespace Wfm.Services.ClockTime
{
    public partial interface ISmartCardService
    {

        #region CRUD

        void Insert(CandidateSmartCard smartCard);

        void Update(CandidateSmartCard smartCard);

        void Delete(CandidateSmartCard smartCard);

        void InsertDefaultSmartCard(string smartCardUid, int candidateId);

        #endregion


        #region SmartCard

        CandidateSmartCard GetSmartCardById(int id);

        bool IsDuplicate(string smartCardUid, int? id = null);

        CandidateSmartCard GetSmartCardBySmartCardUid(string smartCardUid);

        CandidateSmartCard GetActiveSmartCardByCandidateId(int candidateId);

        Candidate GetCandidateBySmartCardUid(string smartCardUid, bool activeOnly = true, DateTime? refDate = null);

        IList<CandidateSmartCard> GetSmartCardsByPartialSmartCardUid(string smartCardUid);

        CandidateSmartCard GetSmartCardByGuid(Guid guid);

        string GetIdString(CandidateSmartCard smartCard, int? idLength);

        Candidate GetCandidateByIdString(CompanyClockDevice clockDevice, string idString, out string smartCardUid, bool activeOnly = true, DateTime? refDate = null);

        IEnumerable<Candidate> GetCandidatesByIdString(int companyId, string idString, out string smartCardUid, bool activeOnly = true, DateTime? refDate = null);

        #endregion


        #region Paged List

        IList<CandidateSmartCard> GetAllSmartCards();

        IList<CandidateSmartCard> GetAllSmartCardsByCandidateId(int candidateId, bool showInactive = false);

        IQueryable<CandidateSmartCard> GetAllSmartCardsAsQueryable(bool showHidden = false, bool showInactive = true);

        #endregion

    }
}

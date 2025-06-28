using System;
using System.Linq;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Services.TimeSheet
{
    public partial interface IMissingHourService
    {
        void InsertCandidateMissingHour(CandidateMissingHour candidateMissingHour);

        void UpdateCandidateMissingHour(CandidateMissingHour candidateMissingHour);

        void DeleteCandidateMissingHour(CandidateMissingHour candidateMissingHour);

        string InsertOrUpdateMissingHour(int jobOrderId, int candidateId, DateTime startDate, decimal hours, string note);

        CandidateMissingHour GetMissingHourById(int id);

        CandidateMissingHour GetMissingHourByCandidateIdAndJobOrderIdAndJobStartDate(int candidateId, int jobOrderId, DateTime jobStartDate);

        IQueryable<CandidateMissingHour> GetAllCandidateMissingHourAsQueryable(bool isAccountBased = true);
    }
}

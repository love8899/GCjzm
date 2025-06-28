using System.Collections.Generic;
using System.Text;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Candidates
{
    public partial interface ICandidateTestResultService
    {

        #region CRUD

        void InsertCandidateTestResult(CandidateTestResult candidateTestResult);

        void UpdateCandidateTestResult(CandidateTestResult candidateTestResult);

        void DeleteCandidateTestResult(CandidateTestResult candidateTestResult);

        string SaveCandiateTestResultToFile(int candidateId, int testCategoryId, StringBuilder sbTestResultContent);

        #endregion


        CandidateTestResult GetCandidateTestResultById(int id);

        List<CandidateTestResult> GetCandidateTestResultsByCandidateId(int candidateId);

    }
}

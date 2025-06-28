using System;
using Wfm.Core.Domain.Tests;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateTestResult : BaseEntity
    {
        public int CandidateId { get; set; }
        public int TestCategoryId { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalScore { get; set; }
        public int PassScore { get; set; }
        public int TestScore { get; set; }
        public DateTime? TestedOnUtc { get; set; }
        public string Duration { get; set; }
        public string ScoreFilePath { get; set; }
        public bool IsPassed { get; set; }
        public string TestResultDetail { get; set; }
        public string Note { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Candidate Candidate { get; set; }
        public virtual TestCategory TestCategory { get; set; }
    }
}

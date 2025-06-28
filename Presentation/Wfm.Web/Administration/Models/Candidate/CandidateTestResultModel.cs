using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Models.Test;

namespace Wfm.Admin.Models.Candidate
{
    public class CandidateTestResultModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TestCategory")]
        public int TestCategoryId { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateTestResult.Fields.TotalQuestions")]
        public int TotalQuestions { get; set; }

        [WfmResourceDisplayName("Common.TotalScore")]
        public int TotalScore { get; set; }

        [WfmResourceDisplayName("Common.PassingScore")]
        public int PassScore { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateTestResult.Fields.TestScore")]
        public int TestScore { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateTestResult.Fields.TestedOnUtc")]
        public DateTime? TestedOnUtc { get; set; }

        public string ScoreFilePath { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateTestResult.Fields.Duration")]
        public int Duration { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateTestResult.Fields.IsPassed")]
        public bool IsPassed { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateTestResult.Fields.TestResultDetail")]
        public string TestResultDetail { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }


        public virtual TestCategoryModel TestCategory { get; set; }
        public virtual CandidateModel Candidate { get; set; }
    }
}
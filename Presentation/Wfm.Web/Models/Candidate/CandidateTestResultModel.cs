using System;
using Wfm.Web.Models.Test;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Web.Models.Candidate
{
    public class CandidateTestResultModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.TestCategory")]
        public int TestCategoryId { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.TotalQuestions")]
        public int TotalQuestions { get; set; }
        [WfmResourceDisplayName("Common.TotalScore")]
        public int TotalScore { get; set; }
        [WfmResourceDisplayName("Common.PassingScore")]
        public int PassScore { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.TestScore")]
        public int TestScore { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.TestedOnUtc")]
        public DateTime? TestedOnUtc { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.Duration")]
        public string Duration { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.ScoreFilePath")]
        public string ScoreFilePath { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.IsPassed")]
        public bool IsPassed { get; set; }
        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.TestResultDetail")]
        public string TestResultDetail { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }


        public bool IsDeleted { get; set; }


        [WfmResourceDisplayName("Web.Candidate.CandidateTestResult.Fields.TestedOn")]
        public virtual DateTime? TestedOn
        {
            get
            {
                DateTime dt = DateTime.MinValue;
                if (TestedOnUtc.HasValue)
                {
                    dt = TestedOnUtc.Value;
                }
                return dt.ToLocalTime();
            }

            set
            {
            }
        }



        public virtual CandidateModel CandidateModel { get; set; }
        public virtual TestCategoryModel TestCategoryModel { get; set; }
    }
}

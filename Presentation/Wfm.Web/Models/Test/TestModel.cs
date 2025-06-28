using System;
using System.Collections.Generic;

namespace Wfm.Web.Models.Test
{
    public class TestModel
    {
        public List<TestQuestionModel> TestQuestions { get; set; }
        public int CategoryId { get; set; }
        public Guid? CandidateGuid { get; set; }
    }
}
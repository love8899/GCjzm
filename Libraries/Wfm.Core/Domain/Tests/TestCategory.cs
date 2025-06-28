using System.Collections.Generic;

namespace Wfm.Core.Domain.Tests
{
    public class TestCategory : BaseEntity
    {
        public string TestCategoryName { get; set; }
        public string TestUrl { get; set; }
        public int TotalScore { get; set; }
        public int PassScore { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequiredWhenRegistration { get; set; }

        public int? CompanyId { get; set; }
        public int? IndustryID { get; set; }

        public virtual List<TestQuestion> TestQuestions { get; set; }
        //public List<TestQuestion> TestQuestions { get; set; }
    }
}

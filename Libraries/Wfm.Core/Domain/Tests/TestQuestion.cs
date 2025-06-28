using System.Collections.Generic;

namespace Wfm.Core.Domain.Tests
{
    public class TestQuestion : BaseEntity
    {
        public int TestCategoryId { get; set; }
        public string Question { get; set; }
        public bool IsSingleChoice { get; set; }
        public bool IsMultipleChoice { get; set; }
        public string ImageFileLocation { get; set; }
        public string Answers { get; set; }
        public int Score { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual TestCategory TestCategory { get; set; }
        public virtual List<TestChoice> TestChoices { get; set; }
    }
}

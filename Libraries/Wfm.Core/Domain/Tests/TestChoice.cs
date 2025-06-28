namespace Wfm.Core.Domain.Tests
{
    public class TestChoice : BaseEntity
    {
        public int TestQuestionId { get; set; }
        public string TestChoiceText { get; set; }
        public string TestChoiceValue { get; set; }
        public string ImageFileLocation { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual TestQuestion TestQuestion { get; set; }
    }
}

namespace Wfm.Core.Domain.Common
{
    public class Skill : BaseEntity
    {
        public string SkillName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}
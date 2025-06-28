namespace Wfm.Core.Domain.Common
{
    public class Gender : BaseEntity
    {
        public string GenderName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}
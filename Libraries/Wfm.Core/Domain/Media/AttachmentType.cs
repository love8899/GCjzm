namespace Wfm.Core.Domain.Media
{
    public class AttachmentType : BaseEntity
    {
        public string TypeName { get; set; }
        public string FileExtensions { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}

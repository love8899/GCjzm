
namespace Wfm.Core.Domain.Media 
{
    public class DocumentType:BaseEntity
    {
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string InternalCode { get; set; }
        public string FileName { get; set; } 
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublic { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}

namespace Wfm.Core.Domain.Common
{
    /// <summary>
    /// Vetran Type
    /// </summary>
    public class VetranType : BaseEntity
    {
        public string VetranTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}
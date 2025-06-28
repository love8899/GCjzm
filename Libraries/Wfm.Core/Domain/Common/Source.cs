namespace Wfm.Core.Domain.Common
{
    /// <summary>
    /// Information Source
    /// </summary>
    public class Source : BaseEntity
    {
        public string SourceName { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}
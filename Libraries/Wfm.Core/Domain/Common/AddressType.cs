namespace Wfm.Core.Domain.Common
{
    public class AddressType : BaseEntity
    {
        public string AddressTypeName { get; set;}
        //public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}

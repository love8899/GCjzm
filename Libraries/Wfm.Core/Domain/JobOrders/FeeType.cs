namespace Wfm.Core.Domain.JobOrders
{
    public class FeeType : BaseEntity
    {
        public string FeeTypeName { get; set; }       
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}

namespace Wfm.Core.Domain.JobOrders
{
    public class JobOrderStatus : BaseEntity
    {
        public string JobOrderStatusName { get; set; }
        //public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    } 
}
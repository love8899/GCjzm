namespace Wfm.Core.Domain.JobOrders
{
    public class JobOrderType : BaseEntity
    {
        public string JobOrderTypeName { get; set; }
        //public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDirectHire { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}

// Type: H (Hire), C2H (Contract to Hire), C (Contract), FL(Freelance)

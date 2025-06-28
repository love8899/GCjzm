namespace Wfm.Core.Domain.Common
{
    public class Bank : BaseEntity
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
    }
}
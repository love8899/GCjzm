namespace Wfm.Core.Domain.Companies
{
    public class Quotation : BaseEntity
    {
        public int CompanyBillingRateId { get; set; }
        public string FileName { get; set; }
        public byte[] Stream { get; set; }

        public virtual CompanyBillingRate CompanyBillingRate { get; set; }
    }
}

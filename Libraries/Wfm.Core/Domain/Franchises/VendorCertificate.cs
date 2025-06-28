using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wfm.Core.Domain.Franchises
{
    public class VendorCertificate : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CertificateGuid { get; set; }
        public int FranchiseId { get; set; }
        public decimal GeneralLiabilityCoverage { get; set; }
        public DateTime? GeneralLiabilityCertificateExpiryDate { get; set; }
        public byte[] CertificateFile { get; set; }

        public string ContentType { get; set; }
        public string CertificateFileName { get; set; }
        public bool HasCertificate { get { if (CertificateFile!=null)return true; else return false; } }
        public string Description { get; set; }
        [ForeignKey("FranchiseId")]
        public virtual Franchise Vendor { get; set; }
    }

    public class SimpleVendorCertificate
    {
        public int FranchiseId { get; set; }
        public string Description { get; set; }
        public DateTime GeneralLiabilityCertificateExpiryDate { get; set; }
    }
}

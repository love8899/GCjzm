using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Franchises
{
    public class VendorCertificateModel : BaseWfmEntityModel
    {
        public Guid? CertificateGuid { get; set; }
        public bool HasCertificate { get; set; }
        public int FranchiseId { get; set; }
        public decimal GeneralLiabilityCoverage { get; set; }
        [UIHint("DateNullable")]
        public DateTime? GeneralLiabilityCertificateExpiryDate { get; set; }
        [Required]
        [MaxLength(80)]
        public string Description { get; set; }

    }
}
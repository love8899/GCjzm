using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wfm.Core.Domain.Franchises
{
    /// <summary>
    /// Franchise 
    /// One Franchise has one to many users.
    /// One User belong to one and only one Franchise.
    /// </summary>
    public class Franchise : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FranchiseGuid { get; set; }
        public string FranchiseId { get; set; }
        public string FranchiseName { get; set; }
        //public string BusinessNumber  { get; set; }
        public string PrimaryContactName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public bool IsSslEnabled { get; set; }
        
        public string Description { get; set; }
        public string ReasonForDisabled { get; set; }
        public string Note { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsDefaultManagedServiceProvider { get; set; }
        public bool IsLinkToPublicSite { get; set; }
         
        public bool EnableStandAloneJobOrders { get; set; }
        public int OwnerId { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }
        public string ShortName { get; set; }
        public byte[] FranchiseLogo { get; set; }
        public string FranchiseLogoFileName { get; set; }

        //public virtual List<FranchiseAddress> FranchiseAddresses { get; set; }
        public virtual List<FranchiseOvertimeRule> FranchiseOvertimeRules { get; set; }

        public virtual List<VendorCertificate> VendorCertificates { get; set; }
    }

}
